import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { ProgressSpinnerMode } from "@angular/material/progress-spinner";
import { ActivatedRoute, Params, Router } from "@angular/router";
import { catchError, of } from "rxjs";
import {
  cita,
  citaAndUser,
  citaCalendar,
  citaForm,
  cobertura,
  hora,
  seguro,
  servicioCobertura,
  UserInfo,
} from "src/app/interfaces/InterfacesDto";
import { AccountService } from "src/app/services/account.service";
import { CitaService } from "src/app/services/cita.service";
import { HorarioMedicoService } from "src/app/services/horario-medico-service.service";
import * as _moment from "moment";
import { STEPPER_GLOBAL_OPTIONS } from "@angular/cdk/stepper";

@Component({
  selector: "app-appointment-modify",
  templateUrl: "./appointment-modify.component.html",
  styleUrls: ["./appointment-modify.component.css"],
  providers: [
    {
      provide: STEPPER_GLOBAL_OPTIONS,
      useValue: { showError: true, displayDefaultIndicatorType: false }
    },
  ],
})
export class AppointmentModifyComponent implements OnInit {
  citaId: number;
  medicoId: number = Number.parseInt(localStorage.getItem("medicoId"));
  mode: ProgressSpinnerMode = "indeterminate";
  count = of(NaN);
  citaFormGroup: FormGroup;
  citaData: citaCalendar;

  seguros: seguro[];
  coberturas: cobertura[];
  servicios: servicioCobertura[];
  diferencia: number = 0;
  pago: number = 0;
  cobertura: number = 0;

  loadingPayment: boolean;
  loadingDateControl: boolean = false;
  loading: boolean = false;

  insuranceOption: boolean = true;

