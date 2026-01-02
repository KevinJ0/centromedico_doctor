using Centromedico.Database.Context;
using CentromedicoDoctor.Services;
using CentromedicoDoctor.Services.Interfaces;
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
    public class NotificationHub : Hub
    {
        private readonly IMedicoRepository _medicoRepo;
        private readonly MyDbContext _db;
        private readonly IGrupoService _grupoSvc;
        private Dictionary<string, DateTime> fechaHoraCitaDic = new Dictionary<string, DateTime>();

        public NotificationHub(
            MyDbContext db,
            IMedicoRepository medicoRepo,
            IGrupoService grupoService)
        {
            _db = db;
            _medicoRepo = medicoRepo;
            _grupoSvc = grupoService;
        }
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor, Secretary, Patient")]
        public async Task JoinTurnGroup(int medicoID)
        {
            try
            {
                var grupoTurno = await _grupoSvc.getGrupoTurnoAsync(medicoID);
                await Groups.AddToGroupAsync(Context.ConnectionId, grupoTurno.group_name);
                //
                //fechaHoraCitaDic.Add(Context.ConnectionId, fecha_hora_cita);

            }
            catch (Exception)
            {

                throw;
            }

        }



        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor, Secretary, Patient")]
        public async Task JoinCitaGroup(int medicoID)
        {
            try
            {
                medicoID = await _medicoRepo.validMedicoIdAsync(medicoID);
                var claims = (Context.User.Identity as System.Security.Claims.ClaimsIdentity).Claims.Skip(2).Take(1).SingleOrDefault();

                using (_db)
                {
                    // Retrieve groups.
                    /* var groupnamelst = await _db.grupo_doctor_secretaria
                         .Where(u => u.type == "CitasNotificacion" && u.medicosID == medicoID)
                         .Select(x => x.group_name).ToListAsync();
                     */


                    var group = await _db.grupo_doctor_secretaria
                        .Where(x => x.type == "CitaNotificacion" && x.medicosID == medicoID).FirstOrDefaultAsync();


                    //foreach (var item in groupnamelst)
                    //{
                    await Groups.AddToGroupAsync(Context.ConnectionId, group.group_name);
                    //}

                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task LeaveGroups(List<string> groupnamelst)
        {
            try
            {
                foreach (var item in groupnamelst)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, item.ToString());
                }
                fechaHoraCitaDic.Remove(Context.ConnectionId);

            }
            catch (Exception)
            {

                throw;
            }


        }
    }
}
