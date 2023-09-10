using AutoMapper;
using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using Doctor.DTO;
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
        private readonly IMapper _mapper;

        public MedicoRepository(
            UserManager<MyIdentityUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            ISecretariaRepository secretaryRepo,
            RoleManager<IdentityRole> roleManager,
            MyDbContext db,
            IMapper mapper)
        {
            _secretaryRepo = secretaryRepo;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager; _db = db;
        }


    /*    public async Task<IdentityResult> Add(RegisterDTO formdata)
        {


            IdentityRole identityRole;
            var user = _mapper.Map<MyIdentityUser>(formdata);
            user.SecurityStamp = Guid.NewGuid().ToString();

            var r = await _userManager.CreateAsync(user, formdata.Password);

            if (r.Succeeded)
            {
                // set user role
                identityRole = new IdentityRole { Name = "Pacient" };
                await _roleManager.CreateAsync(identityRole);
                await _userManager.AddToRoleAsync(user, "Pacient");

            }
            return r;
        }
    */
        public medicos validateAndGetId(int medicoId)
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

        public medicos getMedicoWithServicesInsurancesEspecs(int medicoID)
        {
            try
            {
                _db.ChangeTracker.LazyLoadingEnabled = false;
                _db.ChangeTracker.AutoDetectChangesEnabled = false;


                medicos medico;

                var query = _db.medicos.Include(m => m.extensiones_telefonicas);

                query.Include(m => m.extensiones_telefonicas).Load();
                query.Include(m => m.especialidades_medicos).ThenInclude(es => es.especialidades).Load();
                query.Include(m => m.cobertura_medicos).ThenInclude(cober => cober.seguros).Load();
                query.Include(m => m.servicios_medicos).ThenInclude(serv => serv.servicios).Load();
                medico = query.FirstOrDefault(m => m.ID == medicoID);

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

        public async Task<bool> existSecretariaAsync(int secretariaId)
        {
            MyIdentityUser user = await _userManager
                 .FindByNameAsync(_httpContextAccessor.HttpContext.User
                 .FindFirst(ClaimTypes.NameIdentifier)?.Value);

            int medicoId = get(user).ID;

            var r = _db.secretarias_medicos.FirstOrDefault(m => m.medicosID == medicoId
                                                                && m.secretariasID == secretariaId);

            return r != null ? true : false;

        }
    }
}
