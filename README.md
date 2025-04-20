# ApiDemo

Example Api to demonstrate EF, .NET, User management etc. TBD...

# Setup

Start your SQL-server.

(on Mac)

```
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=MyPass@word" -e "MSSQL_PID=Developer" -e "MSSQL_USER=SA" -p 1433:1433 -d --name=sql mcr.microsoft.com/azure-sql-edge
```

Navigate into the ExampleApi.Api folder.

`dotnet ef database update`

Then start the project and navigate to `http://localhost:5259/scalar/` and make some requests.
