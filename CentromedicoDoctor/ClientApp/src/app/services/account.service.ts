import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { BehaviorSubject, throwError, of, Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { SignalrCustomService } from './signalr-custom.service';
import { GrupoService } from './grupo.service';
import { CustomError, group, medico, ResetPassword, secretaria, TokenResponse, UserInfo } from '../interfaces/InterfacesDto';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl: string;
  private baseUrlToken: string = "api/token/auth";

  // User related properties
  private loginStatus = new BehaviorSubject<boolean>(this.CheckLoginStatus());
  private UserName = new BehaviorSubject<string>(sessionStorage.getItem('userName'));
  private UserRole = new BehaviorSubject<string>(sessionStorage.getItem('userRole'));
  public groups: group[] = [];


  constructor(
    private signalR: SignalrCustomService,
    private gruposSvc: GrupoService,
    private router: Router,
    private http: HttpClient,
    @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }


  SetMedico(id: number): void {

    sessionStorage.setItem("medicoId", String(id));

    this.gruposSvc.GetGrupoList(id).subscribe((r: group[]) => {

      sessionStorage.setItem('groups', JSON.stringify(r)); // guardo la lista de los grupos para las notificaciones con signalr

      this.router.navigate(['app/dashboard']);

    });
  }

  GetNewRefreshToken(): Observable<TokenResponse> {

    let userCredential = sessionStorage.getItem('userName');
    let refreshToken = sessionStorage.getItem('refreshToken');
    const grantType = "refresh_token";


    return this.http.post<TokenResponse>(this.baseUrl + this.baseUrlToken, { userCredential, refreshToken, grantType }).pipe(
      map((result: TokenResponse) => {
        console.log(result)

        if (result && result.token) {
          this.SetUserResult(result)
        }

        return <TokenResponse>result;

      }),
      catchError((err: CustomError) => {
        return throwError(err.message);
      })
    );
  }


  //Login Method
  Login(userCredential: string, password: string): Observable<TokenResponse | any> {
    const grantType = "password";

    return this.http.post<TokenResponse>(this.baseUrl + this.baseUrlToken, { userCredential, password, grantType })
      .pipe(
        map((result: TokenResponse) => {

          // login successful if there's a jwt token in the response
          if (result && result.token) {
            // store user details and jwt token in local storage to keep user logged in between page refreshes
            this.SetUserResult(result);
            return result;

          } throwError("No se ha provisto del token de seguridad.");
          return null;

          console.log(result);

        }),
        catchError(err => throwError(err))
      );
  }


  SetUserResult(result: TokenResponse): void {
    this.loginStatus.next(true);


    sessionStorage.setItem('loginStatus', '1');
    sessionStorage.setItem('jwt', result.token);
    sessionStorage.setItem('userName', result.username);
    sessionStorage.setItem('expiration', result.expiration);
    sessionStorage.setItem('userRole', result.roles);
    sessionStorage.setItem('refreshToken', result.refresh_token);

    this.UserName.next(result.username);
    this.UserRole.next(result.roles);

  }

  CheckLoginStatus(): boolean {

    var loginCookie = sessionStorage.getItem("loginStatus");

    if (loginCookie == "1") {
      if (sessionStorage.getItem('jwt') != null || sessionStorage.getItem('jwt') != undefined) {
        return true;
      }
    }
    return false;
  }

  isUserDocIdentConfirm(): Observable<boolean> {
    return this.http.get<boolean>(this.baseUrl + "api/account/isUserDocIdentConfirm")
      .pipe(map((result: boolean) => {

        return result;

      }));
  }

  GetUserInfo(): Observable<medico> {

    try {

      return this.http.get<medico>(this.baseUrl +
        `api/account/getUserInfo`)
        .pipe(map(result => {
          console.log(result)
          return result;
        }), catchError(err => {
          console.log('Ha ocurrido un error al tratar de obtener los datos del médico', err);
          return of();
        }));

    } catch (error) {
      console.log('Ha ocurrido un error al tratar de obtener los datos del médico', error);
      return of();
    }

  }

  SaveUserInfo(userInfo: any): Observable<boolean> {

    return this.http.post<boolean>(this.baseUrl + "api/account/saveUserInfo", userInfo,
      /*{
        headers: {  ['Content-Type']: 'multipart/form-data' }
      }*/)
      .pipe(
        map(() => true),
        catchError(err => {
          return throwError(() => new Error(err));
        })
      );
  }

  ChangePassword(_resetPassword: ResetPassword): Observable<boolean> {

    return this.http.put<boolean>(this.baseUrl + "api/account/changePassword", _resetPassword,
      /*{
        headers: {  ['Content-Type']: 'multipart/form-data' }
      }*/)
      .pipe(
        map((r) => r),
        catchError((err: CustomError) => {
          return throwError(() => new Error(err.message));
        })
      );
  }

  SetUserInfo(userInfo: UserInfo): Observable<boolean> {
    return this.http.post<boolean>(this.baseUrl + "api/account/setUserInfo", userInfo)
      .pipe(
        map(() => true),
        catchError(err => {
          return throwError(() => new Error(err));
        })
      );
  }


  async Logout(): Promise<void> {
    // Set Loginstatus to false and delete saved jwt cookie
    await this.signalR.Disconnect().then(async () => {
      //   this.signalR.hubConnection;
      this.loginStatus.next(false);
      this.UserName.next(null);
      this.UserRole.next(null);

      sessionStorage.removeItem('jwt');
      sessionStorage.removeItem('refreshToken');
      sessionStorage.removeItem('userRole');
      sessionStorage.removeItem('userName');
      sessionStorage.removeItem('expiration');
      sessionStorage.removeItem('groups');
      sessionStorage.removeItem('medicoId');
      sessionStorage.removeItem('medicoId');
      sessionStorage.setItem('loginStatus', '0');
      console.log("Logged Out Successfully");
    });

  }

  GetStartingBalance(medicoId: number): Observable<number> {

    return this.http.get<number>(this.baseUrl + `api/account/getStartingBalance?medicoId=${medicoId}`)
      .pipe(
        map((r: number) => r),
        catchError((err: CustomError) => {
          return throwError(() => new Error(err.message));
        })
      );
  }

  SetStartingBalance(medicoId: number, balance: number): Observable<boolean> {

    return this.http.post<boolean>(this.baseUrl + `api/account/setStartingBalance`,
      { "medicoId": medicoId, "balance": balance })
      .pipe(
        map((r: boolean) => r),
        catchError((err: CustomError) => {
          return throwError(() => new Error(err.message));
        })
      );

  }


  GetAllSecretary(): Observable<secretaria[]> {
    return this.http.get<secretaria[]>(this.baseUrl + `api/account/getAllSecretary`)
      .pipe(
        map((secretarias: secretaria[]) => secretarias),
        catchError((err: CustomError) => {
          return throwError(err.message);
        })
      );
  }


  setUserInfo(userInfo: UserInfo): Observable<boolean> {
    return this.http.post<boolean>(this.baseUrl + "api/account/setUserInfo", userInfo)
      .pipe(
        map(() => true),
        catchError(err => {
          return throwError(err);
        })
      );
  }

  getUserInfo(): Observable<UserInfo> {
    return this.http.get<UserInfo>(this.baseUrl + "api/account/getUserInfo")
      .pipe(map((data: UserInfo) => data),
        catchError(err => {
          return throwError(err);
        })
      );

  }

  isStartingBalanceConfirmed(): Observable<boolean> {
    return this.http.get<boolean>(this.baseUrl + `api/account/isStartingBalanceConfirmed?medicoId=1`)
      .pipe(map((data: boolean) => data),
        catchError(err => {
          return throwError(err);
        })
      );
  }

  get isLoggesIn() {
    if (sessionStorage.getItem("loginStatus"))
      this.loginStatus.next((sessionStorage.getItem("loginStatus").toLowerCase() == '1'));
    console.log(sessionStorage.getItem("loginStatus"));
    return this.loginStatus.asObservable();
  }

  get currentUserName() {
    this.UserName.next((sessionStorage.getItem("userName")));
    return this.UserName.asObservable();
  }

  get currentUserRole() {
    this.UserRole.next((sessionStorage.getItem("userRole")));
    return this.UserRole.asObservable();
  }

  get GetMedicoId() {
    return sessionStorage.getItem("medicoId");
  }
}
