var currentUrl = window.location.href;

const sendHttpRequest = (method, url, data) => {
  const promise = new Promise((resolve, reject) => {
    const xhr = new XMLHttpRequest();
    xhr.open(method, url);
    xhr.responseType = 'json';
    xhr.setRequestHeader('Access-Control-Allow-Origin', '*');
    //if (data) {
    //  xhr.setRequestHeader('Access-Control-Allow-Origin', '*');
   // }

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

const sendDataAccessToken = () => {
    sendHttpRequest('POST', "https://azuredfwassapplicationcreation.azurewebsites.net/api/CustomerLoginTrigger?code=VdNpeT7r1CQeP3xeBaHobwNCqSxe0h5z7rY6/R87pnaPNFDKb33Zog==", {
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