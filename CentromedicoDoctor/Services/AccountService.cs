using AutoMapper;
using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using CentromedicoDoctor.Exceptions;
using CentromedicoDoctor.Services.Interfaces;
using Doctor.DTO;
using Doctor.Repository.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IMedicoRepository _medicoRepos;

        public AccountService(RoleManager<IdentityRole> roleManager,
            IHttpContextAccessor httpContextAccessor,
          UserManager<MyIdentityUser> userManager,
            MyDbContext db,
            IMapper mapper,
            IMedicoRepository medicoRepos,
            IAccountRepository accountRepo)
        {
            _accountRepo = accountRepo;
            _medicoRepos = medicoRepos;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _db = db;
            _mapper = mapper;
        }


        public async Task<bool> saveUserInfoAsync(medicoDTO formuser)
        {
            try
            {
                MyIdentityUser user = await _userManager
                 .FindByNameAsync(_httpContextAccessor.HttpContext.User
                 .FindFirst(ClaimTypes.NameIdentifier)?.Value);

                if (user.UserName == null)
                    throw new BadHttpRequestException("Este usuario no existe en la base de datos.");


                user.nombre = formuser.nombre;
                user.apellido = formuser.apellido;
                user.contacto = formuser.telefono1;

                medicos _medico = _db.medicos.First(x => x.MyIdentityUsers == user);
                _mapper.Map<medicoDTO, medicos>(formuser, _medico);
                var medis = (from m in _db.extensiones_telefonicas
                             where m.medicosID == user.medicos.First().ID
                             select m).ToList();

                /*       .Select(x => new extensiones_telefonicas()
                       { ID = x.ID, medicosID = x.medicosID });
                */
                _db.extensiones_telefonicas.RemoveRange(medis);
               // _db.extensiones_telefonicas.AddRange(_medico.extensiones_telefonicas);
                _db.medicos.Update(_medico);

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


        public async Task<medicoDTO> getUserInfoAsync(MyIdentityUser user)
        {
            try
            {

                medicos _medico = await _db.medicos.FirstOrDefaultAsync(x => x.MyIdentityUsers == user);
                var horarios = await _db.horarios_medicos.FirstOrDefaultAsync(x => x.medicosID == _medico.ID);
                medicoDTO _medicoDTO = _mapper.Map<medicoDTO>(_medicoRepos.getMedicoWithServicesInsurancesEspecs(_medico.ID));


                if (horarios == null || _medicoDTO is null)
                    throw new NoContentException();

                Dictionary<string, List<string>> schedulelst = new Dictionary<string, List<string>>();

                List<string> lunesHoras = getMonday(horarios);
                List<string> martesHoras = getTuesday(horarios);
                List<string> miercolesHoras = getWednesday(horarios);
                List<string> juevesHoras = getThursday(horarios);
                List<string> viernesHoras = getFriday(horarios);
                List<string> sabadosHoras = getSaturday(horarios);
                List<string> domingosHoras = getSunday(horarios);

                schedulelst.Add("Lunes", lunesHoras);
                schedulelst.Add("Martes", martesHoras);
                schedulelst.Add("Miercoles", miercolesHoras);
                schedulelst.Add("Jueves", juevesHoras);
                schedulelst.Add("Viernes", viernesHoras);
                schedulelst.Add("Sabados", sabadosHoras);
                schedulelst.Add("Domingos", domingosHoras);

                _medicoDTO.horarios = schedulelst;

                return _medicoDTO;

            }
            catch (Exception e)
            {
                throw new Exception("Ha ocurrido un error al tratar de conseguir al médico: " + e.Message);
            }
        }


        private List<string> getSunday(horarios_medicos horarios)
        {
            if (horarios.sunday_from != null && horarios.sunday_until != null)
            {
                return getHoursList(
               horarios.sunday_from.Value,
               horarios.sunday_until.Value,
               horarios.free_time_from.Value,
               horarios.free_time_until.Value
               );
            }
            else
                return null;
        }

        private List<string> getSaturday(horarios_medicos horarios)
        {
            if (horarios.saturday_from != null && horarios.saturday_until != null)
            {
                return getHoursList(
                horarios.saturday_from.Value,
                horarios.saturday_until.Value,
                horarios.free_time_from.Value,
                horarios.free_time_until.Value
                );
            }
            else
                return null;
        }

        private List<string> getWednesday(horarios_medicos horarios)
        {
            if (horarios.wednesday_from != null && horarios.wednesday_until != null)
            {
                return getHoursList(
                 horarios.wednesday_from.Value,
                 horarios.wednesday_until.Value,
                 horarios.free_time_from.Value,
                 horarios.free_time_until.Value
                 );

            }
            else
                return null;
        }

        private List<string> getFriday(horarios_medicos horarios)
        {
            if (horarios.friday_from != null && horarios.friday_until != null)
            {
                return getHoursList(
                 horarios.friday_from.Value,
                 horarios.friday_until.Value,
                 horarios.free_time_from.Value,
                 horarios.free_time_until.Value
                 );

            }
            else
                return null;
        }

        private List<string> getThursday(horarios_medicos horarios)
        {
            if (horarios.thursday_from != null && horarios.thursday_until != null)
            {
                return getHoursList(
                  horarios.thursday_from.Value,
                  horarios.thursday_until.Value,
                  horarios.free_time_from.Value,
                  horarios.free_time_until.Value
                  );
            }
            else
                return null;
        }

        private List<string> getTuesday(horarios_medicos horarios)
        {
            if (horarios.tuesday_from != null && horarios.tuesday_until != null)
            {
                return getHoursList(
                    horarios.tuesday_from.Value,
                    horarios.tuesday_until.Value,
                    horarios.free_time_from.Value,
                    horarios.free_time_until.Value
                    );
            }
            else
                return null;
        }

        private List<string> getMonday(horarios_medicos horarios)
        {
            if (horarios.monday_from != null && horarios.monday_until != null)
            {
                return getHoursList(
                   horarios.monday_from.Value,
                   horarios.monday_until.Value,
                   horarios.free_time_from.Value,
                   horarios.free_time_until.Value
                   );
            }
            else
                return null;
        }

        private List<string> getHoursList(TimeSpan WDStartH,
        TimeSpan WDEndH, TimeSpan FreeTimeFrom, TimeSpan FreeTimeUntil)
        {
            //primera tanda 
            List<string> hourslst = new List<string>();
            var _time = WDStartH.ToString();
            string _hour = Convert.ToDateTime(_time).ToString("h:mm:tt");
            _time = FreeTimeFrom.ToString();
            _hour = _hour + " - " + Convert.ToDateTime(_time).ToString("h:mm:tt");
            hourslst.Add(_hour);
            //segunda tanda 
            _time = FreeTimeUntil.ToString();
            _hour = Convert.ToDateTime(_time).ToString("h:mm:tt");
            _time = WDEndH.ToString();
            _hour = _hour + " - " + Convert.ToDateTime(_time).ToString("h:mm:tt");
            hourslst.Add(_hour);
            return hourslst;
        }



    }
}
