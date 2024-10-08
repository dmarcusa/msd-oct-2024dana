using System.Reflection;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;

namespace HtTemplate.Configuration;

public static class ServicesExtensions
{
    public static WebApplicationBuilder AddCustomFeatureManagement(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<ApiFeatureManagementOptions>(
            builder.Configuration.GetSection(ApiFeatureManagementOptions.FeatureManagement));
        builder.Services.AddFeatureManagement();
        return builder;
    }

    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services.Configure<JsonOptions>(DefaultJsonOptions.Configure);
        services.AddAuthentication().AddJwtBearer();
        services.AddFeatureManagement();
        services.AddSingleton(() => TimeProvider.System);
        services.AddHttpContextAccessor();

        return services;
    }

    public static IServiceCollection AddCustomOasGeneration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(
            options =>
            {
                options.TagActionsBy(api =>
                {
                    if (api.GroupName != null) return new[] { api.GroupName };

                    var controllerActionDescriptor = api.ActionDescriptor as ControllerActionDescriptor;
                    if (controllerActionDescriptor != null) return new[] { controllerActionDescriptor.ControllerName };

                    throw new InvalidOperationException("Unable to determine tag for endpoint.");
                });
                options.DocInclusionPredicate((name, api) => true);
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header with bearer token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            },
                            Scheme = "oauth2",
                            Name = "Bearer ",
                            In = ParameterLocation.Header
                        },
                        []
                    }
                });
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
        services.AddFluentValidationRulesToSwagger();
        return services;
    }
}