using System;

namespace TabbedEditor.Interfaces
{
    public interface IEditorControl
    {
        event Action<string> TitleChangedEvent;
        string FilePath { get; }
        bool UnsavedChanges { get; }
        InspectorContent InspectorContent { get; }
        /// <returns>If null, initialization was successful. Otherwise, returns error message.</returns>
        Exception Open(string path);
        void SaveAs();
        void Save();
    }
}