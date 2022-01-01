using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using Doctor.Repository.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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

        public SecretariaRepository(
             UserManager<MyIdentityUser> userManager,
             MyDbContext db,
             IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _db = db;
        }

        public async Task<bool> existDoctorAsync( int medicoID)
        {
            MyIdentityUser user = await _userManager
                 .FindByNameAsync(_httpContextAccessor.HttpContext.User
                 .FindFirst(ClaimTypes.NameIdentifier)?.Value);

            int secretaryId = get(user).ID;

            var r = _db.secretarias_medicos.FirstOrDefault(m => m.medicosID == medicoID && m.secretariasID == secretaryId);

            return r != null ? true : false;
        }

        public secretarias get(MyIdentityUser user)
        {
            secretarias _secretaria = _db.secretarias.FirstOrDefault(x => x.MyIdentityUsers == user);
            return _secretaria;
        }
    }
}

