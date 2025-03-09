document.addEventListener("DOMContentLoaded", function () {
  const loginButton = document.querySelector("button[type='submit']");

  loginButton.addEventListener("click", async function (event) {
    event.preventDefault();

    // select email and password for login
    const email = document.querySelector("input[name='email']").value;
    const password = document.querySelector("input[name='psw']").value;

    //check if fields have been filled
    if (!email || !password) {
      alert("Please fill in all fields");
      return;
    }

    
    const response = await fetch("http://localhost:5228/api/user/login", {
      method: "POST",  
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify({
        email: email,  
        password: password     
      })
    });
    

    const data = await response.json();
    // check of the account exist in the db
    if (data.status) {
      alert("Login successful!");
    } else {
      alert("Login failed: Email or password is not correct");
    }
  });
});
