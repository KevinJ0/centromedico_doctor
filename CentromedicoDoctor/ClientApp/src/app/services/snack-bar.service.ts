import { Injectable } from "@angular/core";
import { MatSnackBar, MatSnackBarConfig } from "@angular/material/snack-bar";

@Injectable({
  providedIn: "root",
})
export class SnackBarService {
  
  constructor(private _snackBar: MatSnackBar) {}

  open(message: string, type?: number) {
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

    config.duration = 5000;
    this._snackBar.open(message, null, config);
  }
}
