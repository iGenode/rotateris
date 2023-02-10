using System.IO;
using UnityEngine;

public static class SaveManager
{
    private const string _fileName = "save.data";

    // TODO: return bool and handle unsuccessful write attempts
    // TODO: add try catch to hanlde exceptions
    public static void WriteSaveData(string data)
    {
        Debug.Log(Application.persistentDataPath);
        var path = $"{Application.persistentDataPath}/{_fileName}";
        FileStream fcreate = File.Open(path, FileMode.Create); // will create the file or overwrite it if it already exists
        using StreamWriter streamWriter = new(fcreate);
        streamWriter.Write(data);
    }

    public static string ReadSaveData()
    {
        var path = $"{Application.persistentDataPath}/{_fileName}";
        if (!File.Exists(path))
        {
            return null;
        }
        using StreamReader streamReader = new(path);
        return streamReader.ReadToEnd();
    }
}
