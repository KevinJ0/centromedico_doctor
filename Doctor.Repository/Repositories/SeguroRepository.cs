using AutoMapper;
using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using Doctor.DTO;
using Doctor.Repository.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Repository.Repositories
{
    public class SeguroRepository : ISeguroRepository
    {

        private readonly MyDbContext _db;
        private readonly IMapper _mapper;
        public SeguroRepository(IMapper mapper, MyDbContext db)
        {
            _db = db;
            _mapper = mapper;
        }


        public async Task<seguros> getByIdAsync(int seguroID)
        {
            seguros seguro = await _db.seguros
                     .Where(x => x.ID == seguroID).FirstOrDefaultAsync();

            return seguro;


        }

        public async Task<List<segurosDTO>> getAllByDoctorIdAsync(int medicoID)
        {
            List<segurosDTO> seguroslst = await _db.cobertura_medicos
                     .Include(x=> x.seguros)
                     .Where(x => x.medicosID == medicoID)
                     .Select(x => new segurosDTO {
                         ID = x.segurosID,
                         descrip = x.seguros.descrip,
                     }).Distinct().ToListAsync();

            return seguroslst;
 
        }

    }
}
