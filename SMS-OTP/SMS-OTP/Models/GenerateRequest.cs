namespace SMS_OTP.Models;

public class GenerateRequest(string userId, int tenorId)
{
    public string UserId { get; set; } = userId;
    public int TenorId { get; set; } = tenorId;
}