using GrpcServiceTest.Database.Entities.Base;
using GrpcServiceTest.Database.Repositories.Base;
using System.Reflection;

namespace GrpcServiceTest.Database.Repositories.Constructor
{
    public interface IRepositoryConstructor
    {
        IBaseRepository<Tentity> GetRepository<Tentity>() where Tentity : class, IBaseEntity;
        TRepository GetRepositoryImplementation<TRepository, Tentity>() where TRepository : IBaseRepository<Tentity> where Tentity : class, IBaseEntity;
    }

    public class RepositoryConstructor : IRepositoryConstructor
    {
        private readonly DatabaseContext Context;
        public RepositoryConstructor(DatabaseContext context)
        {
            Context = context;
        }
        public IBaseRepository<Tentity> GetRepository<Tentity>() where Tentity : class, IBaseEntity => new BaseRepository<Tentity>(Context);

        public TRepository GetRepositoryImplementation<TRepository, Tentity>() where TRepository : IBaseRepository<Tentity> where Tentity : class, IBaseEntity
        {
            Type tConcreteRepository = null;
            Assembly.GetExecutingAssembly().GetTypes().ToList().ForEach(t =>
            {
                if (typeof(TRepository).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
                {
                    tConcreteRepository = t;
                }
            });
            return (TRepository)Activator.CreateInstance(tConcreteRepository, Context);
        }
    }
}
