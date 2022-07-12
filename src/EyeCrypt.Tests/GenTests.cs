using System;
using EyeCrypt.App.Core;
using EyeCrypt.App.Crypts.Rijndael;
using EyeCrypt.App.Crypts.Simple;
using NUnit.Framework;

namespace EyeCrypt.Tests
{
    [TestFixture]
    public class GenTests
    {
        [TestCase(typeof(RijndaelGen))]
        [TestCase(typeof(SimpleGen))]
        public void SimpleTest(Type testType)
        {
            testType.Name.Log();
            var instance = Activator.CreateInstance(testType);
            Assert.IsInstanceOf<IGen>(instance);

            var crypt = instance as IGen;
            Assert.IsNotNull(crypt);

            var pwd = crypt.GeneratePwd();
            pwd.Log();

            Assert.GreaterOrEqual(pwd.Length, 12);
        }
    }
}