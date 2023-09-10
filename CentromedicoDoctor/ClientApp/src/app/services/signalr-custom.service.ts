import { Inject, Injectable } from '@angular/core';
import { HubConnectionBuilder, HubConnection, HubConnectionState } from '@microsoft/signalr';
import { Observable, throwError } from 'rxjs';
import { group } from '../interfaces/InterfacesDto';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})
export class SignalrCustomService {
  public hubConnection: HubConnection;
  baseUrl: string;
  accessToken: string = "";

  constructor(@Inject("BASE_URL") baseUrl: string) {

    this.baseUrl = baseUrl;

  }

  async Connect(): Promise<void> {

    this.accessToken = sessionStorage.getItem("jwt") || "";

    let builder = new HubConnectionBuilder();
    this.hubConnection = builder
      .withUrl(this.baseUrl + "citas", {
        accessTokenFactory: () => this.accessToken
      })
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: retryContext => {
          if (retryContext.elapsedMilliseconds < 60000) {
            // If we've been reconnecting for less than 60 seconds so far,
            // wait between 0 and 10 seconds before the next reconnect attempt.
            return 5000;
          } else {
            // If we've been reconnecting for more than 60 seconds so far, stop reconnecting.
            return null;
          }
        }
      }).build();

    let medicoId: number = Number.parseInt(sessionStorage.getItem("medicoId") || '');

    await this.hubConnection
      .start()
      .then(() => {

        this.hubConnection.invoke("JoinGroups", medicoId).then((msj) => {
          // console.log(msj);
        })
          .catch(err => {
            console.log(err);
          });

        console.log('Connection started')
      })
      .catch(err => console.error('Error while starting connection: ' + err));


  }


  async Disconnect(): Promise<void> {

    if (this.hubConnection) {
      let group: Array<any>;
      let groupsValue = JSON.parse(sessionStorage.getItem("groups"));
      group = Object.entries(groupsValue).map((k, v) => {
        return k[1];
      });


      await this.hubConnection.invoke("LeaveGroups", group).then(async (msj) => {

        //set all connection groups off
        let groups = JSON.parse(sessionStorage.getItem("groups"));
        let groupCitaName: string = groups?.CitasNotificacion;

        if (groupCitaName)
          this.hubConnection.off(groupCitaName);

        //stop the connection
        await this.hubConnection.stop();
        console.log("se ha desconectado del hub")

      }).catch(err => {
        throwError(() => new Error(err));
      });
    }
  }
}
