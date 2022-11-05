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
        public async Task<ActionResult<medicoDTO>> saveUserInfoAsync(medicoDTO formuser)
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

                if (user.UserName == null)
                    return BadRequest("Este usuario no existe en la base de datos.");


                medicoDTO userInfo = await _accountSvc.getUserInfoAsync(user);
                return userInfo;
            }
            catch (Exception e)
            {
                throw new Exception("Ha ocurrido un error al tratar de hacer la solicitud: " + e.Message);
            }
        }

    }
}
