using System.Windows.Input;
using TabbedEditor.WorldEditor.Data;

namespace TabbedEditor.WorldEditor.Tools
{
    public class LandBrushTool : IWorldEditorTool
    {
        private WorldEditorControl _editor;

        public LandBrushTool(WorldEditorControl editor)
        {
            _editor = editor;
        }
        
        public void OnClick(WorldTileControl tileControl, MouseButtonEventArgs e)
        {
            tileControl.TileType = (TileType)_editor.TileTypeSelector.SelectedValue;
        }

        public void OnDeselect()
        {
            throw new System.NotImplementedException();
        }
    }
}