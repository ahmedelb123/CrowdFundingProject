// campaignModal.js

document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('campaignForm');
    const feedbackDiv = document.getElementById('campaignFeedback');
  
    // Handle "Launch Campaign" form submission
    form.addEventListener('submit', async (event) => {
      event.preventDefault();
  
      feedbackDiv.style.display = 'none';
      feedbackDiv.innerHTML = '';
  
      try {
        // Gather input
        const title = document.getElementById('campaignTitle').value.trim();
        const content = document.getElementById('campaignContent').value.trim();
        const targetAmount = parseFloat(document.getElementById('campaignTarget').value.trim());
  
        // Convert file to base64 if present
        const fileInput = document.getElementById('campaignImage');
        const file = fileInput.files[0];
        let base64String = null;
        if (file) {
          base64String = await readFileAsBase64(file);
        }
  
        // Build request
        const requestBody = {
          userId: 0, // or let the server override
          title: title,
          content: content,
          targetAmount: targetAmount,
          mediaUrl: base64String // "data:image/...base64" or null
        };
  
        const response = await fetch('http://localhost:5228/api/post/create', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(requestBody)
        });
  
        const data = await response.json();
  
        if (!response.ok) {
          feedbackDiv.innerHTML = `
            <p style="color:red;">
              ${data.message || 'Error'}<br>
              ${data.error || ''}
            </p>`;
          feedbackDiv.style.display = 'block';
        } else {
          feedbackDiv.innerHTML = `
            <p style="color:green;">
              ${data.message || 'Campaign created successfully!'}
            </p>`;
          feedbackDiv.style.display = 'block';
          form.reset();
          fetchAllCampaigns();
        }
      } catch (err) {
        console.error('Fetch error:', err);
        feedbackDiv.innerHTML = `
          <p style="color:red;">
            Server connection error: ${err.message}
          </p>`;
        feedbackDiv.style.display = 'block';
      }
    });
  
    // Load campaigns on page load
    fetchAllCampaigns();
  });
  
  /**
   * Convert File to base64 string
   */
  function readFileAsBase64(file) {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = () => resolve(reader.result);
      reader.onerror = reject;
      reader.readAsDataURL(file);
    });
  }
  
  /**
   * Fetch campaigns and display in #campaignsGrid
   */
  async function fetchAllCampaigns() {
    try {
      const response = await fetch('http://localhost:5228/api/post/all');
      if (!response.ok) {
        console.error('Failed to fetch campaigns:', response.status);
        return;
      }
  
      const campaigns = await response.json();
      console.log('Raw campaigns data:', campaigns); // LOG to see actual property names
  
      const grid = document.getElementById('campaignsGrid');
      grid.innerHTML = '';
  
      campaigns.forEach(c => {
        // Sometimes ASP.NET returns "MediaUrl" instead of "mediaUrl"
        const media = c.mediaUrl || c.MediaUrl;
        const target = c.targetAmount || c.TargetAmount;
        const gained = c.amountGained || c.AmountGained;
  
        // Build the card
        const article = document.createElement('article');
        article.classList.add('campaign-card');
  
        const imgDiv = document.createElement('div');
        imgDiv.classList.add('campaign-image');
  
        const img = document.createElement('img');
        img.src = media || 'assets/images/default.png'; // fallback
        img.alt = c.title || 'Campaign Image';
  
        const categorySpan = document.createElement('span');
        categorySpan.classList.add('category');
        categorySpan.innerText = 'Tech'; // or c.category if you have one
  
        imgDiv.appendChild(img);
        imgDiv.appendChild(categorySpan);
  
        const contentDiv = document.createElement('div');
        contentDiv.classList.add('campaign-content');
  
        const h3 = document.createElement('h3');
        h3.innerText = c.title;
  
        // Truncate content
        const desc = document.createElement('p');
        desc.classList.add('description');
        const snippet = c.content && c.content.length > 80 
          ? c.content.substring(0, 80) + '...'
          : c.content || '';
        desc.innerText = snippet;
  
        const progressWrapper = document.createElement('div');
        progressWrapper.classList.add('progress-wrapper');
  
        const statsDiv = document.createElement('div');
        statsDiv.classList.add('campaign-stats');
  
        // Show gained vs. target
        const amountDiv = document.createElement('div');
        amountDiv.classList.add('amount');
        amountDiv.innerText = `$${gained || 0} raised`;
  
        const targetDiv = document.createElement('div');
        targetDiv.classList.add('target');
        targetDiv.innerText = `of $${target || 0}`;
  
        statsDiv.appendChild(amountDiv);
        statsDiv.appendChild(targetDiv);
  
        const metaDiv = document.createElement('div');
        metaDiv.classList.add('campaign-meta');
        metaDiv.innerHTML = ''; // Remove "0 backers" etc.
  
        progressWrapper.appendChild(statsDiv);
        progressWrapper.appendChild(metaDiv);
  
        contentDiv.appendChild(h3);
        contentDiv.appendChild(desc);
        contentDiv.appendChild(progressWrapper);
  
        article.appendChild(imgDiv);
        article.appendChild(contentDiv);
  
        // Clicking => open detail modal
        article.addEventListener('click', () => openDetailModal(c));
  
        grid.appendChild(article);
      });
    } catch (err) {
      console.error('Error fetching campaigns:', err);
    }
  }
  
  /**
   * Show detail modal
   */
  function openDetailModal(campaign) {
    const detailModal = new bootstrap.Modal(document.getElementById('campaignDetailModal'));
  
    const media = campaign.mediaUrl || campaign.MediaUrl;
    const target = campaign.targetAmount || campaign.TargetAmount;
    const gained = campaign.amountGained || campaign.AmountGained;
  
    document.getElementById('detailModalTitle').innerText = campaign.title;
    document.getElementById('detailModalDescription').innerText = campaign.content;
    document.getElementById('detailModalTarget').innerText = target || 0;
    document.getElementById('detailModalRaised').innerText = gained || 0;
  
    const imgElem = document.getElementById('detailModalImage');
    imgElem.src = media || 'assets/images/default.png';
  
    // e.g. if you want donation logic or comments
    window.currentCampaignId = campaign.id;
  
    detailModal.show();
  }
  