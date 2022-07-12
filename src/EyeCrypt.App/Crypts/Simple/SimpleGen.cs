using System;
using System.Text;
using EyeCrypt.App.Core;

namespace EyeCrypt.App.Crypts.Simple
{
    public class SimpleGen : IGen
    {
        private const int Size = 12;

        private readonly char[] _alphabet =
        {
            '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '-', '=',
            'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', '[', ']',
            'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', ';', '\'', '\\', '|',
            'z', 'x', 'c', 'v', 'b', 'n', 'm', ',', '.', '/', '`',

            '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_', '+',
            'Q', 'W', 'E', 'R', 'T', 'Y', 'U', 'I', 'O', 'P', '{', '}',
            'A', 'S', 'D', 'F', 'G', 'H', 'J', 'K', 'L', ':', '"',
            'Z', 'X', 'C', 'V', 'B', 'N', 'M', '<', '>', '?', '~'
        };

        private readonly Random _random = new Random();

        public string GeneratePwd()
        {
            var pswd = new StringBuilder();
            for (var i = 0; i < Size; i += 2)
            {
                pswd.Append(_alphabet.GetValue(_random.Next(0, _alphabet.Length)));
                pswd.Append(_alphabet.GetValue(_random.Next(0, _alphabet.Length)));
            }

            return pswd.ToString();
        }
    }
}