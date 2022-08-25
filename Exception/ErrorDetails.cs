using System.Text.Json;

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
        return JsonSerializer.Serialize(this);
    }
}