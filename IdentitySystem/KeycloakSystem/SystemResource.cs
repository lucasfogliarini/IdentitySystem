/// <summary>
/// A service or server or container
/// https://c4model.com/abstractions/container
/// <see cref="Uri"/> like
/// </summary>
public class Service(string name, int port, string scheme = "http", string host = "localhost") : Resource(name)
{
    public string Scheme { get; set; } = scheme;
    public string Host { get; set; } = host;
    public int Port { get; set; } = port;
    public string AbsolutePath { get { return $"{Scheme}://{Host}:{Port}"; } }
}

public class DatabaseResource(string Name) : Resource(Name);
public class DatabaseServer(string Name, int Port) : Service(Name, Port)
{
    public DatabaseResource Database { get; set; } = new DatabaseResource($"{Name.ToLower()}-database");
}

/// <summary>
/// https://c4model.com/abstractions/software-system
/// </summary>
public class SystemResource(string Name, int Port = 2000) : Service(Name, Port)
{
    public DatabaseServer DatabaseServer { get; set; } = new DatabaseServer($"{Name.ToLower()}-dbserver", Port + 1);
    public Service MainService { get; set; } = new Service($"{Name.ToLower()}-mainservice", Port + 2);
    public Service ObservabilityService { get; set; } = new Service($"{Name.ToLower()}-observabilityservice", Port + 3);
}

// Structurize C4 Pattern https://docs.structurizr.com/dsl
// Backstage Pattern https://backstage.io/docs/features/software-catalog/system-model/
