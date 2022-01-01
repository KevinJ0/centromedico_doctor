import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from 'rxjs';

import { take, map } from 'rxjs/operators';
import { AccountService } from '../services/account.service';


@Injectable({
  providedIn: 'root'
})
export class AuthGuardService implements CanActivate {

  constructor(private acct: AccountService, private router: Router) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.acct.isLoggesIn.pipe(take(1), map((loginStatus: boolean) => {
      const destination: string = state.url;
      // To check if user is not logged in
      
      if (!loginStatus) {
        this.router.navigate(['login'], { queryParams: { returnUrl: state.url } });
        return false;
      }
      console.log(destination)
      // if the user is already logged in

      if (destination.includes('/resultados')) {
        if (localStorage.getItem("userRole") === "Manager") {
          this.router.navigate(['/access-denied'])
          return false;
        }
      }
      else if (destination.includes('doctor/dashboard') &&localStorage.getItem("userRole") === "Secretary" || localStorage.getItem("userRole") === "Doctor")
        return true;
        else if (destination.includes('doctor/cita') &&localStorage.getItem("userRole") === "Secretary" || localStorage.getItem("userRole") === "Doctor")
        return true;
      else if (destination.includes('doctor/citas') && localStorage.getItem("userRole") === "Doctor")
        return true;
      else if (destination.includes('doctor/reportes') && localStorage.getItem("userRole") === "Doctor")
        return true;
      else if (destination.includes('doctor/configuracion') && localStorage.getItem("userRole") === "Doctor")
        return true;
      else
        return false;
    }));
  }
}
