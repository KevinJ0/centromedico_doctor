
using Centromedico.Database.DbModels;
using System.Threading.Tasks;

namespace Doctor.Repository.Repositories.Interfaces
{
    public interface IMedicoRepository
    {
        medicos get(MyIdentityUser user);
        medicos getById(int medicoId);
        medicos validateAndGetId(int medicoId);
        Task<int> getMedicoIdAsync(int? medicoID);
        medicos getMedicoWithServicesInsurancesEspecs(int medicoID);
        Task<bool> existSecretariaAsync(int secretariaId);
    }
}