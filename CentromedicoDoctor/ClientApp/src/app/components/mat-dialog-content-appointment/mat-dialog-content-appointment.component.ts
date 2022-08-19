import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-mat-dialog-content-appointment',
  templateUrl: './mat-dialog-content-appointment.component.html',
  styleUrls: ['./mat-dialog-content-appointment.component.css']
})
export class MatDialogContentAppointmentComponent implements OnInit {
  @Input() data;

  constructor() { }

  ngOnInit(): void {
  }
  tutor = {
    'grid-template-areas': "'name document'" + "'insurance service'" + "'nota nota'",
  }

  adult = {
    'grid-template-areas': "'name name'" + "'tutor document'" + "'insurance service'" + "'nota nota'",
  }
}
