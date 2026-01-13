/// <summary>
/// Represents a container as defined by the C4 Model.
/// https://c4model.com/abstractions/container
/// </summary>
public class Service
{
    public string Name { get; set; } = "";
    public string Scheme { get; set; } = "http";
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 2000;
    public Uri Uri => new($"{Scheme}://{Host}:{Port}");
    
}
public class Service<TResource> : Service where TResource : Resource
{
    public IResourceBuilder<TResource> Resource { get; set; }
}

public record class Database(string Name);
public class DatabaseServer<TResource> : Service<TResource> where TResource : Resource
{
    public Database? Database { get; set; }
}

/// <summary>
/// Represents a Software System as defined by the C4 Model.
/// https://c4model.com/abstractions/software-system
///
/// Architectural diagrams for this system should be maintained using a C4-compliant tool.
/// IcePanel is recommended for creating and maintaining these diagrams: https://icepanel.io/
/// </summary>
public abstract class SoftwareSystem(IDistributedApplicationBuilder builder, int Port = 2000)
{
    protected abstract string Name { get; init; }
    protected abstract string SystemDiagramUrl { get; init; }
    public IDistributedApplicationBuilder Builder { get; init; } = builder;
    public IList<Service> Services { get; init; } = [];

    public virtual IResourceBuilder<ExternalServiceResource> AddSystem()
    {
        var system = new Service<ExternalServiceResource>
        {
            Name = Name,
            Port = Port,
            Resource = Builder.AddExternalService(Name, SystemDiagramUrl)
        };
        Services.Add(system);

        var observabilityService = AddService("observabilityservice");

        Builder.Configuration["ASPIRE_ALLOW_UNSECURED_TRANSPORT"] = "true";
        Builder.Configuration["ASPIRE_DASHBOARD_OTLP_HTTP_ENDPOINT_URL"] = observabilityService!.Uri.ToString();
        Builder.Configuration["ASPNETCORE_URLS"] = system.Uri.ToString();

        return system.Resource;
    }

    public Service<Resource> AddService(string sufixName, int? servicePort = null)
    {
        return AddService<Service<Resource>>(sufixName, servicePort);
    }
    public TService AddService<TService>(string sufixName, int? servicePort = null) where TService : Service
    {
        servicePort ??= Services.Any() ? Services.Max(e => e.Port) + 1 : Port + 1;
        var serviceName = $"{Name}-{sufixName}";
        var service = Activator.CreateInstance<TService>();
        service.Port = servicePort.Value;
        service.Name = serviceName;
        Services.Add(service);
        return service;
    }
    public TService? GetService<TService>() where TService : Service
    {
        return Services.OfType<TService>().FirstOrDefault();
    }

    public static TSystem CreateBuilder<TSystem>() where TSystem : SoftwareSystem
    {
        var builder = DistributedApplication.CreateBuilder();
        var system = (TSystem)Activator.CreateInstance(typeof(TSystem), builder)!;
        system.AddSystem();
        return system;
    }
}

// Structurize C4 Pattern https://docs.structurizr.com/dsl
// Backstage Pattern https://backstage.io/docs/features/software-catalog/system-model/
