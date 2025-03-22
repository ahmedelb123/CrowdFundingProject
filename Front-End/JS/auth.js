document.addEventListener("DOMContentLoaded", () => {
    const authButtons = document.getElementById("authButtons");
  
    if (!authButtons) return;
  
    //Assume user is logged in if userId is in localStorage
    const isLoggedIn = !!localStorage.getItem("userId");
  
    if (isLoggedIn) {
        //if user is logged in show Launch Campaign and Sign Out Buttons
      authButtons.innerHTML = `
        <button
        id="launchBtn"
        type="button"
        class="btn-primary"
        data-bs-toggle="modal"
        data-bs-target="#campaignModal"
        >
            Launch Campaign
        </button>
        <button class="btn btn-outline-danger" onclick="logout()">Logout</button>
      `;
    } else {
      authButtons.innerHTML = `
        <a href="login.html" class="btn-secondary">Sign In</a>
      `;
    }
  });
  
  async function logout() {
    try {
      const response = await fetch("http://localhost:5228/api/user/logout", {
        method: "POST",
        credentials: "include"
      });
  
      const result = await response.json();
  
      //clear user id and data
      localStorage.removeItem("userId");
  
      if (response.ok) {
        window.location.href = "login.html";
      } else {
        alert("Logout failed: " + result.message);
      }
    } catch (err) {
      alert("Logout error: " + err.message);
    }
  }