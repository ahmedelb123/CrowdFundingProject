using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Threading.Tasks;

[TestClass]
public class UserHandlerTests
{
    private Mock<AppDbContext> _mockDbContext;
    private Mock<IConfiguration> _mockConfiguration;
    private UserHandler _userHandler;

    [TestInitialize]
    public void Setup()
    {
        _mockDbContext = new Mock<AppDbContext>();
        _mockConfiguration = new Mock<IConfiguration>();
        
        // Setup configuration for JWT
        _mockConfiguration.Setup(c => c["Jwt:SecretKey"]).Returns("your-test-secret-key");
        _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
        _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("test-audience");

        _userHandler = new UserHandler(_mockDbContext.Object, _mockConfiguration.Object);
    }

    [TestMethod]
    public async Task CreateUser_NewEmail_ReturnsSuccess()
    {
        // Arrange
        var name = "Test";
        var surname = "User";
        var email = "test@example.com";
        var password = "password123";

        _mockDbContext.Setup(db => db.Users.AnyAsync(
            It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>()))
            .ReturnsAsync(false);

        // Act
        var result = await _userHandler.CreateUser(name, surname, email, password);

        // Assert
        Assert.IsTrue(result.Status);
        Assert.AreEqual("The account is created successfully", result.Message);
    }

    [TestMethod]
    public async Task CreateUser_ExistingEmail_ReturnsFail()
    {
        // Arrange
        var name = "Test";
        var surname = "User";
        var email = "existing@example.com";
        var password = "password123";

        _mockDbContext.Setup(db => db.Users.AnyAsync(
            It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>()))
            .ReturnsAsync(true);

        // Act
        var result = await _userHandler.CreateUser(name, surname, email, password);

        // Assert
        Assert.IsFalse(result.Status);
        Assert.AreEqual("There is already an account with this email", result.Message);
    }

    [TestMethod]
    public async Task Login_ValidCredentials_ReturnsSuccessWithToken()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password123";
        var testUser = new User("Test", "User", email, password);

        _mockDbContext.Setup(db => db.Users.FirstOrDefaultAsync(
            It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>()))
            .ReturnsAsync(testUser);

        // Act
        var result = await _userHandler.Login(email, password);

        // Assert
        Assert.IsTrue(result.Status);
        Assert.AreEqual("You logged in successfully", result.Message);
        Assert.IsNotNull(result.Token);
    }

    [TestMethod]
    public async Task Login_InvalidEmail_ReturnsFail()
    {
        // Arrange
        var email = "nonexistent@example.com";
        var password = "password123";

        _mockDbContext.Setup(db => db.Users.FirstOrDefaultAsync(
            It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>()))
            .ReturnsAsync((User)null);

        // Act
        var result = await _userHandler.Login(email, password);

        // Assert
        Assert.IsFalse(result.Status);
        Assert.AreEqual("This account doesn't exist", result.Message);
    }

    [TestMethod]
    public async Task Login_InvalidPassword_ReturnsFail()
    {
        // Arrange
        var email = "test@example.com";
        var password = "wrongpassword";
        var testUser = new User("Test", "User", email, "correctpassword");

        _mockDbContext.Setup(db => db.Users.FirstOrDefaultAsync(
            It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>()))
            .ReturnsAsync(testUser);

        // Act
        var result = await _userHandler.Login(email, password);

        // Assert
        Assert.IsFalse(result.Status);
        Assert.AreEqual("Your email or password is incorrect", result.Message);
    }
}
