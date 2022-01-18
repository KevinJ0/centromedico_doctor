using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Doctor.DTO;
using System.Linq;
using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using CentromedicoDoctor.Exceptions;
using CentromedicoDoctor.Services.Interfaces;
using Doctor.Repository.Repositories.Interfaces;

namespace CentromedicoDoctor.Services
{
    public class CitaService : ICitaService
    {
        private readonly ICitaRepository _citaRepo;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly MyDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHorarioMedicoRepository _horarioMedicoRepo;
        private readonly IMedicoRepository _medicoRepo;
        private readonly ISecretariaRepository _secretaryRepo;
        private readonly IServicioRepository _servicioRepo;
        private readonly ICoberturaRepository _coberturaRepo;
        private readonly ISeguroRepository _seguroRepo;
        private readonly IPacienteRepository _pacienteRepo;
        private readonly IHorarioMedicoReservaRepository _horarioMRRepo;

        public CitaService(
           IHorarioMedicoReservaRepository horarioMRRepo,
            ISeguroRepository seguroRepo,
            IServicioRepository servicioRepo,
            ICoberturaRepository coberturaRepo,
            IPacienteRepository pacienteRepo,
            IHorarioMedicoRepository horarioMedicoRepo,
            ICitaRepository citaRepo,
            IHttpContextAccessor httpContextAccessor,
            INotificationService notificationService,
            UserManager<MyIdentityUser> userManager,
            ISecretariaRepository secretaryRepo,
            IMedicoRepository medicoRepo,
            MyDbContext db, IMapper mapper)
        {
            _pacienteRepo = pacienteRepo;
            _horarioMedicoRepo = horarioMedicoRepo;
            _seguroRepo = seguroRepo;
            _horarioMRRepo = horarioMRRepo;
            _coberturaRepo = coberturaRepo;
            _servicioRepo = servicioRepo;
            _secretaryRepo = secretaryRepo;
            _medicoRepo = medicoRepo;
            _horarioMedicoRepo = horarioMedicoRepo;
            _citaRepo = citaRepo;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _citaRepo = citaRepo;
            _notificationService = notificationService;
            _db = db;
            _mapper = mapper;
        }


