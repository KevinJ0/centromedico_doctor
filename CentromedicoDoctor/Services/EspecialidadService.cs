using AutoMapper;
using Centromedico.Database.Context;
using CentromedicoDoctor.Services.Interfaces;
using Doctor.DTO;
using Doctor.Repository.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Centromedico.Database.DbModels;
using Microsoft.AspNetCore.Identity;
using CentromedicoDoctor.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CentromedicoDoctor.Services
{
    public class EspecialidadService : IEspecialidadService
    {
        private readonly ISeguroRepository _seguroRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly IEspecialidadRepository _especRepo;
        private readonly MyDbContext _db;
        private readonly IMedicoRepository _medicoRepos;
        public EspecialidadService(
            MyDbContext db,
            UserManager<MyIdentityUser> userManager,
            IEspecialidadRepository especRepo,
            IMedicoRepository medicoRepos,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _medicoRepos = medicoRepos;
            _db = db;
            _especRepo = especRepo;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public void deleteEspecMedico(int id_espec)
        {

            try
            {

                MyIdentityUser user = _userManager
                   .FindByNameAsync(_httpContextAccessor.HttpContext.User
                   .FindFirst(ClaimTypes.NameIdentifier)?.Value).Result;

                if (user.UserName == null)
                    throw new BadHttpRequestException("Este usuario no existe en la base de datos.");


                var medico = _medicoRepos.get(user);
                var especMedico = _db.especialidades_medicos.FirstOrDefault(x => x.especialidadesID == id_espec
                                                                             && x.medicos == medico);

                if (especMedico == null)
                    throw new BadHttpRequestException("Esta especialidad no existe en la base de datos.");

                _especRepo.deleteEspecMedico(especMedico);
                _db.SaveChanges();

            }
            catch (Exception)
            {
                throw;
            }
        }

        public void addEspecMedico(int id_espec)
        {

            try
            {

                MyIdentityUser user = _userManager
                   .FindByNameAsync(_httpContextAccessor.HttpContext.User
                   .FindFirst(ClaimTypes.NameIdentifier)?.Value).Result;

                if (user.UserName == null)
                    throw new BadHttpRequestException("Este usuario no existe en la base de datos.");

                var medico = _medicoRepos.get(user);
                var espec = _db.especialidades.Find(id_espec);

                if (espec == null)
                    throw new BadHttpRequestException("Esta especialidad no existe en la base de datos.");

                especialidades_medicos especMedico =
                    new especialidades_medicos
                    {
                        especialidadesID = id_espec,
                        medicosID = medico.ID
                    };

                _especRepo.addEspecMedico(especMedico);
                _db.SaveChanges();

            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
