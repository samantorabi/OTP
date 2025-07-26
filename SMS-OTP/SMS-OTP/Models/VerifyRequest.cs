namespace SMS_OTP.Models;

public class VerifyRequest(string userId, int code)
{
    public string UserId { get; set; } = userId;
    public int Code { get; set; } = code;
}