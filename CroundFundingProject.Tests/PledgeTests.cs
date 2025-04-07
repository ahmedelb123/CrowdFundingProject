using Microsoft.VisualStudio.TestTools.UnitTesting;
using CroundFundingProject.Models;

namespace CroundFundingProject.Tests
{
    [TestClass]
    public class PledgeTests
    {
        [TestMethod]
        public void Pledge_Creation_Sets_Properties_Correctly()
        {
            // Arrange
            var amount = 100m;
            var user = new User { Username = "backer" };
            var project = new Project { Title = "Test Project" };

            // Act
            var pledge = new Pledge
            {
                Amount = amount,
                User = user,
                Project = project
            };

            // Assert
            Assert.AreEqual(amount, pledge.Amount);
            Assert.AreEqual(user, pledge.User);
            Assert.AreEqual(project, pledge.Project);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-100)]
        public void Pledge_Amount_Must_Be_Positive(decimal invalidAmount)
        {
            // Arrange & Act & Assert
            Assert.ThrowsException<ArgumentException>(() => new Pledge { Amount = invalidAmount });
        }
    }
} 