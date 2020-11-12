using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media;
using TabbedEditor.IO;
using VerticalAlignment = System.Windows.VerticalAlignment;

namespace TabbedEditor
{
    public class InspectorTableEntry
    {
        public readonly string ID;
        public readonly string Value;
        public readonly string ToolTip;

        public InspectorTableEntry(string id, string value, string toolTip = null)
        {
            ID = id;
            Value = value;
            ToolTip = toolTip;
        }
    }
    public class InspectorContent
    {
        public string Title { get; private set; }
        public InspectorTableEntry[] TableEntries { get; private set; }
        public Control[] CustomControls { get; private set; }
        public Image Icon { get; private set; }

        public InspectorContent(string title, Image image)
        {
            Icon = image;
        }
        public InspectorContent(Control[] tableEntries) => CustomControls = CustomControls;
        public InspectorContent(InspectorTableEntry[] tableEntries) => TableEntries = tableEntries;
        public InspectorContent(string title, Control[] customControls, Image icon = null)
        {
            Title = title;
            CustomControls = customControls;
            Icon = icon;
        }
        public InspectorContent(string title, InspectorTableEntry[] tableEntries, Image icon = null)
        {
            Title = title;
            TableEntries = tableEntries;
            Icon = icon;
        }
        public InspectorContent(string title, InspectorTableEntry[] tableEntries, Control[] customControls, Image icon = null)
        {
            Title = title;
            TableEntries = tableEntries;
            CustomControls = customControls;
            Icon = icon;
        }

        public void Show() => Inspector.Show(this);
        public void Set() => Inspector.Set(this);
    }
    
    public static class Inspector
    {
        private const int Width = 300;
        
        #region Static objects
        private static Label _nothingSelectedLabel = new Label()
        {
            Content = "Nothing is selected",
            Foreground = Brushes.Gray,
            FontSize = 22,
            FontFamily = new FontFamily("Segoe UI Light"),
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalContentAlignment = HorizontalAlignment.Center
        };
        private static Label _failedToBuildLabel = new Label()
        {
            Content = "Failed to build preview",
            Foreground = Brushes.Gray,
            FontSize = 22,
            FontFamily = new FontFamily("Segoe UI Light"),
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalContentAlignment = HorizontalAlignment.Center
        };
        
        private static GroupBox _groupBox;
        private static bool IsEmpty = false;

        private static readonly Separator Separator = new Separator() { Margin = new Thickness(0, 15, 0, 2) };
        private static readonly FontFamily FontFamily = new FontFamily("Segoe UI");
        private static readonly Thickness IconMargin = new Thickness(0, 7, 0, 0);
        #endregion

        private static Grid _grid;
        private static Image _image;
        private static Label _titleLabel;
        private static readonly ScrollViewer ScrollViewer = new ScrollViewer();
        private static readonly StackPanel Panel = new StackPanel();
        private static readonly List<Control> CustomControls = new List<Control>();
        private static readonly Dictionary<string, Label[]> Table = new Dictionary<string, Label[]>();

        public static void Initialize(GroupBox inspectorObject)
        {
            _groupBox = inspectorObject;
            ScrollViewer.Content = Panel;
            Clear();
        }

        public static void Set(InspectorContent content)
        {
            try
            {
                if (content == null)
                {
                    Clear();
                    return;
                }

                if (IsEmpty)
                {
                    _groupBox.Content = ScrollViewer;
                    IsEmpty = false;
                }

                // Add Image (and separator) if aviable
                if (!(content.Icon is null))
                {
                    _image = content.Icon;
                    int size = 150;
                    _image.Width = size;
                    _image.Height = size;
                    _image.VerticalAlignment = VerticalAlignment.Top;
                    _image.HorizontalAlignment = HorizontalAlignment.Center;
                    _image.Margin = IconMargin;
                    Panel.Children.Add(_image);
                    // TODO Fix wired bug
                    Panel.Children.Add(Separator);
                }

                // Add Title
                if (content.Title is null)
                    _titleLabel = null;
                else
                {
                    _titleLabel = new Label()
                    {
                        Content = content.Title,
                        FontSize = 22,
                        FontFamily = FontFamily
                    };
                    Panel.Children.Add(_titleLabel);   
                }

                // Add Data
                Table.Clear();
                _grid = new Grid()
                {
                    ColumnDefinitions = {new ColumnDefinition() {Width = new GridLength(100)}, new ColumnDefinition()}
                };
                foreach (InspectorTableEntry entry in content.TableEntries)
                {
                    _grid.RowDefinitions.Add(new RowDefinition());
                    int id = Table.Count;
                    Label keyLabel = new Label() {Content = entry.ID, FontWeight = FontWeights.Bold};
                    keyLabel.SetValue(Grid.RowProperty, id);
                    keyLabel.SetValue(Grid.ColumnProperty, 0);
                    Label valueLabel = new Label() {Content = entry.Value};
                    if (!(entry.ToolTip is null))
                        valueLabel.ToolTip = entry.ToolTip;
                    valueLabel.SetValue(Grid.RowProperty, id);
                    valueLabel.SetValue(Grid.ColumnProperty, 1);
                    _grid.Children.Add(keyLabel);
                    _grid.Children.Add(valueLabel);
                    Table.Add(entry.ID, new[] {keyLabel, valueLabel});
                }

                Panel.Children.Add(_grid);
                
                // TODO Fix custom control
                /*CustomControls.Clear();
                foreach (Control control in content.CustomControls)
                {
                    control.Width = 276;
                    _panel.Children.Add(control);
                    CustomControls.Add(control);
                }*/
            }
            catch (Exception e)
            {
                Logger.DumpException(e);
                ShowFailedToBuild();
            }
        }

