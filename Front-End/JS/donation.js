// donationScript.js

document.addEventListener('DOMContentLoaded', () => {
    const donateButton = document.getElementById('donateButton');
    const donationModal = new bootstrap.Modal(document.getElementById('donationModal'));
    const confirmDonateButton = document.getElementById('confirmDonateButton');
    const donationFeedback = document.getElementById('donationFeedback');
  
    // Helper function to display feedback messages and auto-hide them
    function showDonationFeedback(message, color = 'red') {
      donationFeedback.textContent = message;
      donationFeedback.style.color = color;
      donationFeedback.style.display = "block";
  
      // Hide the alert automatically after 3 seconds
      setTimeout(() => {
        donationFeedback.style.display = "none";
      }, 3000);
    }
  
    let currentPostId = null;
  
    // When "Donate Now!" is clicked in detail modal
    donateButton.addEventListener('click', () => {
      currentPostId = window.currentCampaignId;
      donationModal.show();
    });
  
    // Confirm donation inside donation modal
    confirmDonateButton.addEventListener('click', async () => {
      // Clear any previous feedback
      donationFeedback.style.display = "none";
      donationFeedback.textContent = "";
  
      try {
        // Get all form values
        const amount = parseFloat(document.getElementById('donationAmount').value);
        const holderName = document.getElementById('holderName').value.trim();
        const cardNumber = document.getElementById('cardNumber').value.trim();
        const secretNumber = parseInt(document.getElementById('secretNumber').value);
        const expiryDate = document.getElementById('expiryDate').value.trim();
        const userId = parseInt(localStorage.getItem("Id") || 0);
  
        // Basic validation
        if (!amount || amount <= 0) {
          showDonationFeedback("Please enter a valid donation amount.");
          return;
        }
        if (!holderName) {
          showDonationFeedback("Please enter cardholder name.");
          return;
        }
        if (!cardNumber || cardNumber.length < 16) {
          showDonationFeedback("Please enter a valid 16-digit card number.");
          return;
        }
        if (!secretNumber || secretNumber.toString().length !== 3) {
          showDonationFeedback("Please enter a valid 3-digit CVV.");
          return;
        }
        if (!expiryDate || !expiryDate.match(/^(0[1-9]|1[0-2])\/?([0-9]{2})$/)) {
          showDonationFeedback("Please enter a valid expiry date (MM/YY).");
          return;
        }
        if (userId === 0) {
          showDonationFeedback("You must be logged in to donate.");
          return;
        }
  
        // Build request body
        const requestBody = {
          postId: currentPostId,
          amount: amount,
          userId: userId,
          holderName: holderName,
          cardNumber: cardNumber,
          secretNumber: secretNumber,
          expiryDate: expiryDate
        };
  
        const response = await fetch("http://localhost:5228/api/donation/create", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify(requestBody)
        });
  
        const data = await response.json();
  
        if (!response.ok) {
          showDonationFeedback(data.message || "Failed to create donation!");
        } else {
          // Show success
          showDonationFeedback(data.message || "Donation successful!", 'green');
          donationModal.hide();
  
          // Clear all fields
          document.getElementById('donationAmount').value = "";
          document.getElementById('holderName').value = "";
          document.getElementById('cardNumber').value = "";
          document.getElementById('secretNumber').value = "";
          document.getElementById('expiryDate').value = "";
  
          // Refresh campaigns (assuming these functions exist elsewhere)
          fetchCampaignsPaginated(1, getPageSize());
        }
      } catch (err) {
        console.error("Donation error:", err);
        showDonationFeedback("Error during donation: " + err.message);
      }
    });
  });
  