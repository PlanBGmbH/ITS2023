# Iberian Technology Summit - Backend
Introduction to Fusion Teams Development for ITS 2023

The components to be used in the Back-End are the following:

   1. [APIM](https://azure.microsoft.com/en-us/services/api-management/): Enables you to deploy API gateways in parallel with APIs hosted in Azure, in other clouds and in the on-premises environment to optimize API traffic flow. Meet security and compliance requirements with a unified management experience and complete visibility into all internal and external APIs.
   2. [App Service](https://azure.microsoft.com/en-us/services/app-service/): Compile, deploy and scale web applications and APIs automatically on your terms. In our case, we will create, publish and host a web API with .NET Core.
   3. [Cosmos DB](https://azure.microsoft.com/en-us/services/cosmos-db/): Azure Cosmos DB is a fully managed serverless NoSQL database for high-performance applications of any size or scale.

We start with the creation of the Backend solution. To do this we open Visual Studio 2022 and create a new project, of type ASP.NET Core Web API:

<img width="514" alt="image" src="https://user-images.githubusercontent.com/18615795/227864928-b462fb8c-d55d-403c-955f-59a2688f0881.png">

We give it a name and a path where to save the solution:

<img width="515" alt="image" src="https://user-images.githubusercontent.com/18615795/227864251-6820bd21-06fb-4a59-963f-785c999dbb53.png">

We select the version of .NET Core to use (7.0), enabling OpenAPI support:

<img width="514" alt="image" src="https://user-images.githubusercontent.com/18615795/227865219-4b81be4e-cd6d-48f6-8f40-c3cd28d0c69d.png">

Once the solution is created, we check and install the necessary Nuget Packages:

<img width="656" alt="image" src="https://user-images.githubusercontent.com/18615795/227865506-6384b566-fde6-492e-91e5-db304c707e66.png">

- Microsoft.AspNetCore.OpenApi: Allows interacting with the OpenAPI specifications of the connection points. The package acts as a link between the OpenAPI models defined in the package and the connection points defined in the APIs.
- Microsoft.Azure.Cosmos / Microsoft.EntityFramework.Cosmos: Libraries that allow us to connect and work with Azure Cosmos DB using the SQL API.
- Newtonsoft.Json: Allows us to serialize and deserialize objects used for communication with Cosmos DB.
- Swashbuckle.AspNetCore: Allows us to use Swagger to document our APIs.

We start by creating the three data models that will be persisted in the database. We create a folder in the solution named Models, where we create three classes:

1) Booking
  ```cs
    public class Booking
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "projectId")]
        public string ProjectId { get; set; }

        [JsonProperty(PropertyName = "resourceId")]
        public string ResourceId { get; set; }

        [JsonProperty(PropertyName = "date")]
        public DateTime Date { get; set; }

        [JsonProperty(PropertyName = "hours")]
        public double Hours { get; set; }
    }
  ```
2) Project
  ```cs
    public class Project
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "customer")]
        public string Customer { get; set; }

        [JsonProperty(PropertyName = "startDate")]
        public DateTime StartDate { get; set; }

        [JsonProperty(PropertyName = "createdBy")]
        public string CreatedBy { get; set; }        
    }
  ```
 3) Resource
  ```cs
    public class Resource
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "fullName")]
        public string FullName { get; set; }

        [JsonProperty(PropertyName = "mail")]
        public string Mail { get; set; }
    }
  ```

Next, we create the three interfaces of what will be our services, which for a first example can simply contain the CRUD operations (Create, Read, Update and Delete) of each of the entities. We create a folder named Interficies (Interfaces), and add them. For example, for Bookings, we create the IBookingsService.cs:

  ```cs
    public interface IBookingsService
    {
        Task<IEnumerable<Booking>> GetBookingsAsync(string query);

        Task AddBookingAsync(Booking booking);

        Task DeleteBookingAsync(string id);

        Task UpdateBookingAsync(Booking booking);
    }
  ```

