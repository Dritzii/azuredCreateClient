const getBtn = document.getElementById('get-btn');
const postBtn = document.getElementById('post-btn');
var currentUrl = window.location.href;

const sendHttpRequest = (method, url, data) => {
  const promise = new Promise((resolve, reject) => {
    const xhr = new XMLHttpRequest();
    xhr.open(method, url);

    xhr.responseType = 'json';

    if (data) {
      //xhr.setRequestHeader('Content-Type', 'application/json');
      xhr.setRequestHeader('Access-Control-Allow-Origin', '*');
      //xhr.setRequestHeader('Access-Control-Allow-Methods', 'GET, POST');
      //xhr.setRequestHeader('Access-Control-Allow-Headers', 'Origin, Content-Type');
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
  sendHttpRequest('GET', 'https://reqres.in/api/users').then(responseData => {
    console.log(responseData);
  });
};

const sendData = () => {
    sendHttpRequest('POST', "https://azuredmicrosoftidentityclient.azurewebsites.net/api/TrimCodeLogin?code=8tka7WSBJDZfQxH69q6vNaf82y5nxQn5D40Xz3l5vgcFTveCXr06yw==", {
    code: currentUrl
  })
    .then(responseData => {
      console.log(responseData);
    })
    .catch(err => {
      console.log(err);
    });
};

const DisplaySubscriptions = () => {
    var url = "";
    var xhr = new XMLHttpRequest()
    xhr.open('GET', url, true)
    xhr.onload = function () {
        var users = JSON.parse(xhr.responseText);
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
    xhr.send(null);

}

getBtn.addEventListener('click', getData);
postBtn.addEventListener('click', sendData);