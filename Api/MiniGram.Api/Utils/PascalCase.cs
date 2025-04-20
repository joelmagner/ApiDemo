namespace MiniGram.Api.Utils;

public static class PascalCase
{
    public static string ToPascalCase(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        return char.ToUpper(str[0]) + str[1..];
    }
}
