# Iris.IdentityServer4.MongoDB

Iris.IdentityServer4.MongoDB is an IdentityServer4 storage adapter that one can use as an alternative to the EntityFrameWork stores which requires one to use a full RDMS like SQL Server, Postgres or MySQL

## Installation

On the terminal install the Nuget package using the following command

```
dotnet add package Iris.IdentityServer4.MongoDB
```

or using the Nuget Package Manager with

```
Install-Package Iris.IdentityServer4.MongoDB
```

## Setup

To setup your project to use this adapter, in your `ConfigureServices` method when you are setting up Identity Server, inject the required services using the `AddMongoDbStores` method.

```
var builder = services.AddIdentityServer()
    .AddMongoDbStores(options =>
    {
        options.ConnectionString = "mongodb connection string here"; // MongoDB connection string excluding the database
        options.Database = "mongodb database here"; // Database default is identity
        options.EnableTokenCleanup = true; // Default is false
        options.TokenCleanupInterval = 3600; // This is in minutes default is 3600 (1 hour)
    });
```

when in development add the following for Key Signing Credentials

```
    builder.AddDeveloperSigningCredential();
```

In Production setup Key Signing as described in the Identity Server documentation.

## Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License

[APACHE](https://github.com/stewartm83/Iris.IdentityServer4.MongoDB/blob/main/LICENSE)
