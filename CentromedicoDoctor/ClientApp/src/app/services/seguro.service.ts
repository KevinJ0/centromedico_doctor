import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, map, Observable, throwError } from 'rxjs';
import { seguro, servicioCobertura } from '../interfaces/InterfacesDto';

@Injectable({
  providedIn: 'root'
})
export class SeguroService {
  baseUrl: string;
  constructor(
    private router: Router,
    private http: HttpClient,
    @Inject("BASE_URL") baseUrl: string
    ) {
      this.baseUrl = baseUrl;
    }
    

    GetAllSeguros(
      medicoId?: string | number
    ): Observable<seguro[]> {
      return this.http
        .get<seguro[]>(
          this.baseUrl +
          `api/seguros/getSeguros?medicoid=${medicoId}`
        )
        .pipe(
          catchError((error) => throwError(() => error)),
          map((result: seguro[]) => result)
        );
    }
 
}
