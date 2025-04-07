using Microsoft.VisualStudio.TestTools.UnitTesting;
using CroundFundingProject.Models;

namespace CroundFundingProject.Tests
{
    [TestClass]
    public class UserTests
    {
        [TestMethod]
        public void User_Creation_Sets_Properties_Correctly()
        {
            // Arrange
            var username = "testuser";
            var email = "test@example.com";

            // Act
            var user = new User
            {
                Username = username,
                Email = email
            };

            // Assert
            Assert.AreEqual(username, user.Username);
            Assert.AreEqual(email, user.Email);
        }

        [TestMethod]
        public void User_Can_Create_Project()
        {
            // Arrange
            var user = new User { Username = "creator" };
            var project = new Project { Title = "Test Project" };

            // Act
            user.CreatedProjects.Add(project);

            // Assert
            CollectionAssert.Contains(user.CreatedProjects.ToList(), project);
        }
    }
} 