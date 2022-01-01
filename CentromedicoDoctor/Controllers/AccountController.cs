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
        private readonly MyDbContext _db;
        private readonly IMapper _mapper;
        private readonly IAccountService _accountSvc;


        public AccountController(IAccountService accountSvc,
            RoleManager<IdentityRole> roleManager, UserManager<MyIdentityUser> userManager,
      SignInManager<MyIdentityUser> signManager, MyDbContext context, IConfiguration configuration, IMapper mapper)
        {
            _accountSvc = accountSvc;
            _userManager = userManager;
            _signManager = signManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _db = context;
            _mapper = mapper;

        }

        /// <summary>
        /// Método que almacena los datos personales del usuario/paciente en la base de datos, que será más tarde utilizados para futuras acciones.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Account/setUserInfo
        ///      {
        ///         nombre = "Kevin",
        ///         email = "Rosario",
        ///         contacto = "8095509090",
        ///         doc_identidad = "402999413213",
        ///         sexo = "f" | "m",
        ///         fecha_nacimiento = "1/1/1998"
        ///      }
        /// </remarks>
        /// <param name="formuser"></param>
        /// <returns>ActionResult</returns>
        /// <response code="400">La fecha suministrada no es válida.</response>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Patient")]
        [HttpPost("[action]")]
        public async Task<ActionResult> setUserInfoAsync(UserInfo formuser)
        {

            bool result = await _accountSvc.saveUserInfoAsync(formuser);

            if (!result)
                return BadRequest("La fecha de nacimiento no es valida, debe ser mayor de edad.");
            else
                return Ok();
        }





        /// <summary>
        /// Método que devuelve los datos personales del usuario/paciente.
        /// </summary>
        /// <remarks>
        /// Sample response:
        ///
        ///     Get /Account/getUserInfo
        ///      {
        ///         nombre = "Pedro",
        ///         email = "Roland",
        ///         contacto = "8095559988",
        ///         doc_identidad = "RD2288354523",
        ///         sexo = "m",
        ///         fecha_nacimiento = "1/1/1998",
        ///         confirm_doc_identidad = "false"
        ///      }
        /// </remarks>
        /// <returns>UserInfo</returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Patient")]
        [HttpGet("[action]")]
        public async Task<ActionResult<UserInfo>> getUserInfoAsync()
        {
            try
            {
                string userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                MyIdentityUser user = await _userManager.FindByNameAsync(userName);
                if (user.doc_identidad == null)
                {
                    return NotFound();
                }
                UserInfo userInfo = _mapper.Map<UserInfo>(user);
                return userInfo;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