The next step is to create the services that implement the interfaces. We create the Services folder, and create the three services. For example, for Bookings, we create the class BookingsService.cs:

  ```cs
    public class BookingsService: IBookingsService  <-- Implementa la interficie
    {
        private Container _container; <-- Container de Cosmos DB

        private static readonly JsonSerializer Serializer = new JsonSerializer();

        public BookingsService(CosmosClient dbClient, string databaseName, string containerName)
        {
            this._container = dbClient.GetContainer(databaseName, containerName); <- El constructor del servicio recibe el cliente de Cosmos DB y obtiene el container.
        }

        public async Task<IEnumerable<Booking>> GetBookingsAsync(string queryString) <- Obtiene un conjunto de reservas basadas en la querystring recibida.
        {
            var query = this._container.GetItemQueryIterator<Booking>(new QueryDefinition(queryString));
            List<Booking> results = new List<Booking>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }
            return results;
        }

        public async Task AddBookingAsync(Booking booking) <- Añade una nueva reserva
        {
            await this._container.CreateItemAsync<Booking>(booking, new PartitionKey(booking.Id));
        }
       
       //... Implementar el resto de métodos definidos en la interficie. La implementación completa está en la carpeta BizzSummitAPI del directorio raíz de este repo.
  ```
Once we have the services, we are going to implement the controllers, using the existing folder of the same name. We create a file for each controller. For example, we create the BookingsController.cs:

  ```cs
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly ILogger<BookingsController> _logger;

        private readonly IBookingsService _bookingsService;

        public BookingsController(IBookingsService bookingsService, ILogger<BookingsController> logger)
        {
            _bookingsService = bookingsService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            try
            {
                var family = await _bookingsService.GetBookingsAsync("SELECT * FROM c");
                return Ok(family);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Some error occured while retreiving data");
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddBooking(Booking booking)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _bookingsService.AddBookingAsync(booking);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Some error occured while inserting data");
            }
        }
        //... Implementar el resto de acciones para completar el CRUD. La implementación completa está en la carpeta BizzSummitAPI del directorio raíz de este repo.
  ```

We configure the services that depend on the project:

<img width="600" alt="image" src="https://user-images.githubusercontent.com/18615795/227866703-c00bb8c4-a814-488b-98c4-8fb8272a3c97.png">

In our Azure subscription, we create a Cosmos DB, preferably within a resource group specific to the project. We need to define the user secrets, which contain all the values needed to access the Cosmos DB.

First, we connect to the Cosmos DB using the wizard provided by the VS.

Second, we create the user secrets needed to connect from our solution:

<img width="337" alt="image" src="https://user-images.githubusercontent.com/18615795/227867418-53bc6f0f-7039-4ae0-a5f0-6b571d5c430b.png">

```cs
{
  "ConnectionStrings": {
    "CosmosDB:Endpoint": "https://itscosmosdb.documents.azure.com:443/",
    "CosmosDB:Key": "PrimaryKeyCosmosDB",
    "CosmosDB:DatabaseName": "ITS",
    "CosmosDB:BookingsContainer": "Bookings",
    "CosmosDB:ProjectsContainer": "Projects",
    "CosmosDB:ResourcesContainer": "Resources"
  }
}
```

The endpoint can be found directly in the overview of the Cosmos DB component:
 
 <img width="241" alt="image" src="https://user-images.githubusercontent.com/18615795/227871670-e41b0115-b874-4b27-9785-3635a4f6032c.png">

It is also inside Keys, where you have to use the Primary Key as Key in the solution:

 <img width="224" alt="image" src="https://user-images.githubusercontent.com/18615795/227871585-2a86e6f6-be93-4d09-a9b3-3ea638e164a5.png">

We create a database for the project within the Cosmos DB and configure, in the solution, the key DatabaseName.


Third, we edit the Program.cs file, adding as services (AddSingleton) through dependency injection the unique initialization of the Cosmos DB and the collections:

```cs
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var CosmosDBEndpoint = builder.Configuration.GetConnectionString("CosmosDB:Endpoint");
var CosmosDBKey = builder.Configuration.GetConnectionString("CosmosDB:Key");
var CosmosDBDatabaseName = builder.Configuration.GetConnectionString("CosmosDB:DatabaseName");
var CosmosDBBookingsContainer = builder.Configuration.GetConnectionString("CosmosDB:BookingsContainer");
var CosmosDBProjectsContainer = builder.Configuration.GetConnectionString("CosmosDB:ProjectsContainer");
var CosmosDBResourcesContainer = builder.Configuration.GetConnectionString("CosmosDB:ResourcesContainer");

builder.Services.AddSingleton<IBookingsService>(InitializeCosmosBookingsClientInstanceAsync(CosmosDBEndpoint, CosmosDBKey, CosmosDBDatabaseName, CosmosDBBookingsContainer).GetAwaiter().GetResult());
builder.Services.AddSingleton<IProjectsService>(InitializeCosmosProjectsClientInstanceAsync(CosmosDBEndpoint, CosmosDBKey, CosmosDBDatabaseName, CosmosDBProjectsContainer).GetAwaiter().GetResult());
builder.Services.AddSingleton<IResourcesService>(InitializeCosmosResourcesClientInstanceAsync(CosmosDBEndpoint, CosmosDBKey, CosmosDBDatabaseName, CosmosDBResourcesContainer).GetAwaiter().GetResult());

var app = builder.Build();
 ```
 
