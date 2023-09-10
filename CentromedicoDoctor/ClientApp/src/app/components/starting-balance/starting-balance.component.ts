import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { map } from 'rxjs';
import { balanceCaja, secretaria } from 'src/app/interfaces/InterfacesDto';
import { AccountService } from 'src/app/services/account.service';
import { BalanceCajaService } from 'src/app/services/balance-caja.service';
import { SnackBarService } from 'src/app/services/snack-bar.service';

@Component({
  selector: 'app-starting-balance',
  templateUrl: './starting-balance.component.html',
  styleUrls: ['./starting-balance.component.css']
})
export class StartingBalanceComponent implements OnInit {

  balanceFormGroup: FormGroup;
  balance_caja: balanceCaja;
  medicoId = Number.parseInt(sessionStorage.getItem("medicoId"));
  _balance_inicial: number;
  loading: boolean = false;
  readonlyBalance: boolean = false;
  secretarias: secretaria[];
  disableSetBalance: boolean;
  balanceReadonly: boolean;
  userRole: string;
  currentUserRole$ = this.accountSvc.currentUserRole;

  constructor(
    private balanceCajaSvc: BalanceCajaService,
    private accountSvc: AccountService,
    private _formBuilder: FormBuilder,
    private openSnackBar: SnackBarService
  ) { }



  SetBalance(): void {

    if (this.userRole == 'Doctor') {
      this.balance_caja = {
        medicosID: this.medicoId,
        secretariasID: this.balanceFormGroup.get("secretariaControl").value,
        balance_inicial: this.balanceFormGroup.get("balanceControl").value
      };

      this.disableSetBalance = true;

      this.balanceCajaSvc.SetStartingBalance(this.balance_caja)
        .subscribe(() => {
          this.openSnackBar.open("Guardado correctamente 👌", 0);
        },
          (err) => {
            console.error(err);
            this.openSnackBar.open(err, 1, "Cerrar", false);
          }, () => {
            this.disableSetBalance = false;
          }
        );


    } else if (this.userRole == 'Secretary')

      this.balanceCajaSvc.ConfirmStartingBalance(this.medicoId).subscribe(() => {
        this.openSnackBar.open("Guardado correctamente 👌", 0);
      },
        (err) => {
          console.error(err);
          this.disableSetBalance = false;
          this.openSnackBar.open(err, 1, "Cerrar", false);
        }
      );

  }




  ngOnInit(): void {

    this.balanceFormGroup = this._formBuilder.group({
      balanceControl: ["", Validators.required],
      secretariaControl: ["", Validators.required],
    });

    this.currentUserRole$.subscribe(
      r => {
        this.userRole = r;

        if (this.userRole == 'Doctor') {

          this.accountSvc.GetAllSecretary()
            .subscribe((secretarias: secretaria[]) => {
              this.secretarias = secretarias;
            }, (err) => {
              console.error(err);
              this.openSnackBar.open(err, 1, "Cerrar", false);
            });

          this.balanceFormGroup
            .get("secretariaControl")
            .valueChanges.subscribe((value: number) => {

              this.getStartingBalance(value);

            }, (err) => {
              console.error(err);
              this.openSnackBar.open(err, 1, "Cerrar", false);
            });
        } else if (this.userRole == 'Secretary') {
          this.balanceReadonly = true;
          this.disableSetBalance = false;
          this.getStartingBalance(this.medicoId);
        }
      });

  }


  

  getStartingBalance(secreId: number) {

    this.disableSetBalance = true;

    this.balanceCajaSvc.GetStartingBalance(secreId)
      .subscribe((value: number) => {
        this.balanceFormGroup.get("balanceControl").setValue(value);
      }, (err) => {
        console.error(err);
        this.balanceFormGroup.get("balanceControl").setValue("");
        this.openSnackBar.open(err, 1, "Cerrar", false);
      }, () => {
        this.disableSetBalance = false;
      });
  }

}
