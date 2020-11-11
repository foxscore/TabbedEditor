using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace TabbedEditor
{
    public partial class ProgressViewer : Window
    {
        #region Hide close button
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private void ProgressViewer_OnLoaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }
        #endregion
        
        private static ProgressViewer _instace = new ProgressViewer();
        private static Thread _threadInstace;
        
        private static string _title;
        private static string _message;
        private static int _progress;
        
        public ProgressViewer()
        {
            InitializeComponent();
        }

        public static void Show(string title, string message, int progress)
        {
            if (!(_threadInstace is null) && _threadInstace.IsAlive)
                _threadInstace.Abort();

            _title = title;
            _message = message;
            _progress = progress;
            
            _threadInstace = new Thread(new ThreadStart(ThreadStartingPoint));
            _threadInstace.SetApartmentState(ApartmentState.STA);
            _threadInstace.IsBackground = true;
            _threadInstace.Start();
        }
        
        private static void ThreadStartingPoint()
        {
                _instace = new ProgressViewer
                {
                    Title = _title,
                    TextBlock =
                    {
                        Text = _message
                    }
                };

                if (_progress < 0)
                    _instace.ProgressBar.IsIndeterminate = true;
                else if (_progress > 100)
                    _instace.ProgressBar.Value = 100;
                else _instace.ProgressBar.Value = _progress;

                _instace.Show();
                Dispatcher.Run();
        }

        public static void Update(string message, int progress)
        {
            if (_threadInstace is null || !_threadInstace.IsAlive)
                return;

            _instace.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                _instace.TextBlock.Text = message;
            
                if (progress < 0)
                    _instace.ProgressBar.IsIndeterminate = true;
                else if (progress > 100)
                {
                    _instace.ProgressBar.IsIndeterminate = false;
                    _instace.ProgressBar.Value = 100;
                }
                else
                {
                    _instace.ProgressBar.IsIndeterminate = false;
                    _instace.ProgressBar.Value = progress;
                }
            }));
        }
        public static void Update(string message)
        {
            if (_threadInstace is null || !_threadInstace.IsAlive)
                return;
            
            _instace.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                _instace.TextBlock.Text = message;
            }));
        }
        public static void Update(int progress)
        {
            if (_threadInstace is null || !_threadInstace.IsAlive)
                return;

            _instace.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                if (progress < 0)
                    _instace.ProgressBar.IsIndeterminate = true;
                else if (progress > 100)
                {
                    _instace.ProgressBar.IsIndeterminate = false;
                    _instace.ProgressBar.Value = 100;
                }
                else
                {
                    _instace.ProgressBar.IsIndeterminate = false;
                    _instace.ProgressBar.Value = progress;
                }
            }));
        }

        public new static void Hide()
        {
            if (_threadInstace is null || !_threadInstace.IsAlive)
                return;

            _instace.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                ((Window) _instace).Close();
            }));
        }
    }
}