  isUserConfirmed: boolean;
  isDependent = false;
  isEditable = false;
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
    private horarioMedicoSvc: HorarioMedicoService,
    private accountSvc: AccountService,
    private _formBuilder: FormBuilder,
    private citaSvc: CitaService,
    private rutaActiva: ActivatedRoute
  ) {
    this.loading = true;
    console.log(this.citaSvc._citasArr);

    this.rutaActiva.params.subscribe((params: Params) => {
      this.citaId = Number.parseInt(params.id);

      if (this.citaId == 0) {
        this.router.navigate([".."]);
      }

      //inicializa las fechas permitidas
      this.citaSvc
        .GetCitaForm()
        .pipe(
          catchError((err) => {
            console.error(
              "Error al tratar de acceder a los pre-datos de la cita"
            );
            return of([]);
          })
        )
        .subscribe((r: citaForm) => {
          console.log(r);
          this.servicios = r.servicios;

          this.diasLaborables = r.diasLaborables.map((r) => {
            return new Date(r);
          });

          this.dateFilter = (d: Date): boolean => {
            const _date = new Date(d);

            return this.diasLaborables.find(
              (x) =>
                _moment.utc(x).format("l") == _moment.utc(_date).format("l")
            )
              ? true
              : false;
          };

          //Relleno los datos del usuario si existe e cita
          this.setCitaAndUserInfo();

          console.table(this.diasLaborables);
        });

      //Establezco las fechas minimas permitidas en las fechas de nacimientos
      this.minDBDDate = new Date(Date.now() + -6574 * 24 * 3600 * 1000);
      this.maxDBDDate = new Date(Date.now() + -31 * 24 * 3600 * 1000);
      this.minBDDate = new Date(Date.now() + -43825 * 24 * 3600 * 1000);
      this.maxBDDate = new Date(Date.now() + -6575 * 24 * 3600 * 1000);
    });
  }

  ngOnInit() {
    this.citaFormGroup = this._formBuilder.group({
      insuranceControl: [""],
      serviceTypeControl: ["", Validators.required],
      dateControl: [""],
      timeControl: [""],
      wsReachControl: [""],
      appointmentTypeControl: [0, Validators.required],
      dependentBirthDateControl: [""],
      dependentNameControl: [""],
      dependentLastNameControl: [""],
      typeIdentityDocControl: [0],
      identityDocControl: [
        "",
        [
          Validators.required,
          Validators.minLength(11),
          Validators.maxLength(15),
        ],
      ],
      userNameControl: ["", Validators.required],
      userLastNameControl: ["", Validators.required],
      birthDateControl: ["", Validators.required],
      contactControl: [""],
      noteControl: [""],
      dependentSexControl: [""],
      userSexControl: ["", Validators.required],
    });

    //actualiza los costos por el seguro que se escoja
    this.citaFormGroup
      .get("insuranceControl")
      .valueChanges.subscribe(() => this.setCostos());

    //actualiza los seguros disponibles al cambiar de servicio
    this.citaFormGroup
      .get("serviceTypeControl")
      .valueChanges.subscribe((value) => this.SetSegurosByServicio(value));

    //actualiza las horas disponibles
    this.citaFormGroup.get("dateControl").valueChanges.subscribe((value) => {
      this.loadingDateControl = true;

      if (value.length != 0) {
        this.citaFormGroup.get("timeControl").reset(null);

        this.horarioMedicoSvc.GetHoursList(value, this.medicoId).subscribe(
          (r: any) => {
            const keys = Object.keys(r);

            console.log(keys);

            this.Horas = keys.map((key, index) => {
              return {
                id: new Date(key),
                descrip:
                  _moment(key).utc().format(" hh:mm A") + " - Turno " + r[key],
              };
            });
            console.log(this.Horas);
            this.loadingDateControl = false;
          },
          (err) => {
            this.loadingDateControl = false;
            //  this.openSnackBar("Ha ocurrido un error al tratar de obtener la lista de las horas disponibles");
            console.error(
              "Ha ocurrido un error al tratar de obtener la lista de las horas disponibles: ",
              err
            );
          }
        );
      }
    });

    this.citaFormGroup
      .get("appointmentTypeControl")
      .valueChanges.subscribe((option) => {
        if (option == 1) this.underAgeShow = "block";
        else this.underAgeShow = "none";
        this.isDependent = Boolean(Number.parseInt(option));
      });

    this.citaFormGroup
      .get("typeIdentityDocControl")
      .valueChanges.subscribe((value) => {
        var identityDoc = this.citaFormGroup.get("identityDocControl");

        switch (value) {
          case 0:
            this.identDocMask = "000-0000000-0";
            identityDoc.setValue(
              identityDoc.value.replace(/\D/g, "").substr(0, 11)
            );
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
    this.coberturas = this.servicios.find((r) => r.id == servicioID).coberturas;
    this.citaFormGroup
      .get("insuranceControl")
      .reset(null, { onlySelf: true, emitEvent: false });
    this.setCostos();
  }

  onClickSubmit() {
    console.log(
      _moment.utc(this.citaFormGroup.get("timeControl").value).format()
    );

    if (!this.citaFormGroup.valid) {
      //    this.openSnackBar("Las información ingresada no es valida");
    } else {
      if (!this.loading) {
        this.loading = true;

        let formdata = Object.assign(this.citaFormGroup.value);
        let _cita: cita;
        let fecha_hora: Date = formdata["timeControl"];
        let contacto = formdata["contactControl"];
        let nombre = formdata["userNameControl"];
        let apellido = formdata["userLastNameControl"];
        let doc_identidad = formdata["identityDocControl"];
        let sexo = formdata["userSexControl"];
        let fecha_nacimiento = _moment(formdata["birthDateControl"]).toDate();

        let userInfo: UserInfo = {
          doc_identidad: formdata["identityDocControl"],
          nombre: formdata["userNameControl"],
          apellido: formdata["userLastNameControl"],
          fecha_nacimiento: _moment(formdata["birthDateControl"]).toDate(),
          sexo: formdata["userSexControl"],
          contacto: contacto,
        };

        if (this.isDependent) {
          nombre = formdata["dependentNameControl"];
          apellido = formdata["dependentLastNameControl"];
          sexo = formdata["dependentSexControl"];
          fecha_nacimiento = _moment(
            formdata["dependentBirthDateControl"]
          ).toDate();
        }

        _cita = {
          nombre: nombre,
          apellido: apellido,
          sexo: sexo,
          doc_identidad: doc_identidad,
          fecha_hora: fecha_hora,
          medicosID: this.medicoId,
          serviciosID: formdata["serviceTypeControl"],
          fecha_nacimiento: fecha_nacimiento,
          contacto: formdata["contactControl"],
          contacto_whatsapp: formdata["wsReachControl"],
          appointment_type: formdata["appointmentTypeControl"],
          segurosID: formdata["insuranceControl"],
          nota: formdata["noteControl"],
        };

        this.accountSvc.setUserInfo(userInfo).subscribe(
          (arg) => {},
          (err) => (this.loading = false),
          () => {
            console.log(_cita);
            this.citaSvc.UpdateCita(_cita).subscribe(
              () => {
                console.log("completado");
              },
              (err: string) => {
                this.loading = false;
                // this.openSnackBar(err);
                console.error(err);
              },
              () => {
                this.loading = false;
              }
            );
          }
        );
      }
    }
  }

  setCostos() {
    var servicioId = Number.parseInt(
      this.citaFormGroup.get("serviceTypeControl").value
    );
    var seguroId = Number.parseInt(
      this.citaFormGroup.get("insuranceControl").value
    );

    if (Number.isInteger(seguroId) && Number.isInteger(servicioId)) {
      this.loadingPayment = true;

      setTimeout(() => {
        let result = this.coberturas.find((r) => r.segurosID == seguroId);
        this.cobertura = result.cobertura;
        this.pago = result.pago;
        this.diferencia = result.diferencia;
        this.loadingPayment = false;
      }, 400);
    } else {
      this.cobertura = 0;
      this.pago = 0;
      this.diferencia = 0;
    }
  }

  setCitaAndUserInfo() {
    this.citaSvc.GetCitaPaciente(this.citaId).subscribe(
      (re: citaAndUser) => {
        console.log(re);
        this.isUserConfirmed = re.userInfo.confirm_doc_identidad;
        this.citaFormGroup.get("userNameControl").setValue(re.userInfo.nombre);
        this.citaFormGroup
          .get("userLastNameControl")
          .setValue(re.userInfo.apellido);
        this.citaFormGroup
          .get("birthDateControl")
          .setValue(re.userInfo.fecha_nacimiento);
        this.citaFormGroup.get("contactControl").setValue(re.userInfo.contacto);
        this.citaFormGroup.get("userSexControl").setValue(re.userInfo.sexo);
        this.citaFormGroup
          .get("identityDocControl")
          .setValue(re.userInfo.doc_identidad);
        var regExp = /[a-zA-Z]/i;

        if (regExp.test(re.userInfo.doc_identidad))
          this.citaFormGroup.get("typeIdentityDocControl").setValue(1);

          this.citaFormGroup.get("serviceTypeControl").setValue(re.serviciosID);
          this.citaFormGroup.get("insuranceControl").setValue(re.segurosID);


        this.loading = false;
      },
      (err) => {
        console.error(err);
      }
    );
  }

  getDSexErrorMessage() {
    return this.citaFormGroup.get("dependentSexControl").hasError("required")
      ? "Debe seleccionar una opción"
      : "";
  }
  getSexErrorMessage() {
    return this.citaFormGroup.get("userSexControl").hasError("required")
      ? "Debe seleccionar una opción"
      : "";
  }
}
