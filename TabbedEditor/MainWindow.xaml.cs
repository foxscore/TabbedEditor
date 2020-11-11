using TabbedEditor.TextEditor;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using TabbedEditor.InitialWindow;
using TabbedEditor.Interfaces;
using TabbedEditor.IO;
using TabbedEditor.WorldEditor;
using TabbedEditor.TargaViewer;

namespace TabbedEditor
{
    public partial class MainWindow
    {
        private void InitializeComponents()
        {
            InitializeComponent();
            Inspector.Initialize(InspectorGroup);
        }
        
        public MainWindow(string path, Type editor)
        {
            if (!typeof(IEditorControl).IsAssignableFrom(editor))
            {
                MessageBox.Show("Editor type iCommandBinding_OnExecutednitialize");
                Close();
            }

            if (!File.Exists(path))
            {
                MessageBox.Show("Startup file does not exist!", "Failed to initialize");
                Close();
            }

            InitializeComponents();

            IEditorControl editorControl = OpenTab(editor);
            Exception openResult = editorControl.Open(path);
            if (!(openResult is null))
            {
                Logger.DumpException(openResult);
                TabController.Items.RemoveAt(TabController.Items.Count - 1);
                MessageBox.Show("Failed to load file.\nReason: " + openResult.Message, "Tabbed Editor - Failed to load file");
            }
            else
            {
                FileHistory.Add(path, editor);
            }

            Inspector.Set(editorControl.InspectorContent);
            
            FileHistory.FileHistoryChanged += UpdateRecentFiles;
            UpdateRecentFiles();
            TabController.SelectionChanged += TabControllerOnSelectionChanged;
        }

        public MainWindow()
        {
            InitializeComponents();
            
            FileHistory.FileHistoryChanged += UpdateRecentFiles;
            UpdateRecentFiles();
            TabController.SelectionChanged += TabControllerOnSelectionChanged;
        }

        private void TabControllerOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem item = e.AddedItems[0] as TabItem;
            IEditorControl content = item.Content as IEditorControl;
            Inspector.Set(content.InspectorContent);
        }

        private TabItem FindTab(string path, Type type)
        {
            // Check if tab with same editor and file already exists
            foreach (TabItem tabItem in TabController.Items)
            {
                if (tabItem.Content is IEditorControl child && child.GetType() == type && child.FilePath == path)
                    return tabItem;
            }

            return null;
        }

        private void UpdateRecentFiles()
        {
            RecentFilesList.Items.Clear();
            EditorFile[] history = FileHistory.GenerateList();

            if (history.Length == 0)
            {
                RecentFilesList.Items.Add(new MenuItem() {Header = "Empty", IsEnabled = false});
                return;
            }

            foreach (EditorFile entry in history)
            {
                MenuItem listEntry = new MenuItem();
                listEntry.Header = $"[{FileHistory.GetEditorName(entry.Editor)}]\t    {entry.Path.Split('\\').Last()}";
                listEntry.ToolTip = entry.Path;
                listEntry.Click += RecentFilesEntryOnClick;
                listEntry.DataContext = entry;
                RecentFilesList.Items.Add(listEntry);
            }

            RecentFilesList.Items.Add(new Separator());
            MenuItem clearHistory = new MenuItem();
            clearHistory.Header = "Clear History";
            clearHistory.ToolTip = "This action will clear your 'Recent files' history.";
            clearHistory.Click += ClearHistoryOnClick;
            RecentFilesList.Items.Add(clearHistory);
        }

        private void ClearHistoryOnClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("This action cannot be undone!\nDo you wish to continue?", "Clear history",
                MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                FileHistory.ResetHistory();
        }

