using System.Text;

public static class EncryptionManager
{
    public static string EncryptDecrypt(string data, int encryptionKey)
    {
        StringBuilder sbIn = new(data);
        StringBuilder sbOut = new(data.Length);
        char ch;
        for (int i = 0; i < data.Length; i++)
        {
            ch = sbIn[i];
            ch = (char)(ch ^ encryptionKey);
            sbOut.Append(ch);
        }
        return sbOut.ToString();
    }
}
