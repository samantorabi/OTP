import http from 'k6/http';
import { check } from 'k6';

export const options = {
  stages: [
    { duration: '30s', target: 100 },   // Ramp up to 100 users
    { duration: '1m', target: 1000 },   // Ramp up to 1000 users
    { duration: '30s', target: 2000 },  // Push to 2000 users to test limits
    { duration: '30s', target: 0 },     // Ramp down to 0
  ],
  thresholds: {
    'http_req_duration': ['p(95)<500'], // 95% of requests should be under 500ms
  },
};

export default function () {
  let res = http.get('http://host.docker.internal:8081/api/otp/generate');
  check(res, {
    'status is 200': (r) => r.status === 200,
  });
}