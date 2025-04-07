using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using CrowdFundingProject.Controllers;
using CrowdFundingProject.Handlers;
using CrowdFundingProject.Models;

namespace CrowdFundingProject.Tests.Controllers
{
    [TestClass]
    public class PostControllerTests
    {
        private Mock<IPostHandler> _mockPostHandler;
        private PostController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockPostHandler = new Mock<IPostHandler>();
            _controller = new PostController(_mockPostHandler.Object);
        }

        [TestMethod]
        public async Task Post_ValidProject_ReturnsOkResult()
        {
            // Arrange
            var project = new Project 
            { 
                Title = "Test Project",
                Description = "Test Description",
                FundingGoal = 1000
            };
            _mockPostHandler.Setup(h => h.CreatePost(It.IsAny<Project>()))
                .ReturnsAsync(new Result { Status = true, Message = "Project created successfully" });

            // Act
            var result = await _controller.Post(project);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            var response = okResult.Value as Result;
            Assert.IsTrue(response.Status);
        }

        [TestMethod]
        public async Task Get_ExistingProject_ReturnsOkResult()
        {
            // Arrange
            var projectId = 1;
            var project = new Project { Id = projectId, Title = "Test Project" };
            _mockPostHandler.Setup(h => h.GetPost(projectId))
                .ReturnsAsync(project);

            // Act
            var result = await _controller.Get(projectId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            var returnedProject = okResult.Value as Project;
            Assert.AreEqual(projectId, returnedProject.Id);
        }

        [TestMethod]
        public async Task GetAll_ReturnsListOfProjects()
        {
            // Arrange
            var projects = new List<Project>
            {
                new Project { Id = 1, Title = "Project 1" },
                new Project { Id = 2, Title = "Project 2" }
            };
            _mockPostHandler.Setup(h => h.GetAllPosts())
                .ReturnsAsync(projects);

            // Act
            var result = await _controller.GetAll();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            var returnedProjects = okResult.Value as List<Project>;
            Assert.AreEqual(2, returnedProjects.Count);
        }

        [TestMethod]
        public async Task Delete_ExistingProject_ReturnsOkResult()
        {
            // Arrange
            var projectId = 1;
            var userId = 1;
            _mockPostHandler.Setup(h => h.DeletePost(projectId, userId))
                .ReturnsAsync(new Result { Status = true, Message = "Project deleted successfully" });

            // Act
            var result = await _controller.Delete(projectId, userId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Post_InvalidProject_ReturnsBadRequest()
        {
            // Arrange
            var project = new Project 
            { 
                // Missing Title
                Description = "Test Description",
                FundingGoal = -1000 // Invalid goal
            };
            _mockPostHandler.Setup(h => h.CreatePost(It.IsAny<Project>()))
                .ReturnsAsync(new Result { Status = false, Message = "Invalid project data" });

            // Act
            var result = await _controller.Post(project);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Get_NonExistingProject_ReturnsNotFound()
        {
            // Arrange
            var projectId = 999;
            _mockPostHandler.Setup(h => h.GetPost(projectId))
                .ReturnsAsync((Project)null);

            // Act
            var result = await _controller.Get(projectId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Update_ValidProject_ReturnsOkResult()
        {
            // Arrange
            var projectId = 1;
            var project = new Project 
            { 
                Id = projectId,
                Title = "Updated Title",
                Description = "Updated Description"
            };
            _mockPostHandler.Setup(h => h.UpdatePost(It.IsAny<Project>()))
                .ReturnsAsync(new Result { Status = true, Message = "Project updated successfully" });

            // Act
            var result = await _controller.Put(projectId, project);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }
    }
}
