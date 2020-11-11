using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
        public Image Icon { get; private set; }

        public InspectorContent(string title, InspectorTableEntry[] tableEntries, Image icon = null)
        {
            Title = title;
            TableEntries = tableEntries;
            Icon = icon;
        }

        public void Show() => Inspector.Show(this);
        public void Set() => Inspector.Set(this);
    }
    
    public static class Inspector
    {
        // TODO Inspector
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
        
        private static GroupBox _groupBox;
        private static StackPanel _panel = new StackPanel();
        private static bool IsEmpty = false;

        private static readonly Separator Separator = new Separator() { Margin = new Thickness(0, 15, 0, 2) };
        private static readonly FontFamily FontFamily = new FontFamily("Segoe UI");
        private static readonly Thickness IconMargin = new Thickness(0, 7, 0, 0);
        #endregion

        private static Grid _grid;
        private static Image _image;
        private static Label _titleLabel;
        private static readonly Dictionary<string, Label[]> Table = new Dictionary<string, Label[]>();

        public static void Initialize(GroupBox inspectorObject)
        {
            _groupBox = inspectorObject;
            Clear();
        }

        public static void Set(InspectorContent content)
        {
            if (IsEmpty)
            {
                _groupBox.Content = _panel;
                IsEmpty = false;
            }

            // TODO Generate content
            
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
                _panel.Children.Add(_image);
                _panel.Children.Add(Separator);
            }
            
            // Add Title
            _titleLabel = new Label()
            {
                Content = content.Title,
                FontSize = 22,
                FontFamily = FontFamily
            };
            _panel.Children.Add(_titleLabel);
            
            // Add Data
            Table.Clear();
            _grid = new Grid() { ColumnDefinitions = { new ColumnDefinition() { Width = new GridLength(100) }, new ColumnDefinition() } };
            foreach (InspectorTableEntry entry in content.TableEntries)
            {
                _grid.RowDefinitions.Add(new RowDefinition());
                int id = Table.Count;
                Label keyLabel = new Label() { Content = entry.ID, FontWeight = FontWeights.Bold };
                keyLabel.SetValue(Grid.RowProperty, id);
                keyLabel.SetValue(Grid.ColumnProperty, 0);
                Label valueLabel = new Label() { Content = entry.Value };
                if (!(entry.ToolTip is null))
                    valueLabel.ToolTip = entry.ToolTip;
                valueLabel.SetValue(Grid.RowProperty, id);
                valueLabel.SetValue(Grid.ColumnProperty, 1);
                _grid.Children.Add(keyLabel);
                _grid.Children.Add(valueLabel);
                Table.Add(entry.ID, new []{ keyLabel, valueLabel });
            }
            _panel.Children.Add(_grid);
        }

        public static void SetTitle(string text)
        {
            _titleLabel.Content = text;
        }

        public static void SetImage(ImageSource imageSource)
        {
            _image.Source = imageSource;
        }

        public static void AddValue(string key, string value)
        {
            
        }
        
        public static void UpdateValue(string key, string value)
        {
            
        }

        public static void RemoveValue(string key)
        {
            
        }

        public static void Clear()
        {
            if (IsEmpty)
                return;
            IsEmpty = true;
            
            _panel.Children.Clear();

            _groupBox.Content = _nothingSelectedLabel;
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