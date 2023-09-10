using AutoMapper;
using AutoMapper.QueryableExtensions;
using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using Doctor.Repository.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Repository.Repositories
{
    public class SecretariaRepository : ISecretariaRepository
    {
        private readonly MyDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly IMapper _mapper;

        public SecretariaRepository(
             UserManager<MyIdentityUser> userManager,
             IMapper mapper,
             MyDbContext db,
             IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _db = db;
        }

        public async Task<bool> existDoctorAsync(int medicoId)
        {
            MyIdentityUser user = await _userManager
                 .FindByNameAsync(_httpContextAccessor.HttpContext.User
                 .FindFirst(ClaimTypes.NameIdentifier)?.Value);

            int secretariaId = get(user).ID;

            var r = _db.secretarias_medicos.FirstOrDefault(m => m.medicosID == medicoId 
                                                                && m.secretariasID == secretariaId);

            return r != null ? true : false;
        }

        public secretarias get(MyIdentityUser user)
        {
            secretarias _secretaria = _db.secretarias.FirstOrDefault(x => x.MyIdentityUsers == user);
            return _secretaria;
        }

        public List<secretariasDTO> getAllByMedicoId(int medicoId)
        {
            List<secretariasDTO> secretariasLst = _db.secretarias.Include("secretarias_medicos")
                                .Where(x => x.estado == true &&
                                       x.secretarias_medicos.Where(
                                                        s => s.secretariasID == x.ID && 
                                                        s.medicosID == medicoId).Any()
                                       ).ProjectTo<secretariasDTO>(_mapper.ConfigurationProvider).ToList();

            return secretariasLst;
        }

        public secretarias getById(int secretariaID)
        {
            secretarias _secretaria = _db.secretarias.FirstOrDefault(x => x.ID == secretariaID);
            return _secretaria;
        }
    }
}

