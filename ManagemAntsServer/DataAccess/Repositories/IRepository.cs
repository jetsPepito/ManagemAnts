using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ManagemAntsServer.DataAccess.Repositories
{
    public interface IRepository<DBEntity, ModelEntity>

    {
        Task<IEnumerable<ModelEntity>> GetAll(params Expression<Func<DBEntity, object>>[] includes);
        Task<IEnumerable<ModelEntity>> GetByPredicate(Func<DBEntity, bool> predicate, params Expression<Func<DBEntity, object>>[] includes);
        Task<ModelEntity> Insert(ModelEntity entity);
        Task<ModelEntity> Update(ModelEntity entity);
        Task<bool> Delete(long idEntity);
    }
}
