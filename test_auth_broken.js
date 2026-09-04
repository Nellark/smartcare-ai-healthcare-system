const http = require('http');

const req3 = http.request({
  hostname: 'localhost',
  port: 5255,
  path: '/api/patients',
  method: 'GET',
  headers: {
    'Authorization': `Bearer null`
  }
}, res3 => {
  console.log(`Patients endpoint status: ${res3.statusCode}`);
});
req3.end();
