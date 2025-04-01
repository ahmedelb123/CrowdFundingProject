document.addEventListener("DOMContentLoaded", () => {
  initializeEventListeners();
  fetchAllCampaigns();
});

function initializeEventListeners() {
  const form = document.getElementById("campaignForm");
  const submitCommentBtn = document.getElementById("submitCommentButton");
  const addCommentSection = document.getElementById("addCommentSection");

  if (!localStorage.getItem("Id")) {
    addCommentSection.style.display = "none";
  }

  form.addEventListener("submit", handleCampaignSubmission);
  submitCommentBtn.addEventListener("click", handleCommentSubmission);
}

async function handleCampaignSubmission(event) {
  event.preventDefault();
  const feedbackDiv = document.getElementById("campaignFeedback");
  feedbackDiv.style.display = "none";
  feedbackDiv.innerHTML = "";

  try {
    const requestBody = await buildCampaignRequest();
    const response = await fetch("http://localhost:5228/api/post/create", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(requestBody),
    });
    await handleCampaignResponse(response, feedbackDiv);
  } catch (err) {
    feedbackDiv.innerHTML = `<p style="color:red;">Server connection error: ${err.message}</p>`;
    feedbackDiv.style.display = "block";
  }
}

async function buildCampaignRequest() {
  const title = document.getElementById("campaignTitle").value.trim();
  const content = document.getElementById("campaignContent").value.trim();
  const targetAmount = parseFloat(
    document.getElementById("campaignTarget").value.trim()
  );
  const file = document.getElementById("campaignImage").files[0];
  const mediaUrl = file ? await readFileAsBase64(file) : null;

  return { userId: 0, title, content, targetAmount, mediaUrl };
}

async function handleCampaignResponse(response, feedbackDiv) {
  const data = await response.json();
  if (!response.ok) {
    feedbackDiv.innerHTML = `<p style="color:red;">${
      data.message || "Error"
    }</p>`;
  } else {
    feedbackDiv.innerHTML = `<p style="color:green;">${
      data.message || "Campaign created successfully!"
    }</p>`;
    document.getElementById("campaignForm").reset();
    fetchAllCampaigns();
  }
  feedbackDiv.style.display = "block";
}

async function fetchAllCampaigns() {
  try {
    const response = await fetch("http://localhost:5228/api/post/all");
    if (!response.ok) return;
    displayCampaigns(await response.json());
  } catch (err) {
    console.error("Error fetching campaigns:", err);
  }
}

function displayCampaigns(campaigns) {
  const grid = document.getElementById("campaignsGrid");
  grid.innerHTML = "";

  campaigns.forEach((c) => {
    const article = document.createElement("article");
    article.classList.add("campaign-card");
    article.innerHTML = `
      <div class="campaign-image">
        <img src="${c.mediaUrl || "assets/images/default.png"}" alt="${
      c.title
    }">
        <span class="category">Tech</span>
      </div>
      <div class="campaign-content">
        <h3>${c.title}</h3>
        <p class="description">${
          c.content.length > 80 ? c.content.substring(0, 80) + "..." : c.content
        }</p>
        <div class="progress-wrapper">
          <div class="campaign-stats">
            <div class="amount">$${c.amountGained || 0} raised</div>
            <div class="target">of $${c.targetAmount || 0}</div>
          </div>
        </div>
      </div>`;
    article.addEventListener("click", () => openDetailModal(c));
    grid.appendChild(article);
  });
}

function openDetailModal(campaign) {
  const detailModal = new bootstrap.Modal(
    document.getElementById("campaignDetailModal")
  );
  document.getElementById("detailModalTitle").innerText = campaign.title;
  document.getElementById("detailModalDescription").innerText =
    campaign.content;
  document.getElementById("detailModalTarget").innerText =
    campaign.targetAmount || 0;
  document.getElementById("detailModalRaised").innerText =
    campaign.amountGained || 0;
  document.getElementById("detailModalImage").src =
    campaign.mediaUrl || "assets/images/default.png";
  window.currentCampaignId = campaign.id;
  loadCommentsForCampaign(campaign.id);
  detailModal.show();
}

async function handleCommentSubmission() {
  const postId = window.currentCampaignId;
  const commentText = document.getElementById("newCommentText").value.trim();
  if (!commentText) return alert("Please enter some comment text!");

  try {
    const userId = parseInt(localStorage.getItem("userId") || "0");
    const requestBody = { postId, userId, commentText };
    const response = await fetch(
      "http://localhost:5228/api/comment/addComment",
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(requestBody),
      }
    );
    if (response.ok) {
      document.getElementById("newCommentText").value = "";
      loadCommentsForCampaign(postId);
    } else {
      alert("Failed to add comment!");
    }
  } catch (err) {
    console.error("Add comment error:", err);
    alert("Error adding comment: " + err.message);
  }
}

async function loadCommentsForCampaign(postId) {
  try {
    const response = await fetch(
      `http://localhost:5228/api/comment/post/${postId}`
    );
    if (!response.ok) return;
    displayComments(await response.json());
  } catch (err) {
    console.error("loadCommentsForCampaign error:", err);
  }
}

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

async function readFileAsBase64(file) {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = () => resolve(reader.result);
    reader.onerror = reject;
    reader.readAsDataURL(file);
  });
}
