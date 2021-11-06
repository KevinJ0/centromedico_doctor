import { animate, style, transition, trigger } from '@angular/animations';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-doctor',
  templateUrl: './doctor.component.html',
  styleUrls: ['./doctor.component.css'],
  animations: [
    trigger(
      'inOutAnimation',
      [
        transition(
          ':enter',
          [
            style({ opacity: 0 }),
            animate('300ms ease-out',
              style({ opacity: 1 }))
          ]
        ),
        transition(
          ':leave',
          [
            style({ opacity: 1, position: "absolute" }),
            animate('600ms ease-in',
              style({ opacity: 0, position: "relative" }))
          ]
        )
      ]
    )
  ]
})
export class DoctorComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

}
