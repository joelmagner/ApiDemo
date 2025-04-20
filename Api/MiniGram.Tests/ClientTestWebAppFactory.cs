using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MiniGram.Api;
using MiniGram.Api.Configuration;
using MiniGram.Api.Storage;
using MiniGram.Api.Storage.Entities;
using Moq;

namespace MiniGram.Tests;

public class ClientTestWebAppFactory : WebApplicationFactory<Program>
{
    readonly ClaimsIdentityOptions Claims = new();
    readonly MiniGramMemoryDb Db = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var options = new Mock<IOptions<AppSettings>>();
        options
            .SetupGet(x => x.Value)
            .Returns(new AppSettings());
        
        var projectDir = Directory.GetCurrentDirectory();
        var configPath = Path.Combine(projectDir, "appsettings.json");

        builder.UseConfiguration(new ConfigurationBuilder().AddJsonFile(configPath).Build());

        builder
            .ConfigureServices(services =>
            {
                services.AddSingleton(Claims);
                var optionsConfig = services.Where(r => r.ServiceType.IsGenericType && r.ServiceType.GetGenericTypeDefinition() == typeof(IDbContextOptionsConfiguration<>)).ToArray();
                foreach (var option in optionsConfig)
                {
                    services.Remove(option);
                }

                var dbContext = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<MiniGramContext>));
                services.Remove(dbContext);

                services.AddDbContext<MiniGramContext>(o =>
                {
                    o.UseSqlite(Db.Connection);
                });
                services.AddSingleton(options.Object);
                var serviceProvider = services.BuildServiceProvider();
                Db.InitializeDatabase(serviceProvider);

            })
            .ConfigureTestServices(services =>
            {
                //TODO: add actual test auth here?
            });

        builder.UseEnvironment("Development");
    }

   
}

public class MiniGramMemoryDb
{
    const string ConnectionString = "DataSource=:memory:";
    public SqliteConnection Connection { get; }

    public MiniGramMemoryDb()
    {
        Connection = new SqliteConnection(ConnectionString);
        Connection.Open();
    }

    public void InitializeDatabase(IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetRequiredService<MiniGramContext>();
        
        dbContext.Database.OpenConnection();
        dbContext.Database.ExecuteSqlRaw("PRAGMA foreign_keys = ON;"); // needed - don't remove.
        
        dbContext.Database.EnsureCreated();
        
        // Seed some initial data
        var user = new User
        {
            UserId = TestHelper.GetTestUserId,
            Username = "testuser",
            Email = "testuser@example.com",
            FirstName = "tester",
            LastName = "testsson"
        };
        
        var photo = new Photo
        {
            Id = TestHelper.GetTestPhotoId,
            UserId = TestHelper.GetTestUserId,
            Description = "test",
            ContentType = "image/jpeg",
            Contents = [1, 2, 3, 4],
        };
        
        dbContext.Users.Add(user);
        dbContext.Photos.Add(photo);
        dbContext.SaveChanges();
    }
}

public static class WebApplicationFactoryExtensions
{
    public static T CreateServiceClient<T>(this WebApplicationFactory<Program> factory, params DelegatingHandler[] delegatingHandlers)
    {
        var httpClient = factory.CreateDefaultClient(delegatingHandlers);
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("TestScheme");
        return (T)Activator.CreateInstance(typeof(T), httpClient);
    }
}
