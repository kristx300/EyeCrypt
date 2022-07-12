using System;
using EyeCrypt.App.Core;
using EyeCrypt.App.Crypts.Aes;
using EyeCrypt.App.Crypts.Rijndael;
using EyeCrypt.App.Crypts.Rsa;
using NUnit.Framework;

namespace EyeCrypt.Tests
{
    [TestFixture]
    public class CryptTests
    {
        private const string Lorem = "Lorem ipsum dolor sit amet, consectetur adipiscing elit";
        private const string Key = "loremkey";

        [TestCase(typeof(RijndaelCrypt))]
        [TestCase(typeof(AesCrypt))]
        [TestCase(typeof(RsaCrypt))]
        public void SimpleTest(Type testType)
        {
            testType.Name.Log();
            var instance = Activator.CreateInstance(testType);
            Assert.IsInstanceOf<ICrypt>(instance);

            var crypt = instance as ICrypt;
            Assert.IsNotNull(crypt);

            var encrypted = crypt.Encrypt(Lorem, Key);
            encrypted.Log();

            var decrypted = crypt.Decrypt(encrypted, Key);
            decrypted.Log();

            Assert.AreEqual(Lorem, decrypted);
        }
    }

    public static class NUnitLogExt
    {
        public static void Log(this object obj)
        {
            TestContext.Progress.WriteLine(obj);
        }
    }
}