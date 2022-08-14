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
        Task<List<serviciosDTO>> getAllByDoctorIdAsync(int medicoID);
        Task<List<servicio_coberturasDTO>> getServicio_coberturaByDoctorIdAsync(int medicoID);
        Task<servicios> getByIdAsync(int? servicioID);
    }
}
