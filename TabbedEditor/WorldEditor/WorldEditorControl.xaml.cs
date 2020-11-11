using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Microsoft.Win32;
using TabbedEditor.Interfaces;
using TabbedEditor.IO;
using TabbedEditor.WorldEditor.Data;
using TabbedEditor.WorldEditor.Tools;

namespace TabbedEditor.WorldEditor
{
    public partial class WorldEditorControl : IEditorControl
    {
        public string FilePath => _file.Path;
        public InspectorContent InspectorContent { get; private set; }

        private readonly WorldFile _file = new WorldFile();
        
        private readonly Dictionary<WorldEditorTool, IWorldEditorTool> _tools = new Dictionary<WorldEditorTool, IWorldEditorTool>();
        private WorldEditorTool _currentTool;
        
        public WorldEditorControl()
        {
            InitializeComponent();
            foreach (TileType tileType in Enum.GetValues(typeof(TileType)))
                TileTypeSelector.Items.Add(tileType);
            TileTypeSelector.SelectedIndex = 0;
            
            _tools.Add(WorldEditorTool.LandBrush, new LandBrushTool(this));
            _tools.Add(WorldEditorTool.AddEnemy, new ChangeEnemyCountTool(this));

            foreach (ToggleButton toolButton in ToolsToolbar.Items)
            {
                toolButton.Click += ToolButtonOnClick;
            }

            ToggleButton firstTool = (ToggleButton) ToolsToolbar.Items[0];
            firstTool.IsChecked = true;
            _currentTool = (WorldEditorTool) firstTool.DataContext;
        }

        private void ToolButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is ToggleButton toolButton)) return;
            
            WorldEditorTool worldEditorControl = (WorldEditorTool) toolButton.DataContext;

            if (_currentTool == worldEditorControl)
                return;
            
            _tools[_currentTool].OnDeselect();
            
            foreach (ToggleButton otherToolButton in ToolsToolbar.Items)
                otherToolButton.IsChecked = false;
            
            toolButton.IsChecked = true;
            _currentTool = worldEditorControl;
        }

        public event Action<string> TitleChangedEvent;
        public bool UnsavedChanges => _file.UnsavedChanges;

        /// <returns>If null, initialization was successful. Otherwise, returns error message.</returns>
        public Exception Open(string path)
        {
            try
            {
                _file.Path = path;
                _file.Data = WorldUtils.LoadWorldData(path);
                if (_file.Data?.TileArray is null)
                    throw new Exception("File is not serializable as a WorldData object.");
                GenerateViews();
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

        private void GenerateViews()
        {
            TileData[,] dataArray = _file.Data.TileArray;
            int xLength = dataArray.GetLength(0);
            int yLength = dataArray.GetLength(1);
            
            #region Define Grid
            GridLength rowHeight = new GridLength(50);
            GridLength colWidth = new GridLength(50);
            for (int y = 0; y < yLength; y++)
            {
                RowDefinition rowDef = new RowDefinition()
                {
                    Height = rowHeight
                };
                WorldGrid.RowDefinitions.Add(rowDef);
            }
            for (int x = 0; x < xLength; x++)
            {
                ColumnDefinition colDef = new ColumnDefinition()
                {
                    Width = colWidth
                };
                WorldGrid.ColumnDefinitions.Add(colDef);
            }
            #endregion

            for (int y = 0; y < yLength; y++)
            {
                for (int x = 0; x < xLength; x++)
                {
                    TileData data = dataArray[x, y];
                    WorldTileControl tileControl = new WorldTileControl(data);
                    Grid.SetColumn(tileControl, x);
                    Grid.SetRow(tileControl, y);
                    WorldGrid.Children.Add(tileControl);
                    tileControl.MouseLeftButtonDown += TileControl_MouseDown;
                }
            }
        }
        
        private void TileControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                _tools[_currentTool].OnClick(sender as WorldTileControl, e);
                _file.UnsavedChanges = true;
                UpdateTitle();
            }
            catch (Exception exception)
            {
                Logger.DumpException(exception);
                MessageBox.Show($"The {_currentTool.ToString()} tool has encountered the following error:\n\n{exception.Message}", "Tool error");
            }
        }

        public byte[] GenerateEmptyFile()
        {
            // TODO Generate empty file function
            GenerateViews();
            _file.UnsavedChanges = false;
            UpdateTitle();
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
            var spf = new SaveFileDialog {Filter = "World File (*.json)|*.json"};
            spf.FileOk += SaveFileDialog_FileOk;
            spf.ShowDialog();
        }
        private void SaveFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveFileDialog sfd = sender as SaveFileDialog;
            _file.Path = sfd.FileName;
            SaveFileToDisk();
        }

        private void SaveFileToDisk()
        {
            if (WorldUtils.TrySaveWorldData(_file.Data, _file.Path))
            {
                _file.UnsavedChanges = false;
                UpdateTitle();   
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