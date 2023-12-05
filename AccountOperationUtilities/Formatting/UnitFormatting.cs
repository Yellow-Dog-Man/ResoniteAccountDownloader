namespace AccountOperationUtilities.Formatting;

// Borrowed from Elements.Core with permission.
public static class UnitFormatting
{
    private static string[] suffixes = new string[6] { "B", "kB", "MB", "GB", "TB", "PB" };

    public static string? FormatBytes(double bytes, int decimalPlaces = 2)
    {
        string text = "";
        if (bytes == 0)
            return "0";

        if (bytes < 0)
            text = "-";

        bytes = Math.Abs(bytes);
        string[] array = suffixes;
        foreach (string text2 in array)
        {
            if (bytes < 1024.0 || text2 == suffixes[suffixes.Length - 1])
            {
                return text + bytes.ToString($"F{decimalPlaces}") + " " + text2;
            }

            bytes /= 1024.0;
        }

        return null;
    }
}
