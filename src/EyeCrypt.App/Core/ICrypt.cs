namespace EyeCrypt.App.Core
{
    public interface ICrypt
    {
        string Encrypt(string text, string key);
        string Decrypt(string text, string key);
    }
}