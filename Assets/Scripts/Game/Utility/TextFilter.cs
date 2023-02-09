public static class TextFilter
{
    // Colorize
    private static string Colorize(string text, string color, bool bold) => $"<color=\"{color}\">{(bold ? Embolden(text) : text)}</color>";
    // Embolden
    private static string Embolden(string text) => $"<b>{text}</b>";
    // Yellow
    public static string Clrz_ylw(string text, bool bold = true) => Colorize(text, "yellow", bold);
    // Green
    public static string Clrz_grn(string text, bool bold = true) => Colorize(text, "green", bold);
    // Red
    public static string Clrz_red(string text, bool bold = true) => Colorize(text, "red", bold);
}
