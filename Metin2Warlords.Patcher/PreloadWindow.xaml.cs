using System;
using System.Windows;

namespace Metin2Warlords.Patcher
{
    public delegate void PreloadFinishedEventHandler(object sender, EventArgs e);

    public partial class PreloadWindow : Window
    {
        public event PreloadFinishedEventHandler finishedEvent;
        public PreloadWindow()
        {
            InitializeComponent();
        }

        public void ShowUpdate()
        {
            statusText.Visibility = Visibility.Visible;
            singleProgressBar.Visibility = Visibility.Visible;
        }

        protected virtual void OnPreloadFinished(EventArgs e)
        {
            if(this.finishedEvent != null)
            {
                finishedEvent(this, e);
            }
        }
    }
}
