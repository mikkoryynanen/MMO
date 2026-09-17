
var builder = DistributedApplication.CreateBuilder(args);

var mysql = builder.AddMySql("mysql")
    .WithPhpMyAdmin()
    .WithDataVolume("mysql-data");

var worldDB = mysql.AddDatabase("worlddb");
var accountDB = mysql.AddDatabase("accountdb");

var migrations = builder.AddProject<Projects.MMO_Database_Migrations>("db-migrations")
    .WithReference(worldDB)
    .WithReference(accountDB)
    .WaitFor(worldDB)
    .WaitFor(accountDB);

var worldServer = builder.AddProject<Projects.MMO_WorldServer>("world-server")
    // TODO Also move to their own class
    .WithEndpoint(24000, 24000, name: "tcp", isProxied: false)
    // .WithEnvironment($"Network:Host", IPAddress.Any.ToString())
    // .WithEnvironment($"Network:Port", "24000")
     
    // TODO Could move to it's own builder class in the future to clean this up
    .WithReference(worldDB)
    .WaitFor(worldDB)
    .WaitForCompletion(migrations);

// builder.AddProject<Projects.MMO_DummyClient>("dummy-client")
//     .WithReplicas(1)
//     .WithReference(worldServer)
//     .WaitFor(worldServer);


var authAPI = builder.AddProject<Projects.MMO_Auth>("auth-api")
    .WithHttpHealthCheck("/health")
    .WithReference(accountDB)
    .WaitFor(accountDB)
    .WaitForCompletion(migrations);

// TODO Only make sure developers have access to this
var devAPI = builder.AddProject<Projects.MMO_Dev_DevAPI>("dev-api")
    // .WithHttpHealthCheck("/health")
    .WithReference(worldDB)
    .WaitFor(worldDB)
    .WaitForCompletion(migrations);

var client = builder.AddProject<Projects.MMO_Client>("client")
    .WithReference(worldServer)
    .WithReference(authAPI)
    .WaitFor(worldServer)
    .WithReplicas(2);

await builder.Build().RunAsync();