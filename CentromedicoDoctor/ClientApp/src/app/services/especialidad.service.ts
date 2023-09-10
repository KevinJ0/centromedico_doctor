import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { catchError, map, Observable, throwError } from 'rxjs';
import { especialidad, } from '../interfaces/InterfacesDto';

@Injectable({
  providedIn: 'root'
})

export class EspecialidadService {


  baseUrl: string;

  constructor(
    private http: HttpClient,
    @Inject("BASE_URL") baseUrl: string
  ) {
    this.baseUrl = baseUrl;
  }


  GetEspecialidades(): Observable<especialidad[]> {
    return this.http
      .get<especialidad[]>(
        this.baseUrl +
        "api/Especialidades/get"
      )
      .pipe(
        catchError((error) => throwError(() => error)),
        map((result: especialidad[]) => result)
      );
  }


  GetAllEspecialidades(): Observable<especialidad[]> {
    return this.http
      .get<especialidad[]>(
        this.baseUrl +
        "api/Especialidades/getAll"
      )
      .pipe(
        catchError((error) => throwError(() => error)),
        map((result: especialidad[]) => result)
      );
  }

  DeleteEspecialidad(id_espec: number): Observable<any> {
    return this.http
      .delete<any>(
        this.baseUrl +
        `api/Especialidades/delete?id_espec=${id_espec}`
      )
      .pipe(
        catchError((error) => throwError(() => error)),
      );
  }

  AddEspecialidad(id_espec: number): Observable<any> {
    return this.http
      .post<any>(
        this.baseUrl +
        `api/Especialidades/add?`, id_espec 
      )
      .pipe(
        catchError((error) => throwError(() => error)),
      );
  }

}
