import { StepperOrientation } from '@angular/cdk/stepper';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Observable, of } from 'rxjs';
import { Router } from '@angular/router';
import { BreakpointObserver } from '@angular/cdk/layout';
import { map, catchError } from 'rxjs/operators';
import * as _moment from 'moment';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { STEPPER_GLOBAL_OPTIONS } from '@angular/cdk/stepper';
import { CitaService } from 'src/app/services/cita.service';
import { AccountService } from 'src/app/services/account.service';
import { hora, seguro, cobertura, citaResult, servicioCobertura, citaForm, CitaCreate, Paciente } from 'src/app/interfaces/InterfacesDto';
import { ProgressSpinnerMode } from '@angular/material/progress-spinner';
import { AutoUnsubscribe } from "ngx-auto-unsubscribe";
import { HorarioMedicoService } from '../../services/horario-medico-service.service';
import { MatDialog } from '@angular/material/dialog';
import { DialogContentComponent } from '../dialog-content/dialog-content.component';
import { PacienteService } from 'src/app/services/paciente.service';

const moment = _moment;

@AutoUnsubscribe()
@Component({
  // changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-appointment-create',
  templateUrl: './appointment-create.component.html',
  styleUrls: ['./appointment-create.component.css'],
  providers: [
    {
      provide: STEPPER_GLOBAL_OPTIONS,
      useValue: { showError: true, displayDefaultIndicatorType: false }
    },
  ],
})
export class AppointmentCreateComponent implements OnInit {

  baseUrl: string;
  mode: ProgressSpinnerMode = 'indeterminate';

  firstFormGroup: FormGroup;
  secondFormGroup: FormGroup;
  thirdFormGroup: FormGroup;

  isUserConfirmed: boolean;

  seguros: seguro[];
  coberturas: cobertura[];
  servicios: servicioCobertura[];
  pacientes: Paciente[];
  paciente: Paciente;

  medicoId: number = 0;
  diferencia: number = 0;
  pago: number = 0;
  cobertura: number = 0;

  loadingPayment: boolean;
  loadingDateControl: boolean = false;
  loading: boolean = false;
  showUndoButton: boolean = false;

  insuranceOption: boolean = true;

  isDependent = false;
  isEditable = false;
  stepperOrientation: Observable<StepperOrientation>;
  minBDDate: Date;
  minDBDDate: Date;
  maxBDDate: Date;
  maxDBDDate: Date;
  underAgeShow: string = "none";
  diasLaborables: Date[];

  Horas: hora[];
  dateFilter;
  identDocMask: string = "000-0000000-0";
  selectedTypeDoc: number = 0;


  constructor(
    private router: Router,
    private _snackBar: MatSnackBar,
    public dialog: MatDialog,
    private horarioMedicoSvc: HorarioMedicoService,
    public citaSvc: CitaService,
    public pacienteSvc: PacienteService,
    private accountSvc: AccountService,
    private _formBuilder: FormBuilder,
    breakpointObserver: BreakpointObserver) {

    this.loading = true;

    this.medicoId = Number.parseInt(sessionStorage.getItem("medicoId"));;
    if (!this.medicoId) {
      this.router.navigate(['']);
    }


    this.stepperOrientation = breakpointObserver.observe('(min-width: 800px)')
      .pipe(map(({ matches }) => matches ? 'horizontal' : 'vertical'));

    //inicializa las fechas permitidas
    this.citaSvc.GetCitaForm()
      .pipe(
        catchError(err => {
          console.error('Error al tratar de acceder a los pre-datos de la cita');
          return of([]);
        })).subscribe((r: citaForm) => {
          console.log(r)
          this.servicios = r.servicios;
          this.diasLaborables =
            r.diasLaborables
              .map(r => {
                return new Date(r)
              });

          this.dateFilter = (d: Date): boolean => {
            const _date = new Date(d);

            return this.diasLaborables.find(x => _moment.utc(x).format("l") ==
              _moment.utc(_date).format("l")) ? true : false;
          }
          this.loading = false;
          console.table(this.diasLaborables);
        })

    pacienteSvc.getAllPaciente().subscribe((value) => {
      this.pacientes = value;
    });


    //Establezco las fechas minimas permitidas en las fechas de nacimientos
    this.minDBDDate = new Date(Date.now() + -6574 * 24 * 3600 * 1000);
    this.maxDBDDate = new Date(Date.now() + -31 * 24 * 3600 * 1000);
    this.minBDDate = new Date(Date.now() + -43825 * 24 * 3600 * 1000);
    this.maxBDDate = new Date(Date.now() + -6575 * 24 * 3600 * 1000);

  }


