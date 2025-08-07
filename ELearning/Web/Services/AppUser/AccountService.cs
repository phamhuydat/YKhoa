using AutoMapper;
using Data.Repositories;
using Web.Areas.Admin.ViewModels.Account;

namespace Web.Services.AppUser
{
    public class AccountService : IAccountService
    {
        private readonly GenericRepository _genericRepository;
        private readonly IMapper _mapper;

        public AccountService(GenericRepository genericRepo, IMapper mapper)
        {
            _genericRepository = genericRepo;
            _mapper = mapper;
        }

        //public async Task<UpdateUserViewModel> GetUserById(int? id)
        //{
        //    if (id is null)
        //    {
        //        throw new Exception("Id was null");
        //    }

        //    return await _genericRepository
        //        .FindAsync<Data.Entities.Users, UpdateUserViewModel>
        //                    (
        //                        id.Value,
        //                        AutoMapperProfile.UpdateConf
        //                    );
        //}

        public async Task UpdateUser(AcceptUpdateViewModel data)
        {
            var currentUser = await _genericRepository.FindAsync<Data.Entities.Users>(data.Id);
            _mapper.Map(data, currentUser);
            await _genericRepository.UpdateAsync<Data.Entities.Users>(currentUser);
        }
    }
}
