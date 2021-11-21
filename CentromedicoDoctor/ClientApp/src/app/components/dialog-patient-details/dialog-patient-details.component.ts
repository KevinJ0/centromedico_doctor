import { Component, OnInit, Inject } from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { citaResponse } from "src/app/interfaces/InterfacesDto";
import * as _moment from "moment";
const moment = _moment;
import { FormBuilder, FormControl, FormGroup } from "@angular/forms";

@Component({
  selector: "app-dialog-patient-details",
  templateUrl: "./dialog-patient-details.component.html",
  styleUrls: ["./dialog-patient-details.component.css"],
})
export class DialogPatientDetailsComponent implements OnInit {
  citaDetailFormGroup: FormGroup;

  constructor(
    private _formBuilder: FormBuilder,
    public dialogRef: MatDialogRef<DialogPatientDetailsComponent>,
    @Inject(MAT_DIALOG_DATA) public data: citaResponse
  ) {
    this.citaDetailFormGroup  = new FormGroup({
      name: new FormControl(data.paciente_nombre),
      identification: new FormControl(data.doc_identidad),
        note: new FormControl(data.nota),
        service: new FormControl(''),
        insurance: new FormControl(''),
        lastName: new FormControl(''),
        tutorName: new FormControl(''),
        tutorLastName: new FormControl(''),
        contact: new FormControl(''),
        difference: new FormControl('')
      }
    );

    data.fecha_hora = _moment(data.fecha_hora).format("D/M/YYYY - hh:mm a");
    console.log(data);
  }

  ngOnInit(): void {}
  onNoClick(): void {
    this.dialogRef.close();
  }
}
