using AutoMapper;
using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using CentromedicoDoctor.Exceptions;
using CentromedicoDoctor.Services.Interfaces;
using Doctor.DTO;
using Doctor.Repository.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        public TokenService(ITokenRepository tokenRepo,
            RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
            UserManager<MyIdentityUser> userManager,
            MyDbContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
            _configuration = configuration;
            _tokenRepo = tokenRepo;
        }





        /// <summary>
        /// Método que crea un New JWT y refresca el token.
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


                var user = await _userManager.FindByNameAsync(model.UserCredential) ?? await _userManager.FindByEmailAsync(model.UserCredential);

                // Validate credentials
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {

                    if (_userManager.IsInRoleAsync(user, "Doctor").Result)
                    {
                        // username & password matches: create the refresh token
                        token newRtoken = CreateRefreshToken(_configuration["Authorization:ClientId"], user.Id, mobile);

                        IQueryable oldrtoken = _tokenRepo.getAllByUserId(user.Id);

                        if (oldrtoken != null)
                        {
                            foreach (var oldrt in oldrtoken)
                            {
                                _tokenRepo.Remove((token)oldrt);
                            }
                        }

                        _tokenRepo.Add(newRtoken);

                        await _db.SaveChangesAsync();

                        TokenResponseDTO accessToken = await CreateAccessToken(user, newRtoken.Value);

                        return new OkObjectResult(new { authToken = accessToken });
                    }
                }
                throw new BadRequestException("El usuario o ontraseña son invalidos, por favor verifique sus credenciales.");
             
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

            // Generate token

            var newtoken = tokenHandler.CreateToken(tokenDescriptor);
            var encodedToken = tokenHandler.WriteToken(newtoken);

            return new TokenResponseDTO()
            {
                token = encodedToken,
                expiration = newtoken.ValidTo,
                refresh_token = refreshToken,
                roles = roles.FirstOrDefault(),
                username = user.UserName
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
                // check if the received refreshToken exists for the given clientId
                var rt = _db.token
                    .FirstOrDefault(t =>
                    t.ClientId == _configuration["Authorization:ClientId"]
                    && t.Value == model.RefreshToken.ToString());

                if (rt == null)
                {
                    // refresh token not found or invalid (or invalid clientId)
                    throw new UnauthorizedException("El refresh token no se ha encontrado o es inválido.");
                }

                // check if refresh token is expired
                if (rt.ExpiryTime < DateTime.UtcNow)
                {
                    throw new UnauthorizedException("El resfresh token ha expirado.");
                }

                // check if there's an user with the refresh token's userId
                var user = await _userManager.FindByIdAsync(rt.UserId);


                if (user == null)
                {
                    // UserId not found or invalid
                    throw new UnauthorizedException("El userId no es valido.");

                }

                // generate a new refresh token 

                var rtNew = CreateRefreshToken(rt.ClientId, rt.UserId, mobile);

                // invalidate the old refresh token (by deleting it)
                _tokenRepo.Remove(rt);

                // add the new refresh token
                _tokenRepo.Add(rtNew);

                // persist changes in the DB
                _db.SaveChanges();


                var response = await CreateAccessToken(user, rtNew.Value);

                return new OkObjectResult(new { authToken = response });

            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}
