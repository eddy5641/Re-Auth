using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using ReAuth.Properties;

namespace ReAuth
{
    /// <summary>
    /// Interaction logic for ProxyFlyout.xaml
    /// </summary>
    public partial class ProxyFlyout : UserControl
    {
        public ProxyFlyout()
        {
            InitializeComponent();
            StaticHelper.ProxyFlyout = this;
            ProxyRotateSlider.Value = StaticHelper.SettingsSave.ProxyChange;
            ProxyGrid.ItemsSource = StaticHelper.ProxyList;
        }

        private void ProxyRotateSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ProxyRotateSliderAmount == null)
                return;
            ProxyRotateSliderAmount.Content = ProxyRotateSlider.Value;
            StaticHelper.SettingsSave.ProxyChange = (int)ProxyRotateSlider.Value;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {

            var findProxyDialog =
                new OpenFileDialog
                {
                    DefaultExt = ".txt",
                    Filter = "Check list|*.txt",
                    Multiselect = false
                };

            var result = findProxyDialog.ShowDialog();
            if (result != true) return;
            
            StaticHelper.LoadProxyFromFile(findProxyDialog.FileName);
        }
    }
}
