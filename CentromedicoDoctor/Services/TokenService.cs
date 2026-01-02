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
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CentromedicoDoctor.Services
{
    public class TokenService : ITokenService
    {
        private readonly ITokenRepository _tokenRepo;
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly MyDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public TokenService(
            ITokenRepository tokenRepo,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            UserManager<MyIdentityUser> userManager,
            MyDbContext db,
            IMapper mapper
            )
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
            _mapper = mapper;
            _configuration = configuration;
            _tokenRepo = tokenRepo;
        }





        /// <summary>
        /// Método que crea un nuevo JWT y refresca el token.
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
        /// <param name="mobile"></param>
        /// <response code="200">Operación exitosa, devuelve un TokenResponseDTO con el Token y refresh token incluido.</response>
        /// <response code="400">Si las credenciales no son validas.</response>  
        public async Task<IActionResult> GenerateNewToken(TokenRequestDTO model, bool mobile = false)
        {
            try
            {

                var user = _userManager.FindByNameAsync(model.UserCredential).Result ?? _userManager.FindByEmailAsync(model.UserCredential).Result;

                // Validate credentials
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    bool isDoctor = _userManager.IsInRoleAsync(user, "Doctor").Result;
                    bool isSecretary = _userManager.IsInRoleAsync(user, "Secretary").Result;

                    if (isDoctor || isSecretary)
                    {
                        // username & password matches: create the refresh token
                        token newRtoken = CreateRefreshToken(_configuration["Authorization:ClientId"], user.Id, mobile);

                        IQueryable oldrtoken = _tokenRepo.getAllByUserId(user.Id);
                        /*
                        if (oldrtoken != null)
                        {
                            foreach (var oldrt in oldrtoken)
                            {
                                _tokenRepo.Remove((token)oldrt);
                            }
                        }*/

                        _tokenRepo.Add(newRtoken);

                        await _db.SaveChangesAsync();

                        TokenResponseDTO accessToken = await CreateAccessToken(user, newRtoken.Value);

                        return new OkObjectResult(accessToken);
                    }
                    else
                        throw new BadHttpRequestException("El usuario no tiene un rol definido.");

                }
                throw new BadHttpRequestException("El usuario o contraseña son invalidos, por favor verifique sus credenciales.");

            }
            catch (Exception)
            {
                throw;
            }
        }

        // Create access Token
        public async Task<TokenResponseDTO> CreateAccessToken(MyIdentityUser user, string refreshToken)
        {

            double tokenExpiryTime = Convert.ToDouble(_configuration["Authorization:ExpireTime"]);

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Authorization:LlaveSecreta"]));

            var roles = await _userManager.GetRolesAsync(user);
            var tokenHandler = new JwtSecurityTokenHandler();


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                        new Claim("LoggedOn", DateTime.Now.ToString()),
                     }),

                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Authorization:Issuer"],
                Audience = _configuration["Authorization:Audience"],
                Expires = DateTime.UtcNow.AddYears((int)tokenExpiryTime)
            };


            var newtoken = tokenHandler.CreateToken(tokenDescriptor);
            var encodedToken = tokenHandler.WriteToken(newtoken);

            return new TokenResponseDTO()
            {
                token = encodedToken,
                expiration = newtoken.ValidTo,
                refresh_token = refreshToken,
                roles = roles.FirstOrDefault(),
                username = user.UserName,
                secretariaId = _db.secretarias.FirstOrDefault(x => x.MyIdentityUsers == user)?.ID,
                medicosOrMedicoId = roles.FirstOrDefault() == "Secretary" ? _db.medicos.Include("secretarias_medicos")
                                                                                    .Where(x => x.secretarias_medicos
                                                                                                .Any(s => s.secretarias.MyIdentityUsers == user))
                                                                                    .ProjectTo<medicoDTO>(_mapper.ConfigurationProvider).ToList()
                                                                          : _db.medicos.FirstOrDefault(x => x.MyIdentityUsers == user).ID,


            };
        }


        public token CreateRefreshToken(string clientId, string userId, bool mobile = false)
        {
            if (mobile)
            {
                return new token()
                {
                    ClientId = clientId,
                    UserId = userId,
                    Value = Guid.NewGuid().ToString("N"),
                    CreatedDate = DateTime.UtcNow,
                    ExpiryTime = DateTime.UtcNow.AddYears(1)
                };
            }

            return new token()
            {
                ClientId = clientId,
                UserId = userId,
                Value = Guid.NewGuid().ToString("N"),
                CreatedDate = DateTime.UtcNow,
                ExpiryTime = DateTime.UtcNow.AddMinutes(60)
            };
        }



        // Method to Refresh JWT and Refresh Token
        public async Task<IActionResult> RefreshToken(TokenRequestDTO model, bool mobile = false)
        {
            try
            {
                var rt = _db.token
                    .FirstOrDefault(t =>
                    t.ClientId == _configuration["Authorization:ClientId"]
                    && t.Value == model.RefreshToken.ToString());

                if (rt == null)
                    throw new UnauthorizedException("El refresh token no se ha encontrado o es inválido.");

                // check if refresh token is expired
                if (rt.ExpiryTime < DateTime.UtcNow)
                    throw new UnauthorizedException("El resfresh token ha expirado.");

                var user = await _userManager.FindByIdAsync(rt.UserId);

                if (user == null)
                    throw new UnauthorizedException("El userId no es valido.");

                // generate a new refresh token 

                var rtNew = CreateRefreshToken(rt.ClientId, rt.UserId, mobile);

                // invalidate the old refresh token (by deleting it)
                _tokenRepo.Remove(rt);

                _tokenRepo.Add(rtNew);

                _db.SaveChanges();


                var response = await CreateAccessToken(user, rtNew.Value);

                return new OkObjectResult(response);

            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}
