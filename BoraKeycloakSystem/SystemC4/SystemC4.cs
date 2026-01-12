/// <summary>
/// A service or server or container
/// https://c4model.com/abstractions/container
/// <see cref="Uri"/> like
/// </summary>
public class Service(string name, int port, string scheme = "http", string host = "localhost")
{
    public string Name { get; set; } = name;
    public string Scheme { get; set; } = scheme;
    public string Host { get; set; } = host;
    public int Port { get; set; } = port;
    public string AbsolutePath { get { return $"{Scheme}://{Host}:{Port}"; } }
}

public record class Database(string Name);
public class DatabaseServer(string Name, int Port) : Service(Name, Port)
{
    public Database Database { get; set; } = new Database($"{Name.ToLower()}-database");
}

/// <summary>
/// https://c4model.com/abstractions/software-system
/// </summary>
public abstract class SystemC4
{
    protected abstract string Name { get; init; }
    protected abstract string Url { get; init; }    
    public IDistributedApplicationBuilder Builder { get; private set; }
    public Service System { get; private set; }
    public DatabaseServer DatabaseServer { get; init; }
    public Service MainService { get; init; }
    public Service ObservabilityService { get; init; }

    protected SystemC4(IDistributedApplicationBuilder builder, int Port = 2000)
    {
        Builder = builder;
        System = new Service($"{Name.ToLower()}", Port);
        DatabaseServer = new DatabaseServer($"{Name.ToLower()}-dbserver", Port + 1);
        MainService = new Service($"{Name.ToLower()}-mainservice", Port + 2);
        ObservabilityService = new Service($"{Name.ToLower()}-observabilityservice", Port + 3);
    }

    public virtual IResourceBuilder<ExternalServiceResource> AddToResources()
    {
        return Builder.AddExternalService(Name, Url);
    }
    public ExternalServiceResource? GetSystem(string name)
    {
        return Builder.Resources.OfType<ExternalServiceResource>().FirstOrDefault(e => e.Name == name);
    }
    public void AddToAspire()
    {
        Builder.Configuration["ASPIRE_ALLOW_UNSECURED_TRANSPORT"] = "true";
        Builder.Configuration["ASPIRE_DASHBOARD_OTLP_HTTP_ENDPOINT_URL"] = ObservabilityService!.AbsolutePath;
        Builder.Configuration["ASPNETCORE_URLS"] = System.AbsolutePath;
    }
}

// Structurize C4 Pattern https://docs.structurizr.com/dsl
// Backstage Pattern https://backstage.io/docs/features/software-catalog/system-model/
