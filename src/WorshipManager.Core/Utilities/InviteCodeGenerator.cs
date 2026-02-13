namespace WorshipManager.Core.Utilities;

public static class InviteCodeGenerator
{
    private static readonly Random _random = new();
    private const string Chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    public static string Generate(int length = 6)
    {
        return new string(Enumerable.Repeat(Chars, length)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
    }
}
