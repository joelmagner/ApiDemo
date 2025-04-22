using System.Net.Http.Headers;
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
    public ClaimsIdentityOptions Claims { get; } = new();
    public MiniGramMemoryDb Db { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var mockAppSettings = new Mock<IOptions<AppSettings>>();
        mockAppSettings.SetupGet(x => x.Value).Returns(new AppSettings());

        var configPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
        builder.UseConfiguration(new ConfigurationBuilder().AddJsonFile(configPath).Build());

        builder.ConfigureServices(services =>
        {
            var optionsConfigs = services
                .Where(r => r.ServiceType.IsGenericType &&
                            r.ServiceType.GetGenericTypeDefinition() == typeof(IDbContextOptionsConfiguration<>))
                .ToArray();
            foreach (var config in optionsConfigs)
                services.Remove(config);

            var dbContext = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<MiniGramContext>));
            if (dbContext is not null)
                services.Remove(dbContext);

            services.AddDbContext<MiniGramContext>(o => o.UseSqlite(Db.Connection));
            services.AddSingleton(mockAppSettings.Object);
            services.AddSingleton(Claims);


            var provider = services.BuildServiceProvider();
            Db.InitializeDatabase(provider);
        });

        builder.ConfigureTestServices(services =>
        {
            // TODO: Add test auth setup here
        });

        builder.UseEnvironment("Development");
    }
}

public class MiniGramMemoryDb
{
    const string ConnectionString = "DataSource=:memory:";

    public MiniGramMemoryDb()
    {
        Connection = new SqliteConnection(ConnectionString);
        Connection.Open();
    }

    public SqliteConnection Connection { get; }
    public MiniGramContext Context { get; private set; }

    public void InitializeDatabase(IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetRequiredService<MiniGramContext>();

        dbContext.Database.OpenConnection();
        dbContext.Database.ExecuteSqlRaw("PRAGMA foreign_keys = ON;");
        dbContext.Database.EnsureCreated();

        Context = dbContext;

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
            Contents = [1, 2, 3, 4]
        };

        dbContext.Users.Add(user);
        dbContext.Photos.Add(photo);
        dbContext.SaveChanges();
    }
}

public static class WebApplicationFactoryExtensions
{
    public static T CreateServiceClient<T>(this WebApplicationFactory<Program> factory,
        params DelegatingHandler[] handlers)
    {
        var client = factory.CreateDefaultClient(handlers);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("TestScheme");
        return (T)Activator.CreateInstance(typeof(T), client)!;
    }
}
