using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace ReAuth
{
    /// <summary>
    /// Interaction logic for AboutFlyout.xaml
    /// </summary>
    public partial class AboutFlyout : UserControl
    {
        public AboutFlyout()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/eddy5641/");
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/eddy5641/Re-Auth/releases");
        }

        private void Donate_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=ZXPDV29P7GNFG");
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/eddy5641/Re-Auth/blob/master/LicenseList.md");
        }
    }
}
