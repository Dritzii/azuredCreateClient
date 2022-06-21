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
    sendHttpRequest('POST', "https://azuredfwassapplicationcreation.azurewebsites.net/api/DeleteRbacFromSubscriptioncs?code=1SCZRt5OPo482v4mPrDRvPqtHvoBIjVRD8BShwjKD8txkVLAgw5a4g==", {
    accessToken: getCookie("authToken"),
    roleName: document.getElementById('roles').value
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


getBtn.addEventListener('click', getData);
getMe1.addEventListener('click', getMe);
