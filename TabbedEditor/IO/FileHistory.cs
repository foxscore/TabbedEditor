using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using TabbedEditor.Interfaces;
using TabbedEditor.TargaViewer;
using TabbedEditor.TextEditor;
using TabbedEditor.WorldEditor;

namespace TabbedEditor.IO
{
    internal class SerializableFileHistory
    {
        public EditorFile[] Entries;
        
        [JsonConstructor]
        public SerializableFileHistory(EditorFile[] entries)
        {
            Entries = entries;
        }
    }
    
    public static class FileHistory
    {
        private static readonly string HistoryFile = "files.history";
        private static List<EditorFile> _history = new List<EditorFile>();

        public static event Action FileHistoryChanged;

        private static Dictionary<string, string> EditorName = new Dictionary<string, string>()
        {
            { typeof(TargaViewerControl).ToString(), "Targa" },
            { typeof(TextEditorControl).ToString(), "Text" },
            { typeof(WorldEditorControl).ToString(), "World" }
        };

        public static string GetEditorName(string type)
        {
            // if (!typeof(IEditorControl).IsAssignableFrom(type)) return "NULL";
            return EditorName.ContainsKey(type) ? EditorName[type] : "NULL";
        }
        
        public static void Load()
        {
            try
            {
                if (AppData.FileExists(HistoryFile))
                {
                    _history = JsonConvert.DeserializeObject<SerializableFileHistory>(AppData.ReadFile(HistoryFile)).Entries.ToList();
                }
            }
            catch (Exception e)
            {
                if (MessageBox.Show("Could not load file history!\nDo you want to reset it?", "Tabbed Editor - History", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    ResetHistory();
                Logger.DumpException(e);
            }
            FileHistoryChanged?.Invoke();
        }

        public static void ResetHistory()
        {
            AppData.DeleteFile(HistoryFile);
            _history.Clear();
            FileHistoryChanged?.Invoke();
        }

        private static void Save()
        {
            try
            {
                AppData.WriteFile(HistoryFile, JsonConvert.SerializeObject(new SerializableFileHistory(_history.ToArray())));
            }
            catch (Exception e)
            {
                MessageBox.Show("Could not save file history!\nNo changes have been made to the file.");
                Logger.DumpException(e);
            }
        }

        public static void Add(string path, Type type)
        {
            EditorFile editorFile = _history.FirstOrDefault(e => e.Path == path);

            if (editorFile is null)
            {
                _history.Insert(0, new EditorFile(path, type));
                while (_history.Count > 10) 
                    _history.RemoveAt(10);
            }
            else
            {
                _history.Remove(editorFile);
                _history.Insert(0, editorFile);
            }

            Save();
            FileHistoryChanged?.Invoke();
        }

        public static void Remove(EditorFile editorFile)
        {
            _history.Remove(editorFile);
            Save();
            FileHistoryChanged?.Invoke();
        }

        public static void RemoveAt(int index)
        {
            if (index >= _history.Count)
                return;
            _history.RemoveAt(index);
            Save();
            FileHistoryChanged?.Invoke();
        }

        public static EditorFile[] GenerateList()
        {
            try
            {
                return _history.ToArray();
            }
            catch (Exception e)
            {
                MessageBox.Show("Could not load file history!\nResetting history automatically.");
                Logger.DumpException(e);
                return new EditorFile[0];
            }
        }
    }
}