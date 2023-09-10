using Centromedico.Database.DbModels;
using Doctor.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentromedicoDoctor.Services.Interfaces
{
    public interface IEspecialidadService
    {
        public void deleteEspecMedico(int medicoID);
        void addEspecMedico(int id_espec);
    }
}
