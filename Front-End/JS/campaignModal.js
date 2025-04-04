document.addEventListener("DOMContentLoaded", () => {
  initializeEventListeners();
  fetchCampaignsPaginated(1, 10); // Default page size is 10
});

/**
 * Initializes event listeners for form submissions and comment submissions
 */
function initializeEventListeners() {
  const form = document.getElementById("campaignForm");
  const submitCommentBtn = document.getElementById("submitCommentButton");
  const addCommentSection = document.getElementById("addCommentSection");

  if (!localStorage.getItem("Id")) {
    addCommentSection.style.display = "none";
  }

  form.addEventListener("submit", handleCampaignSubmission);
  submitCommentBtn.addEventListener("click", handleCommentSubmission);

  // Add event listener for page size dropdown change
  const pageSizeDropdown = document.getElementById("pageSizeDropdown");
  pageSizeDropdown.addEventListener("change", handlePageSizeChange);
  document.getElementById('fundraising-type').addEventListener('change', filterCampaignsByType);
}
// function to filter campaigns by type
async function filterCampaignsByType() {
  const type = document.getElementById('fundraising-type').value;
  const pageSize = getPageSize();
  
  if (type === 'All') {
    fetchCampaignsPaginated(1, pageSize);
  } else {
    fetchCampaignsByType(type, 1, pageSize);
  }
}

// function to fetch campaigns by type
async function fetchCampaignsByType(type, page, pageSize) {
  try {
    const response = await fetch(
      `http://localhost:5228/api/post/getPostsByType/${type}?page=${page}&pageSize=${pageSize}`
    );
    if (!response.ok) return;
    const data = await response.json();
    displayCampaigns(data.items);
    generatePagination(data.totalPages, page, pageSize);
  } catch (err) {
    console.error("Error fetching campaigns by type:", err);
  }
}

// Handles the campaign submission (creating a new campaign)
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

// Builds the request body for creating a campaign
async function buildCampaignRequest() {
  const title = document.getElementById("campaignTitle").value.trim();
  const content = document.getElementById("campaignContent").value.trim();
  const targetAmount = parseFloat(
    document.getElementById("campaignTarget").value.trim()
  );
  const type = document.getElementById("campaignType").value;
  const file = document.getElementById("campaignImage").files[0];
  const mediaUrl = file ? await readFileAsBase64(file) : null;
  const userId = localStorage.getItem("Id");

  return {
    userId,
    title,
    content,
    targetAmount,
    mediaUrl,
    type, // Add the type to the request
  };
}

// Handles the response after submitting a campaign
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
    fetchCampaignsPaginated(1, getPageSize()); // Fetch campaigns with the default page size after creating
  }
  feedbackDiv.style.display = "block";
}

// Fetches paginated campaigns
async function fetchCampaignsPaginated(page, pageSize) {
  try {
    const response = await fetch(
      `http://localhost:5228/api/post/all?page=${page}&pageSize=${pageSize}`
    );
    if (!response.ok) return;
    const data = await response.json();
    displayCampaigns(data.items);
    generatePagination(data.totalPages, page, pageSize);
  } catch (err) {
    console.error("Error fetching campaigns:", err);
  }
}

// Displays the fetched campaigns in the grid
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

// Generates the pagination controls based on the total pages and current page
function generatePagination(totalPages, currentPage, pageSize) {
  const paginationContainer = document.getElementById("pagination");
  paginationContainer.innerHTML = "";

  for (let i = 1; i <= totalPages; i++) {
    const button = document.createElement("button");
    button.innerText = i;
    button.classList.add("pagination-btn");
    if (i === currentPage) {
      button.classList.add("active");
    }
    button.addEventListener("click", () =>
      fetchCampaignsPaginated(i, pageSize)
    );
    paginationContainer.appendChild(button);
  }
}

// Opens a modal displaying detailed information for a selected campaign
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

// Handles the submission of a new comment for a campaign
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

// Loads the comments for a specific campaign
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

// Displays the list of comments for a campaign
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

// Reads a file and returns its contents as a Base64 encoded string
async function readFileAsBase64(file) {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = () => resolve(reader.result);
    reader.onerror = reject;
    reader.readAsDataURL(file);
  });
}

// Handles the page size change from the dropdown
function handlePageSizeChange() {
  const pageSize = getPageSize();
  fetchCampaignsPaginated(1, pageSize); // Fetch campaigns with new page size
}

// Retrieves the selected page size from the dropdown
function getPageSize() {
  const pageSizeDropdown = document.getElementById("pageSizeDropdown");
  return parseInt(pageSizeDropdown.value);
}