        private void RecentFilesEntryOnClick(object sender, RoutedEventArgs e)
        {
            var fileHistoryEntry = (sender as MenuItem)?.DataContext as EditorFile;
            // ReSharper disable once PossibleNullReferenceException
            var type = fileHistoryEntry.GetEditorType();

            var tab = FindTab(fileHistoryEntry.Path, type);
            if (!(tab is null))
            {
                TabController.SelectedValue = tab;
                return;
            }

            if (!typeof(IEditorControl).IsAssignableFrom(type))
            {
                if (MessageBox.Show("Invalid entry in history!\nDo you want to remove it from recently opened files?",
                    "Invalid entry", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    FileHistory.Remove(fileHistoryEntry);
                return;
            }

            if (!File.Exists(fileHistoryEntry.Path))
            {
                if (MessageBox.Show(
                    "The file does not exist anymore!\nDo you want to remove it from recently opened files?",
                    "Missing file", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    FileHistory.Remove(fileHistoryEntry);
                return;
            }


// TODO Check if file type is supported by editor before calling this part
/*
if (FileChecker.IsBinary(fileHistoryEntry.Path))
{
    if (MessageBox.Show("Binary files are not supported at this time.\nDo you want to remove it from recently opened files?", "File is binary", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        FileHistory.Remove(fileHistoryEntry);
    return;
}
*/

            if (FileChecker.IsTooBig(fileHistoryEntry.Path))
                if (MessageBox.Show(
                    "Files with a size over 10MB are not supported.\nOpen anyways? (Program may freeze or stop working)",
                    "File too big", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    return;

            IEditorControl editorControl = OpenTab(type);
            Exception openResult = editorControl.Open(fileHistoryEntry.Path);
            if (!(openResult is null))
            {
                Logger.DumpException(openResult);
                TabController.Items.RemoveAt(TabController.Items.Count - 1);
                MessageBox.Show("Failed to load file.\nReason: " + openResult.Message,
                    "Tabbed Editor - Failed to load file");
            }
            else
            {
                FileHistory.Add(fileHistoryEntry.Path, type);
            }
        }

        private void CommandBinding_Open(object sender, ExecutedRoutedEventArgs e) => Open();
        private void CommandBinding_Save(object sender, ExecutedRoutedEventArgs e) => Save();
        private void CommandBinding_SaveAs(object sender, ExecutedRoutedEventArgs e) => SaveAs();

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (TabItem tab in TabController.Items)
            {
                if (((IEditorControl) tab.Content).UnsavedChanges)
                {
                    TabController.SelectedItem = tab;
                    if (MessageBox.Show("You have unsaved changes! Close anyway?", "Unsaved changes",
                        MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                        e.Cancel = true;
                    return;
                }
            }
        }

        static readonly string FileDialogFilter = "World File (*.json)|*.json|Text file (*.txt)|*.txt|Tagra Image (*.tga)|*.tga";

        public static Dictionary<string, Type> EndingToType = new Dictionary<string, Type>()
        {
            {"txt", typeof(TextEditorControl)},
            {"json", typeof(WorldEditorControl)},
            {"tga", typeof(TargaViewerControl)}
        };

        private IEditorControl OpenTab(Type type)
        {
            TabItem tab = new TabItem();
            tab.Header = "Loading...";
            IEditorControl editorControl = Activator.CreateInstance(type) as IEditorControl;

// ReSharper disable once PossibleNullReferenceException
            editorControl.TitleChangedEvent += (title) => { tab.Header = title.Replace("_", "__"); };

            tab.Content = editorControl;
            TabController.Items.Add(tab);
            TabController.SelectedIndex = TabController.Items.Count - 1;
            return editorControl;
        }

        private void NewFile()
        {
            EditorFile newFile = NewFileWindow.ShowDialog();
            if ((newFile is null))
                return;

            Type type = newFile.GetType();

            IEditorControl editorControl = OpenTab(type);
            Exception openResult = editorControl.Open(newFile.Path);
            if (!(openResult is null))
            {
                Logger.DumpException(openResult);
                TabController.Items.RemoveAt(TabController.Items.Count - 1);
                MessageBox.Show("Failed to load file.\nReason: " + openResult.Message,
                    "Tabbed Editor - Failed to load file");
            }
            else
            {
                FileHistory.Add(newFile.Path, type);
            }
        }

        private void WindowEvent_New(object sender, ExecutedRoutedEventArgs e) => NewFile();

        private void OpenRaw(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = FileChecker.FileDialogRawFilter;
            ofd.FileOk += OpenRAW_FileOk;
            ofd.ShowDialog();
        }

        private void OpenRAW_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpenFileDialog ofd = sender as OpenFileDialog;
            Type type = typeof(TextEditorControl);

            TabItem tab = FindTab(ofd.FileName, type);
            if (!(tab is null))
            {
                TabController.SelectedValue = tab;
                return;
            }

// ReSharper disable once PossibleNullReferenceException
            if (FileChecker.IsBinary(ofd.FileName))
            {
                MessageBox.Show("Binary files are not supported at this time.", "File is binary");
                return;
            }

            if (FileChecker.IsTooBig(ofd.FileName))
                if (MessageBox.Show(
                    "Files with a size over 10MB are not supported.\nOpen anyways? (Program may freeze or stop working)",
                    "File too big", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    return;

            IEditorControl editorControl = OpenTab(type);
            Exception openResult = editorControl.Open(ofd.FileName);
            if (!(openResult is null))
            {
                Logger.DumpException(openResult);
                TabController.Items.RemoveAt(TabController.Items.Count - 1);
                MessageBox.Show("Failed to load file.\nReason: " + openResult.Message,
                    "Tabbed Editor - Failed to load file");
            }
            else
            {
                FileHistory.Add(ofd.FileName, type);
            }
        }
// TODO Add a 'Editor' Menu next to the File menu, which contains 

        private void Open()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = FileDialogFilter;
            ofd.FileOk += Open_FileOk;
            ofd.ShowDialog();
        }

        private void Open_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpenFileDialog ofd = sender as OpenFileDialog;
            string fileType = ofd.FileName.Split('.').Last();
            Type type = EndingToType[fileType];

            TabItem tab = FindTab(ofd.FileName, type);
            if (!(tab is null))
            {
                TabController.SelectedValue = tab;
                return;
            }

            if (FileChecker.IsTooBig(ofd.FileName))
                if (MessageBox.Show(
                    "Files with a size over 10MB are not supported.\nOpen anyways? (Program may freeze or stop working)",
                    "File too big", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    return;

            IEditorControl editorControl = OpenTab(type);
            Exception openResult = editorControl.Open(ofd.FileName);
            if (!(openResult is null))
            {
                Logger.DumpException(openResult);
                TabController.Items.RemoveAt(TabController.Items.Count - 1);
                MessageBox.Show("Failed to load file.\nReason: " + openResult.Message,
                    "Tabbed Editor - Failed to load file");
            }
            else
            {
                FileHistory.Add(ofd.FileName, type);
            }
        }

        private bool TryCloseTab(TabItem tab)
        {
            if (tab is null)
            {
                MessageBox.Show("Tab is null");
                return false;
            }

            if (tab.Content is null)
            {
                MessageBox.Show("Tab.Content is null");
                return false;
            }

            if ((tab.Content as IEditorControl).UnsavedChanges)
            {
                TabController.SelectedItem = tab;
                if (MessageBox.Show("You have unsaved changes! Close anyway?", "Unsaved changes",
                    MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    return false;
            }

            int prevIndex = TabController.SelectedIndex;
            TabController.Items.Remove(tab);
            int count = TabController.Items.Count;
            if (count != 0)
                TabController.SelectedIndex = (prevIndex < count) ? prevIndex : prevIndex - 1;

            return true;
        }

        private void Save() => (TabController.SelectedContent as IEditorControl)?.Save();

        private void SaveAs() => (TabController.SelectedContent as IEditorControl)?.SaveAs();

        private void HelpButtonOnClick(object sender, RoutedEventArgs e)
        {
            // TODO Help window
        }

        private void ToggleInspectorOnClick(object sender, RoutedEventArgs e) => Inspector.Toggle();

        private void AboutButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (About.Instance is null)
                new About().Show();
        }

        private void WindowEvent_Close(object sender, ExecutedRoutedEventArgs e) => TryCloseTab((TabItem) TabController.SelectedItem);

        private void ExitOnClick(object sender, RoutedEventArgs e) => Close();

        private void TabController_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabController.Items.Count != 0) return;
            new StartupSelect().Show();
            Close();
        }
    }
}
