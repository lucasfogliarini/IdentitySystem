public class BoraKeycloakSystem(IDistributedApplicationBuilder builder) : KeycloakSystem(builder)
{
    const string SystemName = nameof(BoraKeycloakSystem);
    protected override string Name { get; init; } = SystemName;
    protected override string Url { get; init; } = $"https://bora.earth/work/{SystemName}/";
    const string BoraRealm = "bora";
    public override IResourceBuilder<ExternalServiceResource> AddToResources()
    {
        var boraAdmin = Builder.AddExternalService($"{BoraRealm}Admin", $"{MainService.AbsolutePath}/admin/{BoraRealm}/console/");
        var boraAccount = Builder.AddExternalService($"{BoraRealm}Account", $"{MainService.AbsolutePath}/realms/{BoraRealm}/account/");

        var system =  base.AddToResources()
            .WithChildRelationship(boraAdmin)
            .WithChildRelationship(boraAccount);

        KeycloakResource
            .WithImport($"{SystemName}/bora-realm");

        return system;
    }
}