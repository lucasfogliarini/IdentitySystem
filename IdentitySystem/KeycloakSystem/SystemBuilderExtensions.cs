public static class SystemBuilderExtensions
{
    public static SystemResource AddSystem(this IDistributedApplicationBuilder builder, string name)
    {
        var system = new SystemResource(name);

        builder.Configuration["ASPIRE_ALLOW_UNSECURED_TRANSPORT"] = "true";
        builder.Configuration["ASPIRE_DASHBOARD_OTLP_HTTP_ENDPOINT_URL"] = system.ObservabilityService!.AbsolutePath;
        builder.Configuration["ASPNETCORE_URLS"] = system.AbsolutePath;

        builder.AddResource(system);
        return system;
    }

    public static SystemResource? GetSystem(this IDistributedApplicationBuilder builder)
    {
        return builder.Resources.OfType<SystemResource>().FirstOrDefault();
    }
}