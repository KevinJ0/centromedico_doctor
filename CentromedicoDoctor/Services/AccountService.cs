using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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
using System.Text.Json;
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
        private readonly IBalanceRepository _balanceRepo;
        private readonly IMedicoRepository _medicoRepos;
        private readonly ISecretariaRepository _secretaryRepo;
        private readonly IAmazonS3 _amazons3;

        public AccountService(RoleManager<IdentityRole> roleManager,
            IHttpContextAccessor httpContextAccessor,
            UserManager<MyIdentityUser> userManager,
            IBalanceRepository balanceRepo,
            MyDbContext db,
            IMapper mapper,
            ISecretariaRepository secretaryRepo,
            IMedicoRepository medicoRepos,
            IAmazonS3 amazonS3)
        {
            _secretaryRepo = secretaryRepo;
            _balanceRepo = balanceRepo;
            _medicoRepos = medicoRepos;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _db = db;
            _amazons3 = amazonS3;
            _mapper = mapper;

        }



        public async Task<bool> changePassword(ResetPasswordDTO resetPassDto)
        {
            if (resetPassDto.Password != resetPassDto.ConfirmPassword)
                throw new IdentityPwException("Las contraseñas no coinciden");

            MyIdentityUser user = await getCurrentUser();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, resetPassDto.Password);

            if (result.Succeeded)
                return true;

            var identErr = result.Errors.FirstOrDefault();

            throw new IdentityPwException(" (" + identErr.Code + ") " + identErr.Description);

        }



        public async Task<bool> saveUserInfoAsync(userMedicoDto formuser)
        {
            try
            {
                MyIdentityUser user = await getCurrentUser();


                user.nombre = formuser.nombre;
                user.apellido = formuser.apellido;
                user.contacto = formuser.telefono1;


                medicos _medico = _db.medicos.First(x => x.MyIdentityUsers == user);
                _mapper.Map<userMedicoDto, medicos>(formuser, _medico);

                if (!String.IsNullOrEmpty(formuser.exten_tel_arrstr))
                {
                    var ext_new = formuser.exten_tel_arrstr.Split(',')
                        .Where(ext => !String.IsNullOrEmpty(ext))
                        .Select(ext =>
                        new extensiones_telefonicas()
                        {
                            medicosID = _medico.ID,
                            ID = ext
                        }).ToList();

                    //remueve las anteriores
                    List<extensiones_telefonicas> extensionesTel = (from m in _db.extensiones_telefonicas
                                                                    where m.medicosID == user.medicos.First().ID
                                                                    select m).ToList();

                    _db.extensiones_telefonicas.RemoveRange(extensionesTel);

                    //agrega las nuevas
                    _db.extensiones_telefonicas.AddRange(ext_new);

                }

                string BucketName = "centromedico-assets";
                IFormFile profilePhoto = formuser.ProfilePhoto;
                if (profilePhoto != null)
                {
                    String realFileName = "D" + _medico.ID + System.IO.Path.GetExtension(profilePhoto.FileName);

                    try
                    {
                        var transferUtility = new TransferUtility(_amazons3);
                        var putRequest = new PutObjectRequest()
                        {
                            BucketName = BucketName,
                            Key = realFileName,
                            InputStream = profilePhoto.OpenReadStream(),
                            ContentType = profilePhoto.ContentType,
                            CannedACL = S3CannedACL.PublicRead,

                        };

                        // Create a CopyObject request
                        GetPreSignedUrlRequest request = new GetPreSignedUrlRequest
                        {
                            BucketName = "centromedico-assets",
                            Key = realFileName,
                        };

                        // Get path for request
                        var result = await _amazons3.PutObjectAsync(putRequest);
                        var url = "https://" + BucketName + ".s3." + _amazons3.Config.RegionEndpoint.SystemName + ".amazonaws.com/" + realFileName;

                        _medico.ProfilePhoto = url;

                    }
                    catch (AmazonS3Exception amazonS3Exception)
                    {
                        if (amazonS3Exception.ErrorCode != null &&
                        (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                        ||
                        amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                        {
                            throw new Exception("Check the provided AWS Credentials.");
                        }

                        throw amazonS3Exception;
                    }
                }

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

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public async Task<MyIdentityUser> getCurrentUser()
        {
            try
            {
                MyIdentityUser user = await _userManager
                    .FindByNameAsync(_httpContextAccessor.HttpContext.User
                    .FindFirst(ClaimTypes.NameIdentifier)?.Value);

                if (user.UserName == null)
                    throw new BadHttpRequestException("Este usuario no existe en la base de datos.");

                return user;

            }
            catch (Exception e)
            {
                throw;
            }

        }

        public async Task addOrUpdateBalanceStartingAsync(balance_cajaDTO dto)
        {
            MyIdentityUser user = await getCurrentUser();

            medicos _medico = _medicoRepos.get(user);

            secretarias _secretaria = _secretaryRepo.getById(dto.secretariasID);

            if (_secretaria == null)
                throw new EntityNotFoundException("Esta secretaria no trabaja con este médico.");

            balance_caja balance = _db.balance_caja.FirstOrDefault(x => x.medicosID == _medico.ID
                                                            && x.fecha.Date == DateTime.Now.Date
                                                            && x.secretariasID == dto.secretariasID);

            if (!String.IsNullOrEmpty(balance?.secretaria_nombre?.Trim()))
                throw new ArgumentException("Ya se ha confirmado el balance inicial del día de hoy.");


            if (balance != null)
                balance.balance_inicial = dto.balance_inicial;
            else
            {

                balance = new balance_caja()
                {
                    medicosID = _medico.ID,
                    medicos = _medico,
                    secretarias = _secretaria,
                    balance_inicial = Math.Abs(dto.balance_inicial),
                    fecha = DateTime.Now.Date,
                    secretariasID = _secretaria.ID,
                };

                _balanceRepo.add(balance);

            }

            _db.SaveChanges();


        }



        public async Task confirmBalanceStartingAsync(int medicoId)
        {

            MyIdentityUser user = await getCurrentUser();

            secretarias _secretaria = _secretaryRepo.get(user);

            bool existDoctor = await _secretaryRepo.existDoctorAsync(medicoId);

            if (!existDoctor)
                throw new EntityNotFoundException("Esta secretaria no tiene relación con este médico.");

            var balance = _db.balance_caja.FirstOrDefault(x => x.medicosID == medicoId
                                                            && x.fecha.Date == DateTime.Now.Date
                                                            && x.secretariasID == _secretaria.ID);

            if (balance == null)
                throw new ArgumentException("No se ha establecido ningún balance inicial para el día de hoy " + DateTime.Now.ToString("dd-MM-yyyy"));


            if (!String.IsNullOrEmpty(balance.secretaria_nombre?.Trim()))
                throw new ArgumentException("Ya se ha confirmado el balance inicial del día de hoy.");

            balance.secretaria_nombre = _secretaria.nombre;

            _balanceRepo.update(balance);

            _db.SaveChanges();

        }

        public MyIdentityUserDto getUserInfo(string docIdentidad)
        {
            var userPacienteDto = _db.MyIdentityUsers
                        .ProjectTo<MyIdentityUserDto>(_mapper.ConfigurationProvider)
                        .FirstOrDefault(x => x.doc_identidad == docIdentidad &&
                                             x.confirm_doc_identidad == true);

            return userPacienteDto;


        }

        public async Task<bool> isBalanceStartingConfirmedAsync(int medicoId) 
        {

            MyIdentityUser user = await getCurrentUser();

            secretarias _secretaria = _secretaryRepo.get(user);

            bool existDoctor = await _secretaryRepo.existDoctorAsync(medicoId);

            if (!existDoctor)
                throw new EntityNotFoundException("Esta secretaria no tiene relación con este médico.");

            var balance = _db.balance_caja.FirstOrDefault(x => x.medicosID == medicoId
                                                            && x.fecha.Date == DateTime.Now.Date
                                                            && x.secretariasID == _secretaria.ID);

            if (balance == null)
                throw new ArgumentException("No se ha establecido ningún balance inicial para el día de hoy " + DateTime.Now.ToString("dd-MM-yyyy"));


            return await Task.FromResult( !String.IsNullOrEmpty(balance.secretaria_nombre?.Trim()) );


        }
    }
}
