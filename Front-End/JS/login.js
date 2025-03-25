document.addEventListener("DOMContentLoaded", function () {
  const loginButton = document.querySelector("button[type='submit']");

  loginButton.addEventListener("click", async function (event) {
    event.preventDefault();

    // select email and password for login
    const email = document.querySelector("input[name='email']").value;
    const password = document.querySelector("input[name='psw']").value;

    //check if fields have been filled
    let emptyFields = false;
    if (email === "") {
      document.querySelector("input[name='email']").style.border =
        "2px solid red";
      document.getElementById("emailErrorMessage").innerHTML =
        "&#9888 Email is required";
      emptyFields = true;
    }

    if (password === "") {
      document.querySelector("input[name='psw']").style.border =
        "2px solid red";
      document.getElementById("passwordErrorMessage").innerHTML =
        "&#9888 Password is required";
      emptyFields = true;
    }

    if (emptyFields) {
      return;
    }

    const response = await fetch("http://localhost:5228/api/user/login", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        email: email,
        password: password,
      }),
      credentials: "include",
    });

    const data = await response.json();
    // check of the account exist in the db
    if (data.status) {
      localStorage.setItem("userId", data.id)
      window.location.href = "Index.html";
    } else {
      document.querySelector("input[name='email']").style.border =
        "2px solid red";
      document.getElementById("emailErrorMessage").innerHTML =
        "&#9888 Email or password is not correct";
      emptyFields = true;
    }
  });
});
