using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Newtonsoft.Json;
using zlib;

namespace ReAuth
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static List<AuthDataGrid> Accounts = new List<AuthDataGrid>();
        public static List<AuthDataGrid> AccountsCheck = new List<AuthDataGrid>();

        public static readonly string version = "v1.5";

        private int _amtCheck = 0;
        public int AmountToCheck;

        public int AmountChecked
        {
            get => _amtCheck;
            set => CheckedLabel.Content = "Checked: " + _amtCheck + "/" + AmountToCheck;
        }

        public MainWindow()
        {
            if (File.Exists("settings.json"))
            {
                StaticHelper.SettingsSave =
                    JsonConvert.DeserializeObject<SettingsSave>(File.ReadAllText("settings.json"));
            }
            else
            {
                StaticHelper.SettingsSave = new SettingsSave
                {
                    InvService = false,
                    Proxy = false,
                    ProxyChange = 2,
                    ProxyList = string.Empty,
                    Store = false
                };
            }

            InitializeComponent();
            using (var client = new WebClient())
            {
                StaticHelper.Latest = client.DownloadString("https://raw.githubusercontent.com/eddy5641/Re-Auth/master/latest.txt")
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.None).First();
                if (!version.Contains("dev") &&!StaticHelper.Latest.Contains(version))
                {
                    this.ShowMessageAsync("Update", "There is an update available. Please download it");
                }
            }
            AccountGrid.ItemsSource = Accounts;
            GetLatestRad();
            GetLatestRadPbe();
            Rso = AuthClass.GetOpenIdConfig();
            Password.Visibility = Visibility.Collapsed;
            StaticHelper.Main = this;


            if (StaticHelper.SettingsSave.Store)
            {
                IPHeader.Visibility = Visibility.Visible;
                RPHeader.Visibility = Visibility.Visible;
            }

            if (StaticHelper.SettingsSave.Proxy && !string.IsNullOrWhiteSpace(StaticHelper.SettingsSave.ProxyList))
            {
                var proxyLocation = JsonConvert.DeserializeObject<string[]>(StaticHelper.SettingsSave.ProxyList);
                foreach (var location in proxyLocation.Where(File.Exists))
                {
                    StaticHelper.LoadProxyFromFile(location);
                }
            }

            if (StaticHelper.SettingsSave.InvService)
            {
                RunesHeader.Visibility = Visibility.Visible;
                ChampsHeader.Visibility = Visibility.Visible;
            }
        }

        public static RiotAuthOpenIdConfiguration Rso;

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            AmountChecked = 0;
            VerifyButton.IsEnabled = false;

            var t = new Timer(1000);
            int amount = 5;
            VerifyButton.Content = $"Verify Accounts ({amount})";
            t.Elapsed += (oat, eat) =>
            {
                amount--;


                Dispatcher.BeginInvoke(DispatcherPriority.Render, (Action)(() => VerifyButton.Content = $"Verify Accounts ({amount})"));
                if (amount == 0)
                {
                    t.Stop();
                    // ReSharper disable once AssignNullToNotNullAttribute
                    var check = (AccountGrid.Items.SourceCollection as List<AuthDataGrid>).Where(x =>
                        string.IsNullOrWhiteSpace(x.Verified) && !string.IsNullOrWhiteSpace(x.Username) &&
                        !string.IsNullOrWhiteSpace(x.Password) && !string.IsNullOrWhiteSpace(x.Region));
                    AmountToCheck = check.Count();
                    AccountsCheck.Clear();
                    Parallel.ForEach(check, (account) =>
                    {

                        if (account.Region.ToUpper() == "PBE")
                        {
                            var pberegiondata =
                                AuthClass.ReadSystemRegionData(Path.Combine(MyLocation, "pbe.system.yaml"), "PBE");

                            if (!pberegiondata.SuccessRead)
                            {
                                account.Verified = "NoRegion";
                                AccountsCheck.Add(account);
                                return;
                            }

                            var login = AuthClass.GetLoginToken(account.Username, account.Password, pberegiondata, Rso);

                            if (login.Result == RiotAuthResult.Success)
                            {
                                var userData = AuthClass.GetUserData(login);
                                if (StaticHelper.SettingsSave.Store)
                                {
                                    try
                                    {
                                        var basicStore = AuthClass.GetStoreIpRp(login, pberegiondata);
                                        account.IP = basicStore.ip.ToString();
                                        account.RP = basicStore.rp.ToString();
                                    }
                                    catch
                                    {
                                        account.IP = "?";
                                        account.RP = "?";
                                    }
                                }

                                if (StaticHelper.SettingsSave.InvService)
                                {
                                    try
                                    {
                                        account.Runes = AuthClass.GetUserSpellBook(login, userData, pberegiondata).Data
                                            .Items.SpellBookPage.First().Quantity.ToString();
                                    }
                                    catch
                                    {
                                        account.Runes = "?";
                                    }

                                    try
                                    {
                                        account.Champs = AuthClass.GetChampionJwt(login, userData, pberegiondata).Items
                                            .Champion.Count(x => !x.Rental).ToString();
                                    }
                                    catch
                                    {
                                        account.Champs = "?";
                                    }
                                }

                                AuthClass.Deauth(login, Rso, pberegiondata);
                            }

                            account.Verified = login.Result == RiotAuthResult.Success ? "Verified" : "Failed";
                            if (login.Result == RiotAuthResult.BadProxy)
                            {
                                account.Verified = "BadProxy";
                            }
                        }
                        else
                        {
                            var regiondata = AuthClass.ReadSystemRegionData(Path.Combine(MyLocation, "system.yaml"),
                                account.Region.ToUpper());

                            if (!regiondata.SuccessRead)
                            {
                                account.Verified = "NoRegion";
                                AccountsCheck.Add(account);
                                return;
                            }

                            var login = AuthClass.GetLoginToken(account.Username, account.Password, regiondata, Rso);

                            if (login.Result == RiotAuthResult.Success)
                            {
                                var userData = AuthClass.GetUserData(login);
                                if (StaticHelper.SettingsSave.Store)
                                {
                                    var basicStore = AuthClass.GetStoreIpRp(login, regiondata);
                                    account.IP = basicStore.ip.ToString();
                                    account.RP = basicStore.rp.ToString();
                                }

                                if (StaticHelper.SettingsSave.InvService)
                                {
                                    account.Runes = AuthClass.GetUserSpellBook(login, userData, regiondata).Data.Items
                                        .SpellBookPage.First().Quantity.ToString();
                                    account.Champs = AuthClass.GetChampionJwt(login, userData, regiondata).Items
                                        .Champion.Length.ToString();
                                }

                                AuthClass.Deauth(login, Rso, regiondata);
                            }

                            account.Verified = login.Result == RiotAuthResult.Success ? "Verified" : "Failed";
                            if (login.Result == RiotAuthResult.BadProxy)
                            {
                                account.Verified = "BadProxy";
                            }
                        }

                        Dispatcher.BeginInvoke(DispatcherPriority.Render, (Action) (() =>
                        {
                            AmountChecked++;
                            AccountsCheck.Add(account);
                        }));
                    });


                    Dispatcher.BeginInvoke(DispatcherPriority.Render, (Action)(() =>
                    {
                        foreach (var acc in AccountsCheck)
                        {
                            var location = Accounts.FindIndex(x => x.Username == acc.Username && x.Password == acc.Password && x.Region == acc.Region);
                            Accounts.RemoveAt(location);
                            Accounts.Insert(location, acc);
                        }

                        AccountGrid.ItemsSource = null;
                        AccountGrid.ItemsSource = Accounts;

                        VerifyButton.IsEnabled = true;

                        VerifyButton.Content = "Verify Accounts";
                        CheckedLabel.Content = "Not checking";
                    }));
                }
            };
            t.Start();
        }

        public static string MyLocation => Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)?.Replace("file:\\", "");

        public static void GetLatestRad()
        {
            if (File.Exists(Path.Combine(MyLocation, "system.yaml")))
            {
                File.Delete(Path.Combine(MyLocation, "system.yaml"));
            }
            using (var client = new WebClient())
            {
                var LatestGCSln = client
                    .DownloadString(
                        "http://l3cdn.riotgames.com/releases/live/solutions/league_client_sln/releases/releaselisting_NA")
                    .Split(new[] {Environment.NewLine}, StringSplitOptions.None).First();
                var LatestSolutionMan = client
                    .DownloadString(
                        $"http://l3cdn.riotgames.com/releases/live/solutions/league_client_sln/releases/{LatestGCSln}/solutionmanifest")
                    .Split(new[] {Environment.NewLine}, StringSplitOptions.None);
                //LatestSolutionMan
                var pos = Array.IndexOf(LatestSolutionMan, "league_client");
                client.DownloadFile(
                    $"http://l3cdn.riotgames.com/releases/live/projects/league_client/releases/{LatestSolutionMan[pos + 1]}/files/system.yaml.compressed",
                    Path.Combine(MyLocation, "system.yaml.compressed"));
                DecompressFile(Path.Combine(MyLocation, "system.yaml.compressed"),
                    Path.Combine(MyLocation, "system.yaml"));
                File.Delete(Path.Combine(MyLocation, "system.yaml.compressed"));
            }
        }

        public static void GetLatestRadPbe()
        {
            if (File.Exists(Path.Combine(MyLocation, "pbe.system.yaml")))
            {
                File.Delete(Path.Combine(MyLocation, "pbe.system.yaml"));
            }
            using (var client = new WebClient())
            {
                var LatestGCSln = client
                    .DownloadString(
                        "http://l3cdn.riotgames.com/releases/pbe/solutions/league_client_sln/releases/releaselisting_PBE")
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.None).First();
                var LatestSolutionMan = client
                    .DownloadString(
                        $"http://l3cdn.riotgames.com/releases/pbe/solutions/league_client_sln/releases/{LatestGCSln}/solutionmanifest")
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                //LatestSolutionMan
                var pos = Array.IndexOf(LatestSolutionMan, "league_client");
                client.DownloadFile(
                    $"http://l3cdn.riotgames.com/releases/pbe/projects/league_client/releases/{LatestSolutionMan[pos + 1]}/files/system.yaml.compressed",
                    Path.Combine(MyLocation, "pbe.system.yaml.compressed"));
                DecompressFile(Path.Combine(MyLocation, "pbe.system.yaml.compressed"),
                    Path.Combine(MyLocation, "pbe.system.yaml"));
                File.Delete(Path.Combine(MyLocation, "pbe.system.yaml.compressed"));
            }
        }


        public static void DecompressFile(string inFile, string outFile)
        {
            int data;
            const int stopByte = -1;
            var outFileStream = new FileStream(outFile, FileMode.Create);
            var inZStream = new ZInputStream(File.Open(inFile, FileMode.Open, FileAccess.Read));

            while (stopByte != (data = inZStream.Read()))
            {
                var databyte = (byte) data;
                outFileStream.WriteByte(databyte);
            }

            inZStream.Close();
            outFileStream.Close();
        }

        private void ButtonBase_OnClick2(object sender, RoutedEventArgs e)
        {
            var findLeagueDialog =
                new OpenFileDialog
                {
                    DefaultExt = ".txt",
                    Filter = "Check list|*.txt",
                    Multiselect = false
                };

            var result = findLeagueDialog.ShowDialog();
            if (result != true) return;

            var accounts = File.ReadAllLines(findLeagueDialog.FileName);
            foreach (var account in accounts.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                var data = account.Split(':');
                switch (data.Length)
                {
                    case 3:
                    {
                        var authGrid = new AuthDataGrid
                        {
                            Username = data[0],
                            Password = data[1],
                            Region = data[2]
                        };
                        Accounts.Add(authGrid);
                        break;
                    }
                    case 4:
                    {

                        var authGrid = new AuthDataGrid
                        {
                            Username = data[0],
                            Password = data[1],
                            Region = data[2],
                            Verified = data[3]
                        };
                        Accounts.Add(authGrid);
                        break;
                    }
                }
            }


            AccountGrid.ItemsSource = null;
            AccountGrid.ItemsSource = Accounts;
        }


        private void ButtonBase_OnClick3(object sender, RoutedEventArgs e)
        {
            var saveLocation = new SaveFileDialog
            {
                DefaultExt = ".txt",
                AddExtension = true,
                OverwritePrompt = true
            };

            var result = saveLocation.ShowDialog();
            if (result != true)
            {
                this.ShowMessageAsync("Failed to save file", "The exported file was not saved");
                return;
            }

            var path = saveLocation.OpenFile();


            using (var sw = new StreamWriter(path))
            {
                foreach (var account in Accounts)
                {
                    sw.WriteLine($"{account.Username}:{account.Password}:{account.Region}:{account.Verified}");
                }
            }
        }

        private void Help_OnClick(object sender, RoutedEventArgs e)
        {
            if (!HelpFlyout.IsOpen)
            {
                SettingFlyout.IsOpen = false;
                ProxyFlyout.IsOpen = false;
                AboutFlyout.IsOpen = false;
            }
            HelpFlyout.IsOpen = !HelpFlyout.IsOpen;
        }

        private void Settings_OnClick(object sender, RoutedEventArgs e)
        {
            if (!SettingFlyout.IsOpen)
            {
                HelpFlyout.IsOpen = false;
                ProxyFlyout.IsOpen = false;
                AboutFlyout.IsOpen = false;
            }
            SettingFlyout.IsOpen = !SettingFlyout.IsOpen;
        }

        private void Proxy_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ProxyFlyout.IsOpen)
            {
                SettingFlyout.IsOpen = false;
                HelpFlyout.IsOpen = false;
                AboutFlyout.IsOpen = false;
            }
            ProxyFlyout.IsOpen = !ProxyFlyout.IsOpen;
        }

        private void About_OnClick(object sender, RoutedEventArgs e)
        {
            if (!AboutFlyout.IsOpen)
            {
                SettingFlyout.IsOpen = false;
                HelpFlyout.IsOpen = false;
                ProxyFlyout.IsOpen = false;
            }
            AboutFlyout.IsOpen = !AboutFlyout.IsOpen;
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            var test = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(StaticHelper.SettingsSave));
            File.Delete("settings.json");
            var file = File.OpenWrite("settings.json");
            file.Write(test, 0, test.Length);
            file.Close();
            Environment.Exit(0);
        }
    }

    public class AuthDataGrid
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Region { get; set; } = string.Empty;

        public string Verified { get; set; } = string.Empty;

        
        //This is critical for the hidden password component.
        // ReSharper disable once UnusedMember.Global
        public string HiddenPassword
        {
            get => new string('*', Password.Length);
            set => Password = value;
        }

        public string Runes { get; set; }

        public string RP { get; set; }

        public string IP { get; set; }

        public string Champs { get; set; }
    }
}
