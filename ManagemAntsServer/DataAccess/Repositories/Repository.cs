using AutoMapper;
using ManagemAntsServer.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication.DataAccess
{
    public class Repository<DBEntity, ModelEntity> : IRepository<DBEntity, ModelEntity>
      where DBEntity : class, new()
      where ModelEntity : class, ManagemAntsServer.Dbo.IObjectWithId, new()

    {
        protected DbSet<DBEntity> _set;
        protected ManagemAntsServer.DataAccess.EfModels.ManagemAntsDbContext _context;
        protected ILogger _logger;
        protected readonly IMapper _mapper;
        public Repository(ManagemAntsServer.DataAccess.EfModels.ManagemAntsDbContext context, ILogger logger, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
            _logger = logger;
            _set = _context.Set<DBEntity>();
        }


        public virtual async Task<IEnumerable<ModelEntity>> GetAll(params Expression<Func<DBEntity, object>>[] includes)
        {
            // No includes
            if (includes.Length == 0)
            {
                return _mapper.Map<ModelEntity[]>(_set.ToList());
            }

            var query = _set.AsQueryable();
            var agr =  includes.Aggregate(query, (current, includeProperty) => current.Include(includeProperty)).ToList();
            return _mapper.Map<ModelEntity[]>(agr);
        }



        public virtual async Task<IEnumerable<ModelEntity>> GetByPredicate(Func<DBEntity, bool> predicate, params Expression<Func<DBEntity, object>>[] includes)
        {
            var query = _set.AsQueryable();
            var agr =  includes.Aggregate(query, (current, includeProperty) => current.Include(includeProperty)).AsEnumerable()
                           .Where(predicate);
            return _mapper.Map<ModelEntity[]>(agr);
        }


        public virtual async Task<ModelEntity> Insert(ModelEntity entity)
        {
            DBEntity dbEntity = _mapper.Map<DBEntity>(entity);
            _set.Add(dbEntity);
            try
            {
                await _context.SaveChangesAsync();
                ModelEntity newEntity = _mapper.Map<ModelEntity>(dbEntity);
                return newEntity;
            }
            catch (Exception ex)
            {
                _logger.LogError("error on db", ex);
                return null;
            }

        }

        public virtual async Task<ModelEntity> Update(ModelEntity entity)
        {
            DBEntity dbEntity = _set.Find(entity.Id);


            if (dbEntity == null)
            {
                return null;
            }
            _mapper.Map(entity, dbEntity);
            if (!_context.ChangeTracker.HasChanges())
            {
                return entity;
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("error on db", ex);

                return null;
            }
            return _mapper.Map<ModelEntity>(dbEntity);

        }

        public virtual async Task<bool> Delete(long idEntity)
        {
            DBEntity dbEntity = _set.Find(idEntity);


            if (dbEntity == null)
            {
                return false;
            }
            _set.Remove(dbEntity);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("error on db", ex);
                return false;
            }
        }
    }
}

