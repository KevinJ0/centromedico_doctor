import { Component, OnInit, Inject } from "@angular/core";
import {
  MatDialog,
  MatDialogRef,
  MAT_DIALOG_DATA,
} from "@angular/material/dialog";
import {
  citaCalendar,
  cobertura,
  seguro,
  servicioCobertura,
} from "src/app/interfaces/InterfacesDto";
import * as _moment from "moment";
const moment = _moment;
import { FormBuilder, FormControl, FormGroup } from "@angular/forms";
import { ProgressSpinnerMode } from "@angular/material/progress-spinner";
import { ServicioService } from "src/app/services/servicio.service";
import { DialogEntryPatientComponent } from "../dialog-entry-patient/dialog-entry-patient.component";
import { CalendarEvent } from "angular-calendar";

@Component({
  selector: "app-dialog-patient-details",
  templateUrl: "./dialog-patient-details.component.html",
  styleUrls: ["./dialog-patient-details.component.css"],
})
export class DialogPatientDetailsComponent implements OnInit {
  private data: citaCalendar = this.event.patientData;

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

  constructor(
    private servicioSvc: ServicioService,
    public dialogEntry: MatDialog,
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

    this.data.fecha_hora = _moment(this.data.fecha_hora)
      .format("D/M/YYYY - hh:mm a")
      .toString();
  }

  ngOnInit(): void {
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
  openDialogEntry(event: CalendarEvent) {
    const dialogRef = this.dialogEntry.open(DialogEntryPatientComponent, {
      data: event,
    });

    dialogRef.afterClosed().subscribe((result) => {

      if (result) 
        this.dialogRef.close({ data: event });
    });
  }
}
