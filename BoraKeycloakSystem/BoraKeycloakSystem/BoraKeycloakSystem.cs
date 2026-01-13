public class BoraKeycloakSystem(IDistributedApplicationBuilder builder) : KeycloakSystem(builder)
{
    const string SystemName = nameof(BoraKeycloakSystem);
    protected override string Name { get; init; } = SystemName;
    protected override string SystemDiagramUrl { get; init; } = $"https://bora.earth/work/{SystemName}/";

    const string BoraRealm = "bora";
    public override IResourceBuilder<ExternalServiceResource> AddSystem()
    {
        var system = base.AddSystem();
        var boraAdmin = Builder.AddExternalService($"{BoraRealm}Admin", $"{KeycloakService.Uri}/admin/{BoraRealm}/console/");
        var boraAccount = Builder.AddExternalService($"{BoraRealm}Account", $"{KeycloakService.Uri}/realms/{BoraRealm}/account/");

        system
            .WithChildRelationship(boraAdmin)
            .WithChildRelationship(boraAccount);

        KeycloakService.Resource
            .WithImport($"{SystemName}/bora-realm");

        return system;
    }
}