import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { group } from 'src/app/interfaces/InterfacesDto';
import { AccountService } from 'src/app/services/account.service';
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
    private router: Router,
    private accountSvc: AccountService) {

       
    this.medicos = this.router.getCurrentNavigation().extras?.state?.medicos; // debe de tener algo
    if (!this.medicos)
      this.router.navigate(['login']);

  }
  ngOnInit(): void {
  }

  
}
