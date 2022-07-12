using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using EyeCrypt.App.Core;

namespace EyeCrypt.App.Crypts.Aes
{
    /// <summary>
    ///     Release
    ///     https://docs.microsoft.com/ru-ru/dotnet/standard/security/encrypting-data
    /// </summary>
    public class AesCrypt : ICrypt
    {
        private static readonly byte[] Salt =
            { 0x64, 0xad, 0xed, 0x55, 0x23, 0xec, 0xaa, 0x48, 0x58, 0x12, 0xa3, 0x7d, 0xef, 0x09, 0x68, 0x89 };

        private readonly Encoding _encoding = new UTF8Encoding(false);

        public string Encrypt(string text, string key)
        {
            using (var pdb = new Rfc2898DeriveBytes(_encoding.GetBytes(key), Salt, 128))
            {
                var encrypted = EncryptStringToBytes_Aes(
                    text,
                    pdb.GetBytes(32),
                    pdb.GetBytes(16));
                return Convert.ToBase64String(encrypted);
            }
        }

        public string Decrypt(string text, string key)
        {
            using (var pdb = new Rfc2898DeriveBytes(_encoding.GetBytes(key), Salt, 128))
            {
                var roundtrip = DecryptStringFromBytes_Aes(
                    Convert.FromBase64String(text),
                    pdb.GetBytes(32),
                    pdb.GetBytes(16));
                return roundtrip;
            }
        }

        private byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (var aesAlg = System.Security.Cryptography.Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt, _encoding))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }

                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        private string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (var aesAlg = System.Security.Cryptography.Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt, _encoding))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}