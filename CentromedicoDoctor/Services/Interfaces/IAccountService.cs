using Centromedico.Database.DbModels;
using Doctor.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CentromedicoDoctor.Services.Interfaces
{
    public interface IAccountService
    {
        MyIdentityUserDto getUserInfo(string docIdentidad);
        Task<bool> saveUserInfoAsync(userMedicoDto formuser);
        Task<medicoDTO> getUserInfoAsync(MyIdentityUser user);
        Task<bool> changePassword(ResetPasswordDTO resetPassDto);
        Task addOrUpdateBalanceStartingAsync(balance_cajaDTO dto);
        Task confirmBalanceStartingAsync(int medicoId);
    }
}
