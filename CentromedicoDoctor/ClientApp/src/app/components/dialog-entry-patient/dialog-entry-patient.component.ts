import { Component, Inject, OnInit } from "@angular/core";
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from "@angular/forms";
import {
  MatDialog,
  MatDialogRef,
  MAT_DIALOG_DATA,
} from "@angular/material/dialog";
import { ProgressSpinnerMode } from "@angular/material/progress-spinner";
import { MatSnackBar, MatSnackBarConfig } from "@angular/material/snack-bar";
import { CalendarEvent } from "angular-calendar";
import {
  cita,
  citaEntry,
  citaCalendar,
} from "src/app/interfaces/InterfacesDto";
import { CitaService } from "src/app/services/cita.service";
import { CalendarComponent } from "../calendar/calendar.component";
@Component({
  selector: "app-dialog-entry-patient",
  templateUrl: "./dialog-entry-patient.component.html",
  styleUrls: ["./dialog-entry-patient.component.css"],
})
export class DialogEntryPatientComponent implements OnInit {
   data: citaCalendar = this.event.patientData;
  totalControl = new FormControl("2");
  mode: ProgressSpinnerMode = "indeterminate";
  loadingC: boolean = false;
  EntryAppntFormGroup: FormGroup;

  constructor(
    private _snackBar: MatSnackBar,
    private _formBuilder: FormBuilder,
    private citaSvc: CitaService,
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<DialogEntryPatientComponent>,
    @Inject(MAT_DIALOG_DATA) public event: CalendarEvent
  ) {
    console.log(this.event)


  }
  total: number = 0;

  updateTotal(discount: number): void {
    let difference = this.data.diferencia;
    if (discount > difference) {
      this.total = 0;
      this.EntryAppntFormGroup.get("discount").setValue(this.data.diferencia);
      this.EntryAppntFormGroup.get("discount").updateValueAndValidity();
    } else {
      this.total = difference - discount;
    }
    this.EntryAppntFormGroup.get("total").setValue(this.total);
  }

  ngOnInit(): void {
    this.EntryAppntFormGroup = this._formBuilder.group({
      discount: [""],
      total: [this.data.diferencia],
      observation: [""],
    });

    this.EntryAppntFormGroup.get("discount").valueChanges.subscribe({
      next: (v) => {
        let n = Number.parseInt(v);

        if (n >= 0) this.updateTotal(n);
      },
    });
  }
  SaveAppointment(): void {
    if (!this.loadingC) {
      this.dialogRef.disableClose = true;
      this.loadingC = true;

      let formdata: citaEntry = {
        id: this.data.id,
        descuento: this.EntryAppntFormGroup.get("discount").value,
        medicoId: localStorage.getItem("medicoId"),
        observacion: String(this.EntryAppntFormGroup.get("observation").value)
          .toString()
          .trim(),
      };

      this.citaSvc.SaveCita(formdata).subscribe({
        next: (r) => {
          if (r) {
            this.dialogRef.close({data:this.event});
            this.openSnackBar(
              "La entrada del paciente se realizó exitosamente",
              0
            );
          }
        },
        error: (err) => {
          this.openSnackBar(err, 1);
          console.error(err);
          this.loadingC = false;
          this.dialogRef.disableClose = false;
        },
        complete: () => {
          this.loadingC = false;
          this.dialogRef.disableClose = false;
        },
      });
    }
  }

  openSnackBar(message: string, type?: number) {
    const config = new MatSnackBarConfig();
    switch (type) {
      case 0:
        config.panelClass = "background-green";
        break;
      case 1:
        config.panelClass = "background-red";
        break;

      default:
        break;
    }

    config.duration = 5000;
    this._snackBar.open(message, null, config);
  }

  onNoClick(): void {

    if (!this.dialogRef.disableClose)
      this.dialogRef.close();
  }
}
