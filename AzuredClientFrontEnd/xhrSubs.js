const getBtn = document.getElementById('get-btn');
//const postBtn = document.getElementById('post-btn');
var currentUrl = window.location.href;
const getMe1 = document.getElementById('getMe');

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

const getData = () => {
    sendHttpRequest('POST', "https://azuredmicrosoftidentityclient.azurewebsites.net/api/SubscriptionsTrigger?code=tHNvW3n0BnP0XdUF4c6x7qMEh0fD5oUMBMTIMmX1nqXYmpLbPY8Stw==", {
    accessToken: getCookie("authToken")
  })
    .then(responseData => {
      console.log(responseData);
    })
    .catch(err => {
      console.log(err);
    });
};

const sendData = () => {
    sendHttpRequest('POST', "https://azuredmicrosoftidentityclient.azurewebsites.net/api/TrimCodeLoginSetRbac?code=96KgteUBMuyE8rcluDzFogZ4ybI8vXkHLWu6cS3xlvxkpXXaDcoqaA==", {
        accessToken: getCookie("authToken")
  })
    .then(responseData => {
      console.log(responseData);
    })
    .catch(err => {
      console.log(err);
    });
};

const getMe = () => {
    sendHttpRequest('POST', "https://azuredmicrosoftidentityclient.azurewebsites.net/api/MicrosoftGraph?code=FpekmviDi0eUq01DYhHeS8QDr7VyBRfyE/HfBJ/WWVmbqfnIMxS9oA==", {
        code: getCookie("GraphauthToken")
    })
        .then(responseData => {
            console.log(responseData);
        })
        .catch(err => {
            console.log(err);
        });
};

const sendDataAccessToken = () => {
    sendHttpRequest('POST', "https://azuredmicrosoftidentityclient.azurewebsites.net/api/CustomerLoginTrigger?code=HHYTmBoEzQ3MYo31UV3viBvBzThw54gx8QdotTPXWWonLAWmx9Hq6g==", {
        code: getCookie("urlcode")
})
  .then(responseData => {
    var accessTokenManagement = JSON.parse(responseData);
    console.log(accessTokenManagement);
      document.cookie = "authToken=" + accessTokenManagement.accessToken;
      document.cookie = "authTokenRefresh=" + accessTokenManagement.managementRefresh;
      document.cookie = "GraphauthToken=" + accessTokenManagement.graphToken;
      document.cookie = "GraphauthTokenRefresh=" + accessTokenManagement.graphRefresh;
  })
  .catch(err => {
    console.log(err);
  });
};

const delData = (role) => {
    sendHttpRequest('POST', "https://azuredmicrosoftidentityclient.azurewebsites.net/api/DeleteRbacFromSubscriptioncs?code=uM7P/n78ewKLjS5Gz7xciHggzRLOnzlNoDbqlzKWxOXi2WRR05oJgg==", {
        accessToken: getCookie("authToken"),
        roleName: role
    })
        .then(responseData => {
            console.log(responseData);
        })
        .catch(err => {
            console.log(err);
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
}

function buildTableSubscriptions() {
        var users = getData();
        if (xhr.readyState == 4 && xhr.status == "200") {

            for (i = 0; i < users.length; i++) {
                var table = document.getElementById("myTable");
                var row = table.insertRow(0);
                var cell1 = row.insertCell(0);
                var cell2 = row.insertCell(1);
                cell1.innerHTML = users[i].value1;
                cell2.innerHTML = users[i].value2;
            }
        } else {
            console.error(users);
        }
    }

function loginToken(accesscode) {
  document.cookie = "authToken" + "=" + accesscode;
}
function checkCookie() {
  document.cookie = "urlcode" + "=" + window.location.href;
  sendDataAccessToken();
}

getBtn.addEventListener('click', getData);
//postBtn.addEventListener('click', sendData);
getMe1.addEventListener('click', getMe);
