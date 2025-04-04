using System;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Editor
{
    public static class CereBroServerImporter
    {
        private static ListRequest _request;
        
        private const string PackageName = "com.cerebro.unity";
        
        [MenuItem("CereBro/Import Server")]
        public static void ImportServer()
        {
            _request = Client.List();    // List packages installed for the project
            EditorApplication.update += Progress;
        }

        private static void Progress()
        {
            if (!_request.IsCompleted)
            {
                return;
            }
            
            try
            {
                switch (_request.Status)
                {
                    case StatusCode.Success:
                        foreach (PackageInfo package in _request.Result)
                        {
                            if (package.name == PackageName)
                            {
                                string folder = "CereBro.Server.Unity";
                                
                                string source = Path.Combine(package.resolvedPath, $"{folder}~");
                                
                                string destination =
                                    Path.GetFullPath(Path.Combine(Application.dataPath, $"../{folder}"));

                                CopyDirectory(source, destination);
                            }
                        }
                        break;
                    case StatusCode.Failure:
                        Debug.LogError($"Error Listing packages: {_request.Error.message}");
                        break;
                }
            }
            finally
            {
                EditorApplication.update -= Progress;
            }
        }
        
        private static void CopyDirectory(string source, string destination, bool copyAsNew = true, bool overwriteFile = true)
        {
            DirectoryInfo sourceDirectory = new DirectoryInfo(source);
        
            DirectoryInfo destinationDirectory = new DirectoryInfo(destination);

            if (copyAsNew)
            {
                if (destinationDirectory.Exists)
                {
                    destinationDirectory.Delete(true);
                }
            
                Directory.CreateDirectory(destination);
            }
        
            DirectoryInfo[] subDirectories = sourceDirectory.GetDirectories();

            foreach (FileInfo file in sourceDirectory.GetFiles())
            {
                file.CopyTo(Path.Combine(destination, file.Name), overwriteFile);
            }

            // Copy Recursively
            foreach (DirectoryInfo directory in subDirectories)
            {
                CopyDirectory(directory.FullName, Path.Combine(destination, directory.Name));
            }
        }
    }
}