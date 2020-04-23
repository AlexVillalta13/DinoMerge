
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace DemiumGames.Notifications
{
    public class ResourcesStaticGenerator
    {

        [MenuItem("DG/UpdateResources")]
        static void CreateStaticImagesResources()
        {

            string path = "./Assets/Plugins/Android/res/drawable";
            string scriptPath = "./Assets/DemiumGames/Notifications/Resources.cs";
            List<string> fileList = new List<string>();
            foreach (string file in System.IO.Directory.GetFiles(path))
            {
                string aux = file.Substring(path.Length + 1);
                string fileName = "";
                for (int i = 0; i < aux.Length - 4; i++)
                {
                    fileName += aux[i];
                }
                if (fileName[fileName.Length - 1] != '.')
                {
                    fileList.Add(fileName);
                }
            }

            for (int i = 0; i < fileList.Count; i++)
            {
                Debug.Log(fileList[i]);
            }


            if (File.Exists(scriptPath))
            {
                File.Delete(scriptPath);
            }


            TextWriter textWriter = new StreamWriter(File.Create(scriptPath));
            textWriter.WriteLine("namespace DemiumGames.Notifications {");
            textWriter.WriteLine("public class Resources {");
            for (int i = 0; i < fileList.Count; i++)
            {
                textWriter.WriteLine("public static string " + fileList[i] + " = " + "\"" +
                    fileList[i] + "\";");
            }
            textWriter.WriteLine("}");
            textWriter.WriteLine("}");
            textWriter.Close();

            AssetDatabase.Refresh();

        }

    }
}