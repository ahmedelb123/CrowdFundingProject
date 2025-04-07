using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using CrowdFundingProject.Controllers;
using CrowdFundingProject.Handlers;
using CrowdFundingProject.Models;
using CrowdFundingProject.Dto;

namespace CrowdFundingProject.Tests.Controllers
{
    [TestClass]
    public class UserControllerTests
    {
        private Mock<IUserHandler> _mockUserHandler;
        private UserController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockUserHandler = new Mock<IUserHandler>();
            _controller = new UserController(_mockUserHandler.Object);
        }

        [TestMethod]
        public async Task Register_ValidUser_ReturnsOkResult()
        {
            // Arrange
            var userDto = new RegisterDto 
            { 
                Name = "Test",
                Surname = "User",
                Email = "test@example.com",
                Password = "Password123!"
            };
            _mockUserHandler.Setup(h => h.CreateUser(
                userDto.Name, userDto.Surname, userDto.Email, userDto.Password))
                .ReturnsAsync(new Result { Status = true, Message = "User registered successfully" });

            // Act
            var result = await _controller.Register(userDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            var response = okResult.Value as Result;
            Assert.IsTrue(response.Status);
        }

        [TestMethod]
        public async Task Login_ValidCredentials_ReturnsOkResult()
        {
            // Arrange
            var loginDto = new LoginDto 
            { 
                Email = "test@example.com",
                Password = "Password123!"
            };
            _mockUserHandler.Setup(h => h.Login(loginDto.Email, loginDto.Password))
                .ReturnsAsync(new Result 
                { 
                    Status = true, 
                    Message = "Login successful",
                    Token = "test-jwt-token"
                });

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            var response = okResult.Value as Result;
            Assert.IsTrue(response.Status);
            Assert.IsNotNull(response.Token);
        }

        [TestMethod]
        public async Task Login_InvalidCredentials_ReturnsBadRequest()
        {
            // Arrange
            var loginDto = new LoginDto 
            { 
                Email = "wrong@example.com",
                Password = "WrongPassword"
            };
            _mockUserHandler.Setup(h => h.Login(loginDto.Email, loginDto.Password))
                .ReturnsAsync(new Result 
                { 
                    Status = false, 
                    Message = "Invalid credentials"
                });

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Register_ExistingEmail_ReturnsBadRequest()
        {
            // Arrange
            var userDto = new RegisterDto 
            { 
                Email = "existing@example.com",
                Password = "Password123!"
            };
            _mockUserHandler.Setup(h => h.CreateUser(
                It.IsAny<string>(), It.IsAny<string>(), userDto.Email, userDto.Password))
                .ReturnsAsync(new Result { Status = false, Message = "Email already exists" });

            // Act
            var result = await _controller.Register(userDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Register_InvalidPassword_ReturnsBadRequest()
        {
            // Arrange
            var userDto = new RegisterDto 
            { 
                Name = "Test",
                Surname = "User",
                Email = "test@example.com",
                Password = "weak" // Too short password
            };
            _mockUserHandler.Setup(h => h.CreateUser(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new Result { Status = false, Message = "Password must be at least 8 characters" });

            // Act
            var result = await _controller.Register(userDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Login_EmptyCredentials_ReturnsBadRequest()
        {
            // Arrange
            var loginDto = new LoginDto 
            { 
                Email = "",
                Password = ""
            };

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task GetUserProfile_ValidUser_ReturnsOkResult()
        {
            // Arrange
            var userId = 1;
            var user = new User 
            { 
                Id = userId,
                Name = "Test",
                Email = "test@example.com"
            };
            _mockUserHandler.Setup(h => h.GetUserProfile(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.GetProfile(userId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            var returnedUser = okResult.Value as User;
            Assert.AreEqual(userId, returnedUser.Id);
        }
    }
}
