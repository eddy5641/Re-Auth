using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using Newtonsoft.Json;
using YamlDotNet.RepresentationModel;

namespace ReAuth
{
    public class AuthClass
    {

        /// <summary>
        /// This retrieves the OpenId Config
        /// </summary>
        /// <param name="url">The url of the OpenId Config</param>
        /// <returns>The config as <seealso cref="RiotAuthOpenIdConfiguration"></seealso></returns>
        public static RiotAuthOpenIdConfiguration GetOpenIdConfig(string url = "https://auth.riotgames.com/.well-known/openid-configuration")
        {
            using (var client = new WebClient())
            {
                return JsonConvert.DeserializeObject<RiotAuthOpenIdConfiguration>(client.DownloadString(url));
            }
        }


        /// <summary>
        ///     Parser for the System.Yaml file.
        ///     There probably is a better way to do this, but honestly I don't know how so somebody pls help meh
        /// </summary>
        /// <param name="systemFile">The input System.Yaml</param>
        /// <param name="region">The region that the user wants to get data for</param>
        /// <returns>The region data contained in System.Yaml</returns>
        public static RegionData ReadSystemRegionData(string systemFile, string region)
        {
            //Read the System.Yaml file
            var input = new StringReader(File.ReadAllText(systemFile));
            var yaml = new YamlStream();
            yaml.Load(input);

            //Use the rootnode to get the data required
            var mapping = (YamlMappingNode)yaml.Documents[0].RootNode;

            //Open the region information and load the selected region from the BaseRegion
            
            var validRegions = mapping["region_data"].AllNodes.Where(x => x.ToString() == region);
            if (!validRegions.Any())
            {
                return new RegionData {SuccessRead = false};
            }

            var regionDataYaml = mapping["region_data"][region];
            var serverRegionDataYaml = regionDataYaml["servers"];

            //Create output and populate it with data
            var regionData = new RegionData();
            regionData.AvailableLocales = new List<Locales>();
            regionData.DefaultLocale = (Locales)Enum.Parse(typeof(Locales), regionDataYaml["default_locale"].ToString());
            regionData.Rso = new RSO
            {
                AllowLoginQueueFallback = bool.Parse(regionDataYaml["rso"]["allow_lq_fallback"].ToString()),
                Kount = new Kount
                {
                    Collecter = regionDataYaml["rso"]["kount"]["collector"].ToString(),
                    Merchant = int.Parse(regionDataYaml["rso"]["kount"]["merchant"].ToString().Replace("'", ""))
                },
                Token = regionDataYaml["rso"]["token"].ToString()
            };
            regionData.PlatformId = (RsoPlatformId)Enum.Parse(typeof(RsoPlatformId), regionDataYaml["rso_platform_id"].ToString());
            regionData.Servers = new Servers();
            regionData.Servers.AccountRecovery = new AccountRecovery
            {
                ForgotPasswordUrl = serverRegionDataYaml["account_recovery"]["forgot_password_url"].ToString(),
                ForgotUsernameUrl = serverRegionDataYaml["account_recovery"]["forgot_username_url"].ToString()
            };
            regionData.Servers.Chat = new Chat
            {
                AllowSelfSignedCert =
                    bool.Parse(serverRegionDataYaml["chat"]["allow_self_signed_cert"].ToString()),
                ChatHost = serverRegionDataYaml["chat"]["chat_host"].ToString(),
                ChatPort = int.Parse(serverRegionDataYaml["chat"]["chat_port"].ToString())
            };
            regionData.Servers.DiscoverousServiceLocation = serverRegionDataYaml["discoverous_service_location"].ToString();
            regionData.Servers.EmailVerification = new EmailVerification
            {
                ExternalUrl = serverRegionDataYaml["email_verification"]["external_url"].ToString()
            };
            regionData.Servers.Entitlements = new Entitlements
            {
                ExternalUrl = serverRegionDataYaml["entitlements"]["entitlements_url"].ToString()
            };
            regionData.Servers.Lcds = new Lcds
            {
                LcdsHost = serverRegionDataYaml["lcds"]["lcds_host"].ToString(),
                LcdsPort = int.Parse(serverRegionDataYaml["lcds"]["lcds_port"].ToString()),
                LoginQueueUrl = serverRegionDataYaml["lcds"]["login_queue_url"].ToString(),
                UseTls = bool.Parse(serverRegionDataYaml["lcds"]["use_tls"].ToString())
            };
            regionData.Servers.LicenseAgrerementUrls = new LicenseAgrerementUrls
            {
                Eula = serverRegionDataYaml["license_agreement_urls"].AllNodes.Contains("eula")
                    ? serverRegionDataYaml["license_agreement_urls"]["eula"].ToString()
                    : "http://leagueoflegends.com/{language}/legal/eula",
                TermsOfUse = serverRegionDataYaml["license_agreement_urls"]["terms_of_use"].ToString()
            };
            regionData.Servers.Payments = new Payments
            {
                PaymentsHost = serverRegionDataYaml.AllNodes.Contains("payments") ? serverRegionDataYaml["payments"]["payments_host"].ToString() : "https://plstore.{Region}.lol.riotgames.com"
            };
            regionData.Servers.PreloginConfig = new PreloginConfig
            {
                PreloginConfigUrl = serverRegionDataYaml["prelogin_config"]["prelogin_config_url"].ToString()
            };
            regionData.Servers.Rms = new Rms
            {
                RmsHeartbeatIntervalSeconds =
                    int.Parse(serverRegionDataYaml["rms"]["rms_heartbeat_interval_seconds"].ToString()),
                RmsUrl = serverRegionDataYaml["rms"]["rms_url"].ToString()
            };
            regionData.Servers.ServiceStatus = new ServiceStatus
            {
                ApiUrl = serverRegionDataYaml["service_status"]["api_url"].ToString(),
                HumanReadableStatusUrl =
                    serverRegionDataYaml["service_status"]["human_readable_status_url"].ToString()
            };
            regionData.Servers.Store = new Store
            {
                StoreUrl = serverRegionDataYaml["store"]["store_url"].ToString()
            };
            regionData.Servers.Voice = new Voice
            {
                AccessTokenUrl = serverRegionDataYaml["voice"]["access_token_uri"].ToString(),
                AuthTokenUrl = serverRegionDataYaml["voice"]["auth_token_uri"].ToString(),
                UseExternalAuth = bool.Parse(serverRegionDataYaml["voice"]["use_external_auth"].ToString()),
                VoiceDomain = serverRegionDataYaml["voice"]["voice_domain"].ToString(),
                VoiceUrl = serverRegionDataYaml["voice"]["voice_url"].ToString()
            };
            regionData.WebRegion = regionDataYaml["web_region"].ToString();
            regionData.SuccessRead = true;

            regionData.AvailableLocales.Clear();
            foreach (var locale in regionDataYaml["available_locales"].AllNodes)
            {
                var localAsEnum = (Locales)Enum.Parse(typeof(Locales),
                    locale.ToString().Replace("[", "").Replace("]", "").Replace(" ", ""));
                if (regionData.AvailableLocales.Contains(localAsEnum))
                    continue;

                regionData.AvailableLocales.Add(localAsEnum);
            }

            return regionData;
        }

