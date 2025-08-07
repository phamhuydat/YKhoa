using Web.Areas.Admin.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Services.AppUser
{
    public interface IAccountService
    {
        //Task<UpdateUserViewModel> GetUserById(int? id);
        Task UpdateUser(AcceptUpdateViewModel data);


    }
}
