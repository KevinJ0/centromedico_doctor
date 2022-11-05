import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { citaCalendar } from 'src/app/interfaces/InterfacesDto';
import { CitaService } from 'src/app/services/cita.service';

@Component({
  selector: 'app-appointment-repo',
  templateUrl: './appointment-repo.component.html',
  styleUrls: ['./appointment-repo.component.css']
})
export class AppointmentRepoComponent implements OnInit {
  displayedColumns: string[] = ['turno', 'estado', 'paciente_nombre', 'servicio_descrip',
    'fecha_hora', 'seguro_descrip', 'cobertura', 'diferencia', 'descuento', 'total'];
  dataSource: MatTableDataSource<citaCalendar>;

  showContent: boolean;
  todayDate: Date = new Date();

  @ViewChild(MatSort) sort: MatSort;

  constructor(
    private citaSvc: CitaService
  ) {
  }

  ngOnInit(): void {
    this.showContent = false;

    if (this.citaSvc._citasDataRepo?.length > 0) {
      this.citaSvc._citasDataRepo.map((v) => {
        v.total = v.diferencia + v.cobertura - v.descuento;
      });

      this.dataSource = new MatTableDataSource(this.citaSvc._citasDataRepo);
      this.dataSource.sort = this.sort;
      this.showContent = true;
    }

  }



}
