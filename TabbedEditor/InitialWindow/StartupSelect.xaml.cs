using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using TabbedEditor.Interfaces;
using TabbedEditor.IO;
using TabbedEditor.TargaViewer;

namespace TabbedEditor.InitialWindow
{
    public partial class StartupSelect
    {
        public StartupSelect()
        {   
            InitializeComponent();
            LoadRecentFiles();
        }

        public void FocusOnLoaded(object a, object b) => Activate();

        void LoadRecentFiles()
        {
            AppData.Init();
            FileHistory.Load();
            FileHistory.FileHistoryChanged += UpdateRecentList;
            UpdateRecentList();
        }

        private void UpdateRecentList()
        {
            RecentList.Items.Clear();
            EditorFile[] history = FileHistory.GenerateList();
            
            if (history.Length == 0)
            {
                ListBoxItem disabled = new ComboBoxItem();
                disabled.Content = "File history is empty";
                disabled.Background = new SolidColorBrush(Colors.Gainsboro);
                disabled.HorizontalContentAlignment = HorizontalAlignment.Center;
                disabled.VerticalContentAlignment = VerticalAlignment.Center;
                disabled.Height = 200;
                disabled.IsEnabled = false;
                RecentList.Items.Add(disabled);
                return;
            }
            
            foreach (EditorFile entry in history)
            {
                ListBoxItem listEntry = new ComboBoxItem();
                listEntry.Content = $"[{FileHistory.GetEditorName(entry.Editor)}]\t    {entry.Path.Split('\\').Last()}";
                listEntry.ToolTip = entry.Path;
                listEntry.MouseDoubleClick += ListBoxItemOnMouseDoubleClick;
                listEntry.MouseRightButtonUp += ListEntryOnRightUp;
                listEntry.DataContext = entry;
                RecentList.Items.Add(listEntry);
            }
        }

        private void ListEntryOnRightUp(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem target = sender as ListBoxItem;

            EditorFile entry = (EditorFile) target.DataContext;
            if (MessageBox.Show(
                "Do you want to remove \"" + entry.Path.Split('\\').Last() + "\" from the recently opened files?",
                "Remove from history", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                FileHistory.Remove(entry);
        }

        private void ListBoxItemOnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem item = sender as ListBoxItem;
            EditorFile editorFile = item.DataContext as EditorFile;
            Type type = editorFile.GetEditorType();
            if (!typeof(IEditorControl).IsAssignableFrom(type))
            {
                if (MessageBox.Show("Invalid entry in history!\nDo you want to remove it from recently opened files?", "Invalid entry", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    FileHistory.Remove(editorFile);
                return;
            }
            if (!File.Exists(editorFile.Path))
            {
                if (MessageBox.Show("The file does not exist anymore!\nDo you want to remove it from recently opened files?", "Missing file", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    FileHistory.Remove(editorFile);
                return;
            }
            
            // TODO Check if file type is supported by editor before calling this part
            /*
            if (FileChecker.IsBinary(editorFile.Path))
            {
                if (MessageBox.Show("Binary files are not supported at this time.\nDo you want to remove it from recently opened files?", "File is binary", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    FileHistory.Remove(editorFile);
                return;
            }
            */
                
            if (FileChecker.IsTooBig(editorFile.Path))
                if (MessageBox.Show("Files with a size over 10MB are not supported.\nOpen anyways? (Program may freeze or stop working)", "File too big", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    return;
            
            new MainWindow(editorFile.Path, type).Show();
            Close();
        }

        private void CreateFileOnClick(object sender, RoutedEventArgs e)
        {
            EditorFile newFile = NewFileWindow.ShowDialog();
            if ((newFile is null))
                return;
            new MainWindow(newFile.Path, newFile.GetType()).Show();
            Close();
        }

        private void OpenFileOnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "World File (*.json)|*.json|Tagra Image (*.tga)|*.tga";
            ofd.FileOk += (o, args) =>
            {
                switch (ofd.FileName.Split('.').Last())
                {
                    case "tga":
                        new MainWindow(ofd.FileName, typeof(TargaViewerControl)).Show();
                        break;
                    case "json":
                        new MainWindow(ofd.FileName, typeof(WorldEditor.WorldEditorControl)).Show();
                        break;
                }
                Close();
            };
            ofd.ShowDialog();
        }

        private void OpenFileRAWOnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = FileChecker.FileDialogRawFilter;
            ofd.FileOk += (o, args) =>
            {
                if (FileChecker.IsBinary(ofd.FileName))
                {
                    MessageBox.Show("Binary files are not supported at this time.", "File is binary");
                    return;
                }
                
                if (FileChecker.IsTooBig(ofd.FileName))
                    if (MessageBox.Show("Files with a size over 10MB are not supported.\nOpen anyways? (Program may freeze or stop working)", "File too big", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                        return;
                
                new MainWindow(ofd.FileName, typeof(TextEditor.TextEditorControl)).Show();
                Close();
            };
            ofd.ShowDialog();
        }
    }
}