  ngOnInit() {

    this.firstFormGroup = this._formBuilder.group({
      //insuranceOptionControl: [true],
      insuranceControl: [''],
      serviceTypeControl: ['', Validators.required],
    });

    this.secondFormGroup = this._formBuilder.group({
      dateControl: [''],
      timeControl: [''],
    });

    this.thirdFormGroup = this._formBuilder.group({
      wsReachControl: [''],
      appointmentTypeControl: [0, Validators.required],
      dependentBirthDateControl: [''],
      dependentNameControl: [''],
      dependentLastNameControl: [''],
      typeIdentityDocControl: [0],
      identityDocControl: ['', [Validators.required, Validators.minLength(11), Validators.maxLength(15)]],
      userNameControl: ['', Validators.required],
      userLastNameControl: ['', Validators.required],
      birthDateControl: ['', Validators.required],
      contactControl: [''],
      noteControl: [''],
      dependentSexControl: [''],
      userSexControl: ['', Validators.required],
      emailControl: ['', Validators.email],
    });

    //actualiza los costos por el seguro que se escoja
    this.firstFormGroup.get("insuranceControl")
      .valueChanges
      .subscribe(() => this.setCostos());

    //actualiza los seguros disponibles al cambiar de servicio
    this.firstFormGroup.get("serviceTypeControl")
      .valueChanges
      .subscribe(value => this.SetSegurosByServicio(value));


    //actualiza las horas disponibles
    this.secondFormGroup.get("dateControl")
      .valueChanges
      .subscribe(value => {
        this.loadingDateControl = true;

        if (value.length != 0) {
          this.secondFormGroup.get("timeControl").reset(null);

          this.horarioMedicoSvc.GetHoursList(value, this.medicoId)
            .subscribe((r: any) => {
              const keys = Object.keys(r);

              console.log(keys);

              this.Horas = keys.map((key, index) => {
                return {
                  id: new Date(key),
                  descrip: _moment(key).utc().format(' hh:mm A') + " - Turno " + r[key]
                };
              });

              console.log(this.Horas)
              this.loadingDateControl = false;

            }, err => {
              this.loadingDateControl = false;
              this.openSnackBar("Ha ocurrido un error al tratar de obtener la lista de las horas disponibles");
              console.error('Ha ocurrido un error al tratar de obtener la lista de las horas disponibles: ', err);
            })
        }
      });



    this.thirdFormGroup.get("appointmentTypeControl")
      .valueChanges
      .subscribe(option => {
        if (option == 1) this.underAgeShow = "block";
        else this.underAgeShow = "none";
        this.isDependent = Boolean(Number.parseInt(option));
      });


    this.thirdFormGroup.get("typeIdentityDocControl")
      .valueChanges
      .subscribe(value => {
        var identityDoc = this.thirdFormGroup.get("identityDocControl");

        switch (value) {
          case 0:
            this.identDocMask = "000-0000000-0";
            identityDoc.setValue(identityDoc.value.replace(/\D/g, '').substr(0, 11));
            identityDoc.setValidators([Validators.minLength(11)]);
            break;
          case 1:
            identityDoc.setValidators([Validators.minLength(8)]);
            this.identDocMask = "AAAAAAAAAAAAAA";
            break;
          case 2:
            this.identDocMask = "AAAAAAAAAAAAAA";
            break;
        }

        this.undoAction();
        identityDoc.updateValueAndValidity();
      });

    this.thirdFormGroup.get("identityDocControl").valueChanges
      .subscribe(value => {
        this.onIdentityDocChange(value);


      });

  }

  onIdentityDocChange(value: string) {
    if (value)
      this.paciente = this.pacientes.find(p => p.doc_identidad === value);

    if (this.paciente) {
      this.thirdFormGroup.get('userNameControl').setValue(this.paciente.nombre);
      this.thirdFormGroup.get('userLastNameControl').setValue(this.paciente.apellido);
      this.thirdFormGroup.get('birthDateControl').setValue(this.paciente.fecha_nacimiento);
      this.thirdFormGroup.get('userSexControl').setValue(this.paciente.sexo);
      this.thirdFormGroup.get('contactControl').setValue(this.paciente.contacto);
      this.thirdFormGroup.get('emailControl').setValue(this.paciente.email);

      var regExp = /[a-zA-Z]/i;
      //
      if (this.paciente.doc_identidad && regExp.test(this.paciente.doc_identidad))
        this.thirdFormGroup.get("typeIdentityDocControl").setValue(1);

      this.showUndoButton = true;
    } else {
      this.showUndoButton = false; // Ocultar el botón si no hay coincidencia
    }
  }

