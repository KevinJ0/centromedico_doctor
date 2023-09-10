using AutoMapper;
using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using Doctor.DTO;
using Doctor.Repository.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Doctor.Repository.Repositories
{
    public class BalanceRepository : IBalanceRepository
    {
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly MyDbContext _db;
        private readonly IAccountRepository _accountRepo;
        private readonly IMedicoRepository _medicoRepo;
        private readonly ISecretariaRepository _secretariaRepo;

        public BalanceRepository(
            MyDbContext db,
            IMapper mapper,
            IMedicoRepository medicoRepo,
            IAccountRepository accountRepo,
            ISecretariaRepository secretariaRepo,
            RoleManager<IdentityRole> roleManager,
            IHttpContextAccessor httpContextAccessor,
            UserManager<MyIdentityUser> userManager)
        {
            _db = db;
            _medicoRepo = medicoRepo;
            _accountRepo = accountRepo;
            _secretariaRepo = secretariaRepo;
            _mapper = mapper;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public void add(balance_caja balance)
        {

            _db.balance_caja.Add(balance);
        }

        public void remove(balance_caja balance)
        {

            _db.balance_caja.Remove(balance);
        }


        public void update(balance_caja balance)
        {
            _db.balance_caja.Update(balance);
        }

        public async Task<decimal?> getBySecretariaIdAndMedicoIdAsync(int? secretariaId, int? medicoId)
        {

            MyIdentityUser user = await _accountRepo.getCurrentUser();


            var _secretaria = _secretariaRepo.get(user);

            if (_secretaria != null)
            {
                if (!medicoId.HasValue)
                    throw new ArgumentNullException("No se a enviado por parámetro el id del médico.");

                bool existDoctor = _secretariaRepo
                                    .existDoctorAsync(medicoId.HasValue ? medicoId.Value : 0).Result;

                if (!existDoctor)
                    throw new BadHttpRequestException("Este médico no trabaja con esta secretaria.");

                secretariaId = _secretaria.ID;
            }
            else
            {

                if (!secretariaId.HasValue)
                    throw new ArgumentNullException("No se a enviado por parámetro el id de la secretaria.");

                var _medico = _medicoRepo.get(user);

                bool existSecretaria = _medicoRepo
                                        .existSecretariaAsync(secretariaId.HasValue ? secretariaId.Value : 0).Result;

                if (!existSecretaria)
                    throw new BadHttpRequestException("Esta secretaria no trabaja con este médico.");

                medicoId = _medico.ID;
            }



            decimal? balance = _db.balance_caja
                                .FirstOrDefault(x => x.medicosID == medicoId
                                                     && x.secretariasID == secretariaId
                                                     && x.fecha.Date == DateTime.Now.Date
                                                )?.balance_inicial;


            return balance;
        }
    }
}
