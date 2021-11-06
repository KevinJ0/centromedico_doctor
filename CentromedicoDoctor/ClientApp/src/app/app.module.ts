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
import { DateAdapter, MatNativeDateModule, MatRippleModule, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTabsModule } from '@angular/material/tabs';
import { MatSidenavModule } from '@angular/material/sidenav';

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

export function tokenGetter() {
  //return "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJKb3NlQGdtYWlsLmNvbSIsImp0aSI6IjdjOGY5ZGIyLTAyNzYtNDJkMS1iNTc3LTUyNTg1NjhjMTdlZSIsIm5hbWVpZCI6IjAxZTNhMjJiLTI2MjctNDgyMS05ZTBlLTE0NzE1MTNhOWY5NCIsInJvbGUiOiJQYXRpZW50IiwiTG9nZ2VkT24iOiI1LzI0LzIwMjEgMTA6Mjk6NTggUE0iLCJuYmYiOjE2MjE5MDk3OTgsImV4cCI6MTcxNDYyMzcxOCwiaWF0IjoxNjIxOTA5Nzk4LCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo0NDMzNyIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjQ0MzM3In0.Auc5Om1B4G5M5BJ31EEEtElCsBTug4WMO1ugChYdcEE";
  return localStorage.getItem("jwt");
}

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
    UserSettingsComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    ReactiveFormsModule,
    HttpClientModule,
    MatStepperModule,
    MatTabsModule,
    MatCheckboxModule,
    BrowserAnimationsModule,
    MatButtonModule,
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
        tokenGetter: tokenGetter,
        allowedDomains: ['localhost:4200', 'centromedico2-001-site1.etempurl.com'],
        disallowedRoutes: [],
        authScheme: "Bearer ",
      }
    }),
    RouterModule.forRoot([
      { path: '', component: LoginComponent, pathMatch: 'full' },
      {
        path: 'doctor', component: DoctorComponent, children: [
          { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
          { path: 'dashboard', component: DashboardComponent, canActivateChild: [AuthGuardService] },
          { path: 'citas', component: AppointmentListComponent, canActivateChild: [AuthGuardService] },
          { path: 'reportes', component: ReportsComponent, canActivateChild: [AuthGuardService] },
          { path: 'configuracion', component: UserSettingsComponent, canActivateChild: [AuthGuardService] },
        ]
      },
      {
        path: 'auxiliar', component: SecretaryComponent, children: [
          { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
          { path: 'dashboard', component: DashboardComponent, canActivateChild: [AuthGuardService] },
          { path: 'citas', component: AppointmentListComponent, canActivateChild: [AuthGuardService] },
          { path: 'reportes', component: ReportsComponent, canActivateChild: [AuthGuardService] },
          { path: 'configuracion', component: UserSettingsComponent, canActivateChild: [AuthGuardService] },
        ]
      },
    ]),
  ],
  providers: [{ provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    AccountService],
  bootstrap: [AppComponent]
})
export class AppModule { }
