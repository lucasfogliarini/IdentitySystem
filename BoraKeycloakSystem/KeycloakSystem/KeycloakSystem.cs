public class KeycloakSystem(IDistributedApplicationBuilder builder) : SoftwareSystem(builder)
{
    const string SystemName = nameof(KeycloakSystem);
    const string PostgresPassword = SystemName;
    protected override string Name { get; init; } = SystemName;
    protected override string SystemDiagramUrl { get; init; } = $"https://bora.earth/work/{SystemName}/";
    public Service<KeycloakResource> KeycloakService { get { return GetService<Service<KeycloakResource>>(); } }
    public DatabaseServer<PostgresServerResource> PostgresServer { get { return GetService<DatabaseServer<PostgresServerResource>>(); } }

    IResourceBuilder<ParameterResource>? PostgresPasswordResource;
    public override IResourceBuilder<ExternalServiceResource> AddSystem()
    {
        var system = base.AddSystem();
        PostgresPasswordResource = Builder.AddParameter("postgres-password", PostgresPassword, secret: false);

        AddKeycloakPostgresServer();
        AddKeycloakService(PostgresServer.Resource);

        return system
            .WithChildRelationship(PostgresServer.Resource)
            .WithChildRelationship(KeycloakService.Resource);
    }
    public static KeycloakSystem Add(IDistributedApplicationBuilder builder)
    {
        var keycloakSystem = new KeycloakSystem(builder);
        keycloakSystem.AddSystem();
        return keycloakSystem;
    }

    /// <summary>
    /// https://www.keycloak.org
    /// </summary>
    public Service AddKeycloakService(IResourceBuilder<PostgresServerResource> keycloakPostgresServer)
    {
        var databaseServer = GetService<DatabaseServer<PostgresServerResource>>();
        var keycloakService = AddService<Service<KeycloakResource>>("keycloak");
        keycloakService.Resource = Builder.AddKeycloakContainer(keycloakService.Name, port: keycloakService.Port)
            .WaitFor(keycloakPostgresServer)
            //.WithLifetime(ContainerLifetime.Persistent)
            .WithDataVolume($"{keycloakService.Name}_data")            
            .WithEnvironment("KC_BOOTSTRAP_ADMIN_USERNAME", "admin")
            .WithEnvironment("KC_BOOTSTRAP_ADMIN_PASSWORD", "admin")

            .WithEnvironment("KC_DB", "postgres")
            .WithEnvironment("KC_DB_URL", $"jdbc:postgresql://{databaseServer.Name}:5432/{databaseServer.Database.Name}")
            .WithEnvironment("KC_DB_USERNAME", "postgres")
            .WithEnvironment("KC_DB_PASSWORD", PostgresPasswordResource)

            .WithEnvironment("KC_HEALTH_ENABLED", "true")
            .WithEnvironment("KC_METRICS_ENABLED", "true");
        return keycloakService;
    }

    /// <summary>
    /// https://www.keycloak.org/server/db
    /// </summary>
    IResourceBuilder<PostgresServerResource> AddKeycloakPostgresServer()
    {
        var postgresServer = AddService<DatabaseServer<PostgresServerResource>>("postgres-server");
        postgresServer.Database = new Database("keycloakdb");
        postgresServer.Resource = Builder.AddPostgres(postgresServer.Name, port: postgresServer.Port)
            .WithLifetime(ContainerLifetime.Persistent)
            .WithDataVolume($"{postgresServer.Name}_data")
            //.WithPgAdmin()
            .WithPassword(PostgresPasswordResource!);
        postgresServer.Resource.AddDatabase(postgresServer.Database.Name);
        return postgresServer.Resource;
    }
}