export interface UserInfo {

  doc_identidad: string
  nombre: string
  apellido: string
  confirm_doc_identidad?: boolean
  contacto: string
  fecha_nacimiento: Date
  sexo: string

}
export interface especialidad {
  id: number | string;
  descrip: string;
}
export interface hora {
  id: Date;
  descrip: string;
}
export interface seguro {
  id: number;
  descrip: string;
}

export interface cobertura {
  segurosID: number;
  descrip: string;
  porciento: number;
  pago: number;
  cobertura: number;
  diferencia: number;

}


export interface cita {
  nombre: string;
  apellido: string;
  sexo: string;
  doc_identidad: string;
  fecha_hora: Date;
  medicosID: number;
  serviciosID: number;
  fecha_nacimiento: Date;
  contacto: string;
  contacto_whatsapp: boolean;
  appointment_type: number;
  segurosID: number;
  nota: string;
}
export interface citaResponse {

  id: number | string;
  doc_identidad: string;
  medicosID: number;
  medico_nombre: string;
  medico_apellido: string;
  serviciosID: number;
  servicio_descrip: string;
  cod_verificacionID: string;
  pacientesID: number;
  paciente_nombre: string;
  paciente_apellido: string;
  paciente_nombre_tutor: string;
  paciente_apellido_tutor: string;
  nota: string;
  contacto: string;
  contacto_whatsapp: string;
  fecha_hora: string;
  segurosID: number;
  seguro_descrip: string;
  diferencia: number;
  cobertura: number;
  descuento: number;
  pago: number;
  turno: number;
  consultorio: number;
  appointmentDuration: string;
}
export interface ticket {

  cod_verificacion: string;
  servicio: string;
  consultorio: number;
  fecha_hora: string;
  medico_nombre_apellido: string;
  seguro: string;
  pago: number;
  cobertura: number;
  diferencia: number;
  paciente_nombre_apellido: string;
  doc_identidad_tutor: string;
  doc_identidad: string;
  tutor_nombre_apellido: string;
  contacto: string;
  correo: string;
  turno: number;

}

export interface medico {
  id: number,
  exequatur: string,
  colegiatura: string,
  cedula: string,
  nombre: string,
  apellido: string,
  sexo: string,
  correo: string,
  url_twitter: string,
  url_facebook: string,
  url_instagram: string,
  telefono1: string,
  telefono2: string,
  consultorio: number,
  estado: Boolean,
  profilePhoto: string,
  especialidades: string[],
  seguros: string[],
  servicios: string[];
  horarios: Object;
}

export interface doctorCard {
  id: number | string;
  exequatur: string;
  colegiatura: string;
  nombre: string;
  apellido: string;
  sexo: string;
  correo: string;
  url_twitter: string;
  url_facebook: string;
  url_instagram: string;
  telefono1: string;
  telefono2: string;
  consultorio: number;
  profilePhoto: string
  telefono1_contact: string;
  telefono2_contact: string;
  especialidades: string[];
}


export interface User {
  id?: string;
  name?: string;
  email?: string;
  role?: string;
  profilePhoto: string

}

export interface TokenResponse {
  authToken: {
    token: string;
    expiration: string; //date
    refresh_token: string;
    roles: string;
    username: string;
  }
}
export interface CorreoPregunta {

  nombre: string;
  correo: string;
  contacto: string;
  motivo: number;
  mensaje: string;

}

export interface servicioCobertura {

  id: number;
  descrip: string;
  coberturas: cobertura[];

}
