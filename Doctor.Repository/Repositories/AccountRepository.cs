using AutoMapper;
using Centromedico.Database.DbModels;
using Doctor.DTO;
using Doctor.Repository.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Repository.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public AccountRepository(IMapper mapper, RoleManager<IdentityRole> roleManager,
            IHttpContextAccessor httpContextAccessor,
          UserManager<MyIdentityUser> userManager)
        {
            _mapper = mapper;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<IdentityResult> Add(RegisterDTO formdata)
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
    }
}
