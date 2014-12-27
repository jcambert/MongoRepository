using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace MongoRepository
{
    public interface IConfigurationManager
    {
        object GetSection(string sectionName);
    }

    public class ExeConfigurationManager : IConfigurationManager
    {

       private static Configuration configuration_;

       static ExeConfigurationManager()
        {
            configuration_ = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
        }

        public object GetSection(string sectionName)
        {
            return configuration_.GetSection(sectionName);
        }
    }

    public class AspConfigurationManager : IConfigurationManager
    {

        static AspConfigurationManager()
        {
         
        }

        public object GetSection(string sectionName)
        {
            return WebConfigurationManager.GetSection(sectionName);
        }
    }

    public static class ConfigurationManagerBuilder
    {
        public static IConfigurationManager Build()
        {
            var configFile = Path.GetFileName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            if (configFile.ToLower().Equals("web.config"))
                return new AspConfigurationManager();
            return new ExeConfigurationManager();
        }
    }
}
