using Tracker.AuthenticationService.Application;
using Microsoft.OpenApi.Models;
using Tracker.AuthenticationService;
using Tracker.AuthenticationService.Auth0;
using Microsoft.EntityFrameworkCore;
using Tracker.AuthenticationService.Auth0.ConfigObjects;
using Tracker.AuthenticationService.Data.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

services.AddControllers();
services.AddAuthenticationWithAuth0(configuration);
services.AddDbContext<TestDataContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
{
    var auth0Config = configuration.GetSection(Auth0Config.Auth0).Get<Auth0Config>();

    c.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Title = "API",
                    Version = "v1",
                    Description = "A REST API"
                });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            Implicit = new OpenApiOAuthFlow
            {
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "Open Id" }
                },
                AuthorizationUrl = new Uri(auth0Config.Authority + "authorize?audience=" + auth0Config.Audience)
            }
        }
    });

    c.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Logging.AddJsonConsole();

services.AddTransient<IAuthenticationService, AuthenticationService>();
services.AddTransient<IAlphaService, AlphaService>();
services.AddHttpClient(nameof(AlphaService));
services.AddTransient<IDbService, DbService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        var auth0Config = configuration.GetSection(Auth0Config.Auth0).Get<Auth0Config>();
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
        c.OAuthClientId(auth0Config.ClientId);
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();