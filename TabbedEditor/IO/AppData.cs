using System;
using System.IO;

namespace TabbedEditor.IO
{
    public static class AppData
    {
        private static bool IsInitialized;
        private static string _root;
        
        public static void Init()
        {
            if (IsInitialized)
                return;
            IsInitialized = true;
            _root = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Fox_score/TabbedEditor/";
            Directory.CreateDirectory(_root);
            Directory.CreateDirectory(_root + "/Traceback");
        }

        public static bool FileExists(string name)
        {
            return File.Exists(_root + name);
        }

        public static void DeleteFile(string path) => File.Delete(_root + path);
        public static void DeleteDirectory(string path) => Directory.Delete(_root + path);
        
        public static void CreateFile(string path) => File.Create(_root + path);
        public static void CreateDirectory(string path) => Directory.CreateDirectory(_root + path);
        
        public static void WriteFile(string path, string contents) => File.WriteAllText(_root + path, contents);
        public static void WriteFile(string path, string[] contents) => File.WriteAllLines(_root + path, contents);

        public static string ReadFile(string path)
        {
            return File.ReadAllText(_root + path);
        }
    }
}