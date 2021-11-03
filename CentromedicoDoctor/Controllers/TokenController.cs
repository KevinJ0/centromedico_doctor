using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using CentromedicoDoctor.Services;
using CentromedicoDoctor.Services.Interfaces;
using CentromedicoDoctor.Exceptions;
using Doctor.DTO;

namespace CentromedicoDoctor.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]

    public class TokenController : Controller
    {
        // jwt and refresh token 
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly MyDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenSvc;


        public TokenController(ITokenService tokenSvc, RoleManager<IdentityRole> roleManager,
            IConfiguration configuration, UserManager<MyIdentityUser> userManager,
             MyDbContext db, IMapper mapper)
        {
            _tokenSvc = tokenSvc;
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _db = db;
            _mapper = mapper;
        }


        /// <summary>
        /// Recibe un TokenRequestDTO, comprueba el GrantType (password o refreshToken) y llama al método pertinente.
        /// Se devuelve un tokenResponseDTO con el token, resfresh token y las credenciales del usuario (en caso de logging).
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Auth
        ///     {
        ///        "UserCredential":"jose@gmail.com", 
        ///        "password":"12345", 
        ///        "granttype":"password" 
        ///     }
        ///
        /// </remarks>
        /// <param name="model"></param>
        /// <returns>TokenResponseDTO</returns>
        /// <response code="400">Si las credenciales no son validas.</response>  
        [HttpPost("[action]")]
        public async Task<IActionResult> AuthAsync([FromBody] TokenRequestDTO model) 
        {
            try
            {
                // I will return Generic 400 HTTP Server Status Error
                // If it receives an invalid payload
                if (model == null)
                {
                    throw new ArgumentNullException("El payload recibido es invalido.");
                }

                switch (model.GrantType)
                {
                    case "password":
                        return await _tokenSvc.GenerateNewToken(model, false);
                    case "refresh_token":
                        return await _tokenSvc.RefreshToken(model, false);
                    case "password_mobile":
                        return await _tokenSvc.GenerateNewToken(model, mobile: true);
                    case "refresh_token_mobile":
                        return await _tokenSvc.RefreshToken(model, mobile: true);
                    default:
                        throw new ArgumentException("El tipo de acción solicitada no es valida.");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// Método que cierra todas las sessiones de un usuario en diferentes dispositivos.
        /// </summary>
        /// <remarks>
        /// ¡Aun no implementado!
        /// </remarks>
        /// <param name="model"></param>
        /// <response code="204">La operación se completó y no hay refresh token registrado a este usuario.</response>
        /// <response code="401">Refresh token not found or invalid (or invalid clientId)</response>  
        /// <response code="401">UserId not found or invalid</response>  
        /// <response code="401">Token refresh is expired</response>  
        [HttpPost, Authorize]
        [Route("revoke")]
        public IActionResult Revoke(TokenRequestDTO model)
        {
            try
            {
                // check if the received refreshToken exists for the given clientId
                var rt = _db.token
                    .FirstOrDefault(t =>
                    t.ClientId == _configuration["Authorization:ClientId"]
                    && t.Value == model.RefreshToken.ToString());


                if (rt == null)
                {
                    // refresh token not found or invalid (or invalid clientId)
                    throw new UnauthorizedException("token de actualización no encontrado o inválido (o clientId inválido)");
                }

                // check if refresh token is expired
                if (rt.ExpiryTime < DateTime.UtcNow)
                {
                    throw new UnauthorizedException("Comprobar si el token de actualización ha caducado");
                }

                // check if there's an user with the refresh token's userId
                var user = _db.MyIdentityUsers.SingleOrDefault(u => u.UserName == rt.User.UserName);

                if (user == null)
                {
                    // UserId not found or invalid
                    throw new UnauthorizedException("UserId no encontrado o inválida (femenino)");
                }

                rt.Value = null;
                _db.SaveChanges();

            }

#pragma warning disable CS0168 // La variable 'ex' se ha declarado pero nunca se usa
            catch (Exception)
#pragma warning restore CS0168 // La variable 'ex' se ha declarado pero nunca se usa
            {
                  throw;
            }

            return NoContent();

        }
    }
}