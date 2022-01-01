import { Component, OnInit } from "@angular/core";
import { ServicioService } from "src/app/services/servicio.service";

@Component({
  selector: "app-doctor",
  templateUrl: "./doctor.component.html",
  styleUrls: ["./doctor.component.css"],
})
export class DoctorComponent implements OnInit {
  medicoId: string; // para la secretaria solamente

  constructor(private servicioSvc: ServicioService) {
    localStorage.setItem("medicoId", "1");
  }

  ngOnInit(): void {
    this.medicoId = localStorage.getItem("medicoId");

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
