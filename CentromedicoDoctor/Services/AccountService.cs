using AutoMapper;
using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using CentromedicoDoctor.Exceptions;
using CentromedicoDoctor.Services.Interfaces;
using Doctor.DTO;
using Doctor.Repository.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CentromedicoDoctor.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly MyDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IAccountRepository _accountRepo;

        public AccountService(RoleManager<IdentityRole> roleManager,
            IHttpContextAccessor httpContextAccessor,
          UserManager<MyIdentityUser> userManager,
            MyDbContext db,
            IMapper mapper,
            IAccountRepository accountRepo)
        {
            _accountRepo = accountRepo;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _db = db;
            _mapper = mapper;
        }

        public async Task<bool> saveUserInfoAsync(UserInfo formuser)
        {
            try
            {
                MyIdentityUser user = await _userManager
                 .FindByNameAsync(_httpContextAccessor.HttpContext.User
                 .FindFirst(ClaimTypes.NameIdentifier)?.Value);

                //si no ha sido confirmado por el auxiliar médico
                if (!user.confirm_doc_identidad)
                {
                    var validateB = validateBirth(formuser.fecha_nacimiento);

                    if (!validateB)
                        throw new ArgumentException("Es necesario que el usuario sea mayor de edad.");

                    user.nombre = formuser.nombre;
                    user.apellido = formuser.apellido;
                    user.sexo = formuser.sexo;
                    user.contacto = formuser.contacto;
                    user.doc_identidad = formuser.doc_identidad;
                    user.fecha_nacimiento = formuser.fecha_nacimiento;

                    //Update patient info
                    //Update tutor's name for all records in the database with this user
                    (from p in _db.pacientes
                     where p.MyIdentityUserID == user.Id && p.doc_identidad_tutor != null
                     select p).ToList()
                   .ForEach(x =>
                   {
                       x.nombre_tutor = user.nombre;
                       x.apellido_tutor = user.apellido;
                       x.doc_identidad_tutor = user.doc_identidad;
                   });

                    var paciente = (from p in _db.pacientes
                                    where p.MyIdentityUserID == user.Id && p.doc_identidad != null
                                    select p).FirstOrDefault();
                    if (paciente != null)
                    {
                        paciente.nombre = user.nombre;
                        paciente.doc_identidad = user.doc_identidad;
                        paciente.apellido = user.apellido;
                    }

                }
                else
                    user.contacto = formuser.contacto;

                _db.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                throw;
            }

        }

        private bool validateBirth(DateTime _fechaNacimiento)
        {
            int _edad = DateTime.Today.AddTicks(-_fechaNacimiento.Ticks).Year - 1;

            if (_edad < 18)
                return false;

            return true;
        }

    }
}
