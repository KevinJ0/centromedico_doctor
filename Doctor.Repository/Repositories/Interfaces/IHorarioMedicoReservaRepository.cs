using Centromedico.Database.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Repository.Repositories.Interfaces
{
    public interface IHorarioMedicoReservaRepository
    {
        public void Add(horarios_medicos_reservados entity);

        }
    }
