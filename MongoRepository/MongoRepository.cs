using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace MongoRepository
{
    public class MongoRepository<TKey, TModel> : IRepository<TModel, TKey>
       // where TModel : IEntity<TKey>
    {

        public event EventHandler<ModelEventArgs<TModel>> onBeforeSave = delegate { };
        public event EventHandler<ModelEventArgs<TModel>> onAfterSave = delegate { };
        public event EventHandler<ModelEventArgs<TModel>> onBeforeAdd = delegate { };
        public event EventHandler<ModelEventArgs<TModel>> onAfterAdd= delegate { };

        /// <summary>
        /// MongoCollection field.
        /// </summary>
        protected internal MongoCollection<TModel> collection;

        internal MongoRepository(PropertyInfo key, string collectionName)
        {
            collection = Repositories.Db.GetCollection<TModel>(collectionName);
            this.Key = key;
            CollectionType = typeof(TModel);
        }

        public Type CollectionType
        {
            get;
            private set;
        }

        public PropertyInfo Key{ get; private set; }

        /// <summary>
        /// Gets the Mongo collection (to perform advanced operations).
        /// </summary>
        /// <remarks>
        /// One can argue that exposing this property (and with that, access to it's Database property for instance
        /// (which is a "parent")) is not the responsibility of this class. Use of this property is highly discouraged;
        /// for most purposes you can use the MongoRepositoryManager&lt;TModel&gt;
        /// </remarks>
        /// <value>The Mongo collection (to perform advanced operations).</value>
        public MongoCollection<TModel> Collection
        {
            get { return this.collection; }
        }

        /// <summary>
        /// Gets the name of the collection
        /// </summary>
        public string CollectionName
        {
            get { return this.collection.Name; }
        }

        /// <summary>
        /// Returns the TModel by its given id.
        /// </summary>
        /// <param name="id">The Id of the entity to retrieve.</param>
        /// <returns>The Entity TModel.</returns>
        public virtual TModel GetById(TKey id)
        {
            //if (typeof(TModel).IsSubclassOf(typeof(Entity)))
           // {
                return this.GetById(new ObjectId(id as string));
            //}
           // this.Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(id));
            //return this.collection.FindOneByIdAs<TModel>(BsonValue.Create(id));
        }

        /// <summary>
        /// Returns the TModel by its given id.
        /// </summary>
        /// <param name="id">The Id of the entity to retrieve.</param>
        /// <returns>The Entity TModel.</returns>
        public virtual TModel GetById(ObjectId id)
        {
            return this.collection.FindOneByIdAs<TModel>(id);
        }

        /// <summary>
        /// Adds the new entity in the repository.
        /// </summary>
        /// <param name="entity">The entity TModel.</param>
        /// <returns>The added entity including its new ObjectId.</returns>
        public virtual TModel Add(TModel entity)
        {
            onBeforeAdd(this, new ModelEventArgs<TModel>(entity));
            this.collection.Insert<TModel>(entity);
            onAfterAdd(this, new ModelEventArgs<TModel>(entity));
            return entity;
        }

        /// <summary>
        /// Adds the new entities in the repository.
        /// </summary>
        /// <param name="entities">The entities of type TModel.</param>
        public virtual void Add(IEnumerable<TModel> entities)
        {
            this.collection.InsertBatch<TModel>(entities);
        }

        /// <summary>
        /// Upserts an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The updated entity.</returns>
        public virtual TModel Update(TModel entity)
        {
            onBeforeSave(this, new ModelEventArgs<TModel>(entity));
            this.collection.Save<TModel>(entity);
            onAfterSave(this, new ModelEventArgs<TModel>(entity));
            return entity;
        }

        /// <summary>
        /// Upserts the entities.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        public virtual void Update(IEnumerable<TModel> entities)
        {
            foreach (TModel entity in entities)
            {
                this.collection.Save<TModel>(entity);
            }
        }

        /// <summary>
        /// Deletes an entity from the repository by its id.
        /// </summary>
        /// <param name="id">The entity's id.</param>
        public virtual void Delete(TKey id)
        {
            if (typeof(TModel).IsSubclassOf(typeof(Entity)))
            {
                this.collection.Remove(Query.EQ("_id", new ObjectId(id as string)));
            }
            else
            {
                this.collection.Remove(Query.EQ("_id", BsonValue.Create(id)));
            }
        }

        /// <summary>
        /// Deletes an entity from the repository by its ObjectId.
        /// </summary>
        /// <param name="id">The ObjectId of the entity.</param>
        public virtual void Delete(ObjectId id)
        {
            this.collection.Remove(Query.EQ("_id", id));
        }

        /// <summary>
        /// Deletes the given entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        public virtual void Delete(TModel entity)
        {
            var id =(TKey) Key.GetGetMethod().Invoke(entity, new object[] { });
            this.Delete(id);
        }

        /// <summary>
        /// Deletes the entities matching the predicate.
        /// </summary>
        /// <param name="predicate">The expression.</param>
        public virtual void Delete(Expression<Func<TModel, bool>> predicate)
        {
            foreach (TModel entity in this.collection.AsQueryable<TModel>().Where(predicate))
            {
                var id = (TKey)Key.GetGetMethod().Invoke(entity, new object[] { });
                this.Delete(id);
            }
        }

        /// <summary>
        /// Deletes all entities in the repository.
        /// </summary>
        public virtual void DeleteAll()
        {
            this.collection.RemoveAll();
        }

        /// <summary>
        /// Deletes all entities in the repository.
        /// Reset autoincrement counter
        /// </summary>
        /// <param name="resetCounter"></param>
        public void DeleteAll(bool resetCounter)
        {

            Collection.Drop();
            if (resetCounter)
            {
                IdGenerator.ResetCounter<TKey, TModel>();
            }
        }

        /// <summary>
        /// Counts the total entities in the repository.
        /// </summary>
        /// <returns>Count of entities in the collection.</returns>
        public virtual long Count()
        {
            return this.collection.Count();
        }

        /// <summary>
        /// Checks if the entity exists for given predicate.
        /// </summary>
        /// <param name="predicate">The expression.</param>
        /// <returns>True when an entity matching the predicate exists, false otherwise.</returns>
        public virtual bool Exists(Expression<Func<TModel, bool>> predicate)
        {
            return this.collection.AsQueryable<TModel>().Any(predicate);
        }

        /// <summary>
        /// Lets the server know that this thread is about to begin a series of related operations that must all occur
        /// on the same connection. The return value of this method implements IDisposable and can be placed in a using
        /// statement (in which case RequestDone will be called automatically when leaving the using statement). 
        /// </summary>
        /// <returns>A helper object that implements IDisposable and calls RequestDone() from the Dispose method.</returns>
        /// <remarks>
        ///     <para>
        ///         Sometimes a series of operations needs to be performed on the same connection in order to guarantee correct
        ///         results. This is rarely the case, and most of the time there is no need to call RequestStart/RequestDone.
        ///         An example of when this might be necessary is when a series of Inserts are called in rapid succession with
        ///         SafeMode off, and you want to query that data in a consistent manner immediately thereafter (with SafeMode
        ///         off the writes can queue up at the server and might not be immediately visible to other connections). Using
        ///         RequestStart you can force a query to be on the same connection as the writes, so the query won'TModel execute
        ///         until the server has caught up with the writes.
        ///     </para>
        ///     <para>
        ///         A thread can temporarily reserve a connection from the connection pool by using RequestStart and
        ///         RequestDone. You are free to use any other databases as well during the request. RequestStart increments a
        ///         counter (for this thread) and RequestDone decrements the counter. The connection that was reserved is not
        ///         actually returned to the connection pool until the count reaches zero again. This means that calls to
        ///         RequestStart/RequestDone can be nested and the right thing will happen.
        ///     </para>
        ///     <para>
        ///         Use the connectionstring to specify the readpreference; add "readPreference=X" where X is one of the following
        ///         values: primary, primaryPreferred, secondary, secondaryPreferred, nearest.
        ///         See http://docs.mongodb.org/manual/applications/replication/#read-preference
        ///     </para>
        /// </remarks>
        public virtual IDisposable RequestStart()
        {
            return this.collection.Database.RequestStart();
        }

        /// <summary>
        /// Lets the server know that this thread is done with a series of related operations.
        /// </summary>
        /// <remarks>
        /// Instead of calling this method it is better to put the return value of RequestStart in a using statement.
        /// </remarks>
        public virtual void RequestDone()
        {
            this.collection.Database.RequestDone();
        }

        #region IQueryable<TModel>
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator&lt;TModel&gt; object that can be used to iterate through the collection.</returns>
        public virtual IEnumerator<TModel> GetEnumerator()
        {
            return this.collection.AsQueryable<TModel>().GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.collection.AsQueryable<TModel>().GetEnumerator();
        }

        /// <summary>
        /// Gets the type of the element(s) that are returned when the expression tree associated with this instance of IQueryable is executed.
        /// </summary>
        public virtual Type ElementType
        {
            get { return this.collection.AsQueryable<TModel>().ElementType; }
        }

        /// <summary>
        /// Gets the expression tree that is associated with the instance of IQueryable.
        /// </summary>
        public virtual Expression Expression
        {
            get { return this.collection.AsQueryable<TModel>().Expression; }
        }

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        public virtual IQueryProvider Provider
        {
            get { return this.collection.AsQueryable<TModel>().Provider; }
        }
        #endregion

        public TModel New()
        {
            ConstructorInfo ctor = typeof(TModel).GetConstructor(new Type[] { });
            TModel Model = (TModel)ctor.Invoke(new object[] { });
            return Model;
        }


    }

    /// <summary>
    /// Deals with entities in MongoDb.
    /// </summary>
    /// <typeparam name="TModel">The type contained in the repository.</typeparam>
    /// <remarks>Entities are assumed to use strings for Id's.</remarks>
    public class MongoRepository<TModel> : MongoRepository<string, TModel>, IRepository<TModel>
        where TModel : IEntity<string>
    {

        internal MongoRepository(PropertyInfo key, string collectionName)
            : base(key, collectionName) { }


    }
}
