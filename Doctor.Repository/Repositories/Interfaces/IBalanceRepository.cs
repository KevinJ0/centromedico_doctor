using Centromedico.Database.DbModels;
using Doctor.DTO;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Repository.Repositories.Interfaces
{
    public interface IBalanceRepository
    {
        void add(balance_caja balance);
        void update(balance_caja balance);
        void remove(balance_caja balance);
        Task<decimal?> getBySecretariaIdAndMedicoIdAsync(int? secretariaId, int? medicoId);
    }
}
