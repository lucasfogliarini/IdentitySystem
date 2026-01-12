public class KeycloakSystem(IDistributedApplicationBuilder builder) : SystemC4(builder)
{
    const string SystemName = nameof(KeycloakSystem);
    const string PostgresPassword = SystemName;
    protected override string Name { get; init; } = SystemName;
    protected override string Url { get; init; } = $"https://bora.earth/work/{SystemName}/";    
    public IResourceBuilder<KeycloakResource> KeycloakResource { get; private set; }
    IResourceBuilder<ParameterResource>? PostgresPasswordResource;
    public override IResourceBuilder<ExternalServiceResource> AddToResources()
    {
        PostgresPasswordResource = Builder.AddParameter("postgres-password", PostgresPassword, secret: false);

        var keycloakPostgresServer = AddKeycloakPostgresServer();
        AddKeycloakServer(keycloakPostgresServer);

        return base.AddToResources()
            .WithChildRelationship(keycloakPostgresServer)
            .WithChildRelationship(KeycloakResource);
    }

    /// <summary>
    /// https://www.keycloak.org
    /// </summary>
    public void AddKeycloakServer(IResourceBuilder<PostgresServerResource> keycloakPostgresServer)
    {
        KeycloakResource = Builder.AddKeycloakContainer(MainService.Name, port: MainService.Port)
            .WaitFor(keycloakPostgresServer)
            //.WithLifetime(ContainerLifetime.Persistent)
            .WithDataVolume($"{MainService.Name}_data")            
            .WithEnvironment("KC_BOOTSTRAP_ADMIN_USERNAME", "admin")
            .WithEnvironment("KC_BOOTSTRAP_ADMIN_PASSWORD", "admin")

            .WithEnvironment("KC_DB", "postgres")
            .WithEnvironment("KC_DB_URL", $"jdbc:postgresql://{DatabaseServer.Name}:5432/{DatabaseServer.Database.Name}")
            .WithEnvironment("KC_DB_USERNAME", "postgres")
            .WithEnvironment("KC_DB_PASSWORD", PostgresPasswordResource)

            .WithEnvironment("KC_HEALTH_ENABLED", "true")
            .WithEnvironment("KC_METRICS_ENABLED", "true");
    }

    /// <summary>
    /// https://www.keycloak.org/server/db
    /// </summary>
    IResourceBuilder<PostgresServerResource> AddKeycloakPostgresServer()
    {
        var postgresServer = Builder.AddPostgres(DatabaseServer.Name, port: DatabaseServer.Port)
            .WithLifetime(ContainerLifetime.Persistent)
            .WithDataVolume($"{DatabaseServer.Name}_data")
            //.WithPgAdmin()
            .WithPassword(PostgresPasswordResource!);
        postgresServer.AddDatabase(DatabaseServer.Database.Name);
        return postgresServer;
    }
}