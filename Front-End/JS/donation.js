// donation.js
document.addEventListener('DOMContentLoaded', () => {
    const donateButton = document.getElementById('donateButton');
    const donationModal = new bootstrap.Modal(document.getElementById('donationModal'));
    const confirmDonateButton = document.getElementById('confirmDonateButton');
  
    let currentPostId = null;
  
    // 1) When "Donate Now!" is clicked in detail modal
    donateButton.addEventListener('click', () => {
      // window.currentCampaignId was set in openDetailModal
      currentPostId = window.currentCampaignId;
      donationModal.show();
    });
  
    // 2) Confirm donation inside donation modal
    confirmDonateButton.addEventListener('click', async () => {
      try {
        const amountInput = document.getElementById('donationAmount');
        const paymentDetails = document.getElementById('paymentDetails').value.trim();
  
        const amount = parseFloat(amountInput.value);
        if (!amount || amount <= 0) {
          alert("Please enter a valid donation amount.");
          return;
        }
  
        // Build request body for CreateDonationDto
        const requestBody = {
          postId: currentPostId,
          amount: amount,
          paymentDetails: paymentDetails
          // Add other fields if your CreateDonationDto expects them
        };
  
        // Example: POST /api/donation/create
        const response = await fetch("http://localhost:5228/api/donation/create", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(requestBody)
        });
  
        const data = await response.json();
  
        if (!response.ok) {
          alert(data.message || "Failed to create donation!");
        } else {
          alert(data.message || "Donation successful!");
          // Optionally close donation modal
          donationModal.hide();
  
          // Clear fields
          amountInput.value = "";
          document.getElementById('paymentDetails').value = "";
  
          // Reload campaign so we see updated amountGained
          fetchAllCampaigns();
        }
      } catch (err) {
        console.error("Donation error:", err);
        alert("Error during donation: " + err.message);
      }
    });
  });
  