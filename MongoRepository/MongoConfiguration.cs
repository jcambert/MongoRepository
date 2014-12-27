using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

namespace MongoRepository
{
    public class MongoConfiguration
    {
        static IConfigurationManager configuration_;
       // static Configuration configuration_;

        static MongoConfiguration()
        {
           // var id=AppDomain.CurrentDomain.ApplicationIdentity;
           // var configFile = Path.GetFileName( AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            //configuration_ = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            configuration_ = ConfigurationManagerBuilder.Build();
        }
        public static MongoSection Section
        {
            get
            {
                var section = configuration_.GetSection("mongodyn/mongo");

                return (MongoSection)section;
            }
        }

      


    }
}