        /// <summary>
        /// Retrieve the User Login Token
        /// </summary>
        /// <param name="username">The username of the player</param>
        /// <param name="password">The password of the player</param>
        /// <param name="regionData">The region the player wants to connect to</param>
        /// <param name="config">The OpenId Config from <see cref="GetOpenIdConfig"/></param>
        /// <returns>The login token or failure information as <see cref="RiotAuthToken"/></returns>
        public static RiotAuthToken GetLoginToken(string username, string password, RegionData regionData, RiotAuthOpenIdConfiguration config)
        {
            //Create the Webrequest and make it look like it is coming from the RiotClient
            var client = (HttpWebRequest)WebRequest.Create(config.TokenEndpoint);
            client.Method = WebRequestMethods.Http.Post;
            if (StaticHelper.SettingsSave.Proxy)
            {
                client.Proxy = ProxyHelper.GetProxy();
            }
            //Lazy Hack
            client.Host = config.TokenEndpoint.Replace("//", "/").Split('/')[1];
            client.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            client.UserAgent = "RiotClient/18.0.0 (rso-auth)";
            client.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
            client.ProtocolVersion = HttpVersion.Version11;
            client.ContentType = "application/x-www-form-urlencoded";
            //No idea what this dsid, but just generate a guid for now because it is the same length and random
            var dsid = Guid.NewGuid().ToString("N");
            client.Headers.Set("X-Riot-DSID", dsid);
            client.Accept = "application/json";

            //The information to Post in the webrequest 
            var postString = "client_assertion_type=urn:ietf:params:oauth:client-assertion-type:jwt-bearer&" +
                             $"client_assertion={regionData.Rso.Token}&" +
                             "grant_type=password&" +
                             $"username={regionData.PlatformId}|{username}&" +
                             $"password={password}&" +
                             "scope=openid offline_access lol ban profile email phone";

            //Convert to bytes
            var postBytes = Encoding.UTF8.GetBytes(postString);
            client.ContentLength = postBytes.Length;

            //More things to try to make it look like it is coming from the riot client
            client.ServicePoint.Expect100Continue = false;
            client.Headers.Remove(HttpRequestHeader.Pragma);

            try
            {
                //Send the POST request
                var requestStream = client.GetRequestStream();
                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Close();
            }
            catch
            {
                return StaticHelper.SettingsSave.Proxy
                    ? new RiotAuthToken(RiotAuthResult.BadProxy, null, dsid, regionData)
                    : new RiotAuthToken(RiotAuthResult.ConProblem, null, dsid, regionData);
            }

            //Retrieve the response
            try
            {
                var response = (HttpWebResponse)client.GetResponse();
                using (var rdr =
                    new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()))
                {
                    //Success, the credentials are correct, allow the login to continue and create a RiotAuthToken
                    var result = rdr.ReadToEnd();
                    return new RiotAuthToken(RiotAuthResult.Success, result, dsid, regionData);
                }
            }
            catch (WebException e)
            {
                //Credentials are wrong, fail the login request and tell the error to the user
                using (var response = e.Response)
                {
                    //var httpResponse = (HttpWebResponse) response;
                    using (var data = response.GetResponseStream())
                    using (var reader = new StreamReader(data ?? throw new InvalidOperationException()))
                    {
                        var text = reader.ReadToEnd();

                        return text.Contains("invalid_credentials")
                            ? new RiotAuthToken(RiotAuthResult.InvalidCredentials, null, dsid, regionData)
                            : new RiotAuthToken(RiotAuthResult.UnknownReason, null, dsid, regionData);
                    }
                }
            }
        }