        public async Task<List<citaDTO>> getCitasListAsync(int? medicoId)
        {

            try
            {

                int medicoID = await _medicoRepo.getMedicoIdAsync(medicoId);

                List<citaDTO> citaslst = await _citaRepo.getCitasListAsync(medicoID);

                if (!citaslst.Any())
                    throw new NoContentException();

                return citaslst;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public List<citaDTO> getCitasListByCv(string codVerificacion)
        {
            try
            {
                List<citaDTO> citaslst = _citaRepo.getCitasListByCv(codVerificacion);

                if (!citaslst.Any())
                    throw new EntityNotFoundException("Este usuario no contiene citas activas.");

                return citaslst;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> entryCita(citaEntryDTO formdata)
        {
            try
            {

                int medicoID = formdata.medicoID.Value;
                decimal descuento = formdata.descuento is not null ? formdata.descuento.Value : decimal.Zero;

                medicoID = await _medicoRepo.getMedicoIdAsync(formdata.medicoID);

                citas _cita = _db.citas.FirstOrDefault(x => x.ID == formdata.ID && x.medicosID == medicoID);

                if (_cita == null)
                    throw new BadHttpRequestException("La cita no se encuentra en la base de datos");

                Math.Abs(descuento);

                if (descuento > _cita.diferencia)
                    descuento = _cita.diferencia;

                _cita.descuento = descuento;

                _cita.observacion = String.IsNullOrWhiteSpace(formdata.observacion) ? null : formdata.observacion;
                _cita.estado = true;

                _citaRepo.Update(_cita);

                var r = _db.SaveChanges();

                if (r <= 0)
                    throw new Exception("Ha ocurrido un error al tratar e guardar la entidad en la base de datos.");

                return true;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public citaDTO get(int Id, int? medicoId)
        {
            try
            {
                int medicoID = _medicoRepo.getMedicoIdAsync(medicoId).Result;

                citaDTO cita = _mapper.Map<citaDTO>(_citaRepo.get(Id, medicoID));

                if (cita is null)
                    throw new EntityNotFoundException("Esta cita no se encuentra almacenada.");

                if (!cita.estado)
                    throw new BadHttpRequestException("Esta cita ya ha sido realizada.");

                return cita;
            }
            catch (Exception)
            {
                throw;
            }
        }



        public async Task<citaFormDTO> getFormCitaAsync(int citaId, int medicoId)
        {

            int medicoID = await _medicoRepo.getMedicoIdAsync(medicoId);
            medicos medico = _medicoRepo.getById(medicoID);
            //Tiene que existir al menos 1 cobertura por defecto que es la privada.
            var coberturaslst = await _coberturaRepo.getAllByDoctorIdAsync(medicoID);

            /* var especialidadeslst = await _db.especialidades_medicos
                 .Where(x => x.medicosID == medicoID)
                 .Select(x => new { x.especialidades.ID, x.especialidades.descrip })
                 .ToListAsync();*/

            //  if (especialidadeslst == null)
            //        throw  new BadHttpRequestException(new { InvalidEspecialidad = "El doctor(a) seleccionado no tiene ninguna especialidad asignada." });

            var servicioslst = await _servicioRepo.getAllByDoctorIdAsync(medicoID);
            var availableDaylst = await _horarioMedicoRepo.getAvailableDayListAsync(medicoID);

            if (medico == null)
                throw new EntityNotFoundException("El doctor seleccionado no se encuentra habilitado en estos momentos.");

            if (coberturaslst == null)
                throw new BadHttpRequestException("El doctor(a) seleccionado no tiene ninguna cobertura.");

            if (!availableDaylst.Any())
                throw new EntityNotFoundException("No hay horario disponible para una cita con este médico.");

            if (servicioslst == null)
                throw new BadHttpRequestException("El doctor(a) seleccionado no tiene ningún servicios asignado.");

            return
                new citaFormDTO
                {
                    medico = new Medico_Info
                    {
                        id = medico.ID,
                        nombre = medico.nombre,
                        apellido = medico.apellido,
                    },
                    servicios = servicioslst,
                    diasLaborables = availableDaylst
                };
        }

        public async Task<citaPacienteDTO> getCitaPatienteAsync(int citaId, int? medicoId)
        {

            try
            {

                int medicoID = await _medicoRepo.getMedicoIdAsync(medicoId);

                citas cita = await _db.citas.Include(x => x.pacientes).FirstOrDefaultAsync(c=> c.ID == citaId && c.medicosID == medicoID);


                if (cita is null)
                    throw new EntityNotFoundException("Esta cita no existe o no es válida para este paciente.");

                citaPacienteDTO citaPaciente = _mapper.Map<citaPacienteDTO>(cita);


                return citaPaciente;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<bool> updateCitaAsync(int citaId, citaPacienteDTO formdata)
        {

            pacientes paciente = null;
            citas cita = null;
            horarios_medicos_reservados newHoraMR = null, oldHoraMR = null;
            seguros seguro;
            int _appointment_type = 0, nTurn = 0;
            bool isSameDateTime = false, horaMRRemoved = false;
            string codVer, patientEmail;
            int pacienteID;
            servicios servicio;

            try
            {

                int medicoID = await _medicoRepo.getMedicoIdAsync(formdata.medicosID);

                //Validate incoming data
                medicos medico = _medicoRepo.getById(medicoID);
                if (medico == null)
                    throw new EntityNotFoundException("El doctor seleccionado no se encuentra habilitado en estos momentos.");


                cita = _citaRepo.get(citaId, medico.ID);
                if (cita == null)
                    throw new BadHttpRequestException("Esta cita no es válida o no pertenece a este médico.");


                if (formdata.segurosID == null)
                    seguro = await _seguroRepo.getByIdAsync(1);//1 by default is None-Insurace
                else
                    seguro = await _seguroRepo.getByIdAsync(formdata.segurosID);

                servicio = await _servicioRepo.getByIdAsync(formdata.serviciosID);
                paciente = _pacienteRepo.get(cita.pacientesID);

                isSameDateTime = formdata.fecha_hora.Equals(cita.fecha_hora);
                if (!isSameDateTime)
                {
                    newHoraMR = await _horarioMedicoRepo.getReservedHourAsync(medico.ID, formdata.fecha_hora);
                    oldHoraMR = await _horarioMedicoRepo.getReservedHourAsync(medico.ID, cita.fecha_hora);

                    nTurn = getTurn(formdata.fecha_hora, formdata.medicosID);
                }
                else
                    nTurn = cita.turno;

                var availableDateHourlst = getAvailableDateHour(formdata.fecha_hora, medico.ID);


                ////////////////////////////VALIDATORS\\\\\\\\\\\\\\\\\\\\\\\\\\\\

                if (paciente == null)
                    throw new BadHttpRequestException("Este paciente no se encuentra en la base de datos.");

                patientEmail = await _pacienteRepo.getEmailAsync(paciente.ID);
                _appointment_type = getAppointmentType(paciente.fecha_nacimiento);

                if (String.IsNullOrWhiteSpace(formdata.doc_identidad))
                    throw new BadHttpRequestException("Este usuario no cuenta con un documento de identidad previamente ingresado.");

                //Determinar si la hora de la cita está disponible en el rango de fechas hábiles
                if ((formdata.fecha_hora < DateTime.Now || formdata.fecha_hora > DateTime.Now.AddDays(30)) && !isSameDateTime)
                    throw new BadHttpRequestException("El día suministrado no está en el rango de fecha disponible.");

                if (seguro == null)
                    throw new BadHttpRequestException("El seguro seleccionado no se encuentra en la base de datos.");

                if (servicio == null)
                    throw new BadHttpRequestException("El servicio seleccionado no se encuentra en la base de datos.");

                coberturaMedicoDTO cobertura = _mapper.Map<coberturaMedicoDTO>(_coberturaRepo.getAsync(formdata.medicosID, formdata.segurosID, formdata.serviciosID).Result);

                if (cobertura == null)
                    throw new BadHttpRequestException("Ha ocurrido un error al tratar de especificar el cobertura del servicio.");

                if (newHoraMR != null)
                    throw new BadHttpRequestException("La fecha y hora para la cita programada está reservada, intente con otra por favor.");

                if (availableDateHourlst == null)
                    throw new BadHttpRequestException("Este doctor(a) no labora el día escogido.");

                if (!availableDateHourlst.Contains(formdata.fecha_hora) && !isSameDateTime)
                    throw new BadHttpRequestException("La hora provista no se encuentra en el rango de horas disponibles para ser reservada.");

                if (nTurn == -1)
                    throw new Exception("Ha ocurrido un error al tratar de generar el turno para la cita.");

                if (_appointment_type == (int)appointment.other && String.IsNullOrWhiteSpace(formdata.paciente_nombre_tutor))
                    throw new BadHttpRequestException("No se ha provisto del nombre del tutor para la cita.");


                var patientData = await getPatientAgeAsync(formdata.fecha_nacimiento, _appointment_type);
                formdata.menor_un_año = patientData.Item2;


                newHoraMR = new horarios_medicos_reservados
                {
                    medicosID = medico.ID,
                    fecha_hora = formdata.fecha_hora,
                }; 

                //vamos a remover el horario reservado y a agregar el nuevo, en caso de lanzar error detenemos la
                //ejecución y volvemos a insertar el antiguo horario
                if (!isSameDateTime)
                {
                    horaMRRemoved = changeHoraMR(oldHoraMR, newHoraMR);

                    _horarioMRRepo.Add(newHoraMR);
                }


                _mapper.Map<citaPacienteDTO, pacientes>(formdata, paciente);

                //Checking appointment type
                switch (_appointment_type) // determinar cual tipo de cita es 
                {
                    case (int)appointment.me:
                        paciente.doc_identidad_tutor = null;
                        paciente.nombre_tutor = null;
                        paciente.apellido_tutor = null;
                        break;

                    case (int)appointment.other:

                        paciente.doc_identidad = null;
                        paciente.doc_identidad_tutor = formdata.doc_identidad;
                        paciente.nombre_tutor = formdata.paciente_nombre_tutor;
                        paciente.apellido_tutor = formdata.paciente_apellido_tutor;

                        break;

                    default:
                        throw new ArgumentException("No se ha definido al tipo de paciente que se va a consultar, especifique si es \"Yo\" ó \"Otra persona\"");
                }



                //calculate costs
                decimal coberturaPorciento = (Decimal.Divide((cobertura.porciento), 100));
                decimal _cobertura = cobertura.pago * coberturaPorciento;
                decimal _diferencia = cobertura.pago - _cobertura;

                cita.fecha_hora = formdata.fecha_hora;
                cita.contacto = formdata.contacto;
                cita.nota = formdata.nota;
                cita.contacto_whatsapp = formdata.contacto_whatsapp;

                cita.pacientes = paciente;
                cita.turno = nTurn;
                cita.servicios = servicio;
                cita.seguros = seguro;
                cita.cobertura = _cobertura;
                cita.pago = cobertura.pago;
                cita.diferencia = _diferencia;



                //Saving entities

                _db.SaveChanges();


                try
                {
                    if (!string.IsNullOrEmpty(patientEmail))
                    {
                        citaResultDTO citaResult = _mapper.Map<citaResultDTO>(cita);
                        // _notificationService.sendTicketMail(citaResult, patientEmail, "Actualización de cita ");
                    }
                }
                catch (Exception)
                {

                    //log
                }


                return true;

            }
            catch (Exception)
            {
                if (horaMRRemoved)
                {
                    removeHoraMR(oldHoraMR);
                }
                throw;
            }

        }

        private void removeHoraMR(horarios_medicos_reservados oldHoraMR)
        {
            try
            {

                _db.ChangeTracker.Entries()
                              .ToList()
                              .ForEach(entry =>
                              {
                                  if (typeof(horarios_medicos_reservados).IsAssignableFrom(entry.Entity.GetType()))
                                      entry.CurrentValues.SetValues(oldHoraMR);
                                  else
                                      entry.CurrentValues.SetValues(entry.OriginalValues);
                              });

                _db.SaveChanges();

            }
            catch (Exception)
            {

                throw;
            }
        }

        private bool changeHoraMR(horarios_medicos_reservados oldHoraMR, horarios_medicos_reservados newHoraMR)
        {

            bool horaMRRemoved = false;
            try
            {
                if (oldHoraMR != null)
                {
                    _db.horarios_medicos_reservados.Remove(oldHoraMR);
                    _db.SaveChanges();
                    horaMRRemoved = true;
                }

                return horaMRRemoved;
            }

            catch (Exception)
            {
                throw new Exception("No se ha podido remover el horario de reservación anterior de la base de datos.");
            }
        }

        private int getAppointmentType(DateTime fecha_nacimiento)
        {
            int _edad = DateTime.Today.AddTicks(-fecha_nacimiento.Ticks).Year - 1;

            if (_edad < 18)
                return (int)appointment.other;
            else
                return (int)appointment.me;
        }

        private List<DateTime> getAvailableDateHour(DateTime fecha_hora, int medicoID)
        {

            try
            {
                var availableTimelst = _horarioMedicoRepo.getAvailableHoursTurnDic(fecha_hora, medicoID)?.Select(x => x.Key).ToList(); ;
                return availableTimelst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task<Tuple<int, bool>> getPatientAgeAsync(DateTime fechaNacimiento, int appointmentType)
        {
            try
            {

                MyIdentityUser user = await _userManager
                   .FindByNameAsync(_httpContextAccessor.HttpContext.User
                   .FindFirst(ClaimTypes.NameIdentifier)?.Value);

                int lessPermititted = DateTime.Compare(fechaNacimiento, new DateTime(1910, 1, 1));
                int graterThanToday = DateTime.Compare(fechaNacimiento, DateTime.Today);

                //Valida la fecha de nacimiento 
                if (lessPermititted < 0 || graterThanToday > 0)
                    throw new ArgumentException("La fecha del nacimiento está fuera de rango permitido.");

                int _edad = DateTime.Today.AddTicks(-fechaNacimiento.Ticks).Year - 1;


                //Valida la edad, tipo de cita y datos de tutor
                if (_edad < 18 && appointmentType is not (int)appointment.other)
                    throw new ArgumentException("El tipo de cita escogido no es valido para un menor de edad.");
                else if (_edad >= 18 && appointmentType is not (int)appointment.me)
                    throw new ArgumentException("El tipo de cita escogido no es valido para un mayor de edad.");
                else if (String.IsNullOrWhiteSpace(user.nombre) && _edad < 18)
                    throw new ArgumentException("Es requerido un nombre para el tutor del menor de edad.");


                if (_edad == 0) //si es un menor de 1 año
                {
                    int _edadMeses = DateTime.Today.AddTicks(-fechaNacimiento.Ticks).Month;

                    return new Tuple<int, bool>((_edadMeses - 1), true); // 12 month minus 1
                }

                return new Tuple<int, bool>(_edad, false); // 12 month minus 1

            }
            catch (Exception)
            {
                throw;
            }
        }

        private int getTurn(DateTime fecha_hora, int medicoID)
        {
            try
            {
                var TimeTurnDic = _horarioMedicoRepo.getAvailableHoursTurnDic(fecha_hora, medicoID);

                if (TimeTurnDic?.Count <= 0)
                    return -1;

                int nTurn = TimeTurnDic.First(x => x.Key.Equals(fecha_hora)).Value;

                return nTurn;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        enum appointment : int
        {
            me = 0,
            other = 1,
        }


    }
}
