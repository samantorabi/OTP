using System.Data;
using Microsoft.AspNetCore.Mvc;
using SMS_OTP.Repository.Interfaces;

namespace SMS_OTP.Controller;

[Route("api/otp")]
public class OtpController(IRepositoryManager repository) : ControllerBase
{
    
    private int counter = 0;
    private const string QUERY = "SELECT * FROM my_table";
    [HttpGet("generate")]
    public async Task<IActionResult> Generate()
    {
        var sampleData = new Random().Next();
        //await repository.SetCacheAsync(counter.ToString(), sampleData);
        await repository.RunQueryAsync(QUERY);
        counter++;
        return Ok(sampleData);
    }

    [HttpPost("verify")]
    public IActionResult Verify()
    {
        return Ok();
    }
}