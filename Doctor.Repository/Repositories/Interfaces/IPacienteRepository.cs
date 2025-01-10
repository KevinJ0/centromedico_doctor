using Centromedico.Database.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Repository.Repositories.Interfaces
{
    public interface IPacienteRepository
    {
        public void Add(pacientes entity);
        public void Update(pacientes entity);
        pacientes get(int pacienteId);
        pacientes getByDocIdent(string doc_identidad);
        Task<string> getEmailAsync(int pacienteId);
    }
}
