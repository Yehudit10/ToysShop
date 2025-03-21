const signUp = async () => {
    const username = document.getElementById("username").value;
    const password = document.getElementById("password").value;
    const firstname = document.getElementById("firstname").value;
    const lastnamepp = document.getElementById("lastname").value;
    const response = await fetch("https://localhost:44386/api/users", {
        method: 'POST',
        body: JSON.stringify({ username, password, firstname, lastname }),
        headers: {
            "Content-Type":'application/json'
        }
    });
    if (!response.ok)
        throw new Error("Error in the server");
const user = await response.json();
    console.log(user);
    alert(`Welcome ${username}, you were successfuly registered`)
    window.location.assign('./login.html')
}

const login = async () => {
    const username = document.getElementById("username").value;
    const password = document.getElementById("password").value;
    const response = await fetch("https://localhost:44386/api/users/login", {
        method: 'POST',
        headers: {
            "Content-Type":'application/json'
        },
        body: JSON.stringify({ username, password })
    })
if (!response.ok)
    throw new Error("Http error. status:"+response.status);
    const user = await response.json();
    sessionStorage.setItem("user", JSON.stringify(user))
 alert(`Welcome ${user.userName}, you were successfuly logged in`)
  window.location.assign("./update.html")

}
const getAllUsers = async() => {
    const response = await fetch("https://localhost:44386/api/users");
    const users = await response.json();
    console.log(users);
}
const updateUser = async() => {
    const username = document.getElementById("username").value;
    const password = document.getElementById("password").value;
    const firstname = document.getElementById("firstname").value;
    const lastname = document.getElementById("lastname").value;
    const userId = JSON.parse(sessionStorage.getItem("user")).id
    const response = await fetch(`https://localhost:44386/api/users/${userId}`, {
        method: 'PUT',
        body: JSON.stringify({id:userId,username, password,firstname,lastname}),
        headers: {
            "Content-Type":"application/json"
        }
    })
    if (!response.ok)
        throw new Error("Http error. status:" + response.status);
    const updatedUser = await response.json()
    console.log(updatedUser)
}