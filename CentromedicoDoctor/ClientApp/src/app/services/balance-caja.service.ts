import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable, catchError, map, throwError } from 'rxjs';
import { CustomError, balanceCaja } from '../interfaces/InterfacesDto';

@Injectable({
  providedIn: 'root'
})
export class BalanceCajaService {
  baseUrl: string;

  constructor(
    private http: HttpClient,
    @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;

  }

  GetStartingBalance(medicoOrSecretariaId?: number): Observable<number> {
    return this.http.get<number>(this.baseUrl + `api/account/getStartingBalance?medicoId=${medicoOrSecretariaId}&secretariaId=${medicoOrSecretariaId}`)
      .pipe(
        map((balance: number) => balance),
        catchError((err: CustomError) => {
          return throwError(err.message);
        })
      );
  }

  SetStartingBalance(balance_caja: balanceCaja): Observable<any> {
    return this.http.post(this.baseUrl + "api/account/setStartingBalance", balance_caja)
      .pipe(
        catchError((err: CustomError) => {
          return throwError(err.message);
        })
      );

  }

  ConfirmStartingBalance(medicoId: number): Observable<any> {
    return this.http.post(this.baseUrl + `api/account/confirmStartingBalance?medicoId=${ medicoId}`,null)
      .pipe(
        catchError((err: CustomError) => {
          return throwError(err.message);
        })
      );

  }
}
