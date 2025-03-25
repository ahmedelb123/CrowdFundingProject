document.addEventListener("DOMContentLoaded", async () => {
  const authButtons = document.getElementById("authButtons");

  if (!authButtons) return;

  try {
    const auth = !!localStorage.getItem("userId");

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
    const response = await fetch("http://localhost:5228/api/user/logout", {
      method: "DELETE",
      credentials: "include",
    });

    const result = await response.json();

    if (response.ok) {
      localStorage.removeItem("userId");
      window.location.href = "login.html";
    } else {
      alert("Logout failed: " + result.message);
    }
  } catch (err) {
    alert("Logout error: " + err.message);
  }
}
