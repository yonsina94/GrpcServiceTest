using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServiceTest.Database.Entities;
using GrpcServiceTest.Database.Repositories.Base;
using GrpcServiceTest.Database.Repositories.Constructor;
using Newtonsoft.Json;

namespace GrpcServiceTest.Services
{
    public class PersonService : Person.PersonBase
    {
        private readonly ILogger<PersonService> _logger;
        private readonly IBaseRepository<PersonEntity> _repo;
        public PersonService(ILogger<PersonService> logger, IRepositoryConstructor constructor)
        {
            _logger = logger;
            _repo = constructor.GetRepository<PersonEntity>();
        }

        public override async Task<Result> Delete(PersonFind request, ServerCallContext context)
        {
            try
            {
                var data =  await _repo.GetAsync(p => p.Id.ToString() == request.Id || p.Email == request.Email);
                if (data != null)
                {
                    await _repo.DeleteAsync(data);
                    await _repo.CommitChangesAsync();
                    return new Result
                    {
                        Success = true,
                        Message = "Requested person has be deleted !"
                    };
                } else
                {
                    return new Result
                    {
                        Success = false,
                        Message = "There's not find any person whit the requested conditions"
                    };
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, null);
                return new Result
                {
                    Success = false,
                    Message = "There's enconuter an error !",
                    Data = Any.Parser.ParseJson(JsonConvert.SerializeObject(e))
                };
            }
        }

        public override async Task<PersonModel> FindOne(PersonFind request, ServerCallContext context)
        {
            try
            {
                var data = (await this._repo.GetAsync(p => p.Id.ToString() == request.Id || p.Email == request.Email)).FirstOrDefault();
                if(data != null)
                {
                    return new PersonModel
                    {
                        Id = data.Id.ToString(),
                        Name = data.Name,
                        LastName = data.LastName,
                        Email = data.Email,
                        Age = data.Age
                    };
                } else
                {
                    return new PersonModel();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, null);
                return new PersonModel();
                throw;
            }
        }

        public override async Task GetAll(IAsyncStreamReader<Empty> requestStream, IServerStreamWriter<Persons> responseStream, ServerCallContext context)
        {

            while (await requestStream.MoveNext())
            {
                try
                {
                    var persons = new Persons();

                    persons.List.AddRange((await _repo.GetAllAsync()).Select(p => new PersonModel { Id = p.Id.ToString(), Name = p.Name, LastName = p.LastName, Email = p.Email, Age = p.Age }).ToList());

                    await responseStream.WriteAsync(persons);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, null);
                    responseStream.WriteAsync(new Persons());
                    throw;
                }
            }
        }

        public override async Task<Result> Save(PersonModel request, ServerCallContext context)
        {
            try
            {
                var result = (await _repo.GetAsync(p => p.Email == request.Email)).FirstOrDefault();
                if(result == null)
                {
                   result = await _repo.InsertAsync(new PersonEntity { Name = request.Name, LastName = request.LastName, Age = request.Age, Email = request.Email });
                    await _repo.CommitChangesAsync();
                    return new Result
                    {
                        Success = true,
                        Data = Any.Pack(new PersonModel { Id = result.Id.ToString(), Name = result.Name, LastName = result.LastName, Email = result.Email, Age = result.Age })
                    };
                } else
                {
                    return new Result { Success = false, Message = "The requested person exists in the database !" };
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, null);
                return new Result
                {
                    Success = false,
                    Message = "There's enconuter an error !",
                    Data = Any.Parser.ParseJson(JsonConvert.SerializeObject(e))
                };
            }
        }
    }
}
