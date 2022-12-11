using System.Text.Json;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Social_Setting.Exception;

public class ErrorDetails
{
    public int StatusCode { get; set; }
    public string Message { get; set; }

    public ErrorDetails()
    {
    }

    public ErrorDetails(SocialSettingException exception)
    {
        StatusCode = (int)exception.StatusCode;
        Message = exception.Message;
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }
}