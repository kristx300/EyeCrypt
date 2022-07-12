namespace EyeCrypt.App.Core
{
    public interface IApplication
    {
        string GeneratePwd();
        void SetupKey(string key);
        void Add(string title, string pwd = null);
        string Show(int index);
        string[] ShowItems();
    }
}