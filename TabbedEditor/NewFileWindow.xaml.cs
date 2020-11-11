using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using TabbedEditor.IO;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;

namespace TabbedEditor
{
    public partial class NewFileWindow
    {
        // TODO New File window
        
        public static EditorFile ShowDialog(Type defaultFileCreator = null)
        {
            NewFileWindow nfw = defaultFileCreator is null ? new NewFileWindow() : new NewFileWindow(defaultFileCreator);
            ((Window) nfw).ShowDialog();
            if (nfw.DialogResult == true)
                MessageBox.Show("Creating file");
            // return new EditorFile(nfw.FilePath, nfw.EditorType);
            return null;
        }

        public string FilePath;
        public Type EditorType;

        public NewFileWindow() => BaseConstructor();

        // ReSharper disable once MemberCanBePrivate.Global
        public NewFileWindow(Type defaultFileCreator)
        {
            BaseConstructor();
            // TODO Set default component
        }

        private void BaseConstructor()
        {
            InitializeComponent();
            PreviewKeyDown += EscapeKeyHandler;
        }
        
        private void EscapeKeyHandler(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Escape) return;
            
            DialogResult = false;
            Close();
        }

        private void CreateFile()
        {
            // TODO Creation process
            // Do checks
                // Check if Path is valid
                // Check if target file ending is supported by selected Type
            // Run create file function of selected editor/file type
            DialogResult = true;
            Close();
        }
        
        private void Create(object sender, RoutedEventArgs e) => CreateFile();

        private void Cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        
        private void SelectFile(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            // TODO Add contextual filter
            sfd.FileOk += SaveFileDialogOnFileOk;
            sfd.ShowDialog();
        }

        private void SaveFileDialogOnFileOk(object sender, CancelEventArgs e)
        {
            PathText.Text = (sender as SaveFileDialog).FileName;
        }

        private void TypeSelector_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Check if selected type is compatible with already selected file location
                // Reset file location if not
            // Update Preferences GroupBox/Grid
        }
    }
}