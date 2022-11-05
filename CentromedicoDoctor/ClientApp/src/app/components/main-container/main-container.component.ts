import { Component, OnInit } from "@angular/core";
import { ServicioService } from "src/app/services/servicio.service";

@Component({
  selector: "app-main-container",
  templateUrl: "./main-container.component.html",
  styleUrls: ["./main-container.component.css"],
})
export class MainContainerComponent implements OnInit {
  medicoId: any; // para la secretaria solamente

  constructor(
    private servicioSvc: ServicioService) {
  }

  ngOnInit(): void {
    this.medicoId = sessionStorage.getItem("medicoId");

    this.servicioSvc.GetServiciosCoberturas(this.medicoId).subscribe({
      next: (r) => {
        this.servicioSvc.serviciosCoberturas$.next(r);
      },
      error: (err) => {
        console.error(err);
      },
    });
  }
}
