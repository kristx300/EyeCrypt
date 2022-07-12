using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using EyeCrypt.App.Core;
using Newtonsoft.Json;

namespace EyeCrypt.App.Crypts.Rsa
{
    /// <summary>
    ///     https://docs.microsoft.com/ru-ru/dotnet/standard/security/encrypting-data#asymmetric-encryption
    ///     https://www.c-sharpcorner.com/UploadFile/75a48f/rsa-algorithm-with-C-Sharp2/
    /// </summary>
    public class RsaCrypt : ICrypt
    {
        private readonly Encoding _encoding = new UTF8Encoding(false);

        private readonly string _rsaParamsPath =
            Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "nosj.asr"
            );

        public RsaCrypt()
        {
            Init();
        }

        public string Encrypt(string text, string key)
        {
            using (var csp = new RSACryptoServiceProvider(2048))
            {
                csp.ImportParameters(Init());

                return Convert.ToBase64String(csp.Encrypt(_encoding.GetBytes(text), false));
            }
        }

        public string Decrypt(string text, string key)
        {
            using (var csp = new RSACryptoServiceProvider(2048))
            {
                csp.ImportParameters(Init());

                return _encoding.GetString(csp.Decrypt(Convert.FromBase64String(text), false));
            }
        }

        private RSAParameters Init()
        {
            //BAD PRACTICE, EXAMPLE ONLY
            //DO NOT USE IT IN PRODUCTION
            //DO NOT SAVE OR TRANSPORT PUBLIC KEY AS OR STRING
            using (var csp = new RSACryptoServiceProvider(2048))
            {
                if (!File.Exists(_rsaParamsPath))
                {
                    var parameters = csp.ExportParameters(true);
                    var fake = new FakeRSAParameters
                    {
                        Exponent = parameters.Exponent,
                        Modulus = parameters.Modulus,
                        P = parameters.P,
                        Q = parameters.Q,
                        DP = parameters.DP,
                        DQ = parameters.DQ,
                        InverseQ = parameters.InverseQ,
                        D = parameters.D
                    };
                    var buffer = JsonConvert.SerializeObject(fake);
                    File.WriteAllText(_rsaParamsPath, buffer, _encoding);
                    return parameters;
                }
                else
                {
                    var buffer = File.ReadAllText(_rsaParamsPath, _encoding);
                    var nonFake = JsonConvert.DeserializeObject<FakeRSAParameters>(buffer);
                    return new RSAParameters
                    {
                        Exponent = nonFake.Exponent,
                        Modulus = nonFake.Modulus,
                        P = nonFake.P,
                        Q = nonFake.Q,
                        DP = nonFake.DP,
                        DQ = nonFake.DQ,
                        InverseQ = nonFake.InverseQ,
                        D = nonFake.D
                    };
                }
            }
        }

        private struct FakeRSAParameters
        {
            public byte[] Exponent;
            public byte[] Modulus;
            public byte[] P;
            public byte[] Q;
            public byte[] DP;
            public byte[] DQ;
            public byte[] InverseQ;
            public byte[] D;
        }
    }
}