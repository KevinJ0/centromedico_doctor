using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using Doctor.Repository.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Doctor.Repository.Repositories
{
    public class MedicoRepository : IMedicoRepository
    {
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly MyDbContext _db;
        private readonly ISecretariaRepository _secretaryRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public MedicoRepository(
            UserManager<MyIdentityUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            ISecretariaRepository secretaryRepo,
            RoleManager<IdentityRole> roleManager,
            MyDbContext db)
        {
            _secretaryRepo = secretaryRepo;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _roleManager = roleManager; _db = db;
        }

        public medicos getById(int medicoId)
        {
            try
            {
                medicos medico = _db.medicos
                    .FirstOrDefault(x => x.ID == medicoId);

                return medico;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<int> getMedicoIdAsync(int? medicoID)
        {
            try
            {


                MyIdentityUser user = await _userManager
                 .FindByNameAsync(_httpContextAccessor.HttpContext.User
                 .FindFirst(ClaimTypes.NameIdentifier)?.Value);

                bool isSecretary = _userManager.IsInRoleAsync(user, "Secretary").Result;
                bool isDoctor = _userManager.IsInRoleAsync(user, "Doctor").Result;

                int _medicoID = 0;

                if (isDoctor)
                    _medicoID = get(user).ID;
                else if (isSecretary)
                {


                    if (medicoID == null)
                        throw new BadHttpRequestException("Debe suministrar el id del médico solicitado.");

                    bool existDoctor = await _secretaryRepo.existDoctorAsync(medicoID.Value);

                    if (!existDoctor)
                        throw new BadHttpRequestException("Este personal no tiene acceso al listado de citas del médico solicitado.");

                    return medicoID.Value;

                }

                return _medicoID;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public medicos get(MyIdentityUser user)
        {
            try
            {

                medicos _medico = _db.medicos.FirstOrDefault(x => x.MyIdentityUsers == user);
                return _medico;

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
