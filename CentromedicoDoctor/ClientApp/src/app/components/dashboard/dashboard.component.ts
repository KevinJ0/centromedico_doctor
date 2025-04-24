
import {
  Component,
  ViewChild,
  TemplateRef,
  OnInit,
  NgZone,
} from "@angular/core";
import {
  startOfDay,
  endOfDay,
  isSameDay,
  isSameMonth,
} from "date-fns";
import { Subject } from "rxjs";
import {
  CalendarEvent,
  CalendarEventAction,
  CalendarEventTimesChangedEvent,
  CalendarView,
} from "angular-calendar";
import { CitaService } from "src/app/services/cita.service";
import { citaCalendar } from "src/app/interfaces/InterfacesDto";
import * as _moment from "moment";
import { ProgressSpinnerMode } from "@angular/material/progress-spinner";
import { MatDialog } from "@angular/material/dialog";
import { MatSnackBar } from "@angular/material/snack-bar";
import { SignalrCustomService } from "src/app/services/signalr-custom.service";
import { HubConnectionState } from "@microsoft/signalr";
import { Router } from "@angular/router";
import { STEPPER_GLOBAL_OPTIONS } from "@angular/cdk/stepper";
import { DialogPatientDetailsComponent } from "../dialog-patient-details/dialog-patient-details.component";

const colors: any = {
  red: {
    primary: "#ad2121",
    secondary: "#FAE3E3",
  },
  blue: {
    primary: "#1e90ff",
    secondary: "#D1E8FF",
  },
  yellow: {
    primary: "#e3bc08",
    secondary: "#FDF1BA",
  },
};

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
  providers: [
    {
      provide: STEPPER_GLOBAL_OPTIONS,
      useValue: { displayDefaultIndicatorType: false },
    },
  ],
})
export class DashboardComponent implements OnInit {

  @ViewChild("modalContent", { static: true }) modalContent: TemplateRef<any>;

  view: CalendarView = CalendarView.Month;
  locale: string = "es-DO";
  CalendarView = CalendarView;
  viewDate: Date = new Date();
  loadingC: boolean = true;
  mode: ProgressSpinnerMode = "indeterminate";
  _citasArr: citaCalendar[];
  modalData: {
    action: string;
    event: CalendarEvent;
  };

  actions: CalendarEventAction[] = [
    {
      label: '<i class="fas fa-fw fa-pencil-alt"></i>',
      a11yLabel: "Edit",
      onClick: ({ event }: { event: CalendarEvent }): void => {
        this.handleEvent("Edited", event);
      },
    },
    {
      label: '<i class="fas fa-fw fa-trash-alt"></i>',
      a11yLabel: "Delete",
      onClick: ({ event }: { event: CalendarEvent }): void => {
        this.events = this.events.filter((iEvent) => iEvent !== event);
        this.handleEvent("Deleted", event);
      },
    },
  ];

  refresh: Subject<any> = new Subject();
  events: CalendarEvent[];
  activeDayIsOpen: boolean = true;

  constructor(
    private zone: NgZone,
    private _snackBar: MatSnackBar,
    public dialog: MatDialog,
    private router: Router,
    private citaSvc: CitaService,
    private signalRSvc: SignalrCustomService
  ) {


  }

  ngOnInit(): void {

    this.setCitaList();

    //me conecto a las notificaciones
    this.signalRSvc.Connect().then(() => {
      const groups = JSON.parse(sessionStorage.getItem("groups"));
      const groupCitaName: string = groups?.CitaNotificacion;
      const medicoId = Number(sessionStorage.getItem("medicoId"));

      if (groupCitaName && medicoId) {
        this.signalRSvc.joinCitaGroup(groupCitaName, medicoId, (msj) => {
          this.zone.run(() => {
            if (this.router.url.includes("dashboard")) {
              this._snackBar.open("Han ocurrido cambios en los registros", "Actualizar", {
                horizontalPosition: "right"
              }).onAction().subscribe(() => {
                this.setCitaList();
              });
            }
          });
        });
      }
    });



  }


  setCitaList(): void {
    this.loadingC = true;
    this.citaSvc.GetCitaList("", "", "true").subscribe({
      next: (re: citaCalendar[]) => {

        re.map((v) => {
          v.total = v.diferencia + v.cobertura - v.descuento;
        });

        this.events = re.map((r: citaCalendar) => {
          this._citasArr = re;

          return {
            start: new Date(r.fecha_hora),
            end: _moment(new Date(r.fecha_hora))
              .add(_moment.duration(r.appointmentDuration))
              .toDate(),
            title: r.paciente_nombre + " " + r.paciente_apellido,
            color: colors.blue,
            actions: this.actions,
            patientData: r,
          };
        });
        console.log(this.events);
      },
      error: (err) => console.error(err),
      complete: () => (this.loadingC = false),
    });
  }

  RandomColor(): any {
    return {
      primary: "#" + Math.floor(Math.random() * 16777215).toString(16),
      secondary: "#" + Math.floor(Math.random() * 16777215).toString(16),
    };
  }

  dayClicked({ date, events }: { date: Date; events: CalendarEvent[] }): void {
    if (isSameMonth(date, this.viewDate)) {
      if (
        (isSameDay(this.viewDate, date) && this.activeDayIsOpen === true) ||
        events.length === 0
      ) {
        this.activeDayIsOpen = false;
      } else {
        this.activeDayIsOpen = true;
      }
      this.viewDate = date;
    }
  }

  eventTimesChanged({
    event,
    newStart,
    newEnd,
  }: CalendarEventTimesChangedEvent): void {
    this.events = this.events.map((iEvent) => {
      if (iEvent === event) {
        return {
          ...event,
          start: newStart,
          end: newEnd,
        };
      }
      return iEvent;
    });
    this.handleEvent("Dropped or resized", event);
  }

  handleEvent(action: string, event: CalendarEvent): void {
    this.modalData = { event, action };
    // this.modal.open(this.modalContent, { size: "lg" });
    console.log(event.patientData);
    this.openDialogDetails(event);
  }

  addEvent(): void {
    this.events = [
      ...this.events,
      {
        title: "New event",
        start: startOfDay(new Date()),
        end: endOfDay(new Date()),
        color: colors.red,
        draggable: true,
        resizable: {
          beforeStart: true,
          afterEnd: true,
        },
      },
    ];
  }

  deleteEvent(eventToDelete: CalendarEvent) {
    this.events = this.events.filter((event) => event !== eventToDelete);
  }

  setView(view: CalendarView) {
    this.view = view;
  }

  closeOpenMonthViewDay() {
    this.activeDayIsOpen = false;
  }

  openDialogDetails(event: CalendarEvent) {
    const dialogRef = this.dialog.open(DialogPatientDetailsComponent, {
      data: event,
    });

    dialogRef.afterClosed().subscribe((result: CalendarEvent) => {
      if (result)
        this.setCitaList();

    });
  }

}
