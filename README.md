
# OTP API Performance Test

This repository features an API that generates and returns a random number via a GET request to `/api/otp/generate`. It compares four implementations—Minimal API, MVC, MVC with Redis, and MVC with PostgreSQL—under load using k6 tests to assess performance differences.

## API Description

- **Functionality**: Generates a random number using `new Random().Next()`.
- **Implementations**:
  - **Minimal API**: Lightweight, no I/O or database operations.
  - **MVC**: Standard MVC with framework overhead.
  - **MVC with Redis**: Writes the random value to Redis before returning.
  - **MVC with PostgreSQL**: Reads from PostgreSQL without deserializing.

## k6 Test Configuration

```javascript
export const options = {
  stages: [
    { duration: '30s', target: 100 },   // Ramp up to 100 users
    { duration: '1m', target: 1000 },   // Ramp up to 1000 users
    { duration: '30s', target: 2000 },  // Push to 2000 users
    { duration: '30s', target: 0 },     // Ramp down
  ],
  thresholds: {
    'http_req_duration': ['p(95)<500'], // 95% of requests under 500ms
  },
};
```

## Test Results Summary

### Minimal API
- **Throughput**: 5,594.53 req/s
- **Avg. Response Time**: 130.13 ms
- **95th Percentile**: 343.86 ms
- **Failure Rate**: 0.00%

### MVC (No Redis)
- **Throughput**: 5,221.84 req/s
- **Avg. Response Time**: 139.43 ms
- **95th Percentile**: 376.34 ms
- **Failure Rate**: 0.00%

### MVC with Redis
- **Throughput**: 4,134.30 req/s
- **Avg. Response Time**: 245.67 ms
- **95th Percentile**: 512.45 ms
- **Failure Rate**: 1.25%

### MVC with PostgreSQL
- **Throughput**: 1,036.83 req/s
- **Avg. Response Time**: 709.98 ms
- **95th Percentile**: 1,950.00 ms
- **Failure Rate**: 8.82%

## Comparison of Results

| Metric                        | Minimal API | MVC (No Redis) | MVC (With Redis) | MVC (With PostgreSQL) |
|-------------------------------|-------------|----------------|------------------|-----------------------|
| Total Requests (`http_reqs`)  | 839,182     | 783,276        | 620,145          | 155,524               |
| Throughput (req/s)            | 5,594.53    | 5,221.84       | 4,134.30         | 1,036.83              |
| Avg. Response Time (ms)       | 130.13      | 139.43         | 245.67           | 709.98                |
| 95th Percentile Response Time | 343.86      | 376.34         | 512.45           | 1,950.00              |
| Max. Response Time (ms)       | 801.90      | 1,210          | 2,340            | 2,740                 |
| Failure Rate                  | 0.00%       | 0.00%          | 1.25%            | 8.82%                |

**Conclusion**:
- **Minimal API** offers the best performance with the highest throughput and lowest response times due to its minimal overhead.
- **MVC (No Redis)** is slightly slower due to framework overhead.
- **MVC with Redis** sees reduced throughput and higher latency from I/O operations, with a small failure rate.
- **MVC with PostgreSQL** performs the worst, with significant latency and near-total failure under load, likely due to database bottlenecks.

## Setup and Running

1. **Clone Repository**:
   ```bash
   git clone <repository-url>
   cd <repository-directory>
   ```

2. **Build & Run API**:
   - Install [.NET 8 SDK](https://dotnet.microsoft.com/download).
   - **Minimal API**: 
     ```bash
     dotnet run --project src/MinimalApi
     ```
   - **MVC (No Redis)**: 
     ```bash
     dotnet run --project src/MvcApi
     ```
   - **MVC with Redis**:
     - Start Redis (e.g., `docker run -d -p 6379:6379 redis`).
     - Configure Redis in the project.
     - Run: 
       ```bash
       dotnet run --project src/MvcApiWithRedis
       ```
   - **MVC with PostgreSQL**:
     - Set up a PostgreSQL database.
     - Configure the connection in the project.
     - Run: 
       ```bash
       dotnet run --project src/MvcApiWithPostgreSQL
       ```

3. **Run k6 Test**:
   - Install [k6](https://k6.io/docs/getting-started/installation/).
   - Execute:
     ```bash
     k6 run k6-script.js
     ```
