using System.ComponentModel;

namespace TabbedEditor
{
    public partial class About
    {
        public static About Instance { get; private set; }
        
        public About()
        {
            Instance = this;
            InitializeComponent();
        }

        private void About_OnClosing(object sender, CancelEventArgs e)
        {
            Instance = null;
        }
    }
}