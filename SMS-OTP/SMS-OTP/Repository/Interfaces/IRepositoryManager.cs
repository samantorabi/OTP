namespace SMS_OTP.Repository.Interfaces;

public interface IRepositoryManager
{
    Task<T> GetCacheAsync<T>(string key);
    Task SetCacheAsync(string key, object value);
    Task RunQueryAsync(string query);
}