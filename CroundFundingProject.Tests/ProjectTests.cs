using Microsoft.VisualStudio.TestTools.UnitTesting;
using CroundFundingProject.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using CrowdFundingProject.Models;
using CrowdFundingProject.Controllers;
using CrowdFundingProject.Handlers;
using CrowdFundingProject.Data;
using CrowdFundingProject.Dto;


namespace CroundFundingProject.Tests
{
    [TestClass]
    public class ProjectTests
    {
        [TestMethod]
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
            Assert.AreEqual(title, project.Title);
            Assert.AreEqual(description, project.Description);
            Assert.AreEqual(goal, project.FundingGoal);
            Assert.AreEqual(deadline, project.Deadline);
        }

        [TestMethod]
        public void Project_FundingProgress_Calculation_Is_Correct()
        {
            // Arrange
            var project = new Project
            {
                FundingGoal = 1000m,
                CurrentAmount = 500m
            };

            // Act & Assert
            Assert.AreEqual(50, project.FundingProgress);
        }
    }
} 