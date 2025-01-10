using AutoMapper;
using Centromedico.Database.DbModels;
using Doctor.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentromedicoDoctor.Profiles
{
    public class MappingProfile : Profile
    {


        public MappingProfile()
        {

            CreateMap<MyIdentityUser, UserInfo>().ReverseMap();

            CreateMap<citas, citaPacienteDTO>()
                //.ForMember(dest => dest.medico_nombre, opt => opt.MapFrom(src => src.medicos.nombre))
                //.ForMember(dest => dest.medico_apellido, opt => opt.MapFrom(src => src.medicos.apellido))
                .ForMember(dest => dest.paciente_nombre, opt => opt.MapFrom(src => src.pacientes.nombre))
                .ForMember(dest => dest.doc_identidad,
                 opt => opt.MapFrom(src => (String.IsNullOrEmpty(src.pacientes.doc_identidad)) ? src.pacientes.doc_identidad_tutor : src.pacientes.doc_identidad))
                .ForMember(dest => dest.paciente_apellido, opt => opt.MapFrom(src => src.pacientes.apellido))
                .ForMember(dest => dest.edad, opt => opt.MapFrom(src => src.pacientes.edad))
                .ForMember(dest => dest.menor_un_año, opt => opt.MapFrom(src => src.pacientes.menor_un_año))
                .ForMember(dest => dest.contacto, opt => opt.MapFrom(src => src.pacientes.contacto))
                .ForMember(dest => dest.paciente_nombre_tutor, opt => opt.MapFrom(src => src.pacientes.nombre_tutor))
                .ForMember(dest => dest.paciente_apellido_tutor, opt => opt.MapFrom(src => src.pacientes.apellido_tutor))
                .ForMember(dest => dest.sexo, opt => opt.MapFrom(src => src.pacientes.sexo))
                .ForMember(dest => dest.fecha_nacimiento, opt => opt.MapFrom(src => src.pacientes.fecha_nacimiento))
                .ReverseMap();


            CreateMap<citas, citaDTO>()
                .ForMember(dest => dest.medico_nombre, opt => opt.MapFrom(src => src.medicos.nombre))
                .ForMember(dest => dest.medico_apellido, opt => opt.MapFrom(src => src.medicos.apellido))
                .ForMember(dest => dest.doc_identidad, opt => opt.MapFrom(src => src.pacientes.doc_identidad))
                .ForMember(dest => dest.paciente_nombre, opt => opt.MapFrom(src => src.pacientes.nombre))
                .ForMember(dest => dest.paciente_apellido, opt => opt.MapFrom(src => src.pacientes.apellido))
                .ForMember(dest => dest.edad, opt => opt.MapFrom(src => src.pacientes.edad))
                .ForMember(dest => dest.menor_un_año, opt => opt.MapFrom(src => src.pacientes.menor_un_año))
                .ForMember(dest => dest.paciente_nombre_tutor, opt => opt.MapFrom(src => src.pacientes.nombre_tutor))
                .ForMember(dest => dest.paciente_apellido_tutor, opt => opt.MapFrom(src => src.pacientes.apellido_tutor))
                .ForMember(dest => dest.servicio_descrip, opt => opt.MapFrom(src => src.servicios.descrip))
                .ForMember(dest => dest.seguro_descrip, opt => opt.MapFrom(src => src.seguros.descrip))
                .ForMember(dest => dest.appointmentDuration, opt => opt.MapFrom(src => src.medicos.horarios_medicos.tiempo_cita));

            CreateMap<cobertura_medicos, coberturaDTO>()
               .ForMember(dest => dest.descrip, opt => opt.MapFrom(src => src.seguros.descrip)).ReverseMap(); ;

            CreateMap<cobertura_medicos, coberturaMedicoDTO>();

            CreateMap<citaPacienteDTO, pacientes>()
                .ForMember(dest => dest.ID, opt => opt.UseDestinationValue())
                .ForMember(dest => dest.apellido, opt => opt.MapFrom(src => src.paciente_apellido))
                .ForMember(dest => dest.apellido_tutor, opt => opt.MapFrom(src => src.paciente_apellido_tutor))
                    .ForMember(dest => dest.nombre, opt => opt.MapFrom(src => src.paciente_nombre))
                    .ForMember(dest => dest.nombre_tutor, opt => opt.MapFrom(src => src.paciente_nombre_tutor))
                    .ReverseMap();



            CreateMap<citas, citaResultDTO>()
                .ForMember(dest => dest.cod_verificacion, opt => opt.MapFrom(src => src.cod_verificacionID))
                .ForMember(dest => dest.servicio, opt => opt.MapFrom(src => src.servicios.descrip))
                .ForMember(dest => dest.consultorio, opt => opt.MapFrom(src => src.consultorio))
                .ForMember(dest => dest.fecha_hora, opt => opt.MapFrom(src => src.fecha_hora))
                .ForMember(dest => dest.medico_nombre_apellido, opt => opt.MapFrom(src => (src.medicos.nombre + " " + src.medicos.apellido).Trim()))
                .ForMember(dest => dest.seguro, opt => opt.MapFrom(src => src.seguros.descrip))
                .ForMember(dest => dest.cobertura, opt => opt.MapFrom(src => src.cobertura))
                .ForMember(dest => dest.diferencia, opt => opt.MapFrom(src => src.diferencia))
                .ForMember(dest => dest.doc_identidad, opt => opt.MapFrom(src => src.pacientes.doc_identidad))
                .ForMember(dest => dest.paciente_nombre_apellido, opt => opt.MapFrom(src => (src.pacientes.nombre + " " + src.pacientes.apellido).Trim()))
                .ForMember(dest => dest.doc_identidad_tutor, opt => opt.MapFrom(src => src.pacientes.doc_identidad_tutor))
                .ForMember(dest => dest.tutor_nombre_apellido, opt => opt.MapFrom(src => (src.pacientes.nombre_tutor + " " + src.pacientes.apellido_tutor).Trim()))
                .ForMember(dest => dest.contacto, opt => opt.MapFrom(src => src.contacto))
                .ForMember(dest => dest.correo, opt => opt.MapFrom(src => src.pacientes.MyIdentityUsers.Email))
                .ForMember(dest => dest.turno, opt => opt.MapFrom(src => src.turno));

            CreateMap<pacientes, PacienteDto>()
                      .ForMember(dest => dest.edad, opt => opt.MapFrom(src => src.edad))
                      .ForMember(dest => dest.confirm_doc_identidad, opt => opt.MapFrom(src => src.confirm_doc_identidad))
                      .ForMember(dest => dest.correo, opt => opt.MapFrom(src => src.MyIdentityUsers.Email))
                      .ForMember(dest => dest.menor_un_año, opt => opt.MapFrom(src => src.menor_un_año));

            CreateMap<medicoDTO, medicos>().ForMember(x => x.nombre, opt => opt.MapFrom(src => src.nombre))
                .ForMember(x => x.apellido, opt => opt.MapFrom(src => src.apellido))
                .ForMember(x => x.telefono1, opt => opt.MapFrom(src => src.telefono1))
                .ForMember(x => x.telefono2, opt => opt.MapFrom(src => src.telefono2))
                .ForMember(x => x.telefono1_contact, opt => opt.MapFrom(src => src.telefono1_contact))
                .ForMember(x => x.telefono2_contact, opt => opt.MapFrom(src => src.telefono2_contact))
                .ForMember(x => x.ProfilePhoto, opt => opt.MapFrom(src => src.ProfilePhoto))
                .ForMember(x => x.consultorio, opt => opt.MapFrom(src => src.consultorio))
                .ForMember(x => x.url_facebook, opt => opt.MapFrom(src => src.url_facebook))
                .ForMember(x => x.url_twitter, opt => opt.MapFrom(src => src.url_twitter))
                .ForMember(x => x.url_instagram, opt => opt.MapFrom(src => src.url_instagram))
                .ForMember(x => x.extensiones_telefonicas, opt => opt.MapFrom(x => x.extensiones_telefonicas
                .Select(et => new extensiones_telefonicas()
                {
                    ID = et,
                    medicosID = x.ID
                }).AsEnumerable()))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<medicos, medicoDTO>().ForMember(dest => dest.especialidades,
                opt => opt.MapFrom(src => src.especialidades_medicos.Select(x => x.especialidades.descrip).ToList()))
             .ForMember(x => x.correo, opt => opt.MapFrom(src => src.MyIdentityUsers.Email))
             .ForMember(dest => dest.seguros, opt => opt.MapFrom(src => src.cobertura_medicos.Select(x => x.seguros.descrip).ToList()))
             .ForMember(dest => dest.servicios, opt => opt.MapFrom(src => src.servicios_medicos.Where(x => x.medicos.ID == src.ID).Select(x => x.servicios.descrip).ToList()))
             .ForMember(dest => dest.extensiones_telefonicas, opt => opt.MapFrom(src => src.extensiones_telefonicas.Select(x => x.ID).ToList()));

            CreateMap<secretariasDTO, secretarias>().ReverseMap();

            CreateMap<userMedicoDto, medicos>().ForMember(x => x.nombre, opt => opt.MapFrom(src => src.nombre))
            .ForMember(x => x.apellido, opt => opt.MapFrom(src => src.apellido))
            .ForMember(x => x.telefono1, opt => opt.MapFrom(src => src.telefono1))
            .ForMember(x => x.telefono2, opt => opt.MapFrom(src => src.telefono2))
            .ForMember(x => x.telefono1_contact, opt => opt.MapFrom(src => src.telefono1_contact))
            .ForMember(x => x.telefono2_contact, opt => opt.MapFrom(src => src.telefono2_contact))
            .ForMember(x => x.ProfilePhoto, opt => opt.Condition(src => src.ProfilePhoto?.FileName != null))
            .ForMember(x => x.ProfilePhoto, opt => opt.MapFrom( src => src.ProfilePhoto.FileName))
            .ForMember(x => x.consultorio, opt => opt.MapFrom(src => src.consultorio))
            .ForMember(x => x.url_facebook, opt => opt.MapFrom(src => src.url_facebook))
            .ForMember(x => x.url_twitter, opt => opt.MapFrom(src => src.url_twitter))
            .ForMember(x => x.url_instagram, opt => opt.MapFrom(src => src.url_instagram))
            .ForMember(x => x.extensiones_telefonicas, opt => opt.MapFrom(x => x.extensiones_telefonicas
            .Select(et => new extensiones_telefonicas()
            {
                ID = et,
                medicosID = x.ID
            }).AsEnumerable()))
            .ForAllOtherMembers(opt => opt.Ignore());

        }
    }
}
