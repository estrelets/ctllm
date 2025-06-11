namespace Lr;

public static class Utils
{
    public static class Message
    {
        public static string? CleanThinkBlock(string? str)
        {
            if (str == null)
                return null;

            var tag = "</think>";
            var index = str.LastIndexOf(tag);

            if (index == -1)
                return str;

            return str.Substring(index + tag.Length).TrimStart();
        }
    }
}
