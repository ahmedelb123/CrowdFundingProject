document.addEventListener("DOMContentLoaded", function () {
  const registerButton = document.querySelector("button[type='submit']");

  registerButton.addEventListener("click", async function (event) {
    event.preventDefault();
    //select all the info for the registration
    const name = document.querySelector("input[name='fname']").value;
    const surname = document.querySelector("input[name='sname']").value;
    const email = document.querySelector("input[name='create-email']").value;
    const password = document.querySelector("input[name='create-psw']").value;

    const emailRegex = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$/;
    const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#%&])[A-Za-z\d!@#%&]{8,16}$/;
    // check if email is valid
    if (!emailRegex.test(email)) {
      alert("Please enter a valid email address.");
      return;
    }
    // check if the password is strong
    if (!passwordRegex.test(password)) {
      alert("Password must be strong ");
      return;
    }

    //check if fields have been filled
    if (!name || !surname || !email || !password) {
      alert("Please fill in all fields");
      return;
    }

    //send registration to backend
    const response = await fetch(
      "http://localhost:5228/api/user/createAccount",
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          name: name,
          surname: surname,
          email: email,
          password: password,
        }),
      }
    );

    const data = await response.json();
    // check if the account has been created in db
    if (response.ok) {
      alert("Registration successful!");
      window.location.href = "login.html"; 
    } else {
      alert("Registration failed: Accoutn already exist");
    }
  });
});
