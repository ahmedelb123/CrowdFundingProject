document.addEventListener("DOMContentLoaded", () => {
  initializeEventListeners();
  fetchCampaignsPaginated(1, 10); // Default page size = 10
});

/**
 * Initializes event listeners for form submissions, comment submissions,
 * page size changes, campaign type filtering, etc.
 */
function initializeEventListeners() {
  const form = document.getElementById("campaignForm");
  form.addEventListener("submit", handleCampaignSubmission);

  const submitCommentBtn = document.getElementById("submitCommentButton");
  submitCommentBtn.addEventListener("click", handleCommentSubmission);

  // Hide comment section if user is not logged in
  const addCommentSection = document.getElementById("addCommentSection");
  if (!localStorage.getItem("Id")) {
    addCommentSection.style.display = "none";
  }

  // Handle page size change
  const pageSizeDropdown = document.getElementById("pageSizeDropdown");
  pageSizeDropdown.addEventListener("change", handlePageSizeChange);

  // Handle filtering by type
  document
    .getElementById("fundraising-type")
    .addEventListener("change", filterCampaignsByType);
}

/** Handles the campaign submission (creating a new campaign) */
async function handleCampaignSubmission(event) {
  event.preventDefault();
  
  // Clear and hide the campaign feedback area
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

/** Builds the request body for creating a campaign */
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
    type,
  };
}

/** Handles the response after submitting a campaign */
async function handleCampaignResponse(response, feedbackDiv) {
  const data = await response.json();
  if (!response.ok) {
    feedbackDiv.innerHTML = `<p style="color:red;">${data.message || "Error"}</p>`;
  } else {
    feedbackDiv.innerHTML = `<p style="color:green;">${data.message || "Campaign created successfully!"}</p>`;
    document.getElementById("campaignForm").reset();
    // Refresh the campaigns list
    fetchCampaignsPaginated(1, getPageSize());
  }
  feedbackDiv.style.display = "block";
}

/** Fetches paginated campaigns */
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

/** Displays the fetched campaigns in the grid */
function displayCampaigns(campaigns) {
  const grid = document.getElementById("campaignsGrid");
  grid.innerHTML = "";

  campaigns.forEach((c) => {
    const article = document.createElement("article");
    article.classList.add("campaign-card");
    article.innerHTML = `
      <div class="campaign-image">
        <img src="${c.mediaUrl || "assets/images/default.png"}" alt="${c.title}">
        <span class="category">${c.type || "N/A"}</span>
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
    // Clicking the campaign card opens the detail modal
    article.addEventListener("click", () => openDetailModal(c));
    grid.appendChild(article);
  });
}

/** Generates pagination controls */
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
    button.addEventListener("click", () => fetchCampaignsPaginated(i, pageSize));
    paginationContainer.appendChild(button);
  }
}

/** Opens a modal displaying detailed information for a selected campaign */
function openDetailModal(campaign) {
  // Example: using bootstrap's modal
  const detailModal = new bootstrap.Modal(
    document.getElementById("campaignDetailModal")
  );

  document.getElementById("detailModalTitle").innerText = campaign.title;
  document.getElementById("detailModalDescription").innerText = campaign.content;
  document.getElementById("detailModalTarget").innerText =
    campaign.targetAmount || 0;
  document.getElementById("detailModalRaised").innerText =
    campaign.amountGained || 0;
  document.getElementById("detailModalImage").src =
    campaign.mediaUrl || "assets/images/default.png";

  // Save the campaign's ID globally so comment logic can access it
  window.currentCampaignId = campaign.id;
  loadCommentsForCampaign(campaign.id);

  detailModal.show();
}

/** Reads a file and returns its contents as a Base64 encoded string */
async function readFileAsBase64(file) {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = () => resolve(reader.result);
    reader.onerror = reject;
    reader.readAsDataURL(file);
  });
}

/** Handles the page size change */
function handlePageSizeChange() {
  const pageSize = getPageSize();
  fetchCampaignsPaginated(1, pageSize);
}

/** Retrieves the selected page size */
function getPageSize() {
  const pageSizeDropdown = document.getElementById("pageSizeDropdown");
  return parseInt(pageSizeDropdown.value);
}

/**
 * Filtering campaigns by type
 */
async function filterCampaignsByType() {
  const type = document.getElementById("fundraising-type").value;
  const pageSize = getPageSize();

  if (type === "All") {
    fetchCampaignsPaginated(1, pageSize);
  } else {
    fetchCampaignsByType(type, 1, pageSize);
  }
}

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

