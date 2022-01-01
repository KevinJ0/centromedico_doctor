import { HttpClient } from "@angular/common/http";
import {
  cita,
  citaCalendar,
  citaEntry,
  citaForm,
  citaAndUser,
} from "../interfaces/InterfacesDto";
import { Observable, of, throwError } from "rxjs";
import { map, catchError } from "rxjs/operators";
import { Router } from "@angular/router";
import { Inject, Injectable } from "@angular/core";

@Injectable({
  providedIn: "root",
})
export class CitaService {



  GetCitaPaciente(citaId: number): Observable<citaAndUser> {
    return this.http
    .get<citaAndUser>(this.baseUrl + `api/citas/getCitaPaciente?citaid=${citaId}&medicoid=${this.medicoId}`)
    .pipe(
      catchError((err) => throwError(() => new Error(err))),
      map((result) => {
        return result;
      })
    );
  }


  GetCita(citaId: number): Observable<citaCalendar> {
    return this.http
      .get<citaCalendar>(this.baseUrl + `api/citas/getCita?citaid=${citaId}`)
      .pipe(
        catchError((err) => throwError(() => new Error(err))),
        map((result) => {
          return result;
        })
      );
  }

  UpdateCita(_cita: cita): Observable<boolean> {
    throw new Error("Method not implemented.");
  }

  baseUrl: string;
  _ticket: any;
  prueba: string;
  _citasArr: citaCalendar[];
  // Url to access our Web API’s
  errorMsg: string;
  medicoId = localStorage.getItem("medicoId");

  constructor(
    private router: Router,
    private http: HttpClient,
    @Inject("BASE_URL") baseUrl: string
  ) {
    this.baseUrl = baseUrl;
  }

  SaveCita(_cita: citaEntry): Observable<boolean> {
    console.info(_cita);

    return this.http.post(this.baseUrl + `api/citas/SaveCita`, _cita).pipe(
      catchError((err) => throwError(() => new Error(err))),
      map(() => true)
    );
  }

  GetCitaList(): Observable<citaCalendar[]> {
    return this.http
      .get<citaCalendar[]>(
        this.baseUrl + `api/citas/getCitasList?medicoid=${this.medicoId}`
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
      .get(this.baseUrl + `api/citas/getCitaForm?medicoid=${this.medicoId}`)
      .pipe(
        catchError((err) => {
          this.errorMsg = err.message;
          return throwError(() => new Error(err));
        }),
        map((result: citaForm) => {
          return result;
        })
      );
  }
}
