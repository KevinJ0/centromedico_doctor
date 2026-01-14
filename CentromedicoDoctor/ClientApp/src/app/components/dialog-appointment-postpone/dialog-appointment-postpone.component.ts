import { Component, Inject, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { ProgressSpinnerMode } from "@angular/material/progress-spinner";
import { catchError, of } from "rxjs";
import { citaCalendar, citaForm, citaPaciente, cobertura, CustomError, hora, seguro, servicioCobertura, UserInfo, } from "src/app/interfaces/InterfacesDto";
import { CitaService } from "src/app/services/cita.service";
import { HorarioMedicoService } from "src/app/services/horario-medico-service.service";
import moment from "moment";
import "moment-timezone";
import { SnackBarService } from "src/app/services/snack-bar.service";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { CalendarEvent } from "angular-calendar";

@Component({
  selector: 'app-dialog-appointment-postpone',
  templateUrl: './dialog-appointment-postpone.component.html',
  styleUrls: ['./dialog-appointment-postpone.component.css']
})
export class DialogAppointmentPostponeComponent implements OnInit {
  citaId: number;
  medicoId: number = Number.parseInt(sessionStorage.getItem("medicoId"));
  mode: ProgressSpinnerMode = "indeterminate";
  dateTimeFormGroup: FormGroup;
  data: citaCalendar = this.event.patientData;

  loadingDateControl: boolean = false;
  loading: boolean = true;

  isEditable = false;
  underAgeShow: string = "none";
  diasLaborables: Date[];
  Horas: hora[];
  dateFilter;

  constructor(
    private openSnackBar: SnackBarService,
    private horarioMedicoSvc: HorarioMedicoService,
    private _formBuilder: FormBuilder,
    private citaSvc: CitaService,
    public dialogRef: MatDialogRef<DialogAppointmentPostponeComponent>,
    @Inject(MAT_DIALOG_DATA) public event: CalendarEvent

  ) {

    //inicializa las fechas permitidas
    this.citaSvc
      .GetCitaForm()
      .pipe(
        catchError((err) => {
          console.error(
            "Custom al tratar de acceder a los pre-datos de la cita"
          );
          this.loading = false;

          return of([]);
        })
      )
      .subscribe((r: citaForm) => {
        this.diasLaborables = r.diasLaborables.map((r) => {
          return new Date(r);
        });

        this.dateFilter = (d: Date): boolean => {
          const _date = new Date(d);

          return this.diasLaborables.find(
            (x) =>
              moment.utc(x).format("l") == moment.utc(_date).format("l")
          )
            ? true
            : false;
        };


        console.table(this.diasLaborables);
        this.dateTimeFormGroup.get("dateControl").setValue(new Date(this.data.fecha_hora));
        this.loading = false;

      });

  }


  ngOnInit(): void {

    this.dateTimeFormGroup = this._formBuilder.group({
      dateControl: [""],
      timeControl: [""],
    });

    //actualiza las horas disponibles
    this.dateTimeFormGroup.get("dateControl").valueChanges.subscribe((value) => {
      this.loadingDateControl = true;

      let _fechaCita = new Date(this.data.fecha_hora);
      let _fechaISOString = new Date(
        moment.tz(this.data.fecha_hora, "GMT").format()
      ).toISOString();

      if (value) {
        this.dateTimeFormGroup.get("timeControl").reset(null);
        this.horarioMedicoSvc.GetHoursList(value, this.medicoId).subscribe(
          (r: string[]) => {
            const keys = Object.keys(r);

            this.Horas = keys.map((key, index) => {
              return {
                id: new Date(moment(key).utc().format()).toISOString(),
                descrip:
                  moment(key).utc().format(" hh:mm A") + " - Turno " + r[key],
              };
            });

            if (
              _fechaCita.toDateString() == value.toDateString() &&
              new Date().getTime() <= _fechaCita.getTime()
            ) {
              this.Horas.push({
                id: _fechaISOString,
                descrip:
                  moment(this.data.fecha_hora).format(" hh:mm A") +
                  " - Turno " +
                  this.data.turno +
                  " Actual",
              });
            }

            this.Horas.sort((n1, n2) => {
              if (n1.id > n2.id) return 1;

              if (n1.id < n2.id) return -1;

              return 0;
            });

            if (new Date(value).toDateString() == _fechaCita.toDateString())
              this.dateTimeFormGroup.get("timeControl").setValue(_fechaISOString);

            this.loadingDateControl = false;
          },
          (err: CustomError) => {
            this.dateTimeFormGroup.get("dateControl").reset(null);
            this.loadingDateControl = false;
            this.openSnackBar.open(err.message, 1);

            console.error(
              "Ha ocurrido un error al tratar de obtener la lista de las horas disponibles: ",
              err
            );
          }
        );
      }
    });
  }

  onClickSubmit() {

    if (!this.dateTimeFormGroup.valid) {
      this.openSnackBar.open("Hay campos que necesitan ser completados.", 1);
      console.error("Las información ingresada no es valida");

    } else {
      if (!this.loading) {
        this.dialogRef.disableClose = true;
        this.loading = true;

        let fecha_hora: string = this.dateTimeFormGroup.get("timeControl").value;

        this.citaSvc.UpdateDateTime(this.data.id, fecha_hora).subscribe(
          () => {
            console.log("completado");
            this.openSnackBar.open("Actualizado", 0);

            setInterval(() => {
              this.dialogRef.close({ data: this.event });
            }, 300);

          },
          (err: CustomError) => {
            this.dialogRef.disableClose = false;
            this.loading = false;

            this.openSnackBar.open(err.message, 1);
            console.error(err);
          },
          () => {
            this.dialogRef.disableClose = false;
          }
        );
      }
    }
  }


  onNoClick(): void {

    if (!this.dialogRef.disableClose)
      this.dialogRef.close();
  }
}
