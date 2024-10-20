namespace WebServiceApp.Extensions;

public static class StringExtensions
{
    public static Guid? ToGuidNull(this string text)
    {
        if (Guid.TryParse(text, out var uid))
        {
            return uid;
        }

        return null;
    }
}