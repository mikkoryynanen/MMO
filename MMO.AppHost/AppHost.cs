using System.Net;
using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var mysql = builder.AddMySql("mysql")
    .WithPhpMyAdmin()
    .WithDataVolume("mysql-data");

var worldDB = mysql.AddDatabase("worlddb");

var worldServer = builder.AddProject<Projects.MMO_WorldServer>("world-server")
     // TODO Could move to it's own builder class in the future to clean this up
//     .WithReferenceRelationship(worldDB.Resource)
    // .WithEnvironment("Database:World:Provider", "MySql")
    // .WithEnvironment(c =>
    // {
    //     c.EnvironmentVariables[$"Database:World:ConnectionString"] =
    //         new ConnectionStringReference(worldDB.Resource, false);
    // })
    
    // TODO Also move to their own class
    .WithEndpoint(24000, 24000, name: "tcp", isProxied: false)
    .WithEnvironment($"Network:Host", IPAddress.Any.ToString())
    .WithEnvironment($"Network:Port", "24000")
    .WaitFor(worldDB);

builder.AddProject<Projects.MMO_DummyClient>("dummy-client")
    .WithReplicas(2)
    .WithReference(worldServer)
    .WaitFor(worldServer);

await builder.Build().RunAsync();