using Centromedico.Database.DbModels;
using Doctor.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Repository.Repositories.Interfaces
{
    public interface ISecretariaRepository
    {
        Task<bool> existDoctorAsync( int medicoID);
        secretarias get(MyIdentityUser user);
    }
}
