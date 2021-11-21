﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using Doctor.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Doctor.Repository.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Doctor.Repository.Repositories
{
    public class CitaRepository : ICitaRepository
    {

        private readonly IMapper _mapper;
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly MyDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CitaRepository(IHttpContextAccessor httpContextAccessor, RoleManager<IdentityRole> roleManager,
            IConfiguration configuration, UserManager<MyIdentityUser> userManager, MyDbContext db, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _db = db;
            _mapper = mapper;
        }



        public List<citaDTO> getCitasListByCv(string codVerificacion)
        {

            List<citaDTO> citaslst = _db.citas
                .Where(p => p.cod_verificacionID == codVerificacion && p.estado == true)
                .ProjectTo<citaDTO>(_mapper.ConfigurationProvider).ToList();

            return citaslst;

        }

        public async Task<List<citaDTO>> getCitasListAsync()
        { 

            try
            { 

                MyIdentityUser user = await _userManager
                                    .FindByNameAsync(_httpContextAccessor.HttpContext.User
                                    .FindFirst(ClaimTypes.NameIdentifier)?.Value);

                List<citaDTO> citaslst = _db.citas
                    .Include(m => m.medicos).ThenInclude(hm => hm.horarios_medicos)
                    .Where(p => (p.medicos.MyIdentityUsers == user || p.medicos.MyIdentityUsers == user.medicos) && p.estado == true)
                    .ProjectTo<citaDTO>(_mapper.ConfigurationProvider).ToList();

                return citaslst;
            }
            catch (Exception e)
            {
                throw new Exception("Error al intentar acceder a la información, por favor intente más tarde." + e.StackTrace);
            }
        }


        private string getCV(MyIdentityUser user)
        {
            cod_verificacion codV = _db.cod_verificacion
                .Include("citas")
                .Where(x => x.citas.pacientes.MyIdentityUsers == user && x.citas.estado == true)
                .FirstOrDefault();

            return codV.value;
        }

        public void Add(citas entity)
        {
            _db.citas.Add(entity);
        }
            public bool Exist(medicos medico, MyIdentityUser user)
        {
            try
            {
                if (_db.citas.FirstOrDefault(x => x.medicos == medico
                  && x.pacientes.MyIdentityUsers == user && x.estado == true) != null)
                    return true;

                return false;

            }
            catch (Exception)
            {

                throw;
            }
        }


        public bool Exist(MyIdentityUser user)
        {

            try
            {
                if (_db.citas.FirstOrDefault(x => x.pacientes.MyIdentityUsers == user && x.estado == true) != null)
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
