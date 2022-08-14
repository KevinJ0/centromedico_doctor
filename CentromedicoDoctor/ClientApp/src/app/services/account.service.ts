import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { UserInfo, group, TokenResponse, CustomError } from '../interfaces/InterfacesDto';
import { BehaviorSubject, throwError, of, Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { SignalrCustomService } from './signalr-custom.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  // Url to access to the Web API
  baseUrl: string;
  // Token Controller
  private baseUrlToken: string = "api/token/auth";

  // User related properties
  private loginStatus = new BehaviorSubject<boolean>(this.checkLoginStatus());
  private UserName = new BehaviorSubject<string>(sessionStorage.getItem('userName'));
  private UserRole = new BehaviorSubject<string>(sessionStorage.getItem('userRole'));
  public groups: group[] = [];


  constructor(
    private signalR: SignalrCustomService,
    private router: Router, private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  getNewRefreshToken(): Observable<TokenResponse> {

    let userCredential = sessionStorage.getItem('userName');
    let refreshToken = sessionStorage.getItem('refreshToken');
    const grantType = "refresh_token";


    return this.http.post<TokenResponse>(this.baseUrl + this.baseUrlToken, { userCredential, refreshToken, grantType }).pipe(
      map((result: TokenResponse) => {
        console.log(result)

        if (result && result.token) {
          this.setUserResult(result)
        }

        return <TokenResponse>result;

      }),
      catchError((err: CustomError) => {
        return throwError(err.message);
      })
    );
  }


  //Login Method
  Login(userCredential: string, password: string): Observable<TokenResponse> {
    const grantType = "password";

    return this.http.post<TokenResponse>(this.baseUrl + this.baseUrlToken, { userCredential, password, grantType })
      .pipe(
        map((result: TokenResponse) => {

          // login successful if there's a jwt token in the response
          if (result && result.token) {
            // store user details and jwt token in local storage to keep user logged in between page refreshes
            this.setUserResult(result);
            return result;

          } else {
            throwError("No se ha provisto del token de seguridad.");
          }

          console.log(result);

        }),
        catchError(err => throwError(err))
      );
  }


  setUserResult(result: TokenResponse): void {
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

  checkLoginStatus(): boolean {

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

  getUserInfo(): Observable<UserInfo> {
    return this.http.get<UserInfo>(this.baseUrl + "api/account/getUserInfo")
      .pipe(map((data: UserInfo) => data),
        catchError(err => {
          return throwError(() => new Error(err));
        })
      );

  }


  setUserInfo(userInfo: UserInfo): Observable<boolean> {
    return this.http.post<boolean>(this.baseUrl + "api/account/setUserInfo", userInfo)
      .pipe(
        map(() => true),
        catchError(err => {
          return throwError(() => new Error(err));
        })
      );
  }


  async logout(): Promise<void> {
    // Set Loginstatus to false and delete saved jwt cookie
    await this.signalR.Disconnect().then(async() => {
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
      sessionStorage.setItem('loginStatus', '0');
      console.log("Logged Out Successfully");
    });

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

}
