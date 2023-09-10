import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { catchError, map, Observable, throwError } from 'rxjs';
import { group } from '../interfaces/InterfacesDto';

@Injectable({
  providedIn: 'root'
})
export class GrupoService {
  baseUrl: string;

  constructor(private http: HttpClient,
    @Inject("BASE_URL") baseUrl: string) {
    this.baseUrl = baseUrl;

  }


  GetGrupoList(medicoID: number | string): Observable<group[]> {
    return this.http
      .get(this.baseUrl + `api/grupos?medicoID=${medicoID}`)
      .pipe(
        catchError((err) => {
          return throwError(() => new Error(err));
        }),
        map((result: group[]) => {
          return result;
        })
      );
  }

  get GetMedicoId() {
    return sessionStorage.getItem("medicoId");
  }
}
