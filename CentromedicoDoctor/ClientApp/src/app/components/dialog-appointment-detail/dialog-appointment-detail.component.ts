import { Component, Inject, Input, OnInit } from "@angular/core";
import { Validators } from "@angular/forms";
import {
  MatDialog,
  MatDialogRef,
  MAT_DIALOG_DATA,
} from "@angular/material/dialog";
import { CalendarEvent } from "angular-calendar";
import * as _moment from "moment";
import { citaCalendar } from "src/app/interfaces/InterfacesDto";

export interface DialogData {
  msj: string;
  title: string;
}

@Component({
  selector: 'app-dialog-appointment-detail',
  templateUrl: './dialog-appointment-detail.component.html',
  styleUrls: ['./dialog-appointment-detail.component.css']
})

export class DialogAppointmentDetailComponent implements OnInit {

  constructor(
    public dialog: MatDialog,
    public dialogRef: MatDialogRef<DialogAppointmentDetailComponent>,
    @Inject(MAT_DIALOG_DATA) public data: citaCalendar) {
  }


  ngOnInit(): void {
    console.log(this.data);
    
  }

  onNoClick(): void {

    if (!this.dialogRef.disableClose)
      this.dialogRef.close();
  }

  onClick(response: any): void {
    this.dialogRef.close({ response: response });
  }



  tutor = {
    'grid-template-areas': "'name document'" + "'insurance service'" + "'nota nota'",
  }

  adult = {
    'grid-template-areas': "'name name'" + "'tutor document'" + "'insurance service'" + "'nota nota'",
  }

}


