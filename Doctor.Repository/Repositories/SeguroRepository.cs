using AutoMapper;
using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using Doctor.DTO;
using Doctor.Repository.Repositories.Interfaces;
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


        public List<seguros> getSegurosByServicio(int medicoID, int servicioID)
        {
            try
            {

                var seguroslst = _db.cobertura_medicos
                       .Where(x => x.medicosID == medicoID && x.serviciosID == servicioID)
                       .Select(x => x.seguros)
                       .ToList();

                return seguroslst;

            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<seguros> getByIdAsync(int? segurosID = 1)
        {
            try
            {
                seguros seguro = await _db.seguros.FindAsync(segurosID);
                //seguroDTO r = _mapper.Map<seguroDTO>(seguro);

                return seguro;

            }
            catch (Exception)
            {

                throw;
            }
        }


        public List<seguros> getAllByDoctorId(int medicoID)
        {
            List<seguros> seguroslst = _db.cobertura_medicos
                     .Where(x => x.medicosID == medicoID).Select(x => x.seguros).ToList();

            return seguroslst;


        }


    }
}
