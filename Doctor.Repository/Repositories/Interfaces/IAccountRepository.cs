using Centromedico.Database.DbModels;
using Doctor.DTO;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Repository.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<IdentityResult> Add(RegisterDTO formdata);
    }
}
