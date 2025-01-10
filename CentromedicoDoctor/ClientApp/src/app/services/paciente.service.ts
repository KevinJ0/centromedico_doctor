import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Paciente, UserInfo } from "../interfaces/InterfacesDto";
import { BehaviorSubject, throwError, of, Observable } from "rxjs";
import { map, catchError } from "rxjs/operators";
import { Router } from "@angular/router";
@Injectable({
  providedIn: "root",
})
export class PacienteService {
  baseUrl: string;
  medicoId = sessionStorage.getItem("medicoId");

  constructor(
    private router: Router,
    private http: HttpClient,
    @Inject("BASE_URL") baseUrl: string
  ) {
    this.baseUrl = baseUrl;
  }

  getUserInfo(citaId: number | string): Observable<UserInfo> {
    return this.http
      .get<UserInfo>(
        this.baseUrl +
        `api/paciente/getUserInfo?citaid=${citaId}&medicoid=${this.medicoId}`
      )
      .pipe(
        map((data: UserInfo) => data),
        catchError((err) => {
          return throwError(err);
        })
      );
  }

  getAllPaciente(): Observable<Paciente[]> {
    return this.http
      .get<Paciente[]>(
        this.baseUrl +
        `api/paciente/getAllPaciente?medicoId=${this.medicoId}`
      )
      .pipe(
        map((data: Paciente[]) => data),
        catchError((err) => {
          return throwError(err);
        })
      );
  }
}
