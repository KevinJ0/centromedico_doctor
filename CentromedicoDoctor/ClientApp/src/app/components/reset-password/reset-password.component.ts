import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { CustomError, ResetPassword } from 'src/app/interfaces/InterfacesDto';
import { AccountService } from 'src/app/services/account.service';
import { SnackBarService } from 'src/app/services/snack-bar.service';
@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit {

  constructor(
    private accountSvc: AccountService,
    private openSnackBar: SnackBarService,
    public dialogRef: MatDialogRef<ResetPasswordComponent>,
  ) { }

  ngOnInit(): void {
  }
  formResetPass: FormGroup = new FormGroup({
    password: new FormControl('', [Validators.required, Validators.min(5)]),
    confirmPassword: new FormControl('', [Validators.required, Validators.min(5)])
  });

  resetPass: ResetPassword;
  hide = true;
  get confirmPasswordInput() { return this.formResetPass.get('confirmPassword'); }
  get passwordInput() { return this.formResetPass.get('password'); }


  Submit(): void {
    const resetPass = { ... this.formResetPass };

    const resetPassDto: ResetPassword = {
      password: resetPass.value.password,
      confirmPassword: resetPass.value.confirmPassword,
    }

    this.accountSvc.ChangePassword(resetPassDto).subscribe(
      (r) => {
        if (r) {
          this.openSnackBar.open("Completado 👌", 0);
          console.log('Completado')
          this.dialogRef.close();
        }

      }, (err: CustomError) => {
        this.openSnackBar.open(err.message, 1);
        console.error(err);
      }
    );
  }

}
