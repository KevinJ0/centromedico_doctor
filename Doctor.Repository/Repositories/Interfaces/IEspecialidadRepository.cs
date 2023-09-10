using Centromedico.Database.DbModels;
using Doctor.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Repository.Repositories.Interfaces
{
    public interface IEspecialidadRepository
    {
        List<especialidadDTO> get();
        List<especialidadDTO> getAll();
        void deleteEspecMedico(especialidades_medicos entity);
        void addEspecMedico(especialidades_medicos entity);
    }
}
