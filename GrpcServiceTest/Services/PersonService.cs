using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace GrpcServiceTest.Services
{
    public class PersonService : Person.PersonBase
    {
        private readonly ILogger<PersonService> _logger;
        public PersonService(ILogger<PersonService> logger)
        {
            _logger = logger;
        }

        public override Task<Result> Delete(PersonFind request, ServerCallContext context)
        {
            return base.Delete(request, context);
        }

        public override Task<PersonModel> Find(PersonFind request, ServerCallContext context)
        {
            return base.Find(request, context);
        }

        public override Task<Persons> GetAll(Empty request, ServerCallContext context)
        {
            return base.GetAll(request, context);
        }

        public override Task<PersonModel> Save(PersonModel request, ServerCallContext context)
        {
            return base.Save(request, context);
        }
    }
}