  undoAction() {
    this.paciente = null;
    this.thirdFormGroup.get('identityDocControl').reset();
    this.thirdFormGroup.get('userNameControl').reset();
    this.thirdFormGroup.get('userLastNameControl').reset();
    this.thirdFormGroup.get('birthDateControl').reset();
    this.thirdFormGroup.get('contactControl').reset();
    this.thirdFormGroup.get('userSexControl').reset();

    this.showUndoButton = false; // Ocultar el botón de deshacer
  }

  showMessage(dataMjs: any) {
    const dialogRef = this.dialog.open(DialogContentComponent, { data: dataMjs });
  }

  SetSegurosByServicio(servicioID: number) {
    this.coberturas = this.servicios.find(r => r.id == servicioID).coberturas;
    this.firstFormGroup.get("insuranceControl").reset(null, { onlySelf: true, emitEvent: false });
    this.setCostos();
  }

  openSnackBar(message: string) {
    const config = new MatSnackBarConfig();
    config.panelClass = 'background-red';
    config.duration = 5000;
    this._snackBar.open(message, null, config);
  }



  onClickSubmit() {

    if (!this.firstFormGroup.valid || !this.secondFormGroup.valid || !this.thirdFormGroup.valid) {
      this.openSnackBar("Las información ingresada no es valida");
      return;
    }

    if (!this.loading) {
      this.loading = true;

      let formdata = Object.assign(this.firstFormGroup.value,
        this.secondFormGroup.value,
        this.thirdFormGroup.value);

      let _cita: CitaCreate;
      let fecha_hora: Date = formdata["timeControl"];
      let contacto = formdata["contactControl"];
      let nombre = formdata["userNameControl"];
      let apellido = formdata["userLastNameControl"];
      let nombreTutor = formdata["userNameControl"];
      let apellidoTutor = formdata["userLastNameControl"];
      let doc_identidad = formdata["identityDocControl"];
      let sexo = formdata["userSexControl"];
      let fecha_nacimiento = moment(formdata["birthDateControl"]).toDate();
      let email = formdata["emailControl"];
      let doc_identidad_tutor = null;

      if (this.isDependent) {
        nombre = formdata["dependentNameControl"];
        apellido = formdata["dependentLastNameControl"];
        doc_identidad_tutor = formdata["identityDocControl"];
        sexo = formdata["dependentSexControl"];
        fecha_nacimiento = moment(formdata["dependentBirthDateControl"]).toDate();
      }

      let paciente: Paciente = {
        Id: 0,
        doc_identidad: doc_identidad,
        doc_identidad_tutor: doc_identidad_tutor,
        nombre: nombre,
        apellido: apellido,
        nombre_tutor: nombreTutor,
        apellido_tutor: apellidoTutor,
        fecha_nacimiento: fecha_nacimiento,
        sexo: sexo,
        contacto_whatsapp: formdata["wsReachControl"],
        contacto: contacto,
      }

      _cita = {
        "fecha_hora": fecha_hora,
        "medicosID": this.medicoId,
        "telefono": contacto,
        "serviciosID": formdata["serviceTypeControl"],
        "email": email,
        "appointment_type": Number.parseInt(formdata["appointmentTypeControl"]),
        "segurosID": formdata["insuranceControl"],
        "nota": formdata["noteControl"],
        "paciente": paciente
      };


      this.citaSvc.CreateCita(_cita).subscribe((r: citaResult) => {
        console.log(r)

        this.showMessage({ type: 3, message: "La cita ha sido creada exitosamente" });
        this.router.navigate(['dashboard']);
      }, (err: any) => {
        this.loading = false;
        this.showMessage({ type: 1, message: err.message });
        //this.openSnackBar(err.message);
        console.error(err);
      }, () => {
        this.loading = false;
      });

    }
  }



  setCostos() {

    var servicioId = Number.parseInt(this.firstFormGroup.get("serviceTypeControl").value);
    var seguroId = Number.parseInt(this.firstFormGroup.get("insuranceControl").value);

    if (Number.isInteger(seguroId) && Number.isInteger(servicioId)) {
      this.loadingPayment = true;

      setTimeout(() => {

        let result = this.coberturas.find(r => r.segurosID == seguroId);
        this.cobertura = result.cobertura;
        this.pago = result.pago;
        this.diferencia = result.diferencia;

        this.loadingPayment = false;

      }, 400)
    }
    else {
      this.cobertura = 0;
      this.pago = 0;
      this.diferencia = 0;
    }
  }




  getDSexErrorMessage() {
    return this.thirdFormGroup.get("dependentSexControl").hasError('required') ? 'Debe seleccionar una opción' : "";
  }
  getSexErrorMessage() {
    return this.thirdFormGroup.get("userSexControl").hasError('required') ? 'Debe seleccionar una opción' : "";
  }


  ngOnDestroy() {

  }
}
