import { Component, OnInit, Inject, Input } from "@angular/core";

import { timer } from 'rxjs';


import {
  MatDialog,
  MatDialogRef,
  MAT_DIALOG_DATA,
} from "@angular/material/dialog";
import {
  citaCalendar,
  cobertura,
  CustomError,
  seguro,
  servicioCobertura,
} from "src/app/interfaces/InterfacesDto";
import * as _moment from "moment";
import { FormBuilder, FormControl, FormGroup, Validators } from "@angular/forms";
import { ProgressSpinnerMode } from "@angular/material/progress-spinner";
import { ServicioService } from "src/app/services/servicio.service";
import { DialogEntryPatientComponent } from "../dialog-entry-patient/dialog-entry-patient.component";
import { CalendarEvent } from "angular-calendar";
import { DialogAppointmentPostponeComponent } from "../dialog-appointment-postpone/dialog-appointment-postpone.component";
import { DialogComponent } from "../dialog/dialog.component";
import { CitaService } from "src/app/services/cita.service";
import { SnackBarService } from "src/app/services/snack-bar.service";

@Component({
  selector: "app-dialog-patient-details",
  templateUrl: "./dialog-patient-details.component.html",
  styleUrls: ["./dialog-patient-details.component.css"],

})
export class DialogPatientDetailsComponent implements OnInit {
  private data: citaCalendar = this.event.patientData;
  isEntryToday: boolean;
  citaDetailFormGroup: FormGroup;
  identDocMask: string = "000-0000000-0";
  seguros: seguro[];
  coberturas: cobertura[];
  servicios: servicioCobertura[];
  mode: ProgressSpinnerMode = "indeterminate";
  diferencia: number = this.data.diferencia;
  pago: number = 0;
  cobertura: number = 0;
  loading: boolean = false;
  deleteCita: boolean = false;
  _fechaHora: string;

  constructor(
    private _formBuilder: FormBuilder,
    private servicioSvc: ServicioService,
    private citaSvc: CitaService,
    private openSnackBar: SnackBarService,
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<DialogPatientDetailsComponent>,
    @Inject(MAT_DIALOG_DATA) public event: CalendarEvent
  ) {


    this.citaDetailFormGroup = new FormGroup({
      name: new FormControl(this.data.paciente_nombre),
      identification: new FormControl(this.data.doc_identidad),
      note: new FormControl(this.data.nota),
      serviceType: new FormControl(this.data.serviciosID),
      insurance: new FormControl(this.data.segurosID),
      lastName: new FormControl(this.data.paciente_apellido),
      tutorName: new FormControl(this.data.paciente_nombre),
      tutorLastName: new FormControl(this.data.paciente_apellido_tutor),
      contact: new FormControl(this.data.contacto),
      tutorIdentification: new FormControl(this.data.doc_identidad),
    });

    this._fechaHora = _moment(this.data.fecha_hora)
      .format("D/M/YYYY - hh:mm a")
      .toString();
  }

  ngOnInit(): void {

    this.isEntryToday = new Date(this.data.fecha_hora).toDateString() == new Date().toDateString();

    this.servicios = this.servicioSvc.serviciosCoberturas$.getValue();

    //actualiza los costos por el seguro que se escoja
    this.citaDetailFormGroup
      .get("insurance")
      .valueChanges.subscribe(() => this.setCostos());

    //actualiza los seguros disponibles al cambiar de servicio
    this.citaDetailFormGroup
      .get("serviceType")
      .valueChanges.subscribe((value) => this.setSegurosByServicio(value));

    this.setSegurosByServicio(this.data.segurosID);

    this.citaDetailFormGroup.get("insurance").setValue(this.data.segurosID);
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  setSegurosByServicio(servicioID: number) {
    console.log(this.servicios);
    this.coberturas = this.servicios.find((r) => r.id == servicioID).coberturas;
    this.citaDetailFormGroup
      .get("insurance")
      .reset(null, { onlySelf: true, emitEvent: false });
    this.setCostos();
  }

  setCostos() {
    var servicioId = Number.parseInt(
      this.citaDetailFormGroup.get("serviceType").value
    );
    var seguroId = Number.parseInt(
      this.citaDetailFormGroup.get("insurance").value
    );

    if (Number.isInteger(seguroId) && Number.isInteger(servicioId)) {
      console.log(this.coberturas);

      let result = this.coberturas.find((r) => r.segurosID == seguroId);
      this.pago = result?.pago;
      this.cobertura = result?.cobertura;
      this.diferencia = result?.diferencia;
    } else {
      this.cobertura = 0;
      this.pago = 0;
      this.diferencia = 0;
    }
  }

  openDeleteDialog(): void {
    const dialogRef = this.dialog.open(DialogComponent, {
      data: { title: "Confirmar eliminación", msj: `¿Está seguro que desea eliminar la cita #${this.data.id} del paciente ${this.data.paciente_nombre + " " + this.data.paciente_apellido}? ` },
    });

    dialogRef.afterClosed().subscribe(result => {

      if (result?.response)
        this.citaSvc.DeleteCita(this.data.id).subscribe(
          () => {
            console.log("completado");
            this.openSnackBar.open("Operación realizada correctamente", 0);
            this.dialogRef.close({ data: true });
          },
          (err: CustomError) => {
            this.loading = false;
            this.openSnackBar.open(err.message, 1);
            console.error(err);
          },
          () => {
            this.loading = false;
          }
        );;

    });
  }

  openDialogAppointmentPostpone(event: CalendarEvent) {
    const dialogRef = this.dialog.open(DialogAppointmentPostponeComponent, {
      data: event,
    });

    dialogRef.afterClosed().subscribe((result: CalendarEvent) => {

      if (result)
        this.dialogRef.close({ data: event });
    });
  }


  openDialogEntry(event: CalendarEvent) {
    const dialogRef = this.dialog.open(DialogEntryPatientComponent, {
      data: event,
    });

    dialogRef.afterClosed().subscribe((result) => {

      if (result)
        this.dialogRef.close({ data: event });

    });
  }

 

}
