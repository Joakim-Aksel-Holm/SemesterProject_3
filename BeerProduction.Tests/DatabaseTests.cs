using BeerProduction.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace BeerProduction.Tests;

public class DatabaseTests
{
    private const string TestConnectionString = "Host=testhost;Username=testuser;Password=testpass;Database=testdb";
    private readonly Mock<IConfiguration> _mockConfiguration = new();
    
    private void SetupConfiguration(string? connectionString)
    {
        // Mock the GetConnectionString extension method behavior
        var mockSection = new Mock<IConfigurationSection>();
        mockSection.Setup(s => s["Default"]).Returns(connectionString);
        
        _mockConfiguration
            .Setup(c => c.GetSection("ConnectionStrings"))
            .Returns(mockSection.Object);
    }
    
    [Fact]
    public void Constructor_ShouldSucceed_WhenConnectionStringIsPresent()
    {
        SetupConfiguration(TestConnectionString);

        // The constructor should execute without throwing an exception
        var connectionService = new DatabaseConnection(_mockConfiguration.Object);
        
        // We can't directly check the private field
        Assert.NotNull(connectionService);
    }
    
    [Fact]
    public void Constructor_ShouldThrowException_WhenConnectionStringIsMissing()
    {
        SetupConfiguration(null!); // Simulate that GetConnectionString returns null

        // The constructor is expected to throw InvalidOperationException
        var exception = Assert.Throws<InvalidOperationException>(
            () => new DatabaseConnection(_mockConfiguration.Object)
        );

        // Further assert on the exception message
        Assert.Contains("Missing ConnectionStrings:Default", exception.Message);
    }
}