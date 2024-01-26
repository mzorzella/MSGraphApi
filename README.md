## How to run the application

First, download and install the [.NET SDK](https://dotnet.microsoft.com/en-us/download/dotnet) on your computer.

Download the code from this repository:

```
https://github.com/mzorzella/MSGraphApi
```

Add your client secret to the .NET Secret Manager. In your command-line interface, change the directory to the location of `MSGraphApi.csproj` and run the following commands, replacing <client-secret> with your client secret.

```
cd MSGraphApi/MSGraphApi
dotnet user-secrets init
dotnet user-secrets set settings:clientSecret <your-client-secret>
```

Run the application

```
 dotnet run
```

## Key assumptions and considerations for the implementation

### High level overview:

I interpreted this assignment as a starting point for a system that allows the download of tenant's data living inside MS Graph, into another storage system.

Therefore, I wanted to organize this code closer to how I'd build it for a prod environment, rather then just a technical exercise.

Furthermore, it has been a couple of years since when I worked with C# last time, I wanted to brush up by C# knowledge a bit and have some fun with my favorite programming language. So, Please forgive if I'm not using the latest cool libraries in today's C# ecosystem :-).

### Key considerations

#### Strategy Pattern

The implementation leverages the strategy pattern to allow the system to be easily extensible as new download strategies are required.

The system currently implements two strategies for downloading Groups and Users (users is not currently enabled with the current Graph api creds).

The core strategy functionality is implemented inside the `MSGraphApi.Downloader` library. The class `GraphDownloader` implements the execution context, The `OperationStrategies` folder contains the concrete strategies implementation. I also encapsulated part of the implementation of the concrete strategies inside `BaseOperationStrategy` class (Template Method Pattern).

#### Dependency Injection

All DI system is configured in `Program.cs`. The system uses services registered in the DI container of type `IOperationStrategy` to dynamically defined the operations available via the CLI of this app.

#### Pagination

To support large data downloads, the system uses pagination to fetch and then store items in batches. The pagination logic is orchestrated by the `GraphDownloader` class.

#### Unit testing

All core logic of the application was test.

#### Error handling

The system adds error handling in some core aspect of the implementation logic.
