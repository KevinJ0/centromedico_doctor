using Centromedico.Database.DbModels;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Repository.Repositories.Interfaces
{
    public interface ITokenRepository
    {
        public void Add(token newRtoken);
        void Remove(token oldrt);
        IQueryable getAllByUserId(string id);
    }
}
