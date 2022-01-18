using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using Doctor.DTO;
using Doctor.Repository.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Doctor.Repository.Repositories
{
    public class CoberturaRepository : ICoberturaRepository
    {
        private readonly IMapper _mapper;
        private readonly MyDbContext _db;
        public CoberturaRepository(IMapper mapper, MyDbContext db)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<cobertura_medicos> getAsync(int medicosID, int? segurosID, int? serviciosID)
        {

            var r = await _db.cobertura_medicos.FirstOrDefaultAsync(x =>
                               x.medicosID == medicosID &&
                               x.segurosID == segurosID &&
                               x.serviciosID == serviciosID);

            return r;
        }

        public async Task<List<coberturaDTO>> getAllByDoctorIdAsync(int medicoID)
        {
            try
            {
                List<coberturaDTO> r = await _db.cobertura_medicos
                   .Where(c => c.medicosID == medicoID)
                    .ProjectTo<coberturaDTO>(_mapper.ConfigurationProvider).ToListAsync();

                return r;
            }
            catch (Exception)
            {
                return null;
            }
        }


       
    }
}
