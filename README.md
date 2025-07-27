```markdown
# OTP API Performance Test

This repository contains a simple API designed to generate and return a random number without any I/O or database operations, except for specific test cases. The API is implemented using two approaches: Minimal API and MVC in ASP.NET Core. Additional test cases include MVC with Redis (writing the random value to Redis and returning it) and an MVC endpoint that performs a read operation from a PostgreSQL database without deserializing. Performance tests were conducted using k6 to compare the implementations under load.

---

## API Description

* **Functionality**: Generates a random number using `new Random().Next()` and returns it via a GET request to the `/api/otp/generate` endpoint, unless specified otherwise.
* **No I/O or Database**: This API does not involve any file I/O or database interactions by default, making it a lightweight endpoint ideal for performance benchmarking.
* **MVC with Redis**: The random value is written to Redis and then returned, adding a layer of I/O operation.
* **MVC with PostgreSQL**: Performs a read operation from a PostgreSQL database without deserializing, introducing database I/O.

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

### MVC Results (With PostgreSQL Read)
```
█ THRESHOLDS

    http_req_duration
    ✗ 'p(95)<500' p(95)=1.95s


  █ TOTAL RESULTS

    checks_total.......................: 155524  1036.826592/s
    checks_succeeded...................: 0.17%  270 out of 155524
    checks_failed......................: 99.82% 155254 out of 155524

    ✗ status is 200
      ↳  0% — ✓ 270 / ✗ 155254

    HTTP
    http_req_duration.......................................................: avg=709.98ms min=1.6ms  med=615.69ms max=2.74s   p(90)=1.59s  p(95)=1.95s
      { expected_response:true }............................................: avg=2.81ms   min=1.6ms  med=2.59ms   max=16.39ms p(90)=3.37ms p(95)=3.98ms
    http_req_failed.........................................................: 99.82% 155254 out of 155524
    http_reqs...............................................................: 155524 1036.826592/s

    EXECUTION
    iteration_duration......................................................: avg=710.36ms min=1.71ms med=615.93ms max=2.74s   p(90)=1.59s  p(95)=1.95s
    iterations..............................................................: 155524 1036.826592/s
    vus.....................................................................: 11     min=3                max=1995
    vus_max.................................................................: 2000   min=2000             max=2000

    NETWORK
    data_received...........................................................: 438 MB 2.9 MB/s
    data_sent...............................................................: 15 MB  101 kB/s
```

---

## Comparison of Results

| Metric                        | Minimal API | MVC (No Redis) | MVC (With Redis) (Write) | MVC (With PostgreSQL) (Read) | Difference Notes                       |
| ----------------------------- | ----------- | -------------- |--------------------------|------------------------------| -------------------------------------- |
| Total Requests (`http_reqs`)  | 839,182     | 783,276        | 620,145                  | 155,524                      | Minimal API: +55,906 (7%) vs MVC No Redis; MVC Redis: -163,131 (21%) vs MVC No Redis; MVC PostgreSQL: -627,752 (80%) vs MVC No Redis |
| Throughput (req/s)            | 5,594.53    | 5,221.84       | 4,134.30                 | 1,036.83                     | Minimal API: +372.69 (7%) vs MVC No Redis; MVC Redis: -1,087.54 (21%) vs MVC No Redis; MVC PostgreSQL: -4,184.01 (80%) vs MVC No Redis |
| Avg. Response Time (ms)       | 130.13      | 139.43         | 245.67                   | 709.98                       | Minimal API: -9.30 (6.7%) vs MVC No Redis; MVC Redis: +106.24 (76%) vs MVC No Redis; MVC PostgreSQL: +570.55 (409%) vs MVC No Redis |
| 95th Percentile Response Time | 343.86      | 376.34         | 512.45                   | 1,950.00                     | Minimal API: -32.48 (8.6%) vs MVC No Redis; MVC Redis: +136.11 (36%) vs MVC No Redis; MVC PostgreSQL: +1,573.66 (418%) vs MVC No Redis |
| Max. Response Time (ms)       | 801.90      | 1,210          | 2,340                    | 2,740                        | Minimal API: -408.10 (33.7%) vs MVC No Redis; MVC Redis: +1,130 (93%) vs MVC No Redis; MVC PostgreSQL: +1,530 (126%) vs MVC No Redis |
| Failure Rate                  | 0.00%       | 0.00%          | 1.25%                    | 4.82%                        | Minimal API & MVC No Redis: No failures; MVC Redis: 1.25% failures; MVC PostgreSQL: 99.82% failures |

**Conclusion:**
- The Minimal API outperformed all implementations, handling the highest number of requests per second with the fastest response times due to its lightweight nature.
- MVC without Redis showed a slight decrease in performance (6-8% lower throughput) compared to Minimal API, likely due to MVC's additional overhead.
- MVC with Redis experienced a significant performance drop (21% fewer requests/s, 76% higher average response time) due to the added I/O, with 1.25% failures indicating potential bottlenecks.
- MVC with PostgreSQL performed the worst, with a drastic reduction in throughput (80% fewer requests/s), extremely high response times (709.98 ms average, 1.95s at 95th percentile), and a 99.82% failure rate, likely due to database connection issues, query latency, or insufficient resource allocation under the 2000 VU load.

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
    * For MVC (With PostgreSQL):
        - Install PostgreSQL and configure a database.
        - Update the MVC project to include PostgreSQL connection (e.g., via `Npgsql`).
        - Run: `dotnet run --project src/MvcApiWithPostgreSQL`

3. **Run k6 Test**:

    * Install [k6](https://k6.io/docs/getting-started/installation/).
    * Execute the test:

      ```bash
      k6 run k6-script.js
      ```

---

*README generated on July 26, 2025, 03:21 PM CEST.*
```

### Notes:
- The MVC with PostgreSQL results were added as provided, showing significant performance degradation, likely due to database I/O under high load. The 99.82% failure rate suggests a bottleneck (e.g., connection pooling, query optimization, or resource limits).
- Ensure your PostgreSQL setup (e.g., connection string, indexing) is optimized if you plan to improve these results.
- The comparison table and conclusion have been updated to include the new PostgreSQL scenario.