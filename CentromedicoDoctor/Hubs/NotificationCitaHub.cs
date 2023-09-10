using Centromedico.Database.Context;
using Doctor.Repository.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CentromedicoDoctor.Hubs
{
    public class NotificationCitaHub : Hub
    {
        private readonly IMedicoRepository _medicoRepo;
        private readonly MyDbContext _db;
        public NotificationCitaHub(
            MyDbContext db,
            IMedicoRepository medicoRepo
            )
        {
            _db = db;
            _medicoRepo = medicoRepo;

        }
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor, Secretary")]
        public async Task JoinGroups(int medicoID)
        {
            try
            {
                int _medicoID = await _medicoRepo.getMedicoIdAsync(medicoID);
                var claims = (Context.User.Identity as System.Security.Claims.ClaimsIdentity).Claims.Skip(2).Take(1).SingleOrDefault();

                using (_db)
                {
                    // Retrieve groups.
                    var groupnamelst = await _db.grupo_doctor_secretaria
                        .Where(u => u.MyIdentityUserID == claims.Value && u.medicosID == _medicoID)
                        .Select(x => x.group_name).ToListAsync();

                    foreach (var item in groupnamelst)
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, item.ToString());
                    }

                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task LeaveGroups(List<string> groupnamelst)
        {

            foreach (var item in groupnamelst)
                    {
                        await Groups.RemoveFromGroupAsync(Context.ConnectionId, item.ToString());
                    }

        
        }
    }
}
