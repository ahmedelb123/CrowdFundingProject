using Xunit;
using CrowdFundingProject.Models;

namespace CrowdFundingProject.Tests
{
    public class ProjectTests
    {
        [Fact]
        public void Project_Creation_Sets_Properties_Correctly()
        {
            // Arrange
            var title = "Test Project";
            var description = "Test Description";
            var goal = 1000m;
            var deadline = DateTime.Now.AddDays(30);

            // Act
            var project = new Project
            {
                Title = title,
                Description = description,
                FundingGoal = goal,
                Deadline = deadline
            };

            // Assert
            Assert.Equal(title, project.Title);
            Assert.Equal(description, project.Description);
            Assert.Equal(goal, project.FundingGoal);
            Assert.Equal(deadline, project.Deadline);
        }
    }
} 