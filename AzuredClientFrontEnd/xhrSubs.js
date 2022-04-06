const getBtn = document.getElementById('get-btn');
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
    sendHttpRequest('POST', "https://azuredfwassapplicationcreation.azurewebsites.net/api/SubscriptionsTrigger?code=k09bi79jXOYpLMZHqbCr6BkMI9JJTPOU/KLikz6jvYdBvtyJUdTaqA==", {
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
    sendHttpRequest('POST', "https://azuredfwassapplicationcreation.azurewebsites.net/api/MicrosoftGraph?code=y8Wb5CH9LwwUBVm0EPjxHTwnEVgBwGDrFnqSXQHS9pwBrBSCwbUakw==", {
        code: getCookie("GraphauthToken")
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



getBtn.addEventListener('click', getData);
getMe1.addEventListener('click', getMe);
