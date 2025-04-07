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
    public class DonationControllerTests
    {
        private Mock<IDonationHandler> _mockDonationHandler;
        private DonationController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockDonationHandler = new Mock<IDonationHandler>();
            _controller = new DonationController(_mockDonationHandler.Object);
        }

        [TestMethod]
        public async Task Post_ValidDonation_ReturnsOkResult()
        {
            // Arrange
            var donation = new Donation { Amount = 100, ProjectId = 1 };
            _mockDonationHandler.Setup(h => h.MakeDonation(It.IsAny<Donation>()))
                .ReturnsAsync(new Result { Status = true, Message = "Donation successful" });

            // Act
            var result = await _controller.Post(donation);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            var response = okResult.Value as Result;
            Assert.IsTrue(response.Status);
        }

        [TestMethod]
        public async Task Get_ValidProjectId_ReturnsOkResult()
        {
            // Arrange
            var projectId = 1;
            var donations = new List<Donation> 
            { 
                new Donation { Id = 1, Amount = 100 } 
            };
            _mockDonationHandler.Setup(h => h.GetDonations(projectId))
                .ReturnsAsync(donations);

            // Act
            var result = await _controller.Get(projectId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            var returnedDonations = okResult.Value as List<Donation>;
            Assert.AreEqual(1, returnedDonations.Count);
        }

        [TestMethod]
        public async Task GetUserDonations_ValidUserId_ReturnsOkResult()
        {
            // Arrange
            var userId = 1;
            var donations = new List<Donation> 
            { 
                new Donation { Id = 1, Amount = 100, UserId = userId } 
            };
            _mockDonationHandler.Setup(h => h.GetUserDonations(userId))
                .ReturnsAsync(donations);

            // Act
            var result = await _controller.GetUserDonations(userId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            var returnedDonations = okResult.Value as List<Donation>;
            Assert.AreEqual(1, returnedDonations.Count);
        }

        [TestMethod]
        public async Task Post_InvalidDonation_ReturnsBadRequest()
        {
            // Arrange
            var donation = new Donation { Amount = -100, ProjectId = 1 };
            _mockDonationHandler.Setup(h => h.MakeDonation(It.IsAny<Donation>()))
                .ReturnsAsync(new Result { Status = false, Message = "Invalid donation amount" });

            // Act
            var result = await _controller.Post(donation);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
    }
}
