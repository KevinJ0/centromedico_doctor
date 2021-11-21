using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Doctor.DTO;
using System.Linq;
using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using CentromedicoDoctor.Exceptions;
using CentromedicoDoctor.Services.Interfaces;
using Doctor.Repository.Repositories.Interfaces;

namespace CentromedicoDoctor.Services
{
    public class CitaService : ICitaService
    {
        private readonly ICitaRepository _citaRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly MyDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHorarioMedicoRepository _horarioMedicoRepo;

        public CitaService(
            IHorarioMedicoRepository horarioMedicoRepo,
            ICitaRepository citaRepo, 
            IHttpContextAccessor httpContextAccessor,
            UserManager<MyIdentityUser> userManager,
            MyDbContext db, IMapper mapper)
        {
            _horarioMedicoRepo = horarioMedicoRepo;
            _citaRepo = citaRepo;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _citaRepo = citaRepo;
            _db = db;
            _mapper = mapper;
        }

      
        public async Task<List<citaDTO>> getCitasListAsync()
        {

            try
            {
                List<citaDTO> citaslst = await _citaRepo.getCitasListAsync();

                if (!citaslst.Any())
                    throw new EntityNotFoundException("No hay contenido que mostrar.");

                return citaslst;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public List<citaDTO> getCitasListByCv(string codVerificacion)
        {
            try
            {
                List<citaDTO> citaslst = _citaRepo.getCitasListByCv(codVerificacion);

                if (!citaslst.Any())
                    throw new EntityNotFoundException("Este usuario no contiene citas activas.");

                return citaslst;
            }
            catch (Exception)
            {
                throw;
            }
        }
         

        enum appointment : int
        {
            me = 0,
            other = 1,
        }

    }
}
