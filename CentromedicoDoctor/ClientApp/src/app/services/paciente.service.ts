import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { UserInfo } from "../interfaces/InterfacesDto";
import { BehaviorSubject, throwError, of, Observable } from "rxjs";
import { map, catchError } from "rxjs/operators";
import { Router } from "@angular/router";
@Injectable({
  providedIn: "root",
})
export class PacienteService {
  baseUrl: string;
  medicoId = localStorage.getItem("medicoId");

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
}
