// comments.js
document.addEventListener("DOMContentLoaded", () => {
  const submitCommentBtn = document.getElementById("submitCommentButton");
  const newCommentText = document.getElementById("newCommentText");
  const addCommentSection = document.getElementById("addCommentSection");

  // We'll assume user is logged in if we find a "userId" or "authToken" in localStorage
  const isLoggedIn = localStorage.getItem("Id"); // or userId
  console.log(isLoggedIn);
  if (!isLoggedIn) {
    // Hide the add comment area if not logged in
    addCommentSection.style.display = "none";
  }

  // When user clicks "Submit Comment"
  submitCommentBtn.addEventListener("click", async () => {
    // window.currentCampaignId holds the post ID
    const postId = window.currentCampaignId;
    const commentText = newCommentText.value.trim();

    if (!commentText) {
      alert("Please enter some comment text!");
      return;
    }

    try {
      // Example: userId might come from localStorage
      const userId = parseInt(localStorage.getItem("userId") || "0");

      // Build CommentDto
      const requestBody = {
        postId: postId,
        userId: userId,
        commentText: commentText,
      };

      const response = await fetch(
        "http://localhost:5228/api/comment/addComment",
        {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(requestBody),
        }
      );

      const data = await response.json();
      if (!response.ok) {
        alert(data.message || "Failed to add comment!");
      } else {
        // Clear input
        newCommentText.value = "";
        // Reload comments
        loadCommentsForCampaign(postId);
      }
    } catch (err) {
      console.error("Add comment error:", err);
      alert("Error adding comment: " + err.message);
    }
  });
});

/**
 * Load all comments for a given campaign (postId) and display in #commentsList
 */
async function loadCommentsForCampaign(postId) {
  console.log(postId);
  try {
    const response = await fetch(
      `http://localhost:5228/api/comment/post/${postId}`
    );
    console.log(response);
    if (!response.ok) {
      console.error("Failed to load comments:", response.status);
      return;
    }

    const comments = await response.json();
    const commentsList = document.getElementById("commentsList");
    commentsList.innerHTML = "";

    comments.forEach((comment) => {
      const commentDiv = document.createElement("div");
      commentDiv.classList.add("single-comment");
      // e.g. show userId or username if you have it
      commentDiv.innerHTML = `
          <p><strong>User #${comment.userId}:</strong> ${comment.text}</p>
        `;
      commentsList.appendChild(commentDiv);
    });
  } catch (err) {
    console.error("loadCommentsForCampaign error:", err);
  }
}
