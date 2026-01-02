using Centromedico.Database.DbModels;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Repository.Repositories.Interfaces
{
    public interface ITurnoRepository
    {
        Task<turnos> getAsync(int medicosID);
        Task addAsync(turnos turno);
        Task updateAsync(turnos turno);
    }
}
