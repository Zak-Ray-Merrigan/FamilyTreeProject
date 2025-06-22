using Azure.Data.AppConfiguration;
using Azure.Identity;

namespace FamilyTreeLibrary.Infrastructure.Resource
{
    public class FamilyTreeConfiguration(string apiConfigurationUri)
    {
        private readonly ConfigurationClient client = new(new Uri(apiConfigurationUri), new DefaultAzureCredential());

        public string this[string key]
        {
            get
            {
                return client.GetConfigurationSetting(key).Value.Value;
            }
            set
            {
                client.SetConfigurationSetting(new(key, value));
            }
        }

        public void RemoveSetting(string key)
        {
            client.DeleteConfigurationSetting(key);
        }
    }
}