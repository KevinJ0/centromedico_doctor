import {
  Component,
  ChangeDetectionStrategy,
  ViewChild,
  TemplateRef,
  OnInit,
} from "@angular/core";
import {
  startOfDay,
  endOfDay,
  subDays,
  addDays,
  endOfMonth,
  isSameDay,
  isSameMonth,
  addHours,
  addMinutes,
} from "date-fns";
import { Observable, Subject } from "rxjs";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
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
import { DialogPatientDetailsComponent } from "../dialog-patient-details/dialog-patient-details.component";
const moment = _moment;

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
  selector: "app-calendar",
  templateUrl: "./calendar.component.html",
  styleUrls: ["./calendar.component.css"],
})
export class CalendarComponent {
  @ViewChild("modalContent", { static: true }) modalContent: TemplateRef<any>;

  view: CalendarView = CalendarView.Month;
  locale: string = "es";
  CalendarView = CalendarView;
  viewDate: Date = new Date("2021-10-10");
  loadingC: boolean = true;
  mode: ProgressSpinnerMode = "indeterminate";

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

  activeDayIsOpen: boolean = false;

  constructor(
    public dialog: MatDialog,
    private modal: NgbModal,
    private citaSvc: CitaService
  ) {}

  ngOnInit(): void {
    this.citaSvc.GetCitaList().subscribe({
      next: (re: citaCalendar[]) => {
        this.events = re.map((r: citaCalendar) => {
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
        console.log(re);
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
      if(result)
      this.deleteEvent(event);
    });
  }
}
