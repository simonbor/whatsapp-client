using Listener.Config.Sections;
using Microsoft.Extensions.Configuration;
using Windows.ApplicationModel;

// inspired by - https://blog.mzikmund.com/2019/11/using-appsettings-json-in-uwp/

namespace Listener.Config
{
    public class AppConfig
    {
        private readonly IConfigurationRoot _configurationRoot;

        public AppConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Package.Current.InstalledLocation.Path)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.production.json", optional: true);

            _configurationRoot = builder.Build();
        }

        private T GetSection<T>(string key) => _configurationRoot.GetSection(key).Get<T>();

        // retrieve sections
        // add each new section to the block
        public Whatsapp Whatsapp => GetSection<Whatsapp>(nameof(Whatsapp));
        public Server Server => GetSection<Server>(nameof(Server));
        public App App => GetSection<App>(nameof(App));
    }
}