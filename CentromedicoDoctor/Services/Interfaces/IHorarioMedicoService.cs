using Doctor.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentromedicoDoctor.Services.Interfaces

{
    public interface IHorarioMedicoService
    {
        public Dictionary<DateTime, int> getHoursList(DateTime fecha_hora, int medicoID);

    }
}