using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Threading;
using Newtonsoft.Json;
using ReAuth.Properties;

namespace ReAuth
{
    public static class StaticHelper
    {
        public static MainWindow Main { get; set; }


        public static List<ProxyData> ProxyList = new List<ProxyData>();

        public static ProxyFlyout ProxyFlyout { get; set; }


        public static SettingsSave SettingsSave = new SettingsSave();

        public static string Latest { get; set; }

        public static List<string> ProxyLocationList { get; set; }

        public static void LoadProxyFromFile(string location)
        {
            var t = new Thread(() =>
            {
                if (ProxyLocationList == null)
                {
                    ProxyLocationList = new List<string>();
                }

                ProxyLocationList.Add(location);

                var accounts = File.ReadAllLines(location);
                Parallel.ForEach(accounts.Where(x => x.Contains(":") && !String.IsNullOrWhiteSpace(x)), async (account) =>
                {
                    var proxyData = account.Split(':');
                    try
                    {
                        var proxy = new ProxyData {Host = proxyData[0], Port = Int32.Parse(proxyData[1])};

                        //Create the Webrequest and make it look like it is coming from the RiotClient
                        var client =
                            (HttpWebRequest) WebRequest.Create(
                                "https://raw.githubusercontent.com/eddy5641/Re-Auth/master/latest.txt");
                        client.Method = WebRequestMethods.Http.Get;
                        client.Proxy = new WebProxy(proxy.Host, proxy.Port);

                        try
                        {

                            //Holy shit this is so much shorter than all of the other stuff. I love how POST requests are just that much longer
                            var response = (HttpWebResponse) client.GetResponse();
                            using (var rdr =
                                new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()))
                            {
                                if (Latest == rdr.ReadToEnd()
                                        .Split(new[] {Environment.NewLine}, StringSplitOptions.None).First())
                                {
                                    if (VerifyRiot(proxy.Host, proxy.Port) && !ProxyList.Contains(proxy))
                                    {
                                        await Main.Dispatcher.BeginInvoke(DispatcherPriority.Send,
                                            (Action) (() =>
                                            {
                                                Debugger.Log(0, "", $"Yay proxy works {Environment.NewLine}");
                                                ProxyList.Add(proxy);

                                                ProxyFlyout.ProxyGrid.ItemsSource = ProxyList;
                                                ProxyFlyout.ProxyGrid.Items.Refresh();
                                                CollectionViewSource.GetDefaultView(ProxyFlyout.ProxyGrid.ItemsSource).Refresh();
                                            }));
                                    }
                                }
                            }
                        }
                        catch
                        {
                            //Proxy doesn't work probably
                            Debugger.Log(0, "", $"Fucking proxy broken {Environment.NewLine}");
                        }
                    }
                    catch
                    {
                        //Fuck
                        Debugger.Log(0, "", $"Fucking proxy broken {Environment.NewLine}");
                    }
                });

                SettingsSave.ProxyList = JsonConvert.SerializeObject(ProxyLocationList);
            });

            t.Start();
        }

        public static bool VerifyRiot(string host, int port)
        {
            var returnVal = false;
            var t = new Thread(() =>
            {
                try
                {
                    //Try to crash with riot
                    var client =
                        (HttpWebRequest)WebRequest.Create(
                            "https://auth.riotgames.com/.well-known/openid-configuration");
                    client.Method = WebRequestMethods.Http.Get;
                    client.Proxy = new WebProxy(host, port);
                    var response = (HttpWebResponse)client.GetResponse();
                    using (var rdr =
                        new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()))
                    {
                        var data = rdr.ReadToEnd();
                        returnVal = true;
                    }
                }
                catch
                {
                    returnVal = false;
                }
            });
            t.Start();
            t.Join();
            return returnVal;
        }
    }

    public class Proxy
    {
        public static string Host { get; set; }

        public static int Port { get; set; }
    }

    public class SettingsSave
    {
        public bool Proxy { get; set; }
        public bool Store { get; set; }
        public bool InvService { get; set; }
        public int ProxyChange { get; set; }
        public string ProxyList { get; set; }
    }
}
