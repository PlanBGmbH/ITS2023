using ITSAPI.Interfaces;
using ITSAPI.Services;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var CosmosDBEndpoint = builder.Configuration["CosmosDB:Endpoint"];
var CosmosDBKey = builder.Configuration["CosmosDB:Key"];
var CosmosDBDatabaseName = builder.Configuration["CosmosDB:DatabaseName"];
var CosmosDBBookingsContainer = builder.Configuration["CosmosDB:BookingsContainer"];
var CosmosDBProjectsContainer = builder.Configuration["CosmosDB:ProjectsContainer"];
var CosmosDBResourcesContainer = builder.Configuration["CosmosDB:ResourcesContainer"];

builder.Services.AddSingleton<IBookingsService>(InitializeCosmosBookingsClientInstanceAsync(CosmosDBEndpoint, CosmosDBKey, CosmosDBDatabaseName, CosmosDBBookingsContainer).GetAwaiter().GetResult());
builder.Services.AddSingleton<IProjectsService>(InitializeCosmosProjectsClientInstanceAsync(CosmosDBEndpoint, CosmosDBKey, CosmosDBDatabaseName, CosmosDBProjectsContainer).GetAwaiter().GetResult());
builder.Services.AddSingleton<IResourcesService>(InitializeCosmosResourcesClientInstanceAsync(CosmosDBEndpoint, CosmosDBKey, CosmosDBDatabaseName, CosmosDBResourcesContainer).GetAwaiter().GetResult());


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ITSAPI V1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>
/// Creates, if not existing, the CosmosDB Database and Bookings Container
/// </summary>
/// <returns></returns>
static async Task<BookingsService> InitializeCosmosBookingsClientInstanceAsync(string CosmosDBEndpoint, string CosmosDBKey, string CosmosDBDatabaseName, string CosmosDBBookingsContainer)
{
    CosmosClient client = new CosmosClient(CosmosDBEndpoint, CosmosDBKey);
    BookingsService bookingsService = new BookingsService(client, CosmosDBDatabaseName, CosmosDBBookingsContainer);
    DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(CosmosDBDatabaseName);
    await database.Database.CreateContainerIfNotExistsAsync(CosmosDBBookingsContainer, "/id");
    return bookingsService;
}

// <summary>
/// Creates, if not existing, the CosmosDB Database and Projects Container
/// </summary>
/// <returns></returns>
static async Task<ProjectsService> InitializeCosmosProjectsClientInstanceAsync(string CosmosDBEndpoint, string CosmosDBKey, string CosmosDBDatabaseName, string CosmosDBProjectsContainer)
{
    CosmosClient client = new CosmosClient(CosmosDBEndpoint, CosmosDBKey);
    ProjectsService projectsService = new ProjectsService(client, CosmosDBDatabaseName, CosmosDBProjectsContainer);
    DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(CosmosDBDatabaseName);
    await database.Database.CreateContainerIfNotExistsAsync(CosmosDBProjectsContainer, "/id");
    return projectsService;
}

// <summary>
/// Creates, if not existing, the CosmosDB Database and Resources Container
/// </summary>
/// <returns></returns>
static async Task<ResourcesService> InitializeCosmosResourcesClientInstanceAsync(string CosmosDBEndpoint, string CosmosDBKey, string CosmosDBDatabaseName, string CosmosDBResourcesContainer)
{
    CosmosClient client = new CosmosClient(CosmosDBEndpoint, CosmosDBKey);
    ResourcesService resourcesService = new ResourcesService(client, CosmosDBDatabaseName, CosmosDBResourcesContainer);
    DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(CosmosDBDatabaseName);
    await database.Database.CreateContainerIfNotExistsAsync(CosmosDBResourcesContainer, "/id");
    return resourcesService;
}