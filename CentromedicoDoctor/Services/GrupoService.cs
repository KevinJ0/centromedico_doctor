using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using CentromedicoDoctor.Exceptions;
using CentromedicoDoctor.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CentromedicoDoctor.Services
{
    public class GrupoService : IGrupoService
    {
        private readonly UserManager<MyIdentityUser> _userManager;

        private readonly MyDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GrupoService(
             UserManager<MyIdentityUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            MyDbContext db)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _db = db;
        }


        public async Task<Dictionary<string, string>> get(int medicoID)
        {
            MyIdentityUser user = await _userManager
               .FindByNameAsync(_httpContextAccessor.HttpContext.User
               .FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var groups = _db.grupo_doctor_secretaria
                  .Where(x => x.type == "CitaNotificacion" && x.medicosID == medicoID)
                    .Select(g => new
                    {
                        type = g.type,
                        grup_name = g.group_name
                    })
                    .ToDictionary(kvp => kvp.type, kvp => kvp.grup_name);

            return groups;
        }

        public async Task<grupo_doctor_secretaria> getGrupoTurnoAsync(int medicoID)
        {
            MyIdentityUser user = await _userManager
               .FindByNameAsync(_httpContextAccessor.HttpContext.User
               .FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var group = await _db.grupo_doctor_secretaria
                .Where(x => x.type == "TurnoNotificacion" && x.medicosID == medicoID).FirstOrDefaultAsync();
                   

            if (group == null)
                throw new NoContentException();

            return group;
        }
    }
}

