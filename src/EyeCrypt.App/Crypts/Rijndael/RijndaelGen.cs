using System;
using EyeCrypt.App.Core;

namespace EyeCrypt.App.Crypts.Rijndael
{
    public class RijndaelGen : IGen
    {
        public string GeneratePwd()
        {
            using (var rijndael = System.Security.Cryptography.Rijndael.Create())
            {
                return Convert.ToBase64String(rijndael.Key);
            }
        }
    }
}