using Microsoft.Extensions.Configuration;

namespace Blog.Data
{
    public static class SiteKeys
    {
        private static IConfigurationSection _configuration;

        public static void Configure(IConfigurationSection configuration)
        {
            _configuration = configuration;
        }

        public static string WebSiteDomain => _configuration["WebSiteDomain"];

        public static string Token => _configuration["Secret"];
    }
}
