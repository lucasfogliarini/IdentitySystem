//#:sdk Aspire.AppHost.Sdk@13.1.0
//#:package Aspire.Hosting.PostgreSQL@13.1.0
//#:package Keycloak.AuthServices.Aspire.Hosting@0.2.0
//using Aspire.Hosting;
//using Aspire.Hosting.ApplicationModel;
//using Microsoft.Extensions.DependencyInjection;

var builder = DistributedApplication.CreateBuilder(args);

var realm = "bora";
builder.AddKeycloakSystem("identity","BoraCSF!123")
       .AddAcountConsole(realm)
       .AddAdminConsole(realm);

var app = builder.Build();

await app.RunAsync();