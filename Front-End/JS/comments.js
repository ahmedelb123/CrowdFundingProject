
document.addEventListener("DOMContentLoaded", () => {
  const submitCommentBtn = document.getElementById("submitCommentButton");
  const newCommentText = document.getElementById("newCommentText");
  const addCommentSection = document.getElementById("addCommentSection");
  const commentFeedback = document.getElementById("commentFeedback");

  // If not logged in, hide comment section
  if (!localStorage.getItem("Id")) {
    addCommentSection.style.display = "none";
  }

  // Handle comment submission
  submitCommentBtn.addEventListener("click", async () => {
    // Reset feedback
    commentFeedback.style.display = "none";
    commentFeedback.innerHTML = "";

    const postId = window.currentCampaignId;
    const commentText = newCommentText.value.trim();

    if (!commentText) {
      commentFeedback.innerHTML = `<p style="color:red;">Please enter some comment text!</p>`;
      commentFeedback.style.display = "block";
      setTimeout(() => {
        commentFeedback.style.display = "none";
      }, 3000);
      return;
    }

    try {
      const userId = parseInt(localStorage.getItem("Id") || "0");
      if (!userId) {
        commentFeedback.innerHTML = `<p style="color:red;">You must be logged in to comment!</p>`;
        commentFeedback.style.display = "block";
        setTimeout(() => {
          commentFeedback.style.display = "none";
        }, 3000);
        return;
      }

      const requestBody = {
        postId: parseInt(postId),
        userId: userId,
        commentText: commentText,
      };

      const response = await fetch("http://localhost:5228/api/comment/addComment", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(requestBody),
      });

      if (response.ok) {
        newCommentText.value = "";
        loadCommentsForCampaign(postId);
        commentFeedback.innerHTML = `<p style="color:green;">Comment added successfully!</p>`;
      } else {
        const data = await response.json();
        commentFeedback.innerHTML = `<p style="color:red;">${data.message || "Failed to add comment"}</p>`;
      }

      commentFeedback.style.display = "block";
      setTimeout(() => {
        commentFeedback.style.display = "none";
      }, 3000);

    } catch (err) {
      console.error("Add comment error:", err);
      commentFeedback.innerHTML = `<p style="color:red;">Error adding comment: ${err.message}</p>`;
      commentFeedback.style.display = "block";
      setTimeout(() => {
        commentFeedback.style.display = "none";
      }, 3000);
    }
  });
});

/** Loads comments for the given post/campaign */
async function loadCommentsForCampaign(postId) {
  const commentFeedback = document.getElementById("commentFeedback");
  try {
    const response = await fetch(`http://localhost:5228/api/comment/post/${postId}`);
    if (!response.ok) {
      commentFeedback.innerHTML = `<p style="color:red;">Error loading comments.</p>`;
      commentFeedback.style.display = "block";
      setTimeout(() => {
        commentFeedback.style.display = "none";
      }, 3000);
      return;
    }
    const comments = await response.json();
    displayComments(comments);
  } catch (err) {
    console.error("loadCommentsForCampaign error:", err);
    commentFeedback.innerHTML = `<p style="color:red;">Error loading comments: ${err.message}</p>`;
    commentFeedback.style.display = "block";
    setTimeout(() => {
      commentFeedback.style.display = "none";
    }, 3000);
  }
}

/** Displays the list of comments */
function displayComments(comments) {
  const commentsList = document.getElementById("commentsList");
  commentsList.innerHTML = "";
  comments.forEach((comment) => {
    const commentDiv = document.createElement("div");
    commentDiv.classList.add("single-comment");
    commentDiv.innerHTML = `<p><strong>User #${comment.userId}:</strong> ${comment.text}</p>`;
    commentsList.appendChild(commentDiv);
  });
}
