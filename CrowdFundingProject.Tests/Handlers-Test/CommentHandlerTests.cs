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
    public class CommentHandlerTests
    {
        private Mock<AppDbContext> _mockDbContext;
        private CommentHandler _commentHandler;

        [TestInitialize]
        public void Setup()
        {
            _mockDbContext = new Mock<AppDbContext>();
            _commentHandler = new CommentHandler(_mockDbContext.Object);
        }

        [TestMethod]
        public async Task AddComment_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var comment = new Comment
            {
                Content = "Test Comment",
                UserId = 1,
                PostId = 1
            };

            _mockDbContext.Setup(db => db.Comments.AddAsync(
                It.IsAny<Comment>(), 
                It.IsAny<CancellationToken>()))
                .Returns(ValueTask.FromResult((Comment)null));

            // Act
            var result = await _commentHandler.AddComment(comment);

            // Assert
            Assert.IsTrue(result.Status);
            Assert.AreEqual("Comment added successfully", result.Message);
        }

        [TestMethod]
        public async Task GetComments_ForValidPost_ReturnsCommentsList()
        {
            // Arrange
            var postId = 1;
            var comments = new List<Comment>
            {
                new Comment { Id = 1, Content = "Comment 1", PostId = postId },
                new Comment { Id = 2, Content = "Comment 2", PostId = postId }
            };

            _mockDbContext.Setup(db => db.Comments
                .Where(c => c.PostId == postId)
                .ToListAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(comments);

            // Act
            var result = await _commentHandler.GetComments(postId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public async Task DeleteComment_ExistingComment_ReturnsSuccess()
        {
            // Arrange
            var commentId = 1;
            var comment = new Comment { Id = commentId, UserId = 1 };

            _mockDbContext.Setup(db => db.Comments.FindAsync(commentId))
                .ReturnsAsync(comment);

            // Act
            var result = await _commentHandler.DeleteComment(commentId, 1);

            // Assert
            Assert.IsTrue(result.Status);
            Assert.AreEqual("Comment deleted successfully", result.Message);
        }

        [TestMethod]
        public async Task DeleteComment_NonExistingComment_ReturnsFail()
        {
            // Arrange
            var commentId = 999;

            _mockDbContext.Setup(db => db.Comments.FindAsync(commentId))
                .ReturnsAsync((Comment)null);

            // Act
            var result = await _commentHandler.DeleteComment(commentId, 1);

            // Assert
            Assert.IsFalse(result.Status);
            Assert.AreEqual("Comment not found", result.Message);
        }

        [TestMethod]
        public async Task AddComment_WithEmptyContent_ReturnsFail()
        {
            // Arrange
            var comment = new Comment
            {
                Content = "",
                UserId = 1,
                PostId = 1
            };

            // Act
            var result = await _commentHandler.AddComment(comment);

            // Assert
            Assert.IsFalse(result.Status);
            Assert.AreEqual("Comment content cannot be empty", result.Message);
        }

        [TestMethod]
        public async Task UpdateComment_ByCorrectUser_ReturnsSuccess()
        {
            // Arrange
            var commentId = 1;
            var userId = 1;
            var comment = new Comment 
            { 
                Id = commentId, 
                UserId = userId,
                Content = "Original content" 
            };
            var updatedContent = "Updated content";

            _mockDbContext.Setup(db => db.Comments.FindAsync(commentId))
                .ReturnsAsync(comment);

            // Act
            var result = await _commentHandler.UpdateComment(commentId, userId, updatedContent);

            // Assert
            Assert.IsTrue(result.Status);
            Assert.AreEqual("Comment updated successfully", result.Message);
        }

        [TestMethod]
        public async Task UpdateComment_ByWrongUser_ReturnsFail()
        {
            // Arrange
            var commentId = 1;
            var userId = 1;
            var wrongUserId = 2;
            var comment = new Comment 
            { 
                Id = commentId, 
                UserId = userId 
            };

            _mockDbContext.Setup(db => db.Comments.FindAsync(commentId))
                .ReturnsAsync(comment);

            // Act
            var result = await _commentHandler.UpdateComment(commentId, wrongUserId, "Updated content");

            // Assert
            Assert.IsFalse(result.Status);
            Assert.AreEqual("Unauthorized to update this comment", result.Message);
        }
    }
}
