import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from 'src/app/services/account.service';
import { AutoUnsubscribe } from "ngx-auto-unsubscribe";

@AutoUnsubscribe()
@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {

  userName: string;
  userRole: string;
  
  isExpanded = false;
  currentUserName$ = this.accountSvc.currentUserName;
  currentUserRole$ = this.accountSvc.currentUserRole;


  constructor(
    private accountSvc: AccountService,
    private router: Router) { }

    ngOnInit(): void {
      this.currentUserName$.subscribe(r => {
        this.userName = r; 
      });
      this.currentUserRole$.subscribe(r => {
        this.userRole = r; 
      });
    }
  logOut() {
    this.accountSvc.logout();
    console.log("logout")
      this.router.navigate(['/']);
  
  }

  ngOnDestroy() {
    // We'll throw an error if it doesn't
  }
}
