using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace ReAuth
{
    public static class ProxyHelper
    {

        private static int CurrentPosition = 1;
        private static int Amount = 0;

        public static WebProxy GetProxy()
        {
            if (!StaticHelper.ProxyList.Any())
                return null;
            Amount++;

            var proxy = new WebProxy(StaticHelper.ProxyList[CurrentPosition - 1].Host, StaticHelper.ProxyList[CurrentPosition - 1].Port);
            if (Amount == StaticHelper.SettingsSave.ProxyChange && CurrentPosition == StaticHelper.ProxyList.Count)
                CurrentPosition = 1;
            else if (Amount == StaticHelper.SettingsSave.ProxyChange)
            {
                CurrentPosition++;
            }

            return proxy;
        }

        
    }

    public class ProxyData
    {
        public string Host { get; set; }
        public int Port { get; set; }

    }
}
