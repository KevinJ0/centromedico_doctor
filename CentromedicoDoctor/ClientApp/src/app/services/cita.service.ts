import { HttpClient } from "@angular/common/http";
import {
  cita,
  citaCalendar,
  citaEntry,
  citaForm,
  citaPaciente,
  citaResult,
  CustomError,
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
  _citasArr: citaCalendar[];
  errorMsg: string;
  medicoId: string;
  _citasDataRepo: citaCalendar[];

  constructor(
    private router: Router,
    private http: HttpClient,
    @Inject("BASE_URL") baseUrl: string
  ) {
    this.baseUrl = baseUrl;

  }

  UpdateDateTime(citaId: number | string, fecha_hora: string) {
    return this.http
      .put(this.baseUrl + `api/citas/updateDateTime/${citaId}`,
        { "medicosID": this.GetMedicoId, "fecha_hora": fecha_hora })
      .pipe(
        catchError((err) => throwError(err))
      );
  }

  DeleteCita(citaId: string | number) {

    return this.http
      .delete(this.baseUrl + `api/citas/${citaId}/${this.GetMedicoId}`)
      .pipe(
        catchError((err) => throwError(err))
      );
  }

  GetCitaPaciente(citaId: number): Observable<citaPaciente> {
    return this.http
      .get<citaPaciente>(this.baseUrl + `api/citas/getCitaPaciente?citaid=${citaId}&medicoid=${this.GetMedicoId}`)
      .pipe(
        catchError((err) => throwError(err)),
        map((result) => {
          return result;
        })
      );
  }


  GetCita(citaId: number): Observable<citaCalendar> {
    return this.http
      .get<citaCalendar>(this.baseUrl + `api/citas/getCita?citaid=${citaId}`)
      .pipe(
        catchError((err) => throwError(err)),
        map((result) => {
          return result;
        })
      );
  }

  UpdateCita(citaId: number, citaP: citaPaciente): Observable<boolean> {
    return this.http
      .put<boolean>(this.baseUrl + `api/citas/${citaId}`, citaP)
      .pipe(
        catchError((err: CustomError) => throwError(err)),
        map((result) => {
          return result;
        })
      );

  }

  SaveCita(_cita: citaEntry): Observable<boolean> {
    console.info(_cita);

    return this.http.post(this.baseUrl + `api/citas/entryCita`, _cita).pipe(
      catchError((err) => throwError(err)),
      map(() => true)
    );
  }

  GetCitaList(inicio = "", fin = "", estado = "", servicioId = "", seguroId = ""): Observable<citaCalendar[]> {
    return this.http
      .get<citaCalendar[]>(
        this.baseUrl + `api/citas/getCitasList?medicoid=${this.GetMedicoId}&inicio=${inicio}&fin=${fin}&seguroId=${seguroId}&servicioId=${servicioId}&estado=${estado}`
      )
      .pipe(
        catchError((err) => {
          this.errorMsg = err.message;
          return of([]);
        }),
        map((result) => {
          this._citasArr = result;
          if (result) return result;
          else return [];
        })
      );
  }


  GetCitaForm(): Observable<citaForm> {
    return this.http
      .get(this.baseUrl + `api/citas/getCitaForm?medicoid=${this.GetMedicoId}`)
      .pipe(
        catchError((err) => {
          this.errorMsg = err.message;
          return throwError(err);
        }),
        map((result: citaForm) => {
          return result;
        })
      );
  }

  CreateCita(_cita: cita): Observable<any> {
    console.info(_cita);
    try {
      return this.http.post<citaResult>(this.baseUrl +
        `api/citas/createCita`, _cita)
        .pipe(map(result => {
          result;
          return result;
        }), catchError(err => {
          return throwError(err);
        }));

    } catch (err) {
      console.log('Ha ocurrido un error al tratar de crear la cita: ', err.error);
      return throwError(err);
    }
  }

  get GetMedicoId() {
    return sessionStorage.getItem("medicoId");
  }
}
