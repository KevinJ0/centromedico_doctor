import { StepperOrientation } from '@angular/cdk/stepper';
import { Component, Inject, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Observable, of } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { BreakpointObserver } from '@angular/cdk/layout';
import { map, catchError } from 'rxjs/operators';
import moment from 'moment';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { STEPPER_GLOBAL_OPTIONS } from '@angular/cdk/stepper';
import { CoberturaService } from 'src/app/services/cobertura.service';
import { CitaService } from 'src/app/services/cita.service';
import { AccountService } from 'src/app/services/account.service';
import { hora, UserInfo, seguro, cita, cobertura, citaResult, servicioCobertura, citaForm } from 'src/app/interfaces/InterfacesDto';
import { SeguroService } from 'src/app/services/seguro.service';
import { ProgressSpinnerMode } from '@angular/material/progress-spinner';
import { AutoUnsubscribe } from "ngx-auto-unsubscribe";
import { HorarioMedicoService } from '../../services/horario-medico-service.service';

@AutoUnsubscribe()
@Component({
  // changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-appointment-add',
  templateUrl: './appointment-add.component.html',
  styleUrls: ['./appointment-add.component.css'],
  providers: [
    {
      provide: STEPPER_GLOBAL_OPTIONS,
      useValue: { showError: true, displayDefaultIndicatorType: false }
    },
  ],
})
export class AppointmentAddComponent implements OnInit {

  baseUrl: string;
  mode: ProgressSpinnerMode = 'indeterminate';

  firstFormGroup: FormGroup;
  secondFormGroup: FormGroup;
  thirdFormGroup: FormGroup;

  isUserConfirmed: boolean;

  seguros: seguro[];
  coberturas: cobertura[];
  servicios: servicioCobertura[];
  medicoId: number = 0;
  diferencia: number = 0;
  pago: number = 0;
  cobertura: number = 0;

  loadingPayment: boolean;
  loadingDateControl: boolean = false;
  loading: boolean = false;

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
    private rutaActiva: ActivatedRoute,
    private _snackBar: MatSnackBar,
    private horarioMedicoSvc: HorarioMedicoService,
    public citaSvc: CitaService,
    private accountSvc: AccountService,
    private _formBuilder: FormBuilder,
    breakpointObserver: BreakpointObserver) {

    this.loading = true;

    this.medicoId = Number.parseInt(this.rutaActiva.snapshot.queryParamMap.get('medicoId'));
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

            return this.diasLaborables.find(x => moment.utc(x).format("l") ==
              moment.utc(_date).format("l")) ? true : false;
          }
          this.loading = false;
          console.table(this.diasLaborables);
        })

    //Establezco las fechas minimas permitidas en las fechas de nacimientos
    this.minDBDDate = new Date(Date.now() + -6574 * 24 * 3600 * 1000);
    this.maxDBDDate = new Date(Date.now() + -31 * 24 * 3600 * 1000);
    this.minBDDate = new Date(Date.now() + -43825 * 24 * 3600 * 1000);
    this.maxBDDate = new Date(Date.now() + -6575 * 24 * 3600 * 1000);

    //Relleno los datos del usuario si existe
    this.setUserInfo();

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

        identityDoc.updateValueAndValidity();
      });
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

      let _cita: cita;
      let fecha_hora: Date = formdata["timeControl"];
      let contacto = formdata["contactControl"];
      let nombre = formdata["userNameControl"];
      let apellido = formdata["userLastNameControl"];
      let doc_identidad = formdata["identityDocControl"];
      let sexo = formdata["userSexControl"];
      let fecha_nacimiento = moment(formdata["birthDateControl"]).toDate();

      let userInfo: UserInfo = {
        doc_identidad: formdata["identityDocControl"],
        nombre: formdata["userNameControl"],
        apellido: formdata["userLastNameControl"],
        fecha_nacimiento: moment(formdata["birthDateControl"]).toDate(),
        sexo: formdata["userSexControl"],
        contacto: contacto
      }

      if (this.isDependent) {
        nombre = formdata["dependentNameControl"];
        apellido = formdata["dependentLastNameControl"];
        sexo = formdata["dependentSexControl"];
        fecha_nacimiento = moment(formdata["dependentBirthDateControl"]).toDate();
      }

      _cita = {
        "nombre": nombre,
        "apellido": apellido,
        "sexo": sexo,
        "doc_identidad": doc_identidad,
        "fecha_hora": fecha_hora,
        "medicosID": this.medicoId,
        "serviciosID": formdata["serviceTypeControl"],
        "fecha_nacimiento": fecha_nacimiento,
        "contacto": formdata["contactControl"],
        "contacto_whatsapp": formdata["wsReachControl"],
        "appointment_type": formdata["appointmentTypeControl"],
        "segurosID": formdata["insuranceControl"],
        "nota": formdata["noteControl"]
      };

      this.accountSvc.setUserInfo(userInfo).subscribe(arg => { },
        err => this.loading = false,
        () => {
          console.log(_cita)
          this.citaSvc.CreateCita(_cita).subscribe((r: citaResult) => {
            console.log(r)
            //this.citaSvc._citaResult = r;
            this.router.navigate(['dashboard']);
          }, (err: string) => {
            this.loading = false;
            this.openSnackBar(err);
            console.error(err);
          }, () => {
            this.loading = false;
          });
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



  setUserInfo() {
    this.accountSvc.getUserInfo()
      .subscribe((re: UserInfo) => {

        console.log(re);
        this.isUserConfirmed = re.confirm_doc_identidad;
        this.thirdFormGroup.get("userNameControl").setValue(re.nombre);
        this.thirdFormGroup.get("userLastNameControl").setValue(re.apellido);
        this.thirdFormGroup.get("birthDateControl").setValue(re.fecha_nacimiento);
        this.thirdFormGroup.get("contactControl").setValue(re.contacto);
        this.thirdFormGroup.get("userSexControl").setValue(re.sexo);
        this.thirdFormGroup.get("identityDocControl").setValue(re.doc_identidad);
        var regExp = /[a-zA-Z]/i;

        if (regExp.test(re.doc_identidad))
          this.thirdFormGroup.get("typeIdentityDocControl").setValue(1);

      }, err => {
        console.error(err)
      });
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
