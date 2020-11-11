namespace TabbedEditor.TextEditor
{
    internal class RawDataFile
    {
        private string _path = "";
        public string Name { get; private set; } = "";
        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                var paths = value.Split('\\');
                Name = paths[paths.Length - 1];
            }
        }
        public string Data = "";
        public bool UnsavedChanges = false;
    }
}
