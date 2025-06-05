using System.Runtime.InteropServices;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml.Serialization;
using UnityEngine.Events;

public class SaveFile
{

    private readonly string fileName = "";
    private string data = "";
    const int ERROR_SHARING_VIOLATION = 32;
    const int ERROR_LOCK_VIOLATION = 33;
    const string ExceptionDir = "/Exceptions";
    private string ExceptionPath;

    public SaveFile(string fileName = "", string extension = "", bool saveInExceptions = false)
    {
        if (fileName == "") fileName = "gamedata";
        if (extension == "") extension = ".dat";
        this.fileName = Application.persistentDataPath + "/" + fileName + extension;
        if (saveInExceptions)
        {
            ExceptionPath = Application.persistentDataPath + ExceptionDir + "/" + fileName;
            if (!Directory.Exists(ExceptionPath))
                Directory.CreateDirectory(ExceptionPath);
            this.fileName = ExceptionPath + "/" + fileName + extension;
        }
    }

    /// <summary>
    /// Save the 'system internal storage' data into the file. Return TRUE when done without errors.
    /// </summary>
    public bool Save()
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream saveFile;
            if (File.Exists(fileName))
                saveFile = File.OpenWrite(fileName);
            else
                saveFile = File.Create(fileName);
            bf.Serialize(saveFile, data);
            saveFile.Close();
            return true;
        }
        catch (System.Exception e)
        {
            int errorCode = Marshal.GetHRForException(e) & ((1 << 16) - 1);
            string message = "File save failed";
            if (e is IOException && (errorCode == ERROR_SHARING_VIOLATION || errorCode == ERROR_LOCK_VIOLATION))
            {
                message += "\n Sharing or Lock violation";
            }
            data = fileName + " Got Exception saving file: CODE: " + errorCode + ", Stack Trace " + message;
            return false;
        }

    }

    public bool SaveSimple()
    {
        try
        {
            StreamWriter sw = new StreamWriter(fileName);
            sw.Write(data);
            sw.Close();
        }
        catch (System.Exception e)
        {
            //Debug.Log("Exception: " + e.Message);
            return false;
        }
        return true;
    }

    /// <summary>
    /// Append the 'system internal storage' data at the end of the current file content.    
    /// </summary>
    public bool Append()
    {
        try
        {
            string fileStorage = "";
            if (FileExists())
            {
                BinaryFormatter bf2 = new BinaryFormatter();
                FileStream openFile = File.Open(fileName, FileMode.Open);
                fileStorage = (string)bf2.Deserialize(openFile);
                openFile.Close();
            }
            else
            {
                fileStorage = data;
            }

            BinaryFormatter bf = new BinaryFormatter();
            FileStream saveFile = File.Create(fileName);
            bf.Serialize(saveFile, fileStorage);
            saveFile.Close();
            Dispose();
            return true;
        }
        catch (System.Exception)
        {
            return false;
        }


    }

    public void Write(string _data)
    {
        data = _data;
    }

    public void SaveWeightsToException()
    {
        string[] files = Directory.GetFiles(Application.streamingAssetsPath);
        string fileRecordPath = Application.streamingAssetsPath + "/records.json";
        if (!File.Exists(fileRecordPath))
            File.Create(fileRecordPath);
        //Clear all previous records
        File.WriteAllText(fileRecordPath, "");
        foreach (string file in files)
        {
            string filename = Path.GetFileName(file);
            //We only need to copy csv files to persistence data
            if (Path.GetExtension(filename) == ".csv")
            {

                File.AppendAllText(fileRecordPath, filename + "~");
            }
        }
        string[] filesToCopy = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "records.json")).Split('~');
        foreach (string fileName in filesToCopy)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                string filePath = Path.Combine(ExceptionPath, fileName);
                byte[] data = File.ReadAllBytes(Path.Combine(Application.streamingAssetsPath, fileName));
                File.WriteAllBytes(filePath, data);
            }
        }
    }

    public bool Load(out string data)
    {
        if (!FileExists())
        {
            data = "File does not exist at the location";
            return false;
        }
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream saveFile = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

            data = (string)bf.Deserialize(saveFile);
            saveFile.Close();
            return true;
        }
        catch (System.Exception e)
        {
            int errorCode = Marshal.GetHRForException(e) & ((1 << 16) - 1);
            string message = "File read failed";
            if (e is IOException && (errorCode == ERROR_SHARING_VIOLATION || errorCode == ERROR_LOCK_VIOLATION))
            {
                message += "\n Sharing or Lock violation";
            }
            //Debug.Log(e.GetType().ToString());
            data = fileName + " Got Exception reading file: CODE: " + errorCode + ", Stack Trace " + message;
            return false;
        }

    }
    private const char CR = '\r';
    private const char LF = '\n';
    private const char NULL = (char)0;
    private const char NC = ',';
    public bool LoadSimple(out System.Collections.Generic.List<float> numbers, out int dbIndex)
    {
        numbers = new System.Collections.Generic.List<float>();
        dbIndex = 0;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        try
        {
            FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            //StreamReader sr = new StreamReader(fileName);
            var byteBuffer = new byte[1024 * 1024];
            int bytesRead = 0;
            bool first = true;
            while ((bytesRead = fs.Read(byteBuffer, 0, byteBuffer.Length)) > 0)
            {
                for (var i = 0; i < bytesRead; i++)
                {
                    char currentChar = (char)byteBuffer[i];
                    switch (currentChar)
                    {

                        case NULL:
                            continue;
                        case NC:
                        case LF:
                            //Read this value into float and continur
                            if (first)
                            {
                                dbIndex = int.Parse(sb.ToString());
                                sb.Clear();
                                first = false;
                            }
                            else if (sb.Length > 0)
                            {
                                numbers.Add(float.Parse(sb.ToString()));
                                sb.Clear();
                            }
                            break;
                        default:
                            sb.Append(currentChar);
                            break;
                    }
                }
             //   if (sb.Length > 0)
             //   {
             //       numbers.Add(float.Parse(sb.ToString()));
             //       sb.Clear();
             //   }
            }
            return true;
        }
        catch (System.Exception e)
        {
            Debug.Log("Exception " + sb + "-- " + e.ToString());
            return false;
        }
    }

    /// <summary>
    /// Return TRUE if the file exists.
    /// </summary>
    /// <returns></returns>
    public bool FileExists()
    {
        return File.Exists(fileName);
    }

    /// <summary>
    /// Delete this file (if it exists).
    /// </summary>
    public void Delete()
    {
        if (FileExists())
        {
            File.Delete(fileName);
            Dispose();
        }
    }

    /// <summary>
    /// Return the current file name with path.
    /// </summary>
    /// <returns></returns>
    public string GetFileName()
    {
        return fileName;
    }
    /// <summary>
    /// Returns the file path for exception data
    /// </summary>
    /// <returns></returns>
    public string GetExceptionPath()
    {
        return ExceptionPath;
    }

    /// <summary>
    /// Delete the 'system internal storage' so to free this part of memory. This method is automatically called after saving. It should be manually called after loading data.
    /// </summary>
    public void Dispose()
    {
        data = "";
    }
}
