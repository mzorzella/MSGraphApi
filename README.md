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

I interpreted this assignment as a starting point for the creation of a system that downloads tenant's data from a MS Graph Api into an external storage system.

The code is organized similarly to how it would have been written for a prod environment, rather then just a technical exercise.

Furthermore, it has been a couple of years since when I worked with C# last time, I wanted to brush up my C# knowledge and have some fun with my favorite programming language. So, Please forgive if I'm not using the latest cool libraries in today's C# ecosystem :-).

### Key considerations

#### Strategy Pattern

The implementation leverages the strategy pattern to allow the system to be easily extended as new download strategies are needed.

The system currently provides two strategies for downloading Groups and Users (users is not currently allowed with current Graph Api creds).

The core functionality (context) is implemented inside the `MSGraphApi.Downloader` library and class `GraphDownloader`. The `OperationStrategies` folder contains the concrete strategy implementations. Part of the strategy logic is also implemented insde the class `BaseOperationStrategy` as an example of Template Method Pattern.

#### Dependency Injection

All DI system is configured in `Program.cs`. The system uses services registered in the DI container of type `IOperationStrategy` to dynamically define the operations available via the CLI selection of this app.

#### Pagination

To support large data downloads, the system uses pagination to fetch and then store items in batches. The pagination logic is orchestrated by the `GraphDownloader` class.

#### Unit testing

All core logic of the application was test. Some tests in certain classes were omitted.

#### Error handling

The system implments error handling in some core aspect of the logic.
