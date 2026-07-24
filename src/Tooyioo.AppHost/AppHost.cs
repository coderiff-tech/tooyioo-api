var builder = DistributedApplication.CreateBuilder(args);

var kurrentDb =
    builder.AddKurrentDB("kurrent", 2113)
        .WithImage("kurrentplatform/kurrentdb", "26.1.1")
        .WithLifetime(ContainerLifetime.Session)
        .WithEnvironment("KURRENTDB_CLUSTER_SIZE", "1")
        .WithEnvironment("KURRENTDB_RUN_PROJECTIONS", "All")
        .WithEnvironment("KURRENTDB_START_STANDARD_PROJECTIONS", "true")
        .WithEnvironment("KURRENTDB_NODE_PORT", "2113")
        .WithEnvironment("KURRENTDB_INSECURE", "true")
        .WithEnvironment("KURRENTDB_ENABLE_ATOM_PUB_OVER_HTTP", "true");

var mongoDb =
        builder.AddMongoDB("mongo", 27017)
            .WithImage("mongo", "8.3")
            .WithEnvironment("GLIBC_TUNABLES", "libc.cpu.hwcaps=-SHSTK")
        .WithLifetime(ContainerLifetime.Session)
        .WithMongoExpress(cfg => cfg.WithHostPort(27018));

builder.AddProject<Projects.Tooyioo_Api>("tooyioo-api")
    .WithReference(kurrentDb, "KurrentDb")
    .WithReference(mongoDb, "MongoDb")
    .WaitFor(kurrentDb)
    .WaitFor(mongoDb);

builder.Build().Run();
