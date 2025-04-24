import { BrowserModule } from "@angular/platform-browser";
import { LOCALE_ID, NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { HttpClientModule, HTTP_INTERCEPTORS } from "@angular/common/http";
import { RouterModule } from "@angular/router";
import * as _moment from "moment";

import { AppComponent } from "./app.component";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { LoginComponent } from "./components/login/login.component";
import { DashboardComponent } from "./components/dashboard/dashboard.component";
import { JwtInterceptor } from "./_helpers/jwt.Interceptor";
import { JwtModule } from "@auth0/angular-jwt";
import { AccountService } from "./services/account.service";
import { MatSelectModule } from "@angular/material/select";
import { MatListModule } from "@angular/material/list";
import { MatRadioModule } from "@angular/material/radio";
import { MatDatepickerModule } from "@angular/material/datepicker";
import { MatButtonToggleModule } from "@angular/material/button-toggle";
import { NgxMaskModule, IConfig } from "ngx-mask";
import { MatCheckboxModule } from "@angular/material/checkbox";
import { MatDividerModule } from "@angular/material/divider";
import { MatInputModule } from "@angular/material/input";
import { MatCardModule } from "@angular/material/card";
import { MatButtonModule } from "@angular/material/button";
import { MatStepperModule } from "@angular/material/stepper";
import { MatFormFieldModule } from "@angular/material/form-field";

import {
  MAT_MOMENT_DATE_ADAPTER_OPTIONS,
  MAT_MOMENT_DATE_FORMATS,
  MomentDateAdapter,
} from "@angular/material-moment-adapter";
import {
  MatNativeDateModule,
  MatRippleModule,
  MAT_DATE_FORMATS,
  MAT_DATE_LOCALE,
} from "@angular/material/core";
import { MatPaginatorModule } from "@angular/material/paginator";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { MatTabsModule } from "@angular/material/tabs";
import { MatSidenavModule } from "@angular/material/sidenav";
import { MatMenuModule } from "@angular/material/menu";

import { MatExpansionModule } from "@angular/material/expansion";
import { MatTableModule } from "@angular/material/table";
import { MatIconModule } from "@angular/material/icon";
import { MatDialogModule } from "@angular/material/dialog";
import { MatPaginatorIntl } from "@angular/material/paginator";
import { DialogContentComponent } from "./components/dialog-content/dialog-content.component";
import { AuthGuardService } from "./guards/auth-guard.service";
import { AppointmentListComponent } from "./components/appointment-list/appointment-list.component";
import { ReportsComponent } from "./components/reports/reports.component";
import { UserSettingsComponent } from "./components/user-settings/user-settings.component";
import { CalendarModule, DateAdapter } from "angular-calendar";
import { adapterFactory } from "angular-calendar/date-adapters/date-fns";
import { NgbModalModule } from "@ng-bootstrap/ng-bootstrap";
import { registerLocaleData } from "@angular/common";
import localeEs from "@angular/common/locales/es";
import { NavbarComponent } from "./components/navbar/navbar.component";
//import { FlatpickrModule } from "angularx-flatpickr";
import { DialogPatientDetailsComponent } from "./components/dialog-patient-details/dialog-patient-details.component";
import { ServicioService } from "./services/servicio.service";
import { CitaService } from "./services/cita.service";
import { DialogEntryPatientComponent } from "./components/dialog-entry-patient/dialog-entry-patient.component";
import { LoadingComponent } from "./components/loading/loading.component";
import { MatSnackBarModule } from "@angular/material/snack-bar";
import { AppointmentModifyComponent } from "./components/appointment-modify/appointment-modify.component";
import { HorarioMedicoService } from "./services/horario-medico-service.service";
import { SnackBarService } from "./services/snack-bar.service";
import { BuildingComponent } from './components/building/building.component';
import { DialogAppointmentPostponeComponent } from './components/dialog-appointment-postpone/dialog-appointment-postpone.component';
import { DialogComponent } from './components/dialog/dialog.component';
import { SignalrCustomService } from "./services/signalr-custom.service";
import { SelectDoctorComponent } from './components/select-doctor/select-doctor.component';
import { GrupoService } from "./services/grupo.service";
import { SnackbarUpdateComponent } from './components/snackbar-update/snackbar-update.component';
import { CustomPaginator } from "./shared/CustomPaginatorConfiguration";
import { AngularSvgIconModule } from 'angular-svg-icon';
import { DialogAppointmentDetailComponent } from './components/dialog-appointment-detail/dialog-appointment-detail.component';
import { TableAppointmentsComponent } from './components/table-appointments/table-appointments.component';
import { MatSortModule } from "@angular/material/sort";
import { MatDialogContentAppointmentComponent } from './components/mat-dialog-content-appointment/mat-dialog-content-appointment.component';
import { AppointmentRepoComponent } from './components/reports/appointment-repo/appointment-repo.component';
import { NgxPrintModule } from 'ngx-print';
import { ImageCropperModule } from 'ngx-image-cropper';
import { NoImagePipe } from "./Pipes/noImage";
import { ImageCropperComponent } from './components/image-cropper/image-cropper.component';
import { MainContainerComponent } from "./components/main-container/main-container.component";
import { FlatpickrModule } from 'angularx-flatpickr';
import { ResetPasswordComponent } from './components/reset-password/reset-password.component';
import { StartingBalanceComponent } from './components/starting-balance/starting-balance.component';
import { SpecialitiesComponent } from './components/specialities/specialities.component';
import { AppointmentCreateComponent } from "./components/appointment-create/appointment-create.component";
import localeEsDO from '@angular/common/locales/es-DO';
import { DEFAULT_CURRENCY_CODE } from '@angular/core';

// Registrar el locale de República Dominicana
registerLocaleData(localeEsDO, 'es-DO');

export function tokenGetter() {
  //return "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJKb3NlQGdtYWlsLmNvbSIsImp0aSI6IjdjOGY5ZGIyLTAyNzYtNDJkMS1iNTc3LTUyNTg1NjhjMTdlZSIsIm5hbWVpZCI6IjAxZTNhMjJiLTI2MjctNDgyMS05ZTBlLTE0NzE1MTNhOWY5NCIsInJvbGUiOiJQYXRpZW50IiwiTG9nZ2VkT24iOiI1LzI0LzIwMjEgMTA6Mjk6NTggUE0iLCJuYmYiOjE2MjE5MDk3OTgsImV4cCI6MTcxNDYyMzcxOCwiaWF0IjoxNjIxOTA5Nzk4LCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo0NDMzNyIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjQ0MzM3In0.Auc5Om1B4G5M5BJ31EEEtElCsBTug4WMO1ugChYdcEE";
  return sessionStorage.getItem("jwt");
}

export const MY_FORMATS = {
  // parse: {
  //   dateInput: 'LL',
  // },
  display: {

    dateInput: 'dddd DD MMM Y',
    monthYearLabel: 'MMM YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM YYYY',
  },
};
@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    DashboardComponent,
    DialogContentComponent,
    MainContainerComponent,
    AppointmentListComponent,
    ReportsComponent,
    UserSettingsComponent,
    NavbarComponent,
    DialogPatientDetailsComponent,
    DialogEntryPatientComponent,
    LoadingComponent,
    AppointmentModifyComponent,
    BuildingComponent,
    DialogAppointmentPostponeComponent,
    DialogComponent,
    SelectDoctorComponent,
    SnackbarUpdateComponent,
    DialogAppointmentDetailComponent,
    TableAppointmentsComponent,
    MatDialogContentAppointmentComponent,
    AppointmentRepoComponent,
    NoImagePipe,
    ImageCropperComponent,
    ResetPasswordComponent,
    StartingBalanceComponent,
    SpecialitiesComponent,
    AppointmentCreateComponent,
  ],
  imports: [
    ImageCropperModule,
    BrowserModule.withServerTransition({ appId: "ng-cli-universal" }),
    ReactiveFormsModule,
    HttpClientModule,
    CalendarModule.forRoot({
      provide: DateAdapter,
      useFactory: adapterFactory,
    }),
    FlatpickrModule.forRoot(),
    NgbModalModule,
    MatSortModule,
    MatStepperModule,
    MatTabsModule,
    MatCheckboxModule,
    BrowserAnimationsModule,
    MatButtonModule,
    MatMenuModule,
    MatFormFieldModule,
    MatExpansionModule,
    MatButtonToggleModule,
    MatTableModule,
    MatSnackBarModule,
    MatListModule,
    MatDialogModule,
    MatDividerModule,
    MatRadioModule,
    MatNativeDateModule,
    MatPaginatorModule,
    NgxMaskModule.forRoot(),
    MatRippleModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatDatepickerModule,
    MatCardModule,
    FormsModule,
    MatSidenavModule,
    MatInputModule,
    NgxPrintModule,
    AngularSvgIconModule.forRoot(),
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        allowedDomains: [
          "localhost:4200",
        ],
        disallowedRoutes: [],
        authScheme: "Bearer ",
      },
    }),
    RouterModule.forRoot([
      { path: "login", component: LoginComponent, pathMatch: "full" },
      { path: "select-doctor", component: SelectDoctorComponent, pathMatch: "full", canActivate: [AuthGuardService] },
      { path: "", component: LoginComponent, pathMatch: "full" },
      {
        path: "app",
        component: MainContainerComponent,
        children: [
          { path: "", redirectTo: "dashboard", pathMatch: "full" },
          {
            path: "dashboard",
            component: DashboardComponent,
            canActivate: [AuthGuardService],
          },
          {
            path: "crear-cita",
            component: AppointmentCreateComponent,
            canActivate: [AuthGuardService],
          },
          {
            path: "cita/:id",
            component: AppointmentModifyComponent,
            canActivate: [AuthGuardService],
          },
          {
            path: "citas",
            component: AppointmentListComponent,
            canActivate: [AuthGuardService],
          },
          {
            path: "reportes",
            component: ReportsComponent,
            children: [{
              path: "reporte-citas",
              component: AppointmentRepoComponent,
            }],
            canActivate: [AuthGuardService],
          },
          {
            path: "configuracion",
            component: UserSettingsComponent,
            canActivate: [AuthGuardService],
          },
        ], canActivate: [AuthGuardService]
      },
      { path: "**", redirectTo: "login", pathMatch: "full" },
    ]),
  ],
  providers: [
    { provide: MatPaginatorIntl, useValue: CustomPaginator() },
    { provide: MAT_DATE_LOCALE, useValue: 'es-DO' },
    //{ provide: MAT_MOMENT_DATE_ADAPTER_OPTIONS, useValue: { useUtc: true } },
    { provide: MAT_DATE_FORMATS, useValue: MY_FORMATS },
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: LOCALE_ID, useValue: 'es-DO' },
    { provide: DEFAULT_CURRENCY_CODE, useValue: 'DOP' },
    AccountService,
    ServicioService,
    HorarioMedicoService,
    SnackBarService,
    CitaService,
    SignalrCustomService,
    GrupoService
  ],
  exports: [DashboardComponent],
  bootstrap: [AppComponent],
})
export class AppModule { }
