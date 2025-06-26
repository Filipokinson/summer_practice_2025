using System;
using System.Linq;

public static class StringExtensions
{
    public static bool IsPalindrome(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }
        string cleaninput = new string(input.ToLower().Where(c => !char.IsWhiteSpace(c) && !char.IsPunctuation(c)).ToArray());

        if (string.IsNullOrEmpty(cleaninput))
        {
            return false;
        }
        string reverse = new string(cleaninput.Reverse().ToArray());

        return reverse == cleaninput;
    }
}
