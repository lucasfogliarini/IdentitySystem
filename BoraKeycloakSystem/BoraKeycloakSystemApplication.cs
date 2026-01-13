//#:sdk Aspire.AppHost.Sdk@13.1.0
//#:package Aspire.Hosting.PostgreSQL@13.1.0
//#:package Keycloak.AuthServices.Aspire.Hosting@0.2.0
//using Aspire.Hosting;
//using Aspire.Hosting.ApplicationModel;
//using Microsoft.Extensions.DependencyInjection;

var boraKeycloakSystem = SoftwareSystem.CreateBuilder<BoraKeycloakSystem>();

var app = boraKeycloakSystem.Builder.Build();

await app.RunAsync();