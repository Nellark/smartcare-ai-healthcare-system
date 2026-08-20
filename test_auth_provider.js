const http = require('http');

const data = JSON.stringify({ email: 'doctor@smartcare.local', password: 'Doctor@123' }); // I need to verify the actual doctor email

const options = {
  hostname: 'localhost',
  port: 5255,
  path: '/api/auth/login',
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
    console.log('Login Response:', json);
    
    if (json.data && json.data.accessToken) {
      const token = json.data.accessToken;
      
      const req2 = http.request({
        hostname: 'localhost',
        port: 5255,
        path: '/api/patients',
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${token}`
        }
      }, res2 => {
        console.log(`Patients endpoint status: ${res2.statusCode}`);
      });
      req2.end();
    }
  });
});

req.write(data);
req.end();
