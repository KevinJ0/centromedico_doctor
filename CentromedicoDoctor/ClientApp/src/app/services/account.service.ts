import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { UserInfo, TokenResponse } from '../interfaces/InterfacesDto';
import { BehaviorSubject, throwError, of, Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl: string;

  // Url to access to the Web API
  // Token Controller
  private baseUrlRegister: string = "api/token/register";
  private baseUrlToken: string = "api/token/auth";


  // User related properties
  private loginStatus = new BehaviorSubject<boolean>(this.checkLoginStatus());
  private UserName = new BehaviorSubject<string>(localStorage.getItem('userName'));
  private UserRole = new BehaviorSubject<string>(localStorage.getItem('userRoles'));

  constructor(private router: Router, private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  getNewRefreshToken(): Observable<TokenResponse> {

    let usercredential = localStorage.getItem('userName');
    let refreshToken = localStorage.getItem('refreshToken');
    const grantType = "refresh_token";


    return this.http.post<TokenResponse>(this.baseUrl + this.baseUrlToken, { usercredential, refreshToken, grantType }).pipe(
      map((result: TokenResponse) => {
        if (result && result.authToken.token) {
          this.loginStatus.next(true);
          localStorage.setItem('loginStatus', '1');
          localStorage.setItem('jwt', result.authToken.token);
          localStorage.setItem('userName', result.authToken.username);
          localStorage.setItem('expiration', result.authToken.expiration);
          localStorage.setItem('userRole', result.authToken.roles);
          localStorage.setItem('refreshToken', result.authToken.refresh_token);
        }

        return <TokenResponse>result;

      })
    );

  }


  //Login Method
  Login(userCredential: string, password: string) {
    const grantType = "password";

    return this.http.post<TokenResponse>(this.baseUrl + this.baseUrlToken, { userCredential, password, grantType }).pipe(
      map((result: TokenResponse) => {

        // login successful if there's a jwt token in the response
        if (result && result.authToken.token) {
          // store user details and jwt token in local storage to keep user logged in between page refreshes
          this.setUserResult(result);
        }
        console.log(result);
        return result;

      })

    );
  }

  //Signup Method
  Signup(email: string, password: string) {
    return this.http.post<TokenResponse>(this.baseUrl + this.baseUrlRegister, { email, password }).pipe(
      map((result: TokenResponse) => {

        // login successful if there's a jwt token in the response
        if (result && result.authToken.token) {
          // store user details and jwt token in local storage to keep user logged in between page refreshes
          this.setUserResult(result);
        }
        console.log(result);
        return result;

      })

    );
  }

  setUserResult(result: TokenResponse): void {
    this.loginStatus.next(true);
    localStorage.setItem('loginStatus', '1');
    localStorage.setItem('jwt', result.authToken.token);
    localStorage.setItem('userName', result.authToken.username);
    localStorage.setItem('expiration', result.authToken.expiration);
    localStorage.setItem('userRole', result.authToken.roles);
    localStorage.setItem('refreshToken', result.authToken.refresh_token);
    this.UserName.next(result.authToken.username);
    this.UserRole.next(result.authToken.roles);

  }
  checkLoginStatus(): boolean {

    var loginCookie = localStorage.getItem("loginStatus");

    if (loginCookie == "1") {
      if (localStorage.getItem('jwt') != null || localStorage.getItem('jwt') != undefined) {
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
          return throwError(err);
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


  logout() {
    // Set Loginstatus to false and delete saved jwt cookie
    this.loginStatus.next(false);
    this.UserName.next(null);
    this.UserRole.next(null);

    localStorage.removeItem('jwt');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('userRole');
    localStorage.removeItem('userName');
    localStorage.removeItem('expiration');
    localStorage.setItem('loginStatus', '0');
    console.log("Logged Out Successfully");

  }


  get isLoggesIn() {
    if(localStorage.getItem("loginStatus"))
    this.loginStatus.next((localStorage.getItem("loginStatus").toLowerCase() == '1'));
    console.log(localStorage.getItem("loginStatus"));
    return this.loginStatus.asObservable();
  }

  get currentUserName() {
    this.UserName.next((localStorage.getItem("userName")));
    return this.UserName.asObservable();
  }

  get currentUserRole() {
    this.UserRole.next((localStorage.getItem("UserRole")));
    return this.UserRole.asObservable();
  }

}
