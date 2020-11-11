using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using TabbedEditor.Interfaces;
using TabbedEditor.IO;

namespace TabbedEditor.TextEditor
{
    /// <summary>
    /// Interaktionslogik für TextEditorControl.xaml
    /// </summary>
    public partial class TextEditorControl : IEditorControl
    {
        public string FilePath => _file.Path;

        public event Action<string> TitleChangedEvent;

        private RawDataFile _file = new RawDataFile();
        public InspectorContent InspectorContent { get; private set; }

        public TextEditorControl()
        {
            InitializeComponent();
            TextEditor.TextChanged += TextEditor_TextChanged;
        }

        private void TextEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            _file.Data = TextEditor.Text;
            _file.UnsavedChanges = true;
            UpdateTitle();
        }

        public bool UnsavedChanges
        {
            get
            {
                return _file.UnsavedChanges;
            }
        }

        /// <returns>If null, initialization was successful. Otherwise, returns error message.</returns>
        public Exception Open(string path)
        {
            try
            {
                _file.Path = path;
                _file.Data = File.ReadAllText(_file.Path);
                TextEditor.Text = _file.Data;
                _file.UnsavedChanges = false;
                UpdateTitle();
            }
            catch (Exception e)
            {
                Logger.DumpException(e);
                return e;
            }

            return null;
        }
        public void Save()
        {
            if (string.IsNullOrEmpty(_file.Path))
                SaveAs();
            else
                SaveFileToDisk();
        }
        public void SaveAs()
        {
            SaveFileDialog spf = new SaveFileDialog();
            spf.Filter = "";
            spf.FileOk += SaveFileDialog_FileOk;
            spf.ShowDialog();
        }
        private void SaveFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveFileDialog sfd = sender as SaveFileDialog;
            if (sfd != null) _file.Path = sfd.FileName;
            SaveFileToDisk();
        }

        public byte[] GenerateEmptyFile()
        {
            _file.UnsavedChanges = false;
            UpdateTitle();
            return new byte[0];
        }

        private void SaveFileToDisk()
        {
            try
            {
                File.WriteAllText(_file.Path, _file.Data);
                _file.UnsavedChanges = false;
                UpdateTitle();
            }
            catch (Exception e)
            {
                Logger.DumpException(e);
                MessageBox.Show("Failed to save File:\n\t" + e.Message);
            }
        }


        private void UpdateTitle()
        {
            string title = "";

            if (string.IsNullOrEmpty(_file.Path))
                title += "New File";
            else
                title += _file.Name;

            if (_file.UnsavedChanges)
                title += '*';

            TitleChangedEvent?.Invoke(title);
        }
    }
}
