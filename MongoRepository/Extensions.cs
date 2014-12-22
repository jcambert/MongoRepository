using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoRepository
{
    public static class Extensions
    {
        public static TModel Add<TModel>(this TModel entity) where TModel : IEntity<string>
        {
            return Repositories.GetCollection<TModel>().Add(entity);
        }

        public static TModel Update<TModel>(this TModel entity) where TModel : IEntity<string>
        {
            return Repositories.GetCollection<TModel>().Update(entity);
        }
    }
}
