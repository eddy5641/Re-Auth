using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls.Dialogs;
using ReAuth.Properties;

namespace ReAuth
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
            InvAPI.IsChecked = StaticHelper.SettingsSave.InvService;
            StoreAPI.IsChecked = StaticHelper.SettingsSave.Store;
            ProxyList.IsChecked = StaticHelper.SettingsSave.Proxy;
        }

        private void PasswordSwitch_OnClick(object sender, RoutedEventArgs e)
        {
            if (PasswordSwitch.IsChecked == null)
            {
                StaticHelper.Main.ShowMessageAsync("Error", "Could not change password visibility");
                return;
            }

            StaticHelper.Main.Password.Visibility = !(bool)PasswordSwitch.IsChecked ? Visibility.Collapsed : Visibility.Visible;
            StaticHelper.Main.HiddenPassword.Visibility = (bool)PasswordSwitch.IsChecked ? Visibility.Collapsed : Visibility.Visible;
        }

        private void AdvAPI_OnClick(object sender, RoutedEventArgs e)
        {
            if (AdvAPI.IsChecked == null)
                return;
            RSOAUTH.IsEnabled = (bool)AdvAPI.IsChecked;
            RTMPSAPI.IsEnabled = (bool)AdvAPI.IsChecked;
        }

        private void ProxyList_OnClick(object sender, RoutedEventArgs e)
        {
            if (ProxyList.IsChecked == null)
                return;

            StaticHelper.SettingsSave.Proxy = (bool) ProxyList.IsChecked;
        }

        private void StoreAPI_OnClick(object sender, RoutedEventArgs e)
        {
            if (StoreAPI.IsChecked == null)
                return;

            StaticHelper.SettingsSave.Store = (bool) StoreAPI.IsChecked;

            StaticHelper.Main.IPHeader.Visibility =
                (bool) StoreAPI.IsChecked ? Visibility.Visible : Visibility.Collapsed;

            StaticHelper.Main.RPHeader.Visibility =
                (bool)StoreAPI.IsChecked ? Visibility.Visible : Visibility.Collapsed;
        }

        private void InvAPI_OnClick(object sender, RoutedEventArgs e)
        {
            if (InvAPI.IsChecked == null)
                return;

            StaticHelper.SettingsSave.InvService = (bool)InvAPI.IsChecked;

            StaticHelper.Main.RunesHeader.Visibility = (bool)InvAPI.IsChecked ? Visibility.Visible : Visibility.Collapsed;
            StaticHelper.Main.ChampsHeader.Visibility = (bool)InvAPI.IsChecked ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
