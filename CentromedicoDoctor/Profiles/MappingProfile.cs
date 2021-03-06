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

            CreateMap<citas, citaUserDTO>().ForMember(dest => dest.medico_nombre, opt => opt.MapFrom(src => src.medicos.nombre))
                .ForMember(dest => dest.medico_apellido, opt => opt.MapFrom(src => src.medicos.apellido))
                .ForMember(dest => dest.userInfo, opt => opt.MapFrom(src => src.pacientes.MyIdentityUsers))
                .ForMember(dest => dest.paciente_nombre, opt => opt.MapFrom(src => src.pacientes.nombre))
                .ForMember(dest => dest.paciente_apellido, opt => opt.MapFrom(src => src.pacientes.apellido))
                .ForMember(dest => dest.edad, opt => opt.MapFrom(src => src.pacientes.edad))
                .ForMember(dest => dest.menor_un_año, opt => opt.MapFrom(src => src.pacientes.menor_un_año))
                .ForMember(dest => dest.paciente_nombre_tutor, opt => opt.MapFrom(src => src.pacientes.nombre_tutor))
                .ForMember(dest => dest.paciente_apellido_tutor, opt => opt.MapFrom(src => src.pacientes.apellido_tutor))
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
        }


    

    }
}
