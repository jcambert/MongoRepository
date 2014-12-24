using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoRepository
{
    internal static class IdGenerator
    {
        private static readonly MongoCollection NextIdTable;
        private const string LastId = "LastId";

        static IdGenerator()
        {
            //var url = new MongoUrl(Dynamic.GetConnString());
            var db = Repositories.Db; 
            NextIdTable = db.GetCollection(LastId);
        }

        /// <summary>
        /// Restart from zero the counter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        internal static void ResetCounter<TModel>()where TModel:IEntity<string>
        {
            var repo = Repositories.GetCollection<TModel>();

            ResetCounter(repo.Key.Name, repo.CollectionName);
        }


        internal static void ResetCounter<TKey,TModel>() //where TModel : IEntity<TKey>
        {
            var repo = Repositories.GetCollection<TKey,TModel>();

            ResetCounter(repo.Key.Name, repo.CollectionName);
        }

        /// <summary>
        /// Restart from zero the counter.
        /// </summary>
        /// <param name="t"></param>
        private static void ResetCounter(string idField,string collectionName)
        {
          
            NextIdTable.FindAndRemove(Query.EQ(idField, BsonValue.Create(collectionName)), SortBy.Null);
        }

        internal static int GetNextIdFor(string idField, string collectionName)
        {
            
            var f = NextIdTable.FindAndModify(Query.EQ(idField, collectionName), SortBy.Null, Update.Inc(LastId, 1), true, true);
            return f.ModifiedDocument[LastId].AsInt32;
        }
    }
}