In each of the InitializeCosmosXXX methods, we make sure that the Cosmos DB and the container exists (otherwise we create it automatically) and we create the single instance of the service:
 
 ```cs
        private static async Task<BookingsService> InitializeCosmosBookingsClientInstanceAsync(string CosmosDBEndpoint, string CosmosDBKey, string CosmosDBDatabaseName, string CosmosDBBookingsContainer)
        {                               
            CosmosClient client = new CosmosClient(CosmosDBEndpoint, CosmosDBKey);
            BookingsService bookingsService = new BookingsService(client, CosmosDBDatabaseName, CosmosDBBookingsContainer);
            DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(CosmosDBDatabaseName);            
            await database.Database.CreateContainerIfNotExistsAsync(CosmosDBBookingsContainer, "/id");            
            return bookingsService;
        }
 ```
We test that the application works. We launch it locally (IIS Express) and check that Swagger is properly configured:
 
 <img width="739" alt="image" src="https://user-images.githubusercontent.com/18615795/227871893-e194ef97-1666-439d-8a0f-131fa38d12de.png">

The next step is to publish the application in Azure. To do this we create, within our subscription, an App Service Plan and an App Service:
 
 <img width="922" alt="image" src="https://user-images.githubusercontent.com/18615795/227872012-8faa093c-0dc3-4ef3-a912-4ce4111890af.png">
 
We also created an API Management component and a Blank API.
 
 <img width="869" alt="image" src="https://user-images.githubusercontent.com/18615795/227873213-0c8c3a5c-f799-4c1e-ba21-e83843826184.png">

In our solution, right click -> Publish:
 
 <img width="308" alt="image" src="https://user-images.githubusercontent.com/18615795/227872242-10b85b3e-ddf5-43e8-9e70-23d3bfd186ec.png">

We configure a new destination and follow the Wizard, selecting the Web App component created in Azure:
 
 ![image](https://user-images.githubusercontent.com/18615795/227872953-40e66944-ac43-4dd9-81ce-226e5bc0f792.png)
 
In the last step we select the API Management component and the Blank API we have created:

![image](https://user-images.githubusercontent.com/18615795/227874659-f3b01c31-9ec3-429d-91f1-762bddff49d7.png)
 
We publish the solution. Once completed, the URL of the webapp will open in Azure with the deployed solution:
 
 ![image](https://user-images.githubusercontent.com/18615795/184013829-f046e732-8156-4166-b06d-349d7e877fdd.png)
 
Finally, we also created a new component in Azure, API Management.
 
 ![image](https://user-images.githubusercontent.com/18615795/184012127-8139c744-6400-40e9-9916-0975fc554e7d.png)

We copy, from the web application, the link to the OpenAPI specification:
 
 ![image](https://user-images.githubusercontent.com/18615795/184014085-fda0f48f-e3b5-4788-a764-abdd395b3c1e.png)

In the API Management component, we create a new API from the OpenAPI definition:
 
 ![image](https://user-images.githubusercontent.com/18615795/184014278-1e477069-1bf9-4565-a770-c29be4f99a28.png)

Enter the link:
 
 ![image](https://user-images.githubusercontent.com/18615795/184014488-319a088b-117c-4c0e-bcf4-eb607c302d51.png)

We check that it has been imported correctly:
 
 ![image](https://user-images.githubusercontent.com/18615795/184014661-24a61d78-efdb-4132-b2c9-2a6a4d325f7a.png)

With this, we have the backend ready and we can move on to the next block, which is the creation of the Custom Connector and its use in Power Platform.
 
Needless to say, for reasons of time and ease of use within the Workshop, we are skipping absolutely necessary steps to be able to put the application in a production environment: Authentication / Authorization, Logging, etc. We are happy, if you are interested, to be contacted at any time to deepen any questions you may have about it.
