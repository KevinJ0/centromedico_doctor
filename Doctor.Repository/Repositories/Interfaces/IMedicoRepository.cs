using Centromedico.Database.DbModels;

namespace Doctor.Repository.Repositories.Interfaces
{
    public interface IMedicoRepository
    {
        medicos get(MyIdentityUser user);
    }
}