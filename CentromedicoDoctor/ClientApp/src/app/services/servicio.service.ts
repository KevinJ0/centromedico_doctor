import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Router } from "@angular/router";
import {
  BehaviorSubject,
  catchError,
  map,
  Observable,
  of,
  Subject,
  throwError,
} from "rxjs";
import { servicioCobertura } from "../interfaces/InterfacesDto";

@Injectable({
  providedIn: "root",
})
export class ServicioService {
  public serviciosCoberturas$ = new BehaviorSubject<servicioCobertura[]>(null);
  baseUrl: string;

  // Url to access our Web API’s
  constructor(
    private router: Router,
    private http: HttpClient,
    @Inject("BASE_URL") baseUrl: string
  ) {
    this.baseUrl = baseUrl;
  }

  GetServiciosCoberturas(
    medicoId?: string | number
  ): Observable<servicioCobertura[]> {
    return this.http
      .get<servicioCobertura[]>(
        this.baseUrl +
          `api/servicios/getServiciosCoberturas?medicoid=${medicoId}`
      )
      .pipe(
        catchError((error) => throwError(() => error)),
        map((result: servicioCobertura[]) => result)
      );
  }
}
