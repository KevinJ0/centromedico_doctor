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
        private readonly IMapper _mapper;
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly MyDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHorarioMedicoRepository _horarioMedicoRepo;
        private readonly IMedicoRepository _medicoRepo;
        private readonly ISecretariaRepository _secretaryRepo;
        private readonly IServicioRepository _servicioRepo;
        private readonly ICoberturaRepository _coberturaRepo;

        public CitaService(
           IHorarioMedicoReservaRepository horarioMRRepo,
            IServicioRepository servicioRepo,
            ICoberturaRepository coberturaRepo,
            IHorarioMedicoRepository horarioMedicoRepo,
            ICitaRepository citaRepo,
            IHttpContextAccessor httpContextAccessor,
            UserManager<MyIdentityUser> userManager,
            ISecretariaRepository secretaryRepo,
            IMedicoRepository medicoRepo,
            MyDbContext db, IMapper mapper)
        {
            _horarioMedicoRepo = horarioMedicoRepo;
            _coberturaRepo = coberturaRepo;
            _servicioRepo = servicioRepo;
            _secretaryRepo = secretaryRepo;
            _medicoRepo = medicoRepo;
            _horarioMedicoRepo = horarioMedicoRepo;
            _citaRepo = citaRepo;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _citaRepo = citaRepo;
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

        public async Task<bool> saveCita(citaEntryDTO formdata)
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
                _citaRepo.saveCita(_cita);

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
                int medicoID =   _medicoRepo.getMedicoIdAsync(medicoId).Result;

                citaDTO cita = _mapper.Map<citaDTO>(_citaRepo.get(Id,medicoID));

                if (cita is null)
                    throw new EntityNotFoundException("Esta cita no se encuentra almacenada.");

                if(!cita.estado)
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

        public async Task<citaUserDTO> getCitaPatienteAsync(int citaId, int? medicoId)
        {

            try
            {

                int medicoID = await _medicoRepo.getMedicoIdAsync(medicoId);

                citas cita = _citaRepo.get(citaId, medicoID);
                
                UserInfo userInfo = _mapper.Map<UserInfo>(_db.citas
                    .Include(user => user.pacientes)
                    .ThenInclude(p => p.MyIdentityUsers)
                    .FirstOrDefault(c => c.ID == citaId && c.medicosID == medicoID)?
                    .pacientes.MyIdentityUsers);

                
                if (userInfo is null)
                    throw new EntityNotFoundException("Esta cita no le pertenece al médico/secretaria suministrado.");

                if (cita is null)
                    throw new EntityNotFoundException("Esta cita no es valida para este paciente.");

                citaUserDTO citaP = _mapper.Map<citaUserDTO>(cita);
                citaP.userInfo = userInfo;


                return citaP;
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
