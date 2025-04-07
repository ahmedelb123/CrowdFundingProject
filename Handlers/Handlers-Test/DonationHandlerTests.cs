using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Threading.Tasks;
using CrowdFundingProject.Handlers;
using CrowdFundingProject.Models;
using CrowdFundingProject.Data;

namespace CrowdFundingProject.Tests.Handlers
{
    [TestClass]
    public class DonationHandlerTests
    {
        private Mock<AppDbContext> _mockDbContext;
        private DonationHandler _donationHandler;

        [TestInitialize]
        public void Setup()
        {
            _mockDbContext = new Mock<AppDbContext>();
            _donationHandler = new DonationHandler(_mockDbContext.Object);
        }

        [TestMethod]
        public async Task MakeDonation_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var donation = new Donation
            {
                Amount = 100,
                UserId = 1,
                ProjectId = 1
            };

            var project = new Project
            {
                Id = 1,
                FundingGoal = 1000,
                CurrentAmount = 0
            };

            _mockDbContext.Setup(db => db.Projects.FindAsync(donation.ProjectId))
                .ReturnsAsync(project);

            _mockDbContext.Setup(db => db.Donations.AddAsync(
                It.IsAny<Donation>(),
                It.IsAny<CancellationToken>()))
                .Returns(ValueTask.FromResult((Donation)null));

            // Act
            var result = await _donationHandler.MakeDonation(donation);

            // Assert
            Assert.IsTrue(result.Status);
            Assert.AreEqual("Donation successful", result.Message);
        }

        [TestMethod]
        public async Task MakeDonation_ProjectNotFound_ReturnsFail()
        {
            // Arrange
            var donation = new Donation
            {
                Amount = 100,
                UserId = 1,
                ProjectId = 999
            };

            _mockDbContext.Setup(db => db.Projects.FindAsync(donation.ProjectId))
                .ReturnsAsync((Project)null);

            // Act
            var result = await _donationHandler.MakeDonation(donation);

            // Assert
            Assert.IsFalse(result.Status);
            Assert.AreEqual("Project not found", result.Message);
        }

        [TestMethod]
        public async Task GetDonations_ForValidProject_ReturnsDonationsList()
        {
            // Arrange
            var projectId = 1;
            var donations = new List<Donation>
            {
                new Donation { Id = 1, Amount = 100, ProjectId = projectId },
                new Donation { Id = 2, Amount = 200, ProjectId = projectId }
            };

            _mockDbContext.Setup(db => db.Donations
                .Where(d => d.ProjectId == projectId)
                .ToListAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(donations);

            // Act
            var result = await _donationHandler.GetDonations(projectId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(300, result.Sum(d => d.Amount));
        }

        [TestMethod]
        public async Task GetUserDonations_ReturnsUserDonationsList()
        {
            // Arrange
            var userId = 1;
            var donations = new List<Donation>
            {
                new Donation { Id = 1, Amount = 100, UserId = userId },
                new Donation { Id = 2, Amount = 200, UserId = userId }
            };

            _mockDbContext.Setup(db => db.Donations
                .Where(d => d.UserId == userId)
                .ToListAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(donations);

            // Act
            var result = await _donationHandler.GetUserDonations(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public async Task MakeDonation_ExceedsFundingGoal_ReturnsPartialSuccess()
        {
            // Arrange
            var donation = new Donation
            {
                Amount = 600,
                UserId = 1,
                ProjectId = 1
            };

            var project = new Project
            {
                Id = 1,
                FundingGoal = 1000,
                CurrentAmount = 800
            };

            _mockDbContext.Setup(db => db.Projects.FindAsync(donation.ProjectId))
                .ReturnsAsync(project);

            // Act
            var result = await _donationHandler.MakeDonation(donation);

            // Assert
            Assert.IsTrue(result.Status);
            Assert.AreEqual("Donation accepted partially. Project goal reached.", result.Message);
        }

        [TestMethod]
        public async Task MakeDonation_ZeroAmount_ReturnsFail()
        {
            // Arrange
            var donation = new Donation
            {
                Amount = 0,
                UserId = 1,
                ProjectId = 1
            };

            // Act
            var result = await _donationHandler.MakeDonation(donation);

            // Assert
            Assert.IsFalse(result.Status);
            Assert.AreEqual("Donation amount must be greater than zero", result.Message);
        }

        [TestMethod]
        public async Task GetDonationStats_ReturnsCorrectStats()
        {
            // Arrange
            var projectId = 1;
            var donations = new List<Donation>
            {
                new Donation { Amount = 100, ProjectId = projectId },
                new Donation { Amount = 200, ProjectId = projectId },
                new Donation { Amount = 300, ProjectId = projectId }
            };

            _mockDbContext.Setup(db => db.Donations
                .Where(d => d.ProjectId == projectId)
                .ToListAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(donations);

            // Act
            var stats = await _donationHandler.GetDonationStats(projectId);

            // Assert
            Assert.AreEqual(600, stats.TotalAmount);
            Assert.AreEqual(3, stats.DonationCount);
            Assert.AreEqual(200, stats.AverageDonation);
        }
    }
}
