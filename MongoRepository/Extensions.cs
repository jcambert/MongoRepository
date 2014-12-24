using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MongoRepository
{
    public static class Extensions
    {
        #region MongoDb Documents Extensions
        public static TModel AddDocument<TModel>(this TModel entity) //where TModel : IEntity<string>
        {
            return Repositories.GetCollection<TModel>().Add(entity);
        }

        public static TModel UpdateDocument<TModel>(this TModel entity) //where TModel : IEntity<string>
        {
            return Repositories.GetCollection<TModel>().Update(entity);
        }
        #endregion

        #region String Extensions
        public static string GetHash(this string input)
        {
            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();

            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);

            byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }
        #endregion
    }
}
