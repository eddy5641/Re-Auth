using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ReAuth
{
    /// <summary>
    /// All the region data from System.Yaml
    /// </summary>
    public class RegionData
    {
        public List<Locales> AvailableLocales { get; set; }

        public Locales DefaultLocale { get; set; }

        public RSO Rso { get; set; }

        public RsoPlatformId PlatformId { get; set; }

        public Servers Servers { get; set; }

        public string WebRegion { get; set; }

        public bool SuccessRead { get; set; }
    }

    #region Locales
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum Locales
    {
        cs_CZ,
        de_DE,
        defaultOrUnknown,
        el_GR,
        en_AU,
        en_GB,
        en_PL,
        en_US,
        es_AR,
        es_ES,
        es_MX,
        fr_FR,
        hu_HU,
        it_IT,
        ja_JP,
        ko_KR,
        pl_PL,
        pt_BR,
        ro_RO,
        ru_RU,
        tr_TR
    }
    #endregion Locales

    // ReSharper disable once InconsistentNaming
    public class RSO
    {
        public bool AllowLoginQueueFallback { get; set; }

        public Kount Kount { get; set; }

        public string Token { get; set; }
    }

    public class Kount
    {
        public string Collecter { get; set; }

        public int Merchant { get; set; }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum RsoPlatformId
    {
        BR1,
        EUN1,
        EUW1,
        JP1,
        LA1,
        LA2,
        NA1,
        OC1,
        RU,
        TR1,
        PBE1
    }

    public class Servers
    {
        public AccountRecovery AccountRecovery { get; set; }

        public Chat Chat { get; set; }

        public string DiscoverousServiceLocation { get; set; }

        public EmailVerification EmailVerification { get; set; }

        public Entitlements Entitlements { get; set; }

        public Lcds Lcds { get; set; }

        public LicenseAgrerementUrls LicenseAgrerementUrls { get; set; }

        public Payments Payments { get; set; }

        public PreloginConfig PreloginConfig { get; set; }

        public Rms Rms { get; set; }

        public ServiceStatus ServiceStatus { get; set; }

        public Store Store { get; set; }

        public Voice Voice { get; set; }
    }

    public class AccountRecovery
    {
        public string ForgotPasswordUrl { get; set; }

        public string ForgotUsernameUrl { get; set; }
    }

    public class Chat
    {
        public bool AllowSelfSignedCert { get; set; }

        public string ChatHost { get; set; }

        public int ChatPort { get; set; }
    }

    public class EmailVerification
    {
        public string ExternalUrl { get; set; }
    }

    public class Entitlements
    {
        public string ExternalUrl { get; set; }
    }

    public class Lcds
    {
        public string LcdsHost { get; set; }

        public int LcdsPort { get; set; }

        public string LoginQueueUrl { get; set; }

        public bool UseTls { get; set; }
    }

    public class LicenseAgrerementUrls
    {
        public string Eula { get; set; }

        public string TermsOfUse { get; set; }
    }

    public class Payments
    {
        public string PaymentsHost { get; set; }
    }

    public class PreloginConfig
    {
        public string PreloginConfigUrl { get; set; }
    }

    public class Rms
    {
        public int RmsHeartbeatIntervalSeconds { get; set; }

        public string RmsUrl { get; set; }
    }

    public class ServiceStatus
    {
        public string ApiUrl { get; set; }

        public string HumanReadableStatusUrl { get; set; }
    }

    public class Store
    {
        public string StoreUrl { get; set; }
    }

    public class Voice
    {
        public string AccessTokenUrl { get; set; }

        public string AuthTokenUrl { get; set; }

        public bool UseExternalAuth { get; set; }

        public string VoiceDomain { get; set; }

        public string VoiceUrl { get; set; }
    }
}
