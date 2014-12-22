using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace MongoRepository
{
    public class MongoConfiguration
    {

        static Configuration configuration_;

        static MongoConfiguration()
        {
            configuration_ = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
        }
        public static MongoSection Section
        {
            get
            {
                var section = ConfigurationManager.GetSection("mongodyn/mongo");

                return (MongoSection)section;
            }
        }

        public static Configuration Configuration
        {
            get
            {
                return configuration_;
            }
        }


    }
}
