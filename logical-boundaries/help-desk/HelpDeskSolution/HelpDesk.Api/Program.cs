
using HelpDesk.Api.Incidents;
using HelpDesk.Api.Services;
using HelpDesk.Api.User.ReadModels;
using HelpDesk.Api.User.Services;
using HtTemplate.Configuration;
using Marten;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;

var builder = WebApplication.CreateBuilder(args);


builder.AddCustomFeatureManagement();

builder.Services.AddCustomServices();
builder.Services.AddCustomOasGeneration();

builder.Services.AddControllers();

if (builder.Environment.IsDevelopment())
{
    // this is just for a classroom - ordinarily I'd replace this in my test context.
    builder.Services
        .AddScoped<IProvideUserInformation, FakeDevelopmentUserInformation>();
}
else
{
    builder.Services.AddScoped<IProvideUserInformation, UserInformationProvider>();
}

var connectionString = builder.Configuration.GetConnectionString("data") ??
                       throw new Exception("No database connection string");
builder.Services.AddMarten(opts =>
{
    opts.Connection(connectionString);
    opts.Schema.For<User>().Index(u => u.Sub, x => x.IsUnique = true);
    opts.Projections.Add<UserProjection>(ProjectionLifecycle.Inline);
    opts.Projections.Snapshot<IncidentReadModel>(SnapshotLifecycle.Inline);

}).UseLightweightSessions().AddAsyncDaemon(DaemonMode.Solo);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
//app.Map("/", async (IProvideUserInformation userProvider) =>
//{
//    var userInfo = await userProvider.GetUserInfoAsync();
//    return Results.Ok(userInfo);
//});
app.Run();