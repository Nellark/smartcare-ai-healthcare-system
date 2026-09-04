const http = require('http');

const data = JSON.stringify({ email: 'test_doctor@smartcare.local', password: 'Doctor@123', role: 'Doctor' });

const options = {
  hostname: 'localhost',
  port: 5255,
  path: '/api/auth/register',
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Content-Length': data.length
  }
};

const req = http.request(options, res => {
  let body = '';
  res.on('data', d => body += d);
  res.on('end', () => {
    const json = JSON.parse(body);
    console.log('Register Response:', json);
    
    // now login
    const loginData = JSON.stringify({ email: 'test_doctor@smartcare.local', password: 'Doctor@123' });
    const req2 = http.request({
      hostname: 'localhost',
      port: 5255,
      path: '/api/auth/login',
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Content-Length': loginData.length
      }
    }, res2 => {
      let body2 = '';
      res2.on('data', d => body2 += d);
      res2.on('end', () => {
        const loginJson = JSON.parse(body2);
        console.log('Login Response:', loginJson);
        const token = loginJson.data.accessToken;

        // now get patients
        const req3 = http.request({
          hostname: 'localhost',
          port: 5255,
          path: '/api/patients',
          method: 'GET',
          headers: {
            'Authorization': `Bearer ${token}`
          }
        }, res3 => {
          console.log(`Patients endpoint status: ${res3.statusCode}`);
        });
        req3.end();
      });
    });
    req2.write(loginData);
    req2.end();
  });
});
req.write(data);
req.end();
