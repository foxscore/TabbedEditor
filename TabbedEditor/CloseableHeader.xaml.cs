using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TabbedEditor
{
    public partial class CloseableHeader : UserControl
    {
        public CloseableHeader()
        {
            InitializeComponent();
        }
    }
    
    class ClosableTab : TabItem
    {
        private CloseableHeader _header;
        
        // Constructor
        public ClosableTab()
        {
            // Create an instance of the usercontrol
            _header = new CloseableHeader();
            // Assign the usercontrol to the tab header
            this.Header = _header;
            
            // Attach to the CloseableHeader events
// (Mouse Enter/Leave, Button Click, and Label resize)
            _header.button_close.MouseEnter += 
                new MouseEventHandler(button_close_MouseEnter);
            _header.button_close.MouseLeave += 
                new MouseEventHandler(button_close_MouseLeave);
            _header.label_TabTitle.SizeChanged += 
                new SizeChangedEventHandler(label_TabTitle_SizeChanged);
        }

        public void HookToCloseButtonClickEvent(Action<object, RoutedEventArgs> action)
        {
            _header.button_close.Click += new RoutedEventHandler(action);
        }
        
        public string HeaderContent
        {
            get
            {
                return (string)((CloseableHeader)this.Header).label_TabTitle.Content;
            }
            set
            {
                ((CloseableHeader)this.Header).label_TabTitle.Content = value;
            }
        }
        
        #region EventHandlers
        // Button MouseEnter - When the mouse is over the button - change color to Red
        void button_close_MouseEnter(object sender, MouseEventArgs e)
        {
            ((CloseableHeader)this.Header).button_close.Foreground = Brushes.Red;
        }
// Button MouseLeave - When mouse is no longer over button - change color back to black
        void button_close_MouseLeave(object sender, MouseEventArgs e)
        {
            ((CloseableHeader)this.Header).button_close.Foreground = Brushes.Black;
        }
// Button Close Click - Remove the Tab - (or raise
// an event indicating a "CloseTab" event has occurred)
        void button_close_Click(object sender, RoutedEventArgs e)
        {
            ((TabControl)this.Parent).Items.Remove(this);
        }
// Label SizeChanged - When the Size of the Label changes
// (due to setting the Title) set position of button properly
        void label_TabTitle_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ((CloseableHeader)this.Header).button_close.Margin = new Thickness(
                ((CloseableHeader)this.Header).label_TabTitle.ActualWidth + 5, 3, 4, 0);
        }
        #endregion
        
        #region EventHandler Functions
        // Override OnSelected - Show the Close Button
        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);
            ((CloseableHeader)this.Header).button_close.Visibility = Visibility.Visible;
        }
        
        // Override OnUnSelected - Hide the Close Button
        protected override void OnUnselected(RoutedEventArgs e)
        {
            base.OnUnselected(e);
            ((CloseableHeader)this.Header).button_close.Visibility = Visibility.Hidden;
        }
        
        // Override OnMouseEnter - Show the Close Button
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            ((CloseableHeader)this.Header).button_close.Visibility = Visibility.Visible;
        }
        
        // Override OnMouseLeave - Hide the Close Button (If it is NOT selected)
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (!this.IsSelected)
            {
                ((CloseableHeader)this.Header).button_close.Visibility = Visibility.Hidden;
            }
        }
        #endregion
    }
}