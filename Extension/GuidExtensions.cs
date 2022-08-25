using System.Net;
using Social_Setting.Exception;

namespace Social_Setting.Extension;

public static class GuidExtensions
{
    public static Guid ParseOrThrow(string stringId)
    {
        try
        {
            return Guid.Parse(stringId);
        }
        catch (System.Exception e)
        {
            throw new SocialSettingException(HttpStatusCode.BadRequest, $"ID {stringId} is invalid.");
        }
    }
}