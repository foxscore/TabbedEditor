using System.Linq;
using TabbedEditor.WorldEditor.Data;

namespace TabbedEditor.WorldEditor
{
    public class WorldFile
    {
        private string _path = "";
        public string Name { get; private set; } = "";
        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                Name = value.Split('\\').Last();
            }
        }
        public WorldData Data;
        public bool UnsavedChanges = false;
    }
}