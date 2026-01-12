public static class KeycloakSystemBuilderExtensions
{
    public static KeycloakSystem AddKeycloakSystem(this IDistributedApplicationBuilder builder)
    {
        var keycloakSystem = new KeycloakSystem(builder);
        keycloakSystem.AddToResources();
        return keycloakSystem;
    }
}