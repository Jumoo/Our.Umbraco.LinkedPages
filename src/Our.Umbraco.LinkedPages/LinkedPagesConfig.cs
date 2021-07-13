#if NETCOREAPP
using Microsoft.Extensions.Configuration;
using Umbraco.Extensions;
#else 
using System.Configuration;
using Umbraco.Core;
#endif


namespace Our.Umbraco.LinkedPages
{
    public class LinkedPagesConfig
    {

#if NETCOREAPP
        private readonly IConfiguration _config;

        public LinkedPagesConfig(IConfiguration configuration)
        {
            _config = configuration;
        }
#endif

        public string RelationType => GetConfigValue("LinkedPages:RelationType", string.Empty);
        public bool ShowType => GetConfigValue("LinkedPages:ShowType", true);

        public string ignoredTypes => GetConfigValue("LinkedPages:Ignore", "umbMedia,umbDocument");

        private TResult GetConfigValue<TResult>(string path, TResult defaultValue)
        {
#if NETCOREAPP
            var value = _config[path]; 
#else
            var value = ConfigurationManager.AppSettings[path.Replace(":", ".")];
#endif
            if (value != null)
            {
                var attempt = value.TryConvertTo<TResult>();
                if (attempt) return attempt.Result;
            }

            return defaultValue;

        }
    }
}
