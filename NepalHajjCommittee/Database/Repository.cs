using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace NepalHajjCommittee.Database
{
    public class Repository<T> where T : class
    {
        #region Private member variables...
        internal DbContext dataContext;
        internal DbSet<T> DbSet;
        #endregion

        #region Public Constructor...
        /// <summary>
        /// Public Constructor,initializes privately declared local variables.
        /// </summary>
        /// <param name="context"></param>
        public Repository(DbContext dataContext)
        {
            this.dataContext = dataContext;
            this.DbSet = dataContext.Set<T>();
        }
        #endregion

        #region Public member methods...

        /// <summary>
        /// generic Get method for Entities
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<T> Get()
        {
            IQueryable<T> query = DbSet;
            return query.AsNoTracking<T>().ToList();
        }

        /// <summary>
        /// Generic get method on the basis of id for Entities.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetByID(object id)
        {
            return DbSet.Find(id);
        }

        /// <summary>
        /// generic Insert method for the entities
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Insert(T entity)
        {
            DbSet.Add(entity);
        }

        public virtual void InsertAll(List<T> entities)
        {
            DbSet.AddRange(entities);
        }

        /// <summary>
        /// Generic Delete method for the entities
        /// </summary>
        /// <param name="id"></param>
        public virtual void Delete(object id)
        {
            T entityToDelete = DbSet.Find(id);
            Delete(entityToDelete);
        }

        /// <summary>
        /// Generic Delete method for the entities
        /// </summary>
        /// <param name="entityToDelete"></param>
        public virtual void Delete(T entityToDelete)
        {
            if (dataContext.Entry(entityToDelete).State == EntityState.Detached)
            {
                DbSet.Attach(entityToDelete);
            }
            DbSet.Remove(entityToDelete);
        }

        /// <summary>
        /// Generic update method for the entities
        /// </summary>
        /// <param name="entityToUpdate"></param>
        public virtual void Update(T entityToUpdate)
        {
            DbSet.Attach(entityToUpdate);
            dataContext.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public void Detach(T entityToDetach)
        {
            dataContext.Entry(entityToDetach).State = EntityState.Detached;
        }

        /// <summary>
        /// generic method to get many record on the basis of a condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetMany(Func<T, bool> where)
        {
            return DbSet.AsNoTracking<T>().Where(where).ToList();
        }

        /// <summary>
        /// generic method to get many record on the basis of a condition but query able.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual IQueryable<T> GetManyQueryable(Func<T, bool> where)
        {
            return DbSet.AsNoTracking<T>().Where(where).AsQueryable();
        }

        /// <summary>
        /// generic get method , fetches data for the entities on the basis of condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public T Get(Func<T, Boolean> where)
        {
            return DbSet.AsNoTracking<T>().Where(where).FirstOrDefault<T>();
        }

        /// <summary>
        /// generic delete method , deletes data for the entities on the basis of condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public void Delete(Func<T, Boolean> where)
        {
            IQueryable<T> objects = DbSet.Where<T>(where).AsQueryable();
            foreach (T obj in objects)
                DbSet.Remove(obj);
        }

        /// <summary>
        /// generic method to fetch all the records from db
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<T> GetAll()
        {
            //return DbSet.ToList();
            return DbSet.AsNoTracking<T>().AsEnumerable();
        }

        /// <summary>
        /// generic method to fetch all the records from db
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<T> GetAllQueryable()
        {
            //return DbSet.ToList();
            return DbSet.AsNoTracking<T>().AsQueryable();
        }

        /// <summary>
        /// generic method to fetch all the records from db
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<T> GetAllQueryableWithTracking()
        {
            //return DbSet.ToList();
            return DbSet.AsQueryable();
        }

        /// <summary>
        /// Inclue multiple
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public IQueryable<T> GetWithInclude(System.Linq.Expressions.Expression<Func<T, bool>> predicate, params string[] include)
        {
            IQueryable<T> query = this.DbSet;
            query = include.Aggregate(query, (current, inc) => current.Include(inc));
            return query.AsNoTracking<T>().Where(predicate);
        }
        /// <summary>
        /// Inclue multiple
        /// </summary>     
        /// <param name="include"></param>
        /// <returns></returns>
        public IQueryable<T> GetWithInclude(params string[] include)
        {
            IQueryable<T> query = this.DbSet;
            query = include.Aggregate(query, (current, inc) => current.Include(inc));
            return query.AsNoTracking<T>();
        }

        /// <summary>
        /// Generic method to check if entity exists
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public bool Exists(object primaryKey)
        {
            return DbSet.Find(primaryKey) != null;
        }

        /// <summary>
        /// Gets a single record by the specified criteria (usually the unique identifier)
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>A single record that matches the specified criteria</returns>
        public T GetSingle(Func<T, bool> predicate)
        {
            return DbSet.AsNoTracking<T>().Single<T>(predicate);
        }
        /// <summary>
        /// Gets a single record by the specified criteria
        /// returns default value if no match found 
        /// throws error if more than one match are found
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>A single record that matches the specified criteria</returns>
        public T GetSingleOrDefault(Func<T, bool> predicate)
        {
            return DbSet.AsNoTracking<T>().SingleOrDefault<T>(predicate);
        }
        /// <summary>
        /// The first record matching the specified criteria
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>A single record containing the first record matching the specified criteria</returns>
        public T GetFirst(Func<T, bool> predicate)
        {
            return DbSet.AsNoTracking<T>().First<T>(predicate);
        }
        /// <summary>
        /// The first record matching the specified criteria
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>A single record containing the first record matching the specified criteria</returns>
        public T GetFirstOrDefault(Func<T, bool> predicate)
        {
            return DbSet.AsNoTracking<T>().FirstOrDefault<T>(predicate);
        }

        /// <summary>
        /// The first record matching the specified criteria
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>A single record containing the first record matching the specified criteria</returns>
        public T GetFirstOrDefaultWithTracking(Func<T, bool> predicate)
        {
            return DbSet.FirstOrDefault<T>(predicate);
        }

        //public virtual IEnumerable<T> ExecuteSqlProc(string query, params object[] parameters)
        public virtual IEnumerable<T> ExecuteSqlProc(string query, params object[] parameters)
        {
            return dataContext.Database.SqlQuery<T>(query, parameters).ToList();
            //return DbSet.SqlQuery(query, parameters).ToList();
        }
        public virtual int ExecuteScalerSqlProc(string query, params object[] parameters)
        {
            return dataContext.Database.ExecuteSqlCommand(query, parameters);
        }
        #endregion

    }
}
