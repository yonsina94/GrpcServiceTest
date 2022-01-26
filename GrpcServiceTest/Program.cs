using GrpcServiceTest.Database;
using GrpcServiceTest.Database.Repositories.Base;
using GrpcServiceTest.Database.Repositories.Constructor;
using GrpcServiceTest.Services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

var connBuilder = new Npgsql.NpgsqlConnectionStringBuilder();

connBuilder.Host = "localhost";
connBuilder.Port = 5432;
connBuilder.Username = "admin";
connBuilder.Password = "admin";

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddNpgsql<DatabaseContext>(connBuilder.ConnectionString);
builder.Services.AddHealthChecks();
builder.Services.AddTransient<IRepositoryConstructor>(factory=>new RepositoryConstructor(factory.GetService<DatabaseContext>()));
var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<PersonService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
