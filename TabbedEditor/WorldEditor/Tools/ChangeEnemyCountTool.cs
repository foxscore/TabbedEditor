using System.Windows.Input;

namespace TabbedEditor.WorldEditor.Tools
{
    public class ChangeEnemyCountTool : IWorldEditorTool
    {
        // ReSharper disable once NotAccessedField.Local
        private WorldEditorControl _editor;
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private int _enemyDelta;

        public ChangeEnemyCountTool(WorldEditorControl editor)
        {
            _editor = editor;
        }
        
        public void OnClick(WorldTileControl tileControl, MouseButtonEventArgs e)
        {
            if (tileControl.EnemyCount + _enemyDelta < 0)
                tileControl.EnemyCount = 0;
            else
                tileControl.EnemyCount += _enemyDelta;
        }

        public void OnDeselect()
        {
            
        }
    }
}