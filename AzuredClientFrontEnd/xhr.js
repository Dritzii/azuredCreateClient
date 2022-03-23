//const getBtn = document.getElementById('get-btn');
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
  sendHttpRequest('POST', "https://azuredmicrosoftidentityclient.azurewebsites.net/api/TrimCode?code=gV8OiNUZ4gvcMNcpCCvv4P5l82aZ0XImO7LfU9CG6McHfbugXGTeUQ==", {
    code: currentUrl
  })
    .then(responseData => {
      console.log(responseData);
    })
    .catch(err => {
      console.log(err);
    });
};

//getBtn.addEventListener('click', getData);
postBtn.addEventListener('click', sendData);