using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace EyeCrypt.App.Core
{
    public class EyeApplication : IApplication
    {
        private const string FileNameKey = "lister";
        private readonly ICrypt _cryptMode;
        private readonly Encoding _encoding = new UTF8Encoding(false);
        private readonly IGen _gen;

        private readonly string _keyListPath =
            Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                FileNameKey
            );

        private readonly List<TitleValue> _keys = new List<TitleValue>();

        private string _key;

        public EyeApplication(ICrypt cryptMode, IGen gen)
        {
            _cryptMode = ThrowIfNull(cryptMode);
            _gen = ThrowIfNull(gen);
        }

        public EyeApplication(ICrypt cryptMode, string key, IGen gen)
        {
            _cryptMode = ThrowIfNull(cryptMode);
            _gen = ThrowIfNull(gen);
            Load(ThrowIfNullOrEmpty(key, "Key is empty or null"));
        }

        public string GeneratePwd()
        {
            return _gen.GeneratePwd();
        }

        public void SetupKey(string key)
        {
            Load(ThrowIfNullOrEmpty(key, "Key is empty or null"));
        }

        public void Add(string title, string pwd = null)
        {
            ThrowIfNullOrEmpty(title, "Title is missing");
            ThrowIfNullOrEmpty(_key, "Key is missing");
            if (string.IsNullOrWhiteSpace(pwd)) pwd = GeneratePwd();

            pwd = _cryptMode.Encrypt(pwd, _key);

            _keys.Add(new TitleValue { Title = title, Value = pwd });
            Save();
        }

        public string Show(int index)
        {
            if (index < _key.Length && index >= 0)
                return _keys[index].Decrypt(_cryptMode, _key);
            return string.Empty;
        }

        public string[] ShowItems()
        {
            return _keys.Select(x => x.Title).ToArray();
        }

        private static T ThrowIfNull<T>(T obj)
        {
            if (obj == null) throw new NullReferenceException($"Object {typeof(T).Name} is null.");
            return obj;
        }

        private static string ThrowIfNullOrEmpty(string obj, string msg)
        {
            if (string.IsNullOrWhiteSpace(obj)) throw new NullReferenceException($"Object is null. {msg}");
            return obj;
        }

        private void Load(string key)
        {
            if (File.Exists(_keyListPath))
            {
                var file = File.ReadAllText(_keyListPath, _encoding);
                var decrypted = _cryptMode.Decrypt(file, key);
                var deserialized = JsonConvert.DeserializeObject<List<TitleValue>>(decrypted);
                if (deserialized != null && deserialized.Count > 0)
                {
                    _keys.Clear();
                    _keys.AddRange(deserialized);
                }
            }

            _key = key;
        }

        private void Save()
        {
            if (_keys.Count <= 0) return;

            var serialized = JsonConvert.SerializeObject(_keys);
            var encrypted = _cryptMode.Encrypt(serialized, _key);
            File.WriteAllText(_keyListPath, encrypted, _encoding);
        }

        private class TitleValue
        {
            public string Title { get; set; }
            public string Value { get; set; }

            public string Decrypt(ICrypt crypt, string key)
            {
                return crypt.Decrypt(Value, key);
            }
        }
    }
}