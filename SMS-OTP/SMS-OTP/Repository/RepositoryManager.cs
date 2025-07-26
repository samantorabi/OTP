using Npgsql;
using SMS_OTP.Repository.Interfaces;
using StackExchange.Redis;

namespace SMS_OTP.Repository;

public class RepositoryManager : IRepositoryManager
{
    private readonly NpgsqlConnection _pgConnection;
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _redisDb;

    public RepositoryManager(IConfiguration configuration)
    {
        
        var pgConnString = "Host=localhost;Port=5432;Username=user;Password=pass;";
        _pgConnection = new NpgsqlConnection(pgConnString);
        _pgConnection.Open();
        
        var uri = new Uri("redis://localhost:6379");
        var option = new ConfigurationOptions
        {
            AbortOnConnectFail = false,
        };
            
        option.EndPoints.Add(uri.Host, uri.Port);

        _redis = ConnectionMultiplexer.Connect(option);
        _redisDb = _redis.GetDatabase();
    }

    public async Task<T> GetCacheAsync<T>(string key)
    {
        var value = await _redisDb.StringGetAsync(key);
        if (!value.HasValue)
            return default!;

        return System.Text.Json.JsonSerializer.Deserialize<T>(value!)!;
    }

    public Task SetCacheAsync(string key, object value)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(value);
        return _redisDb.StringSetAsync(key, json);
    }

    public async Task RunQueryAsync(string query)
    {
        await using var cmd = new NpgsqlCommand(query, _pgConnection);
        await cmd.ExecuteNonQueryAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _pgConnection.DisposeAsync();
        _redis.Dispose();
    }
}