document.addEventListener("DOMContentLoaded", function () {
  const registerButton = document.querySelector("button[type='submit']");
  const inputs = document.querySelectorAll("input");

  //resets the red border & error messages for when user starts typing - empty fields
  inputs.forEach((input) => {
    input.addEventListener("input", function () {
      this.style.border = "1px solid #ccc";

      //map input fields to their corresponding error message IDs
      const errorMessages = {
        fname: "nameErrorMessage",
        sname: "surnameErrorMessage",
        "create-email": "emailErrorMessage",
        "create-psw": "passwordErrorMessage",
      };

      //get the error message element corresponding to the current input
      const errorMessageId = errorMessages[this.name];
      if (errorMessageId) {
        const errorMessageElement = document.getElementById(errorMessageId);
        if (errorMessageElement) {
          errorMessageElement.innerHTML = "";
        }
      }
    });
  });

  //function to check valid email format
  function validEmail(email) {
    const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    return emailRegex.test(email);
  }

  //function to check for strong password creation
  function strongPassword(password) {
    const passwordRegex =
      /^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
    return passwordRegex.test(password);
  }

  registerButton.addEventListener("click", async function (event) {
    event.preventDefault();
    //select all the info for the registration
    const name = document.querySelector("input[name='fname']").value;
    const surname = document.querySelector("input[name='sname']").value;
    const email = document.querySelector("input[name='create-email']").value;
    const password = document.querySelector("input[name='create-psw']").value;

    //check if input fields are empty
    let emptyFields = false;
    if (name === "") {
      document.querySelector("input[name='fname']").style.border =
        "2px solid red";
      document.getElementById("nameErrorMessage").innerHTML =
        "&#9888 Name is required";
      emptyFields = true;
    }

    if (surname === "") {
      document.querySelector("input[name='sname']").style.border =
        "2px solid red";
      document.getElementById("surnameErrorMessage").innerHTML =
        "&#9888 Surname is required";
      emptyFields = true;
    }

    if (email === "") {
      document.querySelector("input[name='create-email']").style.border =
        "2px solid red";
      document.getElementById("emailErrorMessage").innerHTML =
        "&#9888 Email is required";
      emptyFields = true;
    }

    if (password === "") {
      document.querySelector("input[name='create-psw']").style.border =
        "2px solid red";
      document.getElementById("passwordErrorMessage").innerHTML =
        "&#9888 Password is required";
      emptyFields = true;
    }

    if (emptyFields) {
      return;
    }

    //check if email is valid
    if (!validEmail(email)) {
      document.getElementById("emailErrorMessage").innerHTML =
        "&#9888 Please enter a valid email";
      return;
    }

    //check if password is strong
    if (!strongPassword(password)) {
      document.getElementById("passwordErrorMessage").innerHTML =
        "&#9888 Please ensure password follows requirements";
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
        credentials: "include",
      }
    );

    const data = await response.json();
    //check if the account has been created in db
    if (response.ok) {
      window.location.href = "login.html";
    } else {
      document.querySelector("input[name='create-email']").style.border =
        "2px solid red";
      document.getElementById("emailErrorMessage").innerHTML =
        "&#9888 Email Already Used. Try another email or try to login";
    }
  });
});
