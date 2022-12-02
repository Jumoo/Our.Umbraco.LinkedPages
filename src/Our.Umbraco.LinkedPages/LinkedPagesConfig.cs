using Microsoft.Extensions.Configuration;

using Umbraco.Extensions;

namespace Our.Umbraco.LinkedPages;

public class LinkedPagesConfig
{

    private readonly IConfiguration _config;

    public LinkedPagesConfig(IConfiguration configuration)
    {
        _config = configuration;
    }

    public string RelationType => GetConfigValue("LinkedPages:RelationType", string.Empty);
    public bool ShowType => GetConfigValue("LinkedPages:ShowType", true);

    public string ignoredTypes => GetConfigValue("LinkedPages:Ignore", "umbMedia,umbDocument");

    private TResult GetConfigValue<TResult>(string path, TResult defaultValue)
    {
        var value = _config[path];
        if (value != null)
        {
            var attempt = value.TryConvertTo<TResult>();
            if (attempt) return attempt.Result;
        }

        return defaultValue;

    }
}
