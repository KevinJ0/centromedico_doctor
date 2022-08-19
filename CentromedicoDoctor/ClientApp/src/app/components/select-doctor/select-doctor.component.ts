import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { group } from 'src/app/interfaces/InterfacesDto';
import { GrupoService } from 'src/app/services/grupo.service';

@Component({
  selector: 'app-select-doctor',
  templateUrl: './select-doctor.component.html',
  styleUrls: ['./select-doctor.component.css']
})
export class SelectDoctorComponent implements OnInit {
  medicos: any;

  constructor(
    private gruposSvc: GrupoService,
    private router: Router) {

      
    // this.medicos = [{
    //   profilePhoto: "https://centromedico-assets.s3.us-east-2.amazonaws.com/paola.jpg",
    //   id: 1, nombre: "Paola Carolina", apellido: "Spear Petterson", especialidades: ["Alergeologo", "Ginecologo", "Cardiologo"]
    // }];


    this.medicos = this.router.getCurrentNavigation().extras?.state?.medicos; // debe de tener algo
    if (!this.medicos)
      this.router.navigate(['login']);

  }
  ngOnInit(): void {
  }

  setMedico(id: number): void {

    sessionStorage.setItem("medicoId", String(id));

    this.gruposSvc.GetGrupoList(id).subscribe((r: group[]) => {

      sessionStorage.setItem('groups', JSON.stringify(r)); // guardo la lista de los grupos para las notificaciones con signalr

      this.router.navigate(['app/dashboard']);

    });
  }
}
