import { Component } from '@angular/core';
import { MatIconRegistry } from '@angular/material/icon';
import { DomSanitizer } from '@angular/platform-browser';
import * as moment from 'moment';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
  title = 'app';
  constructor(
    private domSanitizer: DomSanitizer,
    private matIconRegistry: MatIconRegistry) {
    moment.locale('es');

    this.matIconRegistry.addSvgIcon(
      "pay",
      this.domSanitizer.bypassSecurityTrustResourceUrl("./assets/icons/pay.svg")
    ).addSvgIcon(
      "insurance",
      this.domSanitizer.bypassSecurityTrustResourceUrl("./assets/icons/insurance.svg")
    ).addSvgIcon(
      "user-heart",
      this.domSanitizer.bypassSecurityTrustResourceUrl("./assets/icons/user-heart.svg")
    ).addSvgIcon(
      "member-card",
      this.domSanitizer.bypassSecurityTrustResourceUrl("./assets/icons/member-card.svg")
    ).addSvgIcon(
      "sum",
      this.domSanitizer.bypassSecurityTrustResourceUrl("./assets/icons/sum.svg")
    ).addSvgIcon(
      "low-price",
      this.domSanitizer.bypassSecurityTrustResourceUrl("./assets/icons/low-price.svg")
    ).addSvgIcon(
      "discount",
      this.domSanitizer.bypassSecurityTrustResourceUrl("./assets/icons/discount.svg")
    ).addSvgIcon(
      "date",
      this.domSanitizer.bypassSecurityTrustResourceUrl("./assets/icons/date.svg")
    ).addSvgIcon(
      "turn",
      this.domSanitizer.bypassSecurityTrustResourceUrl("./assets/icons/turn.svg")
    ).addSvgIcon(
      "id_appointment",
      this.domSanitizer.bypassSecurityTrustResourceUrl("./assets/icons/sharp.svg")
    );
  }
}
