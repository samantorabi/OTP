using Microsoft.AspNetCore.Mvc;

namespace SMS_OTP.Controller;

[Route("api1/otp")]
public class OtpController : ControllerBase
{
    /*[HttpPost("verify")]
    public IActionResult Verify([FromBody] VerifyObj verifyObj)
    {
        return Ok();
    }*/

    /*[HttpGet("generate")]
    public IActionResult Generate([FromBody] GenerateRequest request)
    {
        return Ok();
    }*/
    
    
    
    [HttpGet("generate")]
    public IActionResult Generate()
    {
        return Ok();
    }

    [HttpPost("verify")]
    public IActionResult Verify()
    {
        return Ok();
    }
}


public class GenerateRequest(string userId, int tenorId)
{
    public string UserId { get; set; } = userId;
    public int TenorId { get; set; } = tenorId;
}

public class VerifyRequest(string userId, int code)
{
    public string UserId { get; set; } = userId;
    public int Code { get; set; } = code;
}