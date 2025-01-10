import { Injectable } from "@angular/core";
import { MatSnackBar, MatSnackBarConfig } from "@angular/material/snack-bar";

@Injectable({
  providedIn: "root",
})
export class SnackBarService {

  constructor(private _snackBar: MatSnackBar) { }

  open(message: string, type?: number, action: string = "Cerrar", autoClose: boolean = true) {
    const config = new MatSnackBarConfig();
    switch (type) {
      case 0:
        config.panelClass = "background-green";
        break;
      case 1:
        config.panelClass = "background-red";
        break;

      default:
        break;
    }

    if (autoClose)

      config.duration = 3000;

    this._snackBar.open(message, action, config);
  }
}
