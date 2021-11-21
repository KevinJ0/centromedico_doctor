import { HttpClient } from "@angular/common/http";
import {
  cita,
  citaResponse,
  ticket,
  UserInfo,
} from "../interfaces/InterfacesDto";
import { Observable, of, throwError } from "rxjs";
import { map, catchError } from "rxjs/operators";
import { Router } from "@angular/router";
import { Inject, Injectable } from "@angular/core";

@Injectable({
  providedIn: "root",
})
export class CitaService {
  baseUrl: string;
  _ticket: any;
  prueba: string;
  // Url to access our Web API’s

  constructor(
    private router: Router,
    private http: HttpClient,
    @Inject("BASE_URL") baseUrl: string
  ) {
    this.baseUrl = baseUrl;
  }

  GetCitaForm(medicoID: number): Observable<any> {
    try {
      return this.http
        .get<any>(this.baseUrl + `api/citas/getCitaForm?medicoID=${medicoID}`)
        .pipe(
          map((result) => {
            return result;
          })
        );
    } catch (error) {
      console.log(
        "Ha ocurrido un error al tratar de obtener los datos del formulario para la cita: ",
        error
      );
      return of([]);
    }
  }

  CreateCita(_cita: cita): Observable<ticket> {
    console.info(_cita);
    try {
      return this.http
        .post<ticket>(this.baseUrl + `api/citas/createCita`, _cita)
        .pipe(
          map((result) => {
            this._ticket = result;
            return result;
          }),
          catchError((err) => {
            return throwError(err);
          })
        );
    } catch (err) {
      console.log(
        "Ha ocurrido un error al tratar de crear la cita: ",
        err.error
      );
      return throwError(err);
    }
  }

  GetCitaList(): Observable<citaResponse[]> {
    return this.http.get<citaResponse[]>(this.baseUrl + `api/citas/getCitasList`)
      .pipe(
        map((result) => {
          return result;
        }),
        catchError((err) => {
          return throwError(err);
        })
      );
  }
}
