import { BrowserModule } from '@angular/platform-browser';
import { NgModule, NgModuleFactoryLoader } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import * as _moment from 'moment';

import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LoginComponent } from './components/login/login.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { JwtInterceptor } from './_helpers/jwt.Interceptor';
import { JwtModule } from '@auth0/angular-jwt';
import { AccountService } from './services/account.service';
import { MatSelectModule } from '@angular/material/select';
import { MatListModule } from '@angular/material/list';
import { MatRadioModule } from '@angular/material/radio';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { NgxMaskModule, IConfig } from 'ngx-mask';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDividerModule } from '@angular/material/divider';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatStepperModule } from '@angular/material/stepper';
import { MatFormFieldModule } from '@angular/material/form-field';

import { MAT_MOMENT_DATE_ADAPTER_OPTIONS, MAT_MOMENT_DATE_FORMATS, MomentDateAdapter } from '@angular/material-moment-adapter';
import { MatNativeDateModule, MatRippleModule, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTabsModule } from '@angular/material/tabs';
import { MatSidenavModule } from '@angular/material/sidenav';
import {MatMenuModule} from '@angular/material/menu';

import { MatExpansionModule } from '@angular/material/expansion';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule } from '@angular/material/dialog';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { DialogContentComponent } from './components/dialog-content/dialog-content.component';
import { AuthGuardService } from './guards/auth-guard.service';
 import { DoctorComponent } from './components/doctor/doctor.component';
import { SecretaryComponent } from './components/secretary/secretary.component';
import { AppointmentListComponent } from './components/appointment-list/appointment-list.component';
import { ReportsComponent } from './components/reports/reports.component';
import { UserSettingsComponent } from './components/user-settings/user-settings.component';
import { CalendarModule, DateAdapter } from 'angular-calendar';
import { adapterFactory } from 'angular-calendar/date-adapters/date-fns';
import { CalendarComponent } from './components/calendar/calendar.component';
import { NgbModalModule } from '@ng-bootstrap/ng-bootstrap';
import { registerLocaleData } from '@angular/common';
import localeEs from '@angular/common/locales/es';
import { NavbarComponent } from './components/navbar/navbar.component';
import { FlatpickrModule } from 'angularx-flatpickr';
import { DialogPatientDetailsComponent } from './components/dialog-patient-details/dialog-patient-details.component';

registerLocaleData(localeEs);

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    DashboardComponent,
    DialogContentComponent,
     DoctorComponent,
    SecretaryComponent,
    AppointmentListComponent,
    ReportsComponent,
    UserSettingsComponent,
    CalendarComponent,
    NavbarComponent,
    DialogPatientDetailsComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    ReactiveFormsModule,
    HttpClientModule, 
    CalendarModule.forRoot({
      provide: DateAdapter,
      useFactory: adapterFactory,
    }),
    FlatpickrModule.forRoot(),
    NgbModalModule,
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
    JwtModule.forRoot({
      config: {
        allowedDomains: ['localhost:4200', 'centromedico2-001-site1.etempurl.com'],
        disallowedRoutes: [],
        authScheme: "Bearer ",
      }
    }),
    RouterModule.forRoot([
      { path: '', redirectTo: 'login', pathMatch: 'full' },
      { path: 'login', component: LoginComponent, pathMatch: 'full' },
      {
        path: 'doctor', component: DoctorComponent, children: [
          { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
          { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuardService] },
          { path: 'citas', component: AppointmentListComponent, canActivate: [AuthGuardService] },
          { path: 'reportes', component: ReportsComponent, canActivate: [AuthGuardService] },
          { path: 'configuracion', component: UserSettingsComponent, canActivate: [AuthGuardService] },
        ]
      },
      {
        path: 'auxiliar', component: SecretaryComponent, children: [
          { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
          { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuardService] },
          { path: 'citas', component: AppointmentListComponent, canActivate: [AuthGuardService] },
          { path: 'reportes', component: ReportsComponent, canActivate: [AuthGuardService] },
          { path: 'configuracion', component: UserSettingsComponent, canActivate: [AuthGuardService] },
        ]
      },
    ]),
  ],
  providers: [{ provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    AccountService],
  exports: [CalendarComponent],
  bootstrap: [AppComponent]
})
export class AppModule { }
