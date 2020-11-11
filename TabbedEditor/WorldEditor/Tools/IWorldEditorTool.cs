using System.Windows.Input;

namespace TabbedEditor.WorldEditor.Tools
{
    public interface IWorldEditorTool
    {
        void OnClick(WorldTileControl tileControl, MouseButtonEventArgs e);
        void OnDeselect();
    }
}