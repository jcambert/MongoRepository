using log4net;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoRepository
{
    public static class Repositories
    {
        private static readonly Dictionary<string, object> repositorories_ = new Dictionary<string, object>();
        private static string _defaultConnectionstringName = "MongoServerSettings";
        private static readonly ILog logger = LogManager.GetLogger(typeof(Repositories));
        public static string ConnectionStringOrName
        {
            get { return _defaultConnectionstringName; }
            set
            {
                Contract.Requires(value != null && value.Trim().Length > 0, "ConnectionStringOrName must not be nul nor empty");
                _defaultConnectionstringName = value;
            }
        }

        public static MongoDatabase Db
        {
            get
            {
                logger.Debug("Dynamic.Db");
                MongoClientSettings c_settings = new MongoClientSettings();
                c_settings.ConnectionMode = ConnectionMode.Automatic;
                c_settings.Server = new MongoServerAddress(getHost(), getPort());

                MongoServer server = new MongoClient(c_settings).GetServer();

                MongoDatabase db = server.GetDatabase(getDatabaseName());



                return db;

            }
        }


        public static string getHost()
        {
            logger.Debug("Dynamic.getHost()");
            return MongoConfiguration.Section.Host;
        }

        public static int getPort()
        {
            logger.Debug("Dynamic.getPort()");
            return MongoConfiguration.Section.Port;
        }


        public static string getDatabaseName()
        {
            logger.Debug("Dynamic.getDatabaseName()");
            return MongoConfiguration.Section.Database;
        }

        public static MongoRepository<TKey, TModel> GetCollection<TKey, TModel>()
             where TModel : IEntity<TKey>
        {
            logger.Debug(string.Format("Dynamic.GetCollection<{0},{1}>()", typeof(TKey).Name, typeof(TModel).Name));
            object value;
            var model = typeof(TModel).Name;
            if (!repositorories_.TryGetValue(model, out value))
            {
                string collectionName=typeof(TModel).Name;
                 var mongoModel = typeof(TModel).GetCustomAttribute<CollectionNameAttribute>(true);
                 if (mongoModel != null)
                     collectionName = mongoModel.Name;

                
                 PropertyInfo key = typeof(TModel).GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance).Where(prop => Attribute.IsDefined(prop, typeof(BsonIdAttribute))).First();

                var repo = new MongoRepository<TKey, TModel>(key.Name, collectionName);
                repositorories_[model] = repo;
            }
            return (MongoRepository<TKey, TModel>)repositorories_[model];
        }

        public static MongoRepository<string, TModel> GetCollection< TModel>()
            where TModel : IEntity<string>
        {
            return GetCollection<string, TModel>();
        }

    }

}
