import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { especialidad } from 'src/app/interfaces/InterfacesDto';
import { EspecialidadService } from 'src/app/services/especialidad.service';
import { SnackBarService } from 'src/app/services/snack-bar.service';

@Component({
  selector: 'app-specialities',
  templateUrl: './specialities.component.html',
  styleUrls: ['./specialities.component.css']
})

export class SpecialitiesComponent implements AfterViewInit {
  especialidades: especialidad[];
  doctorEspecialidades: especialidad[];
  specForm: FormGroup;
  displayedColumns: string[] = ['descrip', 'action'];
  dataSource: MatTableDataSource<especialidad>;
  loading: boolean = true;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;

  constructor(
    private _formBuilder: FormBuilder,
    private openSnackBar: SnackBarService,
    private especialidadSvc: EspecialidadService) {

    this.specForm = this._formBuilder.group({
      specControl: [""],
    });

  }

  ngAfterViewInit(): void {
    this.setDoctorEspecialidades();
  }


  deleteSpec(id_espec: number): void {
    this.loading = true;

    this.especialidadSvc
      .DeleteEspecialidad(id_espec)
      .subscribe((r) => {
        this.setDoctorEspecialidades();
      }, (error) => {
        console.log(error);
        this.openSnackBar.open("Ha ocurrido un error 😥", 1);
        this.loading = false;
      });
  }

  addSpec(): void {

    let id_espec = this.specForm.get('specControl').value;

    if (id_espec) {

      this.loading = true;

      this.especialidadSvc
        .AddEspecialidad(id_espec)
        .subscribe((r) => {
          this.setDoctorEspecialidades();
        }, (error) => {
          console.log(error);
          this.openSnackBar.open("Ha ocurrido un error 😥", 1);
          this.loading = false;
        });
    }
  }


  setDoctorEspecialidades(): void {
    this.especialidadSvc.GetEspecialidades().subscribe((r) => {
      this.doctorEspecialidades = r;

      this.dataSource = new MatTableDataSource(this.especialidades);
      this.dataSource.data = r;
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;

      this.loading = false;

    });

    this.especialidadSvc.GetAllEspecialidades().subscribe((r) => {
      this.especialidades = r;
    });
    this.specForm.get('specControl').setValue("");
  }
}