        public static StoreInfo GetStoreIpRp(RiotAuthToken token, RegionData regionData)
        {
            //Create the Webrequest and make it look like it is coming from the RiotClient
            var client = (HttpWebRequest)WebRequest.Create(regionData.Servers.Store.StoreUrl + "/storefront/v2/wallet?language=en_US");
            client.Method = WebRequestMethods.Http.Get;
            if (token.Proxy != null)
            {
                client.Proxy = token.Proxy;
            }
            client.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            client.UserAgent = "RiotClient/18.0.0 (lol-store)";
            client.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
            client.ProtocolVersion = HttpVersion.Version11;
            client.Accept = "application/json";
            client.Headers.Set(HttpRequestHeader.Authorization, token.AccessTokenJson.TokenType + " " + token.AccessTokenJson.IdToken);
            client.KeepAlive = false;

            try
            {

                //Holy shit this is so much shorter than all of the other stuff. I love how POST requests are just that much longer
                var response = (HttpWebResponse) client.GetResponse();
                using (var rdr =
                    new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()))
                {
                    return JsonConvert.DeserializeObject<StoreInfo>(rdr.ReadToEnd());
                }
            }
            catch
            {
                return new StoreInfo {ip = 0, rp = 0};
            }
        }

        public static UserData GetUserData(RiotAuthToken token)
        {
            var tokenString = token.AccessTokenJson.AccessToken.Split('.')[1];
            int mod4 = tokenString.Length % 4;
            if (mod4 > 0)
            {
                tokenString += new string('=', 4 - mod4);
            }
            return JsonConvert.DeserializeObject<UserData>(Encoding.UTF8.GetString(Convert.FromBase64String(tokenString)));
        }

        public static ChampionJwt GetChampionJwt(RiotAuthToken token, UserData userData, RegionData regionData)
        {
            //Create the Webrequest and make it look like it is coming from the RiotClient
            var client = (HttpWebRequest)WebRequest.Create($"https://{regionData.PlatformId}.cap.riotgames.com/lolinventoryservice/v2/inventories?puuid={userData.Sub}&location={regionData.Servers.DiscoverousServiceLocation}&accountId={userData.Dat.U}&inventoryTypes=CHAMPION&signed=true");
            client.Method = WebRequestMethods.Http.Get;
            if (token.Proxy != null)
            {
                client.Proxy = token.Proxy;
            }
            client.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            client.UserAgent = "RiotClient/18.0.0 (lol-inventory)";
            client.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
            client.ProtocolVersion = HttpVersion.Version11;
            client.ContentType = "application/json";
            client.Accept = "application/json";
            client.Headers.Set(HttpRequestHeader.Authorization, token.AccessTokenJson.TokenType + " " + token.AccessTokenJson.IdToken);
            client.KeepAlive = false;
            try
            {

                //Holy shit this is so much shorter than all of the other stuff. I love how POST requests are just that much longer
                var response = (HttpWebResponse)client.GetResponse();
                using (var rdr =
                    new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()))
                {
                    var rawJson = JsonConvert.DeserializeObject<ChampData>(rdr.ReadToEnd());
                    //ChampData

                    var tokenString = rawJson.Data.ItemsJwt.Split('.')[1];
                    var mod4 = tokenString.Length % 4;
                    if (mod4 > 0)
                    {
                        tokenString += new string('=', 4 - mod4);
                    }

                    return JsonConvert.DeserializeObject<ChampionJwt>(Encoding.UTF8.GetString(Convert.FromBase64String(tokenString)));
                }
            }
            catch
            {
                return null;
            }
        }

        public static SpellBook GetUserSpellBook(RiotAuthToken token, UserData userData, RegionData regionData)
        {

            //Create the Webrequest and make it look like it is coming from the RiotClient
            var client = (HttpWebRequest)WebRequest.Create($"https://{regionData.PlatformId}.cap.riotgames.com/lolinventoryservice/v2/inventories?puuid={userData.Sub}&location={regionData.Servers.DiscoverousServiceLocation}&accountId={userData.Dat.U}&inventoryTypes=SPELL_BOOK_PAGE");
            client.Method = WebRequestMethods.Http.Get;
            if (token.Proxy != null)
            {
                client.Proxy = token.Proxy;
            }
            client.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            client.UserAgent = "RiotClient/18.0.0 (lol-inventory)";
            client.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
            client.ProtocolVersion = HttpVersion.Version11;
            client.ContentType = "application/json";
            client.Accept = "application/json";
            client.Headers.Set(HttpRequestHeader.Authorization, token.AccessTokenJson.TokenType + " " + token.AccessTokenJson.IdToken);
            client.KeepAlive = false;
            try
            {

                //Holy shit this is so much shorter than all of the other stuff. I love how POST requests are just that much longer
                var response = (HttpWebResponse) client.GetResponse();
                using (var rdr =
                    new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()))
                {
                    return JsonConvert.DeserializeObject<SpellBook>(rdr.ReadToEnd());
                }
            }
            catch
            {
                return new SpellBook
                {
                    Data = new Data
                    {
                        Items = new Items {SpellBookPage = new[] {new SpellBookPage { Quantity = 0 }}}
                    }
                };
            }
        }

        public static bool Deauth(RiotAuthToken token, RiotAuthOpenIdConfiguration config, RegionData regionData)
        {
            //Create the Webrequest and make it look like it is coming from the RiotClient
            var client = (HttpWebRequest)WebRequest.Create(config.RevocationEndpoint);
            client.Method = WebRequestMethods.Http.Post;
            if (token.Proxy != null)
            {
                client.Proxy = token.Proxy;
            }
            //Lazy Hack
            client.Host = config.RevocationEndpoint.Replace("//", "/").Split('/')[1];
            client.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            client.UserAgent = "RiotClient/18.0.0 (rso-auth)";
            client.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
            client.ProtocolVersion = HttpVersion.Version11;
            client.ContentType = "application/x-www-form-urlencoded";
            //Use the DSID from RiotAuthToekn
            client.Headers.Set("X-Riot-DSID", token.Dsid);
            client.Accept = "application/json";


            //The information to Post in the webrequest 
            var postString = "client_assertion_type=urn:ietf:params:oauth:client-assertion-type:jwt-bearer&" +
                             $"client_assertion={regionData.Rso.Token}&" +
                             "grant_type=password&" +
                             $"token={token.AccessTokenJson.RefreshToken}&" +
                             "token_type_hint=refresh_token";

            //Convert to bytes
            var postBytes = Encoding.UTF8.GetBytes(postString);
            client.ContentLength = postBytes.Length;

            //More things to try to make it look like it is coming from the riot client
            client.ServicePoint.Expect100Continue = false;
            client.Headers.Remove(HttpRequestHeader.Pragma);

            //Send the POST request
            var requestStream = client.GetRequestStream();
            requestStream.Write(postBytes, 0, postBytes.Length);
            requestStream.Close();


            //Retrieve the response
            try
            {
                var response = (HttpWebResponse)client.GetResponse();
                using (var rdr =
                    new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()))
                {
                    //Success
                    return true;
                }
            }
            catch
            {
                //Something happened
                return false;
            }
        }
    }
    public class StoreInfo
    {
        public int ip { get; set; }
        public int rp { get; set; }
    }

    public class RiotAuthToken
    {
        public RiotAuthToken(RiotAuthResult result, string accessTokenJson, string dsid, RegionData regionData)
        {
            Result = result;
            Dsid = dsid;
            RegionData = regionData;

            if (result == RiotAuthResult.Success)
                AccessTokenJson = JsonConvert.DeserializeObject<AccessTokenJson>(accessTokenJson);
        }
        public RiotAuthToken(RiotAuthResult result, string accessTokenJson, string dsid, RegionData regionData, WebProxy proxy)
        {
            Result = result;
            Dsid = dsid;
            RegionData = regionData;
            Proxy = proxy;

            if (result == RiotAuthResult.Success)
                AccessTokenJson = JsonConvert.DeserializeObject<AccessTokenJson>(accessTokenJson);
        }

        public WebProxy Proxy { get; }

        public RiotAuthResult Result { get; }

        public string Dsid { get; }

        public AccessTokenJson AccessTokenJson { get; }

        public RegionData RegionData { get; }
    }


    public enum RiotAuthResult
    {
        Success,
        InvalidCredentials,
        BadProxy,
        Banned,
        ConProblem,
        TooManyReq,
        UnknownReason
    }

    public class AccessTokenJson
    {

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("id_token")]
        public string IdToken { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }
    }

    public class EntitlementsTokenJson
    {
        [JsonProperty("entitlements_token")]
        public string EntitlementsToken { get; set; }
    }

    public class SpellBook
    {
        [JsonProperty("data")]
        public Data Data { get; set; }
    }

    public class Data
    {
        [JsonProperty("puuid")]
        public string Puuid { get; set; }

        [JsonProperty("accountId")]
        public long AccountId { get; set; }

        [JsonProperty("expires")]
        public string Expires { get; set; }

        [JsonProperty("items")]
        public Items Items { get; set; }
    }

    public class Items
    {
        [JsonProperty("SPELL_BOOK_PAGE")]
        public SpellBookPage[] SpellBookPage { get; set; }
    }

    public class SpellBookPage
    {
        [JsonProperty("itemId")]
        public long ItemId { get; set; }

        [JsonProperty("inventoryType")]
        public string InventoryType { get; set; }

        [JsonProperty("ownedQuantity")]
        public long Quantity { get; set; }

        [JsonProperty("f2p")]
        public bool F2P { get; set; }

        [JsonProperty("rental")]
        public bool Rental { get; set; }

        [JsonProperty("lsb")]
        public bool Lsb { get; set; }
    }


    public class UserData
    {
        [JsonProperty("sub")]
        public string Sub { get; set; }

        [JsonProperty("scp")]
        public string[] Scp { get; set; }

        [JsonProperty("clm")]
        public string[] Clm { get; set; }

        [JsonProperty("dat")]
        public Dat Dat { get; set; }

        [JsonProperty("iss")]
        public string Iss { get; set; }

        [JsonProperty("exp")]
        public long Exp { get; set; }

        [JsonProperty("iat")]
        public long Iat { get; set; }

        [JsonProperty("jti")]
        public string Jti { get; set; }

        [JsonProperty("cid")]
        public string Cid { get; set; }
    }

    public class Dat
    {
        [JsonProperty("r")]
        public string R { get; set; }

        [JsonProperty("c")]
        public string C { get; set; }

        [JsonProperty("u")]
        public long U { get; set; }
    }

    public class ChampionJwt
    {
        [JsonProperty("sub")]
        public string Sub { get; set; }

        [JsonProperty("exp")]
        public long Exp { get; set; }

        [JsonProperty("items")]
        public ItemsChamp Items { get; set; }

        [JsonProperty("iat")]
        public long Iat { get; set; }
    }

    public class ItemsChamp
    {
        [JsonProperty("CHAMPION")]
        public Champion[] Champion { get; set; }
    }

    public class Champion
    {
        [JsonProperty("itemId")]
        public long ItemId { get; set; }

        [JsonProperty("inventoryType")]
        public string InventoryType { get; set; }

        [JsonProperty("expirationDate")]
        public object ExpirationDate { get; set; }

        [JsonProperty("purchaseDate")]
        public string PurchaseDate { get; set; }

        [JsonProperty("quantity")]
        public long Quantity { get; set; }

        [JsonProperty("ownedQuantity")]
        public long OwnedQuantity { get; set; }

        [JsonProperty("usedInGameDate")]
        public string UsedInGameDate { get; set; }

        [JsonProperty("entitlementId")]
        public object EntitlementId { get; set; }

        [JsonProperty("entitlementTypeId")]
        public string EntitlementTypeId { get; set; }

        [JsonProperty("instanceId")]
        public object InstanceId { get; set; }

        [JsonProperty("instanceTypeId")]
        public string InstanceTypeId { get; set; }

        [JsonProperty("payload")]
        public object Payload { get; set; }

        [JsonProperty("f2p")]
        public bool F2P { get; set; }

        [JsonProperty("rental")]
        public bool Rental { get; set; }

        [JsonProperty("loyalty")]
        public bool Loyalty { get; set; }

        [JsonProperty("wins")]
        public object Wins { get; set; }
    }

    public class ChampData
    {
        [JsonProperty("data")]
        public DataCS Data { get; set; }
    }

    public class DataCS
    {
        [JsonProperty("puuid")]
        public string Puuid { get; set; }

        [JsonProperty("accountId")]
        public long AccountId { get; set; }

        [JsonProperty("expires")]
        public string Expires { get; set; }

        [JsonProperty("items")]
        public Items Items { get; set; }

        [JsonProperty("itemsJwt")]
        public string ItemsJwt { get; set; }
    }
}
