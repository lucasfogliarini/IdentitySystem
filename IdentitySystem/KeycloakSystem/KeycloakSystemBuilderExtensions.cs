public static class KeycloakSystemBuilderExtensions
{
    /// <summary>
    /// https://www.keycloak.org
    /// </summary>
    public static IDistributedApplicationBuilder AddKeycloakSystem(this IDistributedApplicationBuilder builder, string name, string postgresPassword)
    {
        var system = builder.AddSystem(name);
        var postgresPasswordResource = builder.AddParameter("postgres-password", postgresPassword, secret: false);
        var keycloakDatabase = builder.AddKeycloakPostgres(postgresPasswordResource);

        var keycloak = builder.AddKeycloakContainer(system.MainService.Name, port: system.MainService.Port)
            .WaitFor(keycloakDatabase)
            //.WithLifetime(ContainerLifetime.Persistent)
            .WithDataVolume($"{system.MainService.Name}_data")
            .WithImport("bora-realm")
            .WithEnvironment("KC_BOOTSTRAP_ADMIN_USERNAME", "admin")
            .WithEnvironment("KC_BOOTSTRAP_ADMIN_PASSWORD", "admin")

            .WithEnvironment("KC_DB", "postgres")
            .WithEnvironment("KC_DB_URL", $"jdbc:postgresql://{system.DatabaseServer.Name}:5432/{system.DatabaseServer.Database.Name}")
            .WithEnvironment("KC_DB_USERNAME", "postgres")
            .WithEnvironment("KC_DB_PASSWORD", postgresPasswordResource)

            .WithEnvironment("KC_HEALTH_ENABLED", "true")
            .WithEnvironment("KC_METRICS_ENABLED", "true");

        return builder;
    }

    public static KeycloakResource? GetKeycloakResource(this IDistributedApplicationBuilder builder)
    {
        return builder.Resources.OfType<KeycloakResource>().FirstOrDefault();
    }

    public static IDistributedApplicationBuilder AddAdminConsole(this IDistributedApplicationBuilder builder, string realm)
    {
        var system = builder.GetSystem();
        var keycloak = builder.GetKeycloakResource();
        builder.AddExternalService($"{realm}Admin", $"{system.MainService.AbsolutePath}/admin/{realm}/console/")
            .WithReferenceRelationship(keycloak);
        return builder;
    }

    public static IDistributedApplicationBuilder AddAcountConsole(this IDistributedApplicationBuilder builder, string realm)
    {
        var system = builder.GetSystem();
        var keycloak = builder.GetKeycloakResource();
        builder.AddExternalService($"{realm}Account", $"{system.MainService.AbsolutePath}/realms/{realm}/account/")
            .WithReferenceRelationship(keycloak);
        return builder;
    }

    /// <summary>
    /// https://www.keycloak.org/server/db
    /// </summary>
    static IResourceBuilder<PostgresDatabaseResource> AddKeycloakPostgres(this IDistributedApplicationBuilder builder, IResourceBuilder<ParameterResource> passwordResource)
    {
        var system = builder.GetSystem();

        var postgresServer = builder.AddPostgres(system.DatabaseServer.Name, port: system.DatabaseServer.Port)
            .WithLifetime(ContainerLifetime.Persistent)
            .WithDataVolume($"{system.DatabaseServer.Name}_data")
            //.WithPgAdmin()
            .WithPassword(passwordResource);
        var keycloakDatabase = postgresServer.AddDatabase(system.DatabaseServer.Database.Name);
        return keycloakDatabase;
    }
}