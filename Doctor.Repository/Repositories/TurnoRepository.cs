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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Doctor.Repository.Repositories
{
    public class TurnoRepository : ITurnoRepository
    {
        private readonly MyDbContext _db;
        public TurnoRepository(MyDbContext db)
        {
            _db = db;
        }

        public async Task<turnos> getAsync(int medicosID)
        {

            var r = await _db.turnos.FirstOrDefaultAsync(x =>
                               x.medicosID == medicosID);
            return r;
        }

        public async Task addAsync(turnos turno)
        {

            var r = await _db.turnos.AddAsync(turno);

        }

        public async Task updateAsync(turnos entity)
        {

            var r =  _db.turnos.Update(entity);
            
            if(r == null)
                throw new BadHttpRequestException("No se pudo actualizar el turno.");


        }

    }
}
