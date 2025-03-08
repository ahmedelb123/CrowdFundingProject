document.addEventListener("DOMContentLoaded", function () {
    const registerButton = document.querySelector("button[type='submit']");

    registerButton.addEventListener("click", async function (event) {
        event.preventDefault();

        const firstName = document.querySelector("input[name='fname']").value;
        const surname = document.querySelector("input[name='sname']").value;
        const email = document.querySelector("input[name='create-email']").value;
        const password = document.querySelector("input[name='create-psw']").value;

        //check if fields have been filled
        if (!firstName || !surname || !email || !password) {
            alert("Please fill in all fields");
            return;
        }

        //send registration to backend
        const response = await fetch("http://localhost:5000/api/user/createAccount", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ firstName, surname, email, password}),
        });

        const data = await response.json();

        if (response.ok) {
            alert("Registration successful!");
            window.location.href = "login.html"; //redirect new registered user to login
        } else {
            alert("Registration failed: " + data.error);
        }
    });
});