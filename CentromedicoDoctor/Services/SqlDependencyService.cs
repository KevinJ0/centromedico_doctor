using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using CentromedicoDoctor.Hubs;
using CentromedicoDoctor.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Security.Permissions;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;

namespace CentromedicoDoctor.Services
{

    public class SqlDependencyService : IDatabaseChangeNotificationService
    {
        private readonly IConfiguration _configuration;
        private readonly IHubContext<NotificationCitaHub> _hubContext;
        private readonly MyDbContext _db;

        public SqlDependencyService(
            MyDbContext db,
            IConfiguration configuration,
            IHubContext<NotificationCitaHub> hubContext)
        {
            _configuration = configuration;
            _db = db;
            _hubContext = hubContext;

        }

        public void Config()
        {
            string connString = _configuration.GetConnectionString("DefaultConnection");

           // new ServiceBrokerHelper().EnableServiceBroker(connString, "centromedico");
            SubscribePacienteTableOnChange();
            SubscribeCitaTableOnChange();

        }

        private async void SubscribePacienteTableOnChange()
        {

            string connString = _configuration.GetConnectionString("DefaultConnection");

            try
            {

                var conn = new SqlTableDependency<pacientes>(connString);

                conn.OnChanged += PacienteCambio;
                conn.Start();


            }
            catch (System.Exception)
            {
                // Log to administration
            }
        }



        private void SubscribeCitaTableOnChange()
        {

            try
            {
                string connString = _configuration.GetConnectionString("DefaultConnection");

                var conn = new SqlTableDependency<citas>(connString);

                conn.OnChanged += CitaCambio;
                conn.Start();
            }
            catch (System.Exception)
            {
                throw;
                // Log to administration
            }
        }



        private void CitaCambio(object sender, RecordChangedEventArgs<citas> e)
        {

            try
            {


                if (e.ChangeType != ChangeType.None)
                {
                    string connString = _configuration.GetConnectionString("DefaultConnection");
                    var changedEntity = e.Entity;

                    string groupname;
                    int medicoID = changedEntity.medicosID;

                    using (var context = new MyDbContext(connString))
                    {

                        //Obtenemos el nombre del grupo principal que llega el médico

                        groupname = context.grupo_doctor_secretaria
                       .FirstOrDefault(g => g.type == "CitasNotificacion" && g.medicosID == medicoID)?
                       .group_name;

                        if (!string.IsNullOrWhiteSpace(groupname))
                        {
                            _hubContext.Clients.Groups(groupname).SendAsync(groupname, "Ha ocurrido cambios en medico codigo: " + medicoID);
                        }
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void PacienteCambio(object sender, RecordChangedEventArgs<pacientes> e)
        {
            try
            {

                if (e.ChangeType != ChangeType.None)
                {

                    string connString = _configuration.GetConnectionString("DefaultConnection");
                    var changedEntity = e.Entity;

                    string groupname = "";

                    using (var context = new MyDbContext(connString))
                    {
                        var medico = context.citas
                                .Include(x => x.medicos)
                                .FirstOrDefault(x => x.pacientesID == changedEntity.ID && x.estado == true)?.medicos;

                        if (medico != null)
                        {
                            groupname = context.grupo_doctor_secretaria
                            .FirstOrDefault(g => g.type == "CitasNotificacion" && g.medicosID == medico.ID)
                            .group_name;

                            if (!string.IsNullOrWhiteSpace(groupname))
                            {
                                _hubContext.Clients.Groups(groupname).SendAsync(groupname,
                                    "Ha ocurrido cambios en el paciente codigo: " + changedEntity.ID);
                            }
                        }
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
