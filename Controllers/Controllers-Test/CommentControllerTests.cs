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
    public class CommentControllerTests
    {
        private Mock<ICommentHandler> _mockCommentHandler;
        private CommentController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockCommentHandler = new Mock<ICommentHandler>();
            _controller = new CommentController(_mockCommentHandler.Object);
        }

        [TestMethod]
        public async Task Post_ValidComment_ReturnsOkResult()
        {
            // Arrange
            var comment = new Comment { Content = "Test comment" };
            _mockCommentHandler.Setup(h => h.AddComment(It.IsAny<Comment>()))
                .ReturnsAsync(new Result { Status = true, Message = "Comment added successfully" });

            // Act
            var result = await _controller.Post(comment);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            var response = okResult.Value as Result;
            Assert.IsTrue(response.Status);
        }

        [TestMethod]
        public async Task Get_ValidPostId_ReturnsOkResult()
        {
            // Arrange
            var postId = 1;
            var comments = new List<Comment> 
            { 
                new Comment { Id = 1, Content = "Test comment" } 
            };
            _mockCommentHandler.Setup(h => h.GetComments(postId))
                .ReturnsAsync(comments);

            // Act
            var result = await _controller.Get(postId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            var returnedComments = okResult.Value as List<Comment>;
            Assert.AreEqual(1, returnedComments.Count);
        }

        [TestMethod]
        public async Task Delete_ExistingComment_ReturnsOkResult()
        {
            // Arrange
            var commentId = 1;
            var userId = 1;
            _mockCommentHandler.Setup(h => h.DeleteComment(commentId, userId))
                .ReturnsAsync(new Result { Status = true, Message = "Comment deleted successfully" });

            // Act
            var result = await _controller.Delete(commentId, userId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Delete_NonExistingComment_ReturnsNotFound()
        {
            // Arrange
            var commentId = 999;
            var userId = 1;
            _mockCommentHandler.Setup(h => h.DeleteComment(commentId, userId))
                .ReturnsAsync(new Result { Status = false, Message = "Comment not found" });

            // Act
            var result = await _controller.Delete(commentId, userId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }
    }
}
