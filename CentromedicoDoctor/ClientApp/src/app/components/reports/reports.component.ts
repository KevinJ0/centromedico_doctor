import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NavigationStart, Router } from '@angular/router';
import { FormGroup, FormBuilder } from '@angular/forms';
import { citaCalendar, seguro, servicio } from 'src/app/interfaces/InterfacesDto';
import * as _moment from 'moment';
import { ServicioService } from 'src/app/services/servicio.service';
import { SeguroService } from 'src/app/services/seguro.service';
import { MatTableDataSource } from '@angular/material/table';
import { CitaService } from 'src/app/services/cita.service';
import { MatDialog } from '@angular/material/dialog';
const moment = _moment;

@Component({
  selector: 'app-reports',
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.css']
})
export class ReportsComponent implements OnInit {

  routerUrl: string;
  filterFormGroup: FormGroup;
  range: FormGroup;
  loading: boolean = true;
  seguros: seguro[];
  servicios: servicio[];
  displayedColumns: string[] = ['turno', 'estado', 'paciente_nombre', 'servicio_descrip',
    'fecha_hora', 'seguro_descrip', 'cobertura', 'diferencia', 'descuento', 'total', 'action'];
  dataSource: MatTableDataSource<citaCalendar>;
  tDiferencia: number;
  tDescuento: number;
  tNoAsegurado: number;
  tAsegurado: number;
  total: number;
  tPatients: number;


  constructor(
    private router: Router,
    private http: HttpClient,
    public dialog: MatDialog,
    private _formBuilder: FormBuilder,
    private citaSvc: CitaService,
    private servicioSvc: ServicioService,
    private seguroSvc: SeguroService
  ) {

    router
      .events
      .subscribe((event: NavigationStart) => {
        this.routerUrl = this.router.url;
      });

  }

  ngOnInit(): void {

    this.range = this._formBuilder.group({
      startDateControl: [],
      endDateControl: [],
    });

    this.filterFormGroup = this._formBuilder.group({
      insuranceControl: [''],
      serviceTypeControl: [''],
      statusControl: ['']
    });

    this.filterFormGroup
      .valueChanges
      .subscribe((v) => {
        console.log(v);

      });

  
    let medicoId = sessionStorage.getItem('medicoId');

    this.seguroSvc.GetAllSeguros(medicoId).subscribe(
      (r: seguro[]) => {
        this.seguros = r.map(value => {
          return { id: value.id, descrip: value.descrip.trim() };
        });
      }, err => {
        console.error(err);
      }
    );
    this.servicioSvc.GetAllServicios(medicoId).subscribe(
      (r: servicio[]) => {
        this.servicios = r;
      }, err => {
        console.error(err);
      }
    );

  }

  GenerateReport(): void {

    this.loading = true;

    let inicio: Date = this.range.get("startDateControl").value;
    let fin: Date = this.range.get("endDateControl").value;
    let estado: string = this.filterFormGroup.get("statusControl").value;
    let servicioId: string = this.filterFormGroup.get("serviceTypeControl").value;
    let seguroId: string = this.filterFormGroup.get("insuranceControl").value;

    this.citaSvc.GetCitaList(inicio?.toUTCString(), fin?.toUTCString(), estado, servicioId, seguroId)
      .subscribe(
        (r) => {
          this.citaSvc._citasDataRepo = r;
          
          this.router.navigate(['app/reportes/reporte-citas']).then(() => {
            this.routerUrl = this.router.url;
          });
        },
        (error: any) => console.error(error),
        () => this.loading = false
      );
  }
}


