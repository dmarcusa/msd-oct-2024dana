using HelpDesk.Api.Catalog.ReadModels;
using HelpDesk.Api.TierOneSupport.ReadModels;
using HelpDesk.Api.User.ReadModels;
using HelpDesk.Api.User.Services;
using HtTemplate.Configuration;
using Marten;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;
using Oakton;
using Wolverine;
using Wolverine.Kafka;
using Wolverine.Marten;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ApplyOaktonExtensions();

builder.AddCustomFeatureManagement();

builder.Services.AddCustomServices();
builder.Services.AddCustomOasGeneration();

builder.Services.AddControllers();


if (builder.Environment.IsDevelopment())
    // this is just for a classroom - ordinarily I'd replace this in my test context.
    builder.Services
        .AddScoped<IProvideUserInformation, FakeDevelopmentUserInformation>();
else
    builder.Services.AddScoped<IProvideUserInformation, UserInformationProvider>();

var connectionString = builder.Configuration.GetConnectionString("data") ??
                       throw new Exception("No database connection string");
builder.Services.AddMarten(opts =>
{
    opts.Connection(connectionString);
    opts.Schema.For<User>().Index(u => u.Sub, x => x.IsUnique = true);
    opts.Projections.Add<UserProjection>(ProjectionLifecycle.Inline);
    opts.Projections.Snapshot<Incident>(SnapshotLifecycle.Inline);
    opts.Projections.Add<CatalogItemProjection>(ProjectionLifecycle.Async);
    opts.Projections.Snapshot<SubmittedIncident>(SnapshotLifecycle.Async);
}).UseLightweightSessions().IntegrateWithWolverine().AddAsyncDaemon(DaemonMode.Solo);

var kafkaConnectionString = builder.Configuration.GetConnectionString("kafka") ?? throw new Exception("Need A Broker");
builder.Host.UseWolverine(opts =>
{
    opts.UseKafka(kafkaConnectionString).ConfigureConsumers(c =>
    {
        c.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
        c.GroupId = "help-desk-issue-tracker";

        // softwarecenter.catalog-item-created
        // softwarecenter.catalog-item-retired
    });
    //opts.ListenToKafkaTopic("xxx").ProcessInline();
    opts.Policies.AutoApplyTransactions();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// hey, if anyone makes a get request to localhost:1338/catalog send them to otherserver/catalog
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

return await app.RunOaktonCommands(args);