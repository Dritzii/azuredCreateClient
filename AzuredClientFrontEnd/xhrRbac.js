const getBtn = document.getElementById('get-btn');
const postBtn = document.getElementById('post-btn');
const addBtn = document.getElementById('add-btn');

const sendHttpRequest = (method, url, data) => {
  const promise = new Promise((resolve, reject) => {
    const xhr = new XMLHttpRequest();
    xhr.open(method, url);
    xhr.responseType = 'json';
    if (data) {
      xhr.setRequestHeader('Access-Control-Allow-Origin', '*');
    }

    xhr.onload = () => {
      if (xhr.status >= 400) {
        reject(xhr.response);
      } else {
        resolve(xhr.response);
      }
    };

    xhr.onerror = () => {
      reject('Something went wrong!');
    };

    xhr.send(JSON.stringify(data));
  });
  return promise;
};

const addFirewall = () => {
    sendHttpRequest('POST', "https://azuredfwassapplicationcreation.azurewebsites.net/api/addFirewall?code=sCc4X5AqLOKl5_LucvG3TWaOXJyqbcqXWmH4q0kLZCBCAzFupnwbkQ==", {
        authToken: getCookie("authToken"),
        GraphauthToken: getCookie("GraphauthToken"),
        firewall : document.getElementById('fname').value
  })
    .then(responseData => {
      alert("Firewall added Successfully" ,responseData);
    })
        .catch(err => {
            console.log("Failed to add Firewall", err);
        });
};

const sendData = () => {
  sendHttpRequest('POST', "https://azuredfwassapplicationcreation.azurewebsites.net/api/TrimCodeLoginSetRbac?code=fJNBXNWkqOTVKNs9gkaO14RMG8CuafYI11/WvfBXbXX/Pu330cazPQ==", {
      authToken: getCookie("authToken"),
      GraphauthToken: getCookie("GraphauthToken")
})
  .then(responseData => {
    console.log(responseData);
  })
  .catch(err => {
    console.log(err);
  });
};

const addCompany = () => {
  sendHttpRequest('POST', "https://azuredfwassapplicationcreation.azurewebsites.net/api/AddCompany?code=GA-YqJ4vzn4TJkdov1f-lk5IARlNrZwfG-5VQr-44o6-AzFuw3tfTA==", {
      authToken: getCookie("authToken"),
      GraphauthToken: getCookie("GraphauthToken")
})
  .then(responseData => {
    console.log("Adding Company Successful",responseData);
  })
  .catch(err => {
    console.log("Failed to add Company" ,err);
  });
};


function getCookie(cname) {
  let name = cname + "=";
  let ca = document.cookie.split(';');
  for(let i = 0; i < ca.length; i++) {
    let c = ca[i];
    while (c.charAt(0) == ' ') {
      c = c.substring(1);
    }
    if (c.indexOf(name) == 0) {
      return c.substring(name.length, c.length);
    }
  }
  return "";
};


function loginToken(accesscode) {
  document.cookie = "authToken" + "=" + accesscode;
};

getBtn.addEventListener('click', addFirewall);
postBtn.addEventListener('click', sendData);
addBtn.addEventListener('click', addCompany);