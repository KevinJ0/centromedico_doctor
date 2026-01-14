import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, Validators, FormBuilder, FormControl } from '@angular/forms';
import { citaCalendar, seguro, servicio } from 'src/app/interfaces/InterfacesDto';
import moment from 'moment';
import { ServicioService } from 'src/app/services/servicio.service';
import { SeguroService } from 'src/app/services/seguro.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { CitaService } from 'src/app/services/cita.service';
import { MatDialog } from '@angular/material/dialog';
import { DialogAppointmentDetailComponent } from '../dialog-appointment-detail/dialog-appointment-detail.component';


@Component({
  selector: 'app-appointment-list',
  templateUrl: './appointment-list.component.html',
  styleUrls: ['./appointment-list.component.css']
})
export class AppointmentListComponent implements OnInit, AfterViewInit {

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

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(
    public dialog: MatDialog,
    private _formBuilder: FormBuilder,
    private citaSvc: CitaService,
    private servicioSvc: ServicioService,
    private seguroSvc: SeguroService
  ) {

  }


  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }

    this.setTotals();

  }

  setTotals(): void {

    if (this.dataSource?.filteredData) {
      this.tPatients = this.dataSource.filteredData.length;

      this.tDiferencia = this.dataSource.filteredData.reduce<number>((accumulator, obj) => {
        return accumulator + obj.diferencia;
      }, 0);

      this.tAsegurado = this.dataSource.filteredData.reduce<number>((accumulator, obj) => {
        if (obj.segurosID != 1)
          return accumulator + obj.cobertura;
        else
          return accumulator;
      }, 0);

      this.tNoAsegurado = this.dataSource.filteredData.reduce<number>((accumulator, obj) => {
        if (obj.segurosID == 1)
          return accumulator + obj.diferencia;
        else
          return accumulator;
      }, 0);

      this.tDescuento = this.dataSource.filteredData.reduce<number>((accumulator, obj) => {
        return accumulator + obj.descuento;
      }, 0);

      this.total = this.dataSource.filteredData.reduce<number>((accumulator, obj) => {
        return accumulator + obj.total;
      }, 0);

    }
  }

  ngAfterViewInit(): void {

    this.citaSvc.GetCitaList().subscribe((r: citaCalendar[]) => {

      r.map((v) => {
        v.total = v.diferencia + v.cobertura - v.descuento;
      });

      this.dataSource = new MatTableDataSource(r);
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.setTotals();
      this.loading = false;

    })

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
        this.getCitas()

      });

    this.range
      .valueChanges
      .subscribe(() => this.getCitas());

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

  getCitas(): void {

    this.loading = true;

    let inicio: Date = this.range.get("startDateControl").value;
    let fin: Date = this.range.get("endDateControl").value;
    let estado: string = this.filterFormGroup.get("statusControl").value;
    let servicioId: string = this.filterFormGroup.get("serviceTypeControl").value;
    let seguroId: string = this.filterFormGroup.get("insuranceControl").value;

    this.citaSvc.GetCitaList(inicio?.toUTCString(), fin?.toUTCString(), estado, servicioId, seguroId).subscribe((r) => {

      this.dataSource.data = r;
      r.map((v) => {
        v.total = v.diferencia + v.cobertura - v.descuento;
      });

      if (this.dataSource.paginator) {
        this.dataSource.paginator.firstPage();
      }

      this.loading = false;
      this.setTotals();

    });

  }

  openDialogDetails(citaId: string) {
    const dialogRef = this.dialog.open(DialogAppointmentDetailComponent, {

      data: this.dataSource.filteredData.find((v) => {

        if (v.id == citaId) {
          return v;
        }

        return undefined;

      })
    });

    dialogRef.afterClosed().subscribe((result: boolean) => {
      // if (result);
      // this.setCitaList();
    });
  }
}