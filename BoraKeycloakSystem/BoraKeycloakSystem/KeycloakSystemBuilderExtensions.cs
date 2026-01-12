public static class BoraKeycloakSystemBuilderExtensions
{
    public static KeycloakSystem AddBoraKeycloakSystem(this IDistributedApplicationBuilder builder)
    {
        var keycloakSystem = new BoraKeycloakSystem(builder);
        keycloakSystem.AddToResources();
        return keycloakSystem;
    }
}