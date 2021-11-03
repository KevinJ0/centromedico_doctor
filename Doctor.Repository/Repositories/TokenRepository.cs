using AutoMapper;
using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using Doctor.DTO;
using Doctor.Repository.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctor.Repository.Repositories
{
    public class TokenRepository : ITokenRepository
    {

        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly MyDbContext _db;

        public TokenRepository(MyDbContext db, IMapper mapper, RoleManager<IdentityRole> roleManager,
            IHttpContextAccessor httpContextAccessor,
          UserManager<MyIdentityUser> userManager)
        {
            _db = db;
            _mapper = mapper;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }
         

        public void Add(token newRtoken)
        {
            try
            {
                _db.token.Add(newRtoken);
            }
            catch (Exception)
            {
                throw;
            }


        }

        public IQueryable getAllByUserId(string id)
        {
            try
            {
                return _db.token.Where(rt => rt.UserId == id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Remove(token oldrt)
        {

            try
            {
                _db.token.Remove(oldrt);
            }
            catch (Exception)
            {
                throw;
            }


        }
    }
}
