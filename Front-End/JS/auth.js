document.addEventListener("DOMContentLoaded", async () => {
  const authButtons = document.getElementById("authButtons");

  if (!authButtons) return;

  try {
    const auth = !!localStorage.getItem("Id");

    if (auth) {
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
  } catch (err) {
    console.error("Error checking auth status:", err);
    authButtons.innerHTML = `<a href="login.html" class="btn-secondary">Sign In</a>`;
  }
});

async function logout() {
  try {
    localStorage.removeItem("Id");
    window.location.href = "login.html";
  } catch (err) {
    alert("Logout error: " + err.message);
  }
}
