import { ChangeDetectionStrategy, Component, Inject, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import * as _moment from 'moment';
import { AccountService } from 'src/app/services/account.service';
import { trigger, style, animate, transition } from '@angular/animations';
import { ProgressSpinnerMode } from '@angular/material/progress-spinner';
import { AutoUnsubscribe } from "ngx-auto-unsubscribe";
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DialogContentComponent } from '../dialog-content/dialog-content.component';

@AutoUnsubscribe()
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  animations: [
    trigger(
      'inOutAnimation',
      [
        transition(
          ':enter',
          [
            style({ opacity: 0 }),
            animate('30ms ease-out',
              style({ opacity: 1 }))
          ]
        ),
        transition(
          ':leave',
          [
            style({ opacity: 1, position: "absolute" }),
            animate('10ms ease-in',
              style({ opacity: 0, position: "relative" }))
          ]
        )
      ]
    )
  ]
})
export class LoginComponent implements OnInit {

  mode: ProgressSpinnerMode = 'indeterminate';
  hide = true;

  loginFormGroup: FormGroup;
  loading: boolean = false;
  returnUrl: string;
  ErrorMessage: string;
  invalidLogin: boolean = false;

  constructor(
    public dialog: MatDialog,
    private router: Router,
    private accountSvc: AccountService,
    private _formBuilder: FormBuilder) {
    //go back user is already logged in
    if (this.accountSvc.CheckLoginStatus())
      this.router.navigate(['app']);
  }

  showError(dataMjs: any) {
    const dialogRef = this.dialog.open(DialogContentComponent, { data: dataMjs });
  }



  ngOnInit(): void {

    //this.returnUrl = this.rutaActiva.snapshot.queryParams['returnUrl'] || '/';

    this.loginFormGroup = this._formBuilder.group({
      loginEmailControl: ['', [
        Validators.required,
        Validators.email,
      ]],
      loginPasswordControl: ['', Validators.required],
    });
  }


  Login(): void {

    if (this.loginFormGroup.valid) {
      if (!this.loading) {
        this.loading = true;
        let userLogin = this.loginFormGroup.value;

        this.accountSvc
          .Login(userLogin.loginEmailControl, userLogin.loginPasswordControl)
          .subscribe(

            result => {

              this.loading = false;

              console.log("User Logged In Successfully");
              this.invalidLogin = false;

              if (result.roles.includes("Doctor")) {
                this.accountSvc.SetMedico(Number.parseInt(result.medicosOrMedicoId.toString()));
                this.router.navigate(['app/dashboard']);

              } else {
                //otherwise secretary

                sessionStorage.setItem("secretariaId", (result.secretariaId));

                this.router.navigate(['select-doctor'], {
                  state: { medicos: result.medicosOrMedicoId },
                  skipLocationChange: true
                });

              }

            },
            (err) => {

              this.invalidLogin = true;
              this.loading = false;
              this.ErrorMessage = "Ha ocurrido un error al tratar de entrar al sistema."
              this.ErrorMessage = err.message;
              this.showError({ type: 1, message: this.ErrorMessage });
            }
          )
      }
    }
  }

  getFontSize() {
    return 17;
  }

  ngOnDestroy() {

  }
}


