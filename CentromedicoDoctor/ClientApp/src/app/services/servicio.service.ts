import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { map, Observable, of } from "rxjs";
import { servicioCobertura } from "../interfaces/InterfacesDto";

@Injectable({
  providedIn: "root",
})
export class ServicioService {
  servicios: servicioCobertura[];

  baseUrl: string;

  // Url to access our Web API’s
  constructor(
    private router: Router,
    private http: HttpClient,
    @Inject("BASE_URL") baseUrl: string
  ) {
    this.baseUrl = baseUrl;
  }

  GetServiciosCoberturas() {
    try {
      this.http
        .get<servicios>(this.baseUrl + `api/service/getServiceCobertura`)
        .pipe(
          map((result) => {
            this.servicios = result;
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
}