        public static void SetTitle(string text)
        {
            try
            {
                _titleLabel.Content = text;
            }
            catch (Exception e)
            {
                Logger.DumpException(e);
                ShowFailedToBuild();
            }
        }

        public static void SetImage(ImageSource imageSource)
        {
            try
            {
                _image.Source = imageSource;   
            }
            catch (Exception e)
            {
                Logger.DumpException(e);
                ShowFailedToBuild();
            }
        }
        
        public static void SetValue(string key, string value, string toolTip = null)
        {
            try
            {
                if (Table.ContainsKey(key))
                {
                    Label label = Table[key][1];
                    label.Content = value;
                    label.ToolTip = toolTip;
                }
                else
                {
                    _grid.RowDefinitions.Add(new RowDefinition());
                    int id = Table.Count;
                    Label keyLabel = new Label() {Content = key, FontWeight = FontWeights.Bold};
                    keyLabel.SetValue(Grid.RowProperty, id);
                    keyLabel.SetValue(Grid.ColumnProperty, 0);
                    Label valueLabel = new Label() {Content = value};
                    if (!(toolTip is null))
                        valueLabel.ToolTip = toolTip;
                    valueLabel.SetValue(Grid.RowProperty, id);
                    valueLabel.SetValue(Grid.ColumnProperty, 1);
                    _grid.Children.Add(keyLabel);
                    _grid.Children.Add(valueLabel);
                    Table.Add(key, new[] {keyLabel, valueLabel});
                }
            }
            catch (Exception e)
            {
                Logger.DumpException(e);
                ShowFailedToBuild();
            }
        }

        public static void RemoveValue(string key)
        {
            try
            {
                if (!Table.ContainsKey(key))
                    return;
                
                Label[] labels = Table[key];
                int row = (int)labels[0].GetValue(Grid.RowProperty);
                
                _grid.Children.Remove(labels[0]);
                _grid.Children.Remove(labels[1]);
                Table.Remove(key);
                
                foreach (string sKey in Table.Keys)
                {
                    var sLabels = Table[sKey];
                    var newRow = (int)sLabels[0].GetValue(Grid.RowProperty) - 1;
                    if (newRow < row) continue;
                    sLabels[0].SetValue(Grid.RowProperty, newRow);
                    sLabels[1].SetValue(Grid.RowProperty, newRow);
                }
            }
            catch (Exception e)
            {
                Logger.DumpException(e);
                ShowFailedToBuild();
            }
        }
        
        /// <summary>
        /// Warning: The controls width will be adjusted.
        /// </summary>
        public static void AddCustomControl(Control control)
        {
            control.Width = 276;
            Panel.Children.Add(control);
            CustomControls.Add(control);
        }

        public static void RemoveCustomControl(Control control)
        {
            if (!CustomControls.Contains(control)) return;
            Panel.Children.Remove(control);
            CustomControls.Remove(control);
        }

        private static void Clear()
        {
            if (IsEmpty)
                return;
            IsEmpty = true;
            
            Panel.Children.Clear();

            _groupBox.Content = _nothingSelectedLabel;
        }
        private static void ShowFailedToBuild()
        {
            IsEmpty = true;
            Panel.Children.Clear();
            _groupBox.Content = _failedToBuildLabel;
        }

        public static void Toggle() => _groupBox.Width = _groupBox.Width == 0 ? Width : 0;

        public static void Show(InspectorContent content)
        {
            Set(content);
            Show();
        }
        public static void Show() => _groupBox.Width = Width;
        public static void Hide() => _groupBox.Width = 0;
    }
}