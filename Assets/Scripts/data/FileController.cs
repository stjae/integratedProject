using UnityEngine;
using System.IO;

namespace Poly.Data
{
    public class FileController
    {
        // filepath under Application.persistentDataPath
        string filepath;
        public string Filepath { get { return filepath; } set { filepath = value; } }

        private string GetFullFilepath(string filepath) { return Application.persistentDataPath + "/" + filepath; }

        public string ReadFile()
        {
            string fullFilepath = GetFullFilepath(filepath);

            if (File.Exists(fullFilepath))
            {
                StreamReader streamReader = new StreamReader(fullFilepath);
                string data = streamReader.ReadToEnd();

                streamReader.Close();
                return data;
            }
            else
            {
                Debug.LogWarningFormat("File not found: {0}", fullFilepath);
                return null;
            }
        }

        public void WriteFile(string data)
        {
            string fullFilepath = GetFullFilepath(filepath);

            FileStream fileStream = new FileStream(fullFilepath, FileMode.Create);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            streamWriter.Write(data);

            streamWriter.Close();
            fileStream.Close();
        }

        public void DeleteFile()
        {
            string fullFilepath = GetFullFilepath(filepath);

            if (File.Exists(fullFilepath))
            {
                File.Delete(fullFilepath);
            }
            else
            {
                Debug.LogWarningFormat("File not found: {0}", fullFilepath);
            }
        }
    }
}