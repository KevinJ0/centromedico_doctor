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

namespace CentromedicoDoctor.Services
{
    public class ServicioService : IServicioService
    {
        private readonly IServicioRepository _servicioRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly IMedicoRepository _medicoRepo;
        private readonly ISecretariaRepository _secretaryRepo;

        public ServicioService(
            IMedicoRepository medicoRepo,
            ISecretariaRepository secretaryRepo,
        UserManager<MyIdentityUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            IServicioRepository servicioRepo)
        {
            _secretaryRepo = secretaryRepo;
            _medicoRepo = medicoRepo;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _servicioRepo = servicioRepo;
        }

        public async Task<List<servicio_coberturasDTO>> getAllByDoctorIdAsync(int medicoID)
        {

            try
            {
                MyIdentityUser user = await _userManager
                   .FindByNameAsync(_httpContextAccessor.HttpContext.User
                   .FindFirst(ClaimTypes.NameIdentifier)?.Value);

                bool isSecretary = _userManager.IsInRoleAsync(user, "Secretary").Result;
                bool isDoctor = _userManager.IsInRoleAsync(user, "Doctor").Result;

                if (isDoctor)
                    medicoID = _medicoRepo.get(user).ID;
                else if (isSecretary)
                {

                    bool existDoctor = await _secretaryRepo.existDoctorAsync(medicoID);

                    if(!existDoctor)
                        throw new BadHttpRequestException("Este personal no tiene acceso al listado de citas del médico solicitado.");

                }

                var result = _servicioRepo.getAllByDoctorIdAsync(medicoID).Result;
                return result;
            }
            catch (Exception)
            {

                throw;
            }

        }


    }
}
