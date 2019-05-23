using Newtonsoft.Json;

namespace ReAuth
{
    /// <summary>
    /// This class helps with gathering data for 
    /// </summary>
    public class RiotAuthOpenIdConfiguration
    {
        [JsonProperty("acr_values_supported")]
        public string[] AcrValuesSupported { get; set; }

        [JsonProperty("token_endpoint_auth_signing_alg_values_supported")]
        public string[] TokenEndpointAuthSigningAlgValuesSupported { get; set; }

        [JsonProperty("response_modes_supported")]
        public string[] ResponseModesSupported { get; set; }

        [JsonProperty("end_session_endpoint")]
        public string EndSessionEndpoint { get; set; }

        [JsonProperty("jwks_uri")]
        public string JwksUri { get; set; }

        [JsonProperty("riot_recaptcha_public_key")]
        public string RiotRecaptchaPublicKey { get; set; }

        [JsonProperty("claim_types_supported")]
        public string[] ClaimTypesSupported { get; set; }

        [JsonProperty("grant_types_supported")]
        public string[] GrantTypesSupported { get; set; }

        [JsonProperty("claims_supported")]
        public string[] ClaimsSupported { get; set; }

        [JsonProperty("id_token_encryption_alg_values_supported")]
        public string[] IdTokenEncryptionAlgValuesSupported { get; set; }

        [JsonProperty("request_parameter_supported")]
        public bool RequestParameterSupported { get; set; }

        [JsonProperty("id_token_encryption_enc_values_supported")]
        public string[] IdTokenEncryptionEncValuesSupported { get; set; }

        [JsonProperty("riot_did_uri")]
        public string RiotDidUri { get; set; }

        [JsonProperty("userinfo_encryption_alg_values_supported")]
        public string[] UserinfoEncryptionAlgValuesSupported { get; set; }

        [JsonProperty("response_types_supported")]
        public string[] ResponseTypesSupported { get; set; }

        [JsonProperty("userinfo_encryption_enc_values_supported")]
        public string[] UserinfoEncryptionEncValuesSupported { get; set; }

        [JsonProperty("scopes_supported")]
        public string[] ScopesSupported { get; set; }

        [JsonProperty("token_endpoint_auth_methods_supported")]
        public string[] TokenEndpointAuthMethodsSupported { get; set; }

        [JsonProperty("display_values_supported")]
        public string[] DisplayValuesSupported { get; set; }

        [JsonProperty("claims_parameter_supported")]
        public bool ClaimsParameterSupported { get; set; }

        [JsonProperty("mutual_tls_sender_constrained_access_tokens")]
        public bool MutualTlsSenderConstrainedAccessTokens { get; set; }

        [JsonProperty("code_challenge_methods_supported")]
        public string[] CodeChallengeMethodsSupported { get; set; }

        [JsonProperty("userinfo_signing_alg_values_supported")]
        public string[] UserinfoSigningAlgValuesSupported { get; set; }

        [JsonProperty("riot_lol_userinfo_regions_enabled")]
        public string[] RiotLolUserinfoRegionsEnabled { get; set; }

        [JsonProperty("request_uri_quota")]
        public long RequestUriQuota { get; set; }

        [JsonProperty("request_uri_parameter_supported")]
        public bool RequestUriParameterSupported { get; set; }

        [JsonProperty("riot_lol_regions_disabled")]
        public string[] RiotLolRegionsDisabled { get; set; }

        [JsonProperty("riot_lol_regions_supported")]
        public string[] RiotLolRegionsSupported { get; set; }

        [JsonProperty("subject_types_supported")]
        public string[] SubjectTypesSupported { get; set; }

        [JsonProperty("revocation_endpoint")]
        public string RevocationEndpoint { get; set; }

        [JsonProperty("id_token_signing_alg_values_supported")]
        public string[] IdTokenSigningAlgValuesSupported { get; set; }

        [JsonProperty("issuer")]
        public string Issuer { get; set; }

        [JsonProperty("userinfo_endpoint")]
        public string UserinfoEndpoint { get; set; }

        [JsonProperty("token_endpoint")]
        public string TokenEndpoint { get; set; }

        [JsonProperty("authorization_endpoint")]
        public string AuthorizationEndpoint { get; set; }

        [JsonProperty("backchannel_logout_supported")]
        public bool BackchannelLogoutSupported { get; set; }

        [JsonProperty("ui_locales_supported")]
        public string[] UiLocalesSupported { get; set; }

        [JsonProperty("request_object_signing_alg_values_supported")]
        public string[] RequestObjectSigningAlgValuesSupported { get; set; }

        [JsonProperty("frontchannel_logout_supported")]
        public bool FrontchannelLogoutSupported { get; set; }
    }
}
