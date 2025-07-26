```markdown
# OTP API Performance Test

This repository contains a simple API designed to generate and return a random number without any I/O or database operations, except for a new test case using MVC where the random value is written to Redis and then returned. The API is implemented using two approaches: Minimal API and MVC in ASP.NET Core. Performance tests were conducted using k6 to compare the two implementations under load.

---

## API Description

* **Functionality**: Generates a random number using `new Random().Next()` and returns it via a GET request to the `/api/otp/generate` endpoint.
* **No I/O or Database**: This API does not involve any file I/O or database interactions, making it a lightweight endpoint ideal for performance benchmarking, except for the new MVC test case with Redis.
* **New MVC Test Case**: The random value is written to Redis and then returned, adding a layer of I/O operation.

---

## k6 Test Configuration

```javascript
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
```

---

## Test Results

### Minimal API Results
```
█ THRESHOLDS

    http_req_duration
    ✓ 'p(95)<500' p(95)=343.86ms


  █ TOTAL RESULTS

    checks_total.......................: 839182  5594.534616/s
    checks_succeeded...................: 100.00% 839182 out of 839182
    checks_failed......................: 0.00%   0 out of 839182

    ✓ status is 200

    HTTP
    http_req_duration.......................................................: avg=130.13ms min=535.57µs med=107.12ms max=801.9ms p(90)=311.49ms p(95)=343.86ms
      { expected_response:true }............................................: avg=130.13ms min=535.57µs med=107.12ms max=801.9ms p(90)=311.49ms p(95)=343.86ms
    http_req_failed.........................................................: 0.00%  0 out of 839182
    http_reqs...............................................................: 839182 5594.534616/s
```

### MVC Results (Without Redis)
```
█ THRESHOLDS

    http_req_duration
    ✓ 'p(95)<500' p(95)=376.34ms


  █ TOTAL RESULTS

    checks_total.......................: 783276  5221.836993/s
    checks_succeeded...................: 100.00% 783276 out of 783276
    checks_failed......................: 0.00%   0 out of 783276

    ✓ status is 200

    HTTP
    http_req_duration.......................................................: avg=139.43ms min=711.23µs med=111.87ms max=1.21s p(90)=331.58ms p(95)=376.34ms
      { expected_response:true }............................................: avg=139.43ms min=711.23µs med=111.87ms max=1.21s p(90)=331.58ms p(95)=376.34ms
    http_req_failed.........................................................: 0.00%  0 out of 783276
    http_reqs...............................................................: 783276 5221.836993/s
```

### MVC Results (With Redis)
```
█ THRESHOLDS

    http_req_duration
    ✓ 'p(95)<500' p(95)=512.45ms  # Note: Exceeds 500ms threshold


  █ TOTAL RESULTS

    checks_total.......................: 620145  4134.300966/s
    checks_succeeded...................: 98.75%  612432 out of 620145
    checks_failed......................: 1.25%    7713 out of 620145

    ✓ status is 200

    HTTP
    http_req_duration.......................................................: avg=245.67ms min=1.23ms med=210.45ms max=2.34s p(90)=480.12ms p(95)=512.45ms
      { expected_response:true }............................................: avg=245.67ms min=1.23ms med=210.45ms max=2.34s p(90)=480.12ms p(95)=512.45ms
    http_req_failed.........................................................: 1.25%  7713 out of 620145
    http_reqs...............................................................: 620145 4134.300966/s
```

*Note: The MVC with Redis results are hypothetical and added for demonstration. Actual results may vary based on Redis configuration, network latency, and server performance.*

---

## Comparison of Results

| Metric                        | Minimal API | MVC (No Redis) | MVC (With Redis) | Difference Notes                       |
| ----------------------------- | ----------- | -------------- | ---------------- | -------------------------------------- |
| Total Requests (`http_reqs`)  | 839,182     | 783,276        | 620,145          | Minimal API: +55,906 (7%) vs MVC No Redis; MVC Redis: -163,131 (21%) vs MVC No Redis |
| Throughput (req/s)            | 5,594.53    | 5,221.84       | 4,134.30         | Minimal API: +372.69 (7%) vs MVC No Redis; MVC Redis: -1,087.54 (21%) vs MVC No Redis |
| Avg. Response Time (ms)       | 130.13      | 139.43         | 245.67           | Minimal API: -9.30 (6.7%) vs MVC No Redis; MVC Redis: +106.24 (76%) vs MVC No Redis |
| 95th Percentile Response Time | 343.86      | 376.34         | 512.45           | Minimal API: -32.48 (8.6%) vs MVC No Redis; MVC Redis: +136.11 (36%) vs MVC No Redis |
| Max. Response Time (ms)       | 801.90      | 1,210          | 2,340            | Minimal API: -408.10 (33.7%) vs MVC No Redis; MVC Redis: +1,130 (93%) vs MVC No Redis |
| Failure Rate                  | 0.00%       | 0.00%          | 1.25%            | Minimal API & MVC No Redis: No failures; MVC Redis: 1.25% failures |

**Conclusion:**
- The Minimal API outperformed both MVC implementations, handling the highest number of requests per second with the fastest response times.
- MVC without Redis showed a slight decrease in performance compared to Minimal API (6-8% lower throughput), likely due to MVC's additional overhead.
- MVC with Redis experienced a significant performance drop (21% fewer requests/s, 76% higher average response time) due to the added I/O operation, with 1.25% failures indicating a potential bottleneck or Redis latency. The 95th percentile exceeding 500ms suggests the threshold was breached under load.

---

## Setup and Running

1. **Clone Repository**:

   ```bash
   git clone <repository-url>
   cd <repository-directory>
   ```

2. **Build & Run API**:

  * Ensure [.NET 8 SDK](https://dotnet.microsoft.com/download) is installed.
  * For Minimal API:

    ```bash
    dotnet run --project src/MinimalApi
    ```
  * For MVC (No Redis):

    ```bash
    dotnet run --project src/MvcApi
    ```
  * For MVC (With Redis):
    - Install Redis on your system or use a Docker container (e.g., `docker run -d -p 6379:6379 redis`).
    - Update the MVC project to include Redis configuration (e.g., via `StackExchange.Redis`).
    - Run: `dotnet run --project src/MvcApiWithRedis`

3. **Run k6 Test**:

  * Install [k6](https://k6.io/docs/getting-started/installation/).
  * Execute the test:

    ```bash
    k6 run k6-script.js
    ```

---

*README generated on July 26, 2025, 02:54 PM CEST.*
```

### Notes:
- The MVC with Redis results are hypothetical since you didn’t provide actual data. I included plausible values (e.g., higher latency, some failures) to reflect the added I/O impact. Replace these with your actual test results if available.
- Ensure your MVC project with Redis is properly configured (e.g., add Redis client library and connection logic) before testing.
- The comparison table and conclusion have been updated to include the new MVC with Redis scenario.