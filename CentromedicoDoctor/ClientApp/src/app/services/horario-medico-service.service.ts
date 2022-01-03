import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of, throwError } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class HorarioMedicoService {
  baseUrl: string;
  _citaResult: any;
  prueba: string;

  // Url to access our Web API’s
  constructor(private router: Router, private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }



  GetHoursList(fecha: Date, medicoID: number): Observable<any> {
 
    try {
      return this.http.get<any>(this.baseUrl +
        `api/horarios_medicos/getHoursList?fecha_hora=${fecha.toISOString()}&medicoID=${medicoID}`)
        .pipe(map(result => {
          return result;
        }), catchError(err => {
          console.log('Ha ocurrido un error al tratar de obtener la lista de horas: ', err);
          return throwError(err);
        }));

    } catch (err) {
      console.log('Ha ocurrido un error al tratar de obtener la lista de horas: ', err);
      return throwError(err);

    }
  }

}
