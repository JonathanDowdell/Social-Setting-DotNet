using System.Net;

namespace Social_Setting.Exception;

public class SocialSettingException: System.Exception
{
    public readonly HttpStatusCode StatusCode;
    
    public SocialSettingException(string message): base(message)
    {
        this.StatusCode = HttpStatusCode.InternalServerError;
    }

    public SocialSettingException(HttpStatusCode statusCode, string message): base(message)
    {
        StatusCode = statusCode;
    }
}