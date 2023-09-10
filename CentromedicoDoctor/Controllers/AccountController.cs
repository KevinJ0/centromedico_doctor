using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Centromedico.Database.Context;
using Doctor.DTO;
using CentromedicoDoctor.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using CentromedicoDoctor.Services;
using Centromedico.Database.DbModels;
using Doctor.Repository.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace CentromedicoDoctor.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {

        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<MyIdentityUser> _signManager;
        private readonly IConfiguration _configuration;
        private readonly IMedicoRepository _medicoRepo;
        private readonly MyDbContext _db;
        private readonly IMapper _mapper;
        private readonly IAccountService _accountSvc;
        private readonly IBalanceRepository _balanceRepo;
        private readonly ISecretariaRepository _secretaryRepo;

        public AccountController(IAccountService accountSvc,
                                IBalanceRepository balanceRepo,
                                ISecretariaRepository secretaryRepo,
                                RoleManager<IdentityRole> roleManager,
                                UserManager<MyIdentityUser> userManager,
                                SignInManager<MyIdentityUser> signManager,
                                MyDbContext context,
                                IConfiguration configuration,
                                IMapper mapper,
                                IMedicoRepository medicoRepo)
        {
            _accountSvc = accountSvc;
            _medicoRepo = medicoRepo;
            _secretaryRepo = secretaryRepo;
            _userManager = userManager;
            _signManager = signManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _db = context;
            _mapper = mapper;
            _balanceRepo = balanceRepo;

        }



        /// <summary>
        /// Método guarda los datos personales del médico.
        /// </summary>
        /// <remarks>
        /// Sample response:
        ///
        ///     Post /Account/saveUserInfo
        ///      {
        ///        
        ///      }
        /// </remarks>
        /// <param name="formuser"></param>
        /// <returns>ActionResult</returns>
        /// <response code="400">Este usuario no existe en la base de datos.</response>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor")]
        [HttpPost("[action]")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> saveUserInfoAsync([FromForm] userMedicoDto formuser)
        {

            try
            {
                bool result = await _accountSvc.saveUserInfoAsync(formuser);

                if (!result)
                    return BadRequest();

                return Ok();
            }
            catch (Exception e)
            {
                throw new Exception("Ha ocurrido un error al tratar de hacer la solicitud: " + e.Message);

            }
        }


        /// <summary>
        /// Método que devuelve los datos personales del médico.
        /// </summary>
        /// <remarks>
        /// Sample response:
        ///
        ///     Get /Account/getUserInfo
        ///      {
        ///        
        ///      }
        /// </remarks>
        /// <returns>UserInfo</returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor")]
        [HttpGet("[action]")]
        public async Task<ActionResult<medicoDTO>> getUserInfoAsync()
        {
            try
            {

                string userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                MyIdentityUser user = await _userManager.FindByNameAsync(userName);

                medicoDTO userInfo = await _accountSvc.getUserInfoAsync(user);
                return userInfo;
            }
            catch (Exception e)
            {
                throw new Exception("Ha ocurrido un error al tratar de hacer la solicitud: " + e.Message);
            }
        }


        /// <summary>
        /// Método que cambia la contraseña del médico.
        /// </summary>
        /// <remarks>
        /// Sample response:
        ///
        ///     Post /Account/changePassword
        ///      {
        ///        String: Password
        ///      }
        /// </remarks>
        /// <returns>bool</returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor, Secretary")]
        [HttpPut("[action]")]
        public async Task<ActionResult<bool>> changePasswordAsync(ResetPasswordDTO resetPassDto)
        {
            try
            {

                string userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                MyIdentityUser user = await _userManager.FindByNameAsync(userName);

                if (user.UserName == null)
                    throw new BadHttpRequestException("Este usuario no existe en la base de datos.");

                bool result = await _accountSvc.changePassword(resetPassDto);
                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Devuelve la lista de las secretarias asociadas a dicho doctor.
        /// </summary>
        /// <remarks>
        /// Sample response:
        ///
        ///     Get /Account/getAllSecretary
        ///      List<secretarias>
        ///      
        /// </remarks>
        /// <returns>List<secretarias></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor")]
        [HttpGet("[action]")]
        public async Task<ActionResult<List<secretariasDTO>>> getAllSecretaryAsync()
        {
            try
            {

                string userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                MyIdentityUser user = await _userManager.FindByNameAsync(userName);

                int medicoId = _medicoRepo.get(user).ID;

                List<secretariasDTO> result = _secretaryRepo.getAllByMedicoId(medicoId);

                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Devuelve el balance en caja del médico para la secretaria.
        /// </summary>
        /// <remarks>
        /// Sample response:
        ///
        ///     Get /Account/getStatingBalance
        ///      20000
        ///        
        ///      
        /// </remarks>
        /// <returns>decimal</returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor, Secretary")]
        [HttpGet("[action]")]
        public ActionResult<decimal?> getStartingBalance(int? secretariaId, int? medicoId)
        {
            try
            {
                decimal? balance = null;

                if (secretariaId.HasValue || medicoId.HasValue)
                    balance = _balanceRepo.getBySecretariaIdAndMedicoIdAsync(secretariaId, medicoId).Result;
                else
                    throw new ArgumentNullException("No se ha enviado ningún parametro identificador de uno de los usuarios.");


                if (balance == null)
                    throw new BadHttpRequestException("No se ha registrado ningún balance en este día.");

                return Ok(balance);
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Establece el balance en caja del médico para la secretaria del día actual.
        /// </summary>
        /// <remarks>
        /// Sample response:
        ///
        ///     Post /Account/setStatingBalance
        ///        
        ///      
        /// </remarks>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor")]
        [HttpPost("[action]")]
        public async Task<ActionResult> setStartingBalanceAsync(balance_cajaDTO balanceDto)
        {
            try
            {

                await _accountSvc.addOrUpdateBalanceStartingAsync(balanceDto);

                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }




        /// <summary>
        /// Establece el balance en caja del médico para la secretaria del día actual.
        /// </summary>
        /// <remarks>
        /// Sample response:
        ///
        ///     Post /Account/confirmStatingBalance
        ///      
        /// </remarks>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Secretary")]
        [HttpPost("[action]")]
        public async Task<ActionResult> confirmStartingBalanceAsync([FromQuery] int medicoId)
        {
            try
            {

                await _accountSvc.confirmBalanceStartingAsync(1);

                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Método que devulve los datos del usuario por medio del documento de identidad que ya haya sido verificado por el personal médico de manera física.
        /// </summary>
        /// <returns>bool</returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor, Secretary")]
        [HttpGet("[action]")]
        public MyIdentityUserDto getUserInfo(string docIdentidad)
        {
            try
            {
                var r = _accountSvc.getUserInfo(docIdentidad);

                return r;

            }
            catch (Exception)
            {

                throw;
            }
        }


    }

}
