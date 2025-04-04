document.addEventListener("DOMContentLoaded", () => {
  const submitCommentBtn = document.getElementById("submitCommentButton");
  const newCommentText = document.getElementById("newCommentText");
  const addCommentSection = document.getElementById("addCommentSection");

  // Check if user is logged in
  const isLoggedIn = localStorage.getItem("Id");
  if (!isLoggedIn) {
    addCommentSection.style.display = "none";
  }

  submitCommentBtn.addEventListener("click", async () => {
    const postId = window.currentCampaignId;
    const commentText = newCommentText.value.trim();

    if (!commentText) {
      alert("Please enter some comment text!");
      return;
    }

    try {
      const userId = localStorage.getItem("Id");
      if (!userId) {
        alert("You must be logged in to comment!");
        return;
      }

      const requestBody = {
        postId: parseInt(postId),
        userId: parseInt(userId),
        commentText: commentText, // This must match the backend DTO
      };
      console.log(userId);
      console.log(postId);

      const response = await fetch(
        "http://localhost:5228/api/comment/addComment",
        {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(requestBody),
        }
      );

      const data = await response.json();
      console.log(data.status);
      if (data.status) {
        newCommentText.value = "";
        loadCommentsForCampaign(postId);
      } else {
        const errorData = await response.json();
        console.error("Backend error:", errorData);
        alert(errorData.message || "Failed to add comment");
        return;
      }
    } catch (err) {
      console.error("Add comment error:", err);
      alert("Error adding comment: " + err.message);
    }
  });
});


// Load comments function remains the same
async function loadCommentsForCampaign(postId) {
  try {
    const response1 = await fetch(
      `http://localhost:5228/api/comment/post/${postId}`
    );

    const comments = await response1.json();
    const userId = localStorage.getItem("Id")
    const response2 = await fetch(
      `http://localhost:5228/api/user/getUser/${userId}`
    );
    if (!response1.ok || !response2.ok) return;

    const user = await response2.json();

    const commentsList = document.getElementById("commentsList");
    commentsList.innerHTML = "";

    comments.forEach((comment) => {
      const commentDiv = document.createElement("div");
      commentDiv.classList.add("single-comment");
      commentDiv.innerHTML = `
              <p><strong>${user.userName}:</strong> ${comment.text}</p>
          `;
      commentsList.appendChild(commentDiv);
    });
  } catch (err) {
    console.error("loadCommentsForCampaign error:", err);
  }
}
