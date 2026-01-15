import { Component, OnInit } from "@angular/core";
import { NavigationEnd, Router } from "@angular/router";
import { filter } from "rxjs/operators";
import { ServicioService } from "src/app/services/servicio.service";
import * as AOS from 'aos';

@Component({
  selector: "app-main-container",
  templateUrl: "./main-container.component.html",
  styleUrls: ["./main-container.component.css"],
})
export class MainContainerComponent implements OnInit {
  medicoId: any;
  showFiller: boolean;
  constructor(
    private router: Router,
    private servicioSvc: ServicioService) {
  }

  ngOnInit(): void {
    AOS.init({
      duration: 600,
      once: true,
      easing: 'ease-out-quad'
    });

    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      AOS.refresh();
      // Small timeout to ensure DOM is ready
      setTimeout(() => AOS.refresh(), 100);
    });

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
