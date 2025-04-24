import { AfterViewInit, ChangeDetectionStrategy, Component, Input, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { citaCalendar, seguro, servicio } from 'src/app/interfaces/InterfacesDto';
import * as _moment from 'moment';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { CitaService } from 'src/app/services/cita.service';
import { MatDialog } from '@angular/material/dialog';
import { DialogAppointmentDetailComponent } from '../dialog-appointment-detail/dialog-appointment-detail.component';

const moment = _moment;

@Component({
  selector: 'app-table-appointments',
  templateUrl: './table-appointments.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  styleUrls: ['./table-appointments.component.css']
})
export class TableAppointmentsComponent implements OnInit, AfterViewInit {

  filterFormGroup: FormGroup;
  range: FormGroup;
  loading: boolean = true;
  seguros: seguro[];
  servicios: servicio[];
  displayedColumns: string[] = ['turno', 'estado', 'paciente_nombre', 'servicio_descrip',
    'fecha_hora', 'seguro_descrip', 'cobertura', 'diferencia', 'descuento', 'total', 'action'];
  dataSource: MatTableDataSource<citaCalendar>;
  tPatients: number;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @Input() citas: citaCalendar[];

  constructor(
    public dialog: MatDialog,
    private _formBuilder: FormBuilder,
    private citaSvc: CitaService
  ) {

  }
  ngAfterViewInit(): void {
    try {
      this.dataSource.sort = this.sort;

    } catch (error) {

    }
  }


  ngOnChanges(changes: SimpleChanges) {

    if (this.citas) {
      this.dataSource = new MatTableDataSource(this.citas);
      this.dataSource.paginator = this.paginator;
      //this.loading = false;
    }
    //!lo moví aqui
    this.loading = false;

  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;

    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }


  ngOnInit(): void {
    this.filterFormGroup = this._formBuilder.group({
      statusControl: ['']
    });


    this.filterFormGroup.get("statusControl").valueChanges.subscribe((v) => this.getCitas());
    try {

      this.dataSource.filterPredicate = (data, filter) => {

        //  console.log(_moment(data.fecha_hora).format('DD/MM/YYYY hh:mm:ss a'));

        if (data.id.toString().toLowerCase().indexOf(filter) !== -1 ||
          data.turno.toString().toLowerCase().indexOf(filter) !== -1 ||
          (data.paciente_nombre + " " + data.paciente_apellido).toLowerCase().indexOf(filter) !== -1 ||
          data.servicio_descrip.toString().toLowerCase().indexOf(filter) !== -1 ||
          data.seguro_descrip.toString().toLowerCase().indexOf(filter) !== -1 ||
          _moment(data.fecha_hora).format('DD/MM/YYYY hh:mm:ss a').toLowerCase().indexOf(filter) !== -1 ||
          data.cobertura.toString().toLowerCase().indexOf(filter) !== -1 ||
          data.diferencia.toString().toLowerCase().indexOf(filter) !== -1 ||
          data.pago.toString().toLowerCase().indexOf(filter) !== -1 ||
          data.descuento.toString().toLowerCase().indexOf(filter) !== -1 ||
          String(data.total).indexOf(filter) !== -1 ||
          data.doc_identidad.toString().toLowerCase().indexOf(filter) !== -1 ||
          data.contacto.toString().toLowerCase().indexOf(filter) !== -1)
          return true;
        else
          return false;

      }

    } catch (error) {

    }
  }

  getCitas(): void {

    this.loading = true;

    let estado: string = this.filterFormGroup.get("statusControl").value;

    this.citaSvc.GetCitaList("", "", estado, "", "").subscribe((r) => {
      r.map((v) => {
        v.total = v.diferencia + v.cobertura - v.descuento;
      });

      this.dataSource.data = r;

      if (this.dataSource.paginator) {
        this.dataSource.paginator.firstPage();
      }

      this.loading = false;

    });

  }

  openDialogDetails(citaId: string) {
    const dialogRef = this.dialog.open(DialogAppointmentDetailComponent, {
      data: this.dataSource.filteredData.find((v) => {

        if (v.id == citaId) {
          return v;
        }
        return null;

      })
    });

    dialogRef.afterClosed().subscribe((result: boolean) => {
      // if (result);
      // this.setCitaList();
    });
  }


}