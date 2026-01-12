//#:sdk Aspire.AppHost.Sdk@13.1.0
//#:package Aspire.Hosting.PostgreSQL@13.1.0
//#:package Keycloak.AuthServices.Aspire.Hosting@0.2.0
//using Aspire.Hosting;
//using Aspire.Hosting.ApplicationModel;
//using Microsoft.Extensions.DependencyInjection;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddBoraKeycloakSystem()
       .AddToAspire();

var app = builder.Build();

await app.RunAsync();