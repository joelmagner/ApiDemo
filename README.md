# ApiDemo

Example Api to demonstrate EF, .NET, Auth etc. TBD... what exactly this is gonna be. Probably a bad file upload clone

#### things not implemented as of now:

- Refresh token
- Auth not enforced as it should everywhere in order for me to create some tests.
- https://github.com/scalar/scalar/issues/3701 (Authorization header is acting strange, hard to verify)

#### what is working?:

- login flow
- file upload
- integration tests
- Authorize\* (it is working, disabled becuase of tests more important)
- Client package, probably needs a rework with the types

# Setup

Start your SQL-server.

(on Mac)

```
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=MyPass@word" -e "MSSQL_PID=Developer" -e "MSSQL_USER=SA" -p 1433:1433 -d --name=sqldb mcr.microsoft.com/azure-sql-edge
```

Navigate into the **MiniGram.Api** folder.

`dotnet ef database update`

Then start the project and navigate to `https://localhost:5259/scalar/` and make some requests.

or start the frontend with:

`ng serve` and navigate to http://localhost:4200 and create an account.

GIF demo:

![](./demo.gif)
