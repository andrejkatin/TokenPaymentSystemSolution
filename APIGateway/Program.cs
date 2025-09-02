using APIGateway.Extensions;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.OpenApi.Models;
using System.Net;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services
    .AddAuthorization(options =>
    {
        options.AddPolicy("authenticated", policy =>
        {
            policy.RequireAuthenticatedUser();
        });
        options.AddPolicy("user", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireRole("ROLE_USER");
        });
        options.AddPolicy("admin", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireRole("ROLE_ADMIN");
        });
        options.AddPolicy("system", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireRole("ROLE_SYSTEM");
        });
    })
    .AddAuthentication()
    .AddZitadelIntrospection(
        "ZITADEL_BASIC",
        o =>
        {
            o.Authority = builder.Configuration.GetValue<string>("Zitadel:Domain");
            o.ClientId = builder.Configuration.GetValue<string>("Zitadel:ClientId");
            o.ClientSecret = builder.Configuration.GetValue<string>("Zitadel:ClientSecret");
        });

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(builderContext =>
    {
        if (!string.IsNullOrEmpty(builderContext.Route.AuthorizationPolicy))
        {

            builderContext.AddRequestTransform(async transformContext =>
            {
                var claimSub = transformContext.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

                if (!string.IsNullOrEmpty(claimSub))
                {
                    transformContext.ProxyRequest.Headers.Add("sub", claimSub);
                }
            });
        }
    });

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

app.MapControllers();

app.Run();
