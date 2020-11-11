using System;
using Newtonsoft.Json;

namespace TabbedEditor.IO
{
    public class EditorFile
    {
        public readonly string Path;
        public readonly string Editor;

        public EditorFile(string path, Type editorType)
        {
            Path = path;
            Editor = editorType.ToString();
        }

        [JsonConstructor]
        public EditorFile(string path, string editor)
        {
            Path = path;
            Editor = editor;
        }

        public Type GetEditorType()
        {
            return Type.GetType(Editor);
        }
    }
}