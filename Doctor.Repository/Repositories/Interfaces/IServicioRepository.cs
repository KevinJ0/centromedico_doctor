using Centromedico.Database.DbModels;
using Doctor.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Repository.Repositories.Interfaces
{
    public interface IServicioRepository
    {
        Task<List<servicio_coberturasDTO>> getAllByDoctorIdAsync(int medicoID);
        Task<servicios> getByIdAsync(int? segurosID);
    }
}
