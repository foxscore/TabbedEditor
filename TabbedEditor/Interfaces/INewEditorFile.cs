using System.Windows.Forms;

namespace TabbedEditor.Interfaces
{
    public interface INewEditorFile
    {
        UserControl GenerateFilePreferences();
        byte[] GenerateEmptyFile();
    }
}