import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { ProgressSpinnerMode } from "@angular/material/progress-spinner";
import { ActivatedRoute, Params, Router } from "@angular/router";
import { catchError, of } from "rxjs";
import { citaForm, citaPaciente, cobertura, CustomError, hora, seguro, servicioCobertura } from "src/app/interfaces/InterfacesDto";
import { CitaService } from "src/app/services/cita.service";
import { HorarioMedicoService } from "src/app/services/horario-medico-service.service";
import moment from "moment";
import "moment-timezone";
import { SnackBarService } from "src/app/services/snack-bar.service";
import { NavigationService } from "src/app/services/navigation.service";

@Component({
  selector: "app-appointment-modify",
  templateUrl: "./appointment-modify.component.html",
  styleUrls: ["./appointment-modify.component.css"],
})
export class AppointmentModifyComponent implements OnInit {
  citaId: number;
  medicoId: number = Number.parseInt(sessionStorage.getItem("medicoId"));
  mode: ProgressSpinnerMode = "indeterminate";
  citaFormGroup: FormGroup;
  citaData: citaPaciente;

  seguros: seguro[];
  coberturas: cobertura[];
  servicios: servicioCobertura[];
  diferencia: number = 0;
  pago: number = 0;
  cobertura: number = 0;

  loadingPayment: boolean;
  loadingDateControl: boolean = false;
  loading: boolean = false;

