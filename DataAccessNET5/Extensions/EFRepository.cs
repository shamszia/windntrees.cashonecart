using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindnTrees.CRUDS.Repository.Core;

namespace DataAccessNET5.Extensions
{
    public class EFRepository<T> : EntityRepository<T> where T : class
    {
        #region Constructors
        /// <summary>
        /// Constructor initialization for entity repository using context, related objects and lazy loading.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="relatedObjects"></param>
        /// <param name="enableLazyLoading"></param>
        public EFRepository(DbContext context, string relatedObjects = "")
            : base(context, relatedObjects)
        {

        }
        #endregion

        #region Update
        /// <summary>
        /// Updates existing content object.
        /// </summary>
        /// <param name="contentObject"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public T Update(T contentObject, Object id = null)
        {
            if (id != null)
            {
                T entityObject = entitySet.Find(GetTypedKey(id));
                context.Entry(entityObject).CurrentValues.SetValues(contentObject);
            }
            else
            {
                entitySet.Attach(contentObject);
                context.Entry(contentObject).State = EntityState.Modified;
            }

            if (context.SaveChanges() > 0)
            {
                contentObject = PostUpdate(contentObject);
                return contentObject;
            }

            return null;
        }
        #endregion

        #region Delete
        /// <summary>
        /// Deletes existing content object.
        /// </summary>
        /// <param name="contentObject"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public T Delete(T contentObject, Object id = null)
        {
            T entityObject = null;
            T removedObject = null;

            if (id != null)
            {
                entityObject = entitySet.Find(GetTypedKey(id));

                removedObject = entitySet.Remove(entityObject).Entity;
            }
            else
            {
                if (context.Entry(contentObject).State == EntityState.Detached)
                {
                    entitySet.Attach(contentObject);
                }

                removedObject = entitySet.Remove(contentObject).Entity;
            }

            if (context.SaveChanges() > 0)
            {
                removedObject = PostDelete(removedObject);
                return contentObject;
            }

            return null;
        }
        #endregion
    }
}