  isDependent = false;
  isEditable = false;
  minDate: Date;
  maxDate: Date;
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
    private navigation: NavigationService,
    private openSnackBar: SnackBarService,
    private router: Router,
    private horarioMedicoSvc: HorarioMedicoService,
    private _formBuilder: FormBuilder,
    private citaSvc: CitaService,
    private rutaActiva: ActivatedRoute
  ) {
    this.loading = true;
    console.log(this.citaSvc._citasArr);

    this.rutaActiva.params.subscribe((params: Params) => {
      this.citaId = Number.parseInt(params["id"]);

      if (this.citaId == 0)
        this.router.navigate([".."]);

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
                moment.utc(x).format("l") == moment.utc(_date).format("l")
            )
              ? true
              : false;
          };

          //Relleno los datos del usuario si existe e cita
          this.setcitaPacienteInfo();

          console.table(this.diasLaborables);
        });

      //Establezco las fechas minimas permitidas en las fechas de nacimientos
      this.minDBDDate = new Date(Date.now() + -6574 * 24 * 3600 * 1000);
      this.maxDBDDate = new Date(Date.now() + -31 * 24 * 3600 * 1000);
      this.minBDDate = new Date(Date.now() + -43825 * 24 * 3600 * 1000);
      this.maxBDDate = new Date(Date.now() + -6575 * 24 * 3600 * 1000);
      this.maxDate = this.maxBDDate;
      this.minDate = this.minBDDate;
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
      userBirthDateControl: ["", Validators.required],
      tutorNameControl: [""],
      tutorLastNameControl: [""],
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
      contactControl: [""],
      noteControl: [""],
      userSexControl: ["", Validators.required],
    });

    //actualiza los costos por el seguro que se escoja
    this.citaFormGroup
      .get("insuranceControl")
      .valueChanges.subscribe(() => this.setCostos());

    //actualiza los seguros disponibles al cambiar de servicio
    this.citaFormGroup.get("serviceTypeControl").valueChanges.subscribe((value) => this.SetSegurosByServicio(value));

    //actualiza las horas disponibles
    this.citaFormGroup.get("dateControl").valueChanges.subscribe((value) => {
      this.loadingDateControl = true;

      let _fechaCita = new Date(this.citaData.fecha_hora);
      let _fechaISOString = new Date(
        moment.tz(this.citaData.fecha_hora, "GMT").format()
      ).toISOString();

      if (value) {
        this.citaFormGroup.get("timeControl").reset(null);
        this.horarioMedicoSvc.GetHoursList(value, this.medicoId).subscribe(
          (r: string[]) => {
            const keys = Object.keys(r);

            this.Horas = keys.map((key, index) => {
              return {
                id: new Date(moment(key).utc().format()).toISOString(),
                descrip:
                  moment(key).utc().format(" hh:mm A") + " - Turno " + r[key],
              };
            });

            // agregamos por ultimo la hora actual selecta que no viene en la api
            if (
              _fechaCita.toDateString() == value.toDateString() &&
              new Date().getTime() <= _fechaCita.getTime()
            ) {
              this.Horas.push({
                id: _fechaISOString,
                descrip:
                  moment(this.citaData.fecha_hora).format(" hh:mm A") +
                  " - Turno " +
                  this.citaData.turno
              });
            }

            this.Horas.sort((n1, n2) => {
              if (n1.id > n2.id) return 1;

              if (n1.id < n2.id) return -1;

              return 0;
            });

            if (new Date(value).toDateString() == _fechaCita.toDateString())
              this.citaFormGroup.get("timeControl").setValue(_fechaISOString);

            this.loadingDateControl = false;
          },
          (err: CustomError) => {
            this.openSnackBar.open(err.message, 1);
            this.citaFormGroup.get("dateControl").reset(null);
            this.loadingDateControl = false;

            console.error(
              "Ha ocurrido un error al tratar de obtener la lista de las horas disponibles: ",
              err.message
            );
          }
        );
      }
    });

    this.citaFormGroup
      .get("appointmentTypeControl")
      .valueChanges.subscribe((option) => {
        if (option == 1) {
          this.underAgeShow = "block";
          this.maxDate = this.maxDBDDate;
          this.minDate = this.minDBDDate;
        } else {
          this.underAgeShow = "none";
          this.maxDate = this.maxBDDate;
          this.minDate = this.minBDDate;
        }
        this.citaFormGroup.get("userBirthDateControl").reset(null);
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
    this.citaFormGroup.get("insuranceControl").reset(null, {
      onlySelf: true,
      emitEvent: false,
    });
    this.setCostos();
  }

  onClickSubmit() {

    if (!this.citaFormGroup.valid) {
      this.openSnackBar.open("Hay campos que necesitan ser completados.", 1);
      console.error("Las información ingresada no es valida");
    } else {
      if (!this.loading) {
        this.loading = true;

        let formdata = Object.assign(this.citaFormGroup.value);
        let cita_paciente: citaPaciente;
        let fecha_hora: string = formdata["timeControl"];
        let nombre = formdata["userNameControl"];
        let apellido = formdata["userLastNameControl"];
        let nombre_tutor = formdata["tutorNameControl"];
        let apellido_tutor = formdata["tutorLastNameControl"];
        let doc_identidad = formdata["identityDocControl"];
        let sexo = formdata["userSexControl"];
        let fecha_nacimiento = moment(
          formdata["userBirthDateControl"]
        ).toISOString();




        cita_paciente = {
          paciente_nombre: nombre,
          paciente_apellido: apellido,
          paciente_apellido_tutor: apellido_tutor,
          paciente_nombre_tutor: nombre_tutor,
          sexo: sexo,
          doc_identidad: doc_identidad,
          fecha_hora: fecha_hora,
          medicosID: this.medicoId,
          serviciosID: formdata["serviceTypeControl"],
          fecha_nacimiento: fecha_nacimiento,
          contacto: formdata["contactControl"],
          contacto_whatsapp: formdata["wsReachControl"],
          segurosID: formdata["insuranceControl"],
          nota: formdata["noteControl"],
        };

        console.log(cita_paciente);

        this.citaSvc.UpdateCita(this.citaId, cita_paciente).subscribe(
          () => {
            console.log("completado");
            this.openSnackBar.open("Actualizado", 0);
          },
          (err: CustomError) => {
            this.loading = false;
            this.openSnackBar.open(err.message, 1);
            console.error(err);
          },
          () => {
            this.loading = false;
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

  setcitaPacienteInfo() {
    this.citaSvc.GetCitaPaciente(this.citaId).subscribe(
      (re: citaPaciente) => {
        this.citaData = re;
        console.log(re);

        if (re.edad < 18) {
          this.citaFormGroup.get("appointmentTypeControl").setValue("1");
          this.citaFormGroup
            .get("tutorNameControl")
            .setValue(re.paciente_nombre);
          this.citaFormGroup
            .get("tutorLastNameControl")
            .setValue(re.paciente_apellido);
          this.citaFormGroup.get("userSexControl").setValue(re.sexo);

          this.citaFormGroup
            .get("userBirthDateControl")
            .setValue(new Date(re.fecha_nacimiento));
        }

        this.citaFormGroup.get("userNameControl").setValue(re.paciente_nombre);
        this.citaFormGroup
          .get("userLastNameControl")
          .setValue(re.paciente_apellido);
        this.citaFormGroup
          .get("userBirthDateControl")
          .setValue(new Date(re.fecha_nacimiento));
        this.citaFormGroup.get("contactControl").setValue(re.contacto);
        this.citaFormGroup.get("userSexControl").setValue(re.sexo);
        this.citaFormGroup.get("noteControl").setValue(re.nota);
        this.citaFormGroup.get("identityDocControl").setValue(re.doc_identidad);
        var regExp = /[a-zA-Z]/i;

        if (regExp.test(re.doc_identidad))
          this.citaFormGroup.get("typeIdentityDocControl").setValue(1);

        this.citaFormGroup.get("dateControl").setValue(new Date(re.fecha_hora));
        this.citaFormGroup.get("serviceTypeControl").setValue(re.serviciosID);
        this.citaFormGroup.get("insuranceControl").setValue(re.segurosID);

        this.citaFormGroup.get("wsReachControl").setValue(re.contacto_whatsapp);

        this.loading = false;
      },
      (err) => {
        console.error(err);
      }
    );
  }

  getSexErrorMessage() {
    return this.citaFormGroup.get("userSexControl").hasError("required")
      ? "Debe seleccionar una opción"
      : "";
  }


  back(): void {
    this.navigation.back()
  }

}
