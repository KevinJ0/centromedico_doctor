import { Inject, Injectable } from '@angular/core';
import { HubConnectionBuilder, HubConnection, HubConnectionState } from '@microsoft/signalr';
import { Observable, throwError } from 'rxjs';
import { AccountService } from './account.service';

@Injectable({
  providedIn: 'root'
})

export class SignalrCustomService {
  public hubConnection: HubConnection;
  baseUrl: string;
  accessToken: string = "";
  private joinedGroups: { grupo: string, callback: (data: any) => void, medicoId: number }[] = [];
  private grupoCitaNombre: string = "";
  private grupoCallback: (data: any) => void = () => { };
  private medicoId: number = 0;

  constructor(@Inject("BASE_URL") baseUrl: string) {

    this.baseUrl = baseUrl;

  }

  async Connect(): Promise<void> {

    let medicoId: number = Number.parseInt(sessionStorage.getItem("medicoId") || '');
    this.accessToken = sessionStorage.getItem("jwt") || "";

    let builder = new HubConnectionBuilder();
    this.hubConnection = builder
      .withUrl(this.baseUrl + "notificacion", {
        accessTokenFactory: () => this.accessToken
      })
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: retryContext => {
          return 5000;
        }
      }).build();


    await this.hubConnection
      .start()
      .then(() => {
        this.hubConnection.invoke("JoinCitaGroup", medicoId).then((msj) => {
          // console.log(msj);z
        }).catch(err => {
          console.log(err);
        });

        console.log('Connection started')
      })
      .catch(err => console.error('Error while starting connection: ' + err));


    this.hubConnection.onreconnected(async () => {
      if (this.grupoCitaNombre && this.medicoId && this.grupoCallback) {
        try {
          await this.hubConnection.invoke("JoinCitaGroup", this.medicoId);
          this.hubConnection.on(this.grupoCitaNombre, this.grupoCallback);
        } catch (err) {
          console.error("Error al re-unirse al grupo tras reconectar", err);
        }
      }
    });


  }

  async joinCitaGroup(grupoNombre: string, medicoId: number, callback: (data: any) => void): Promise<void> {
    this.grupoCitaNombre = grupoNombre;
    this.grupoCallback = callback;
    this.medicoId = medicoId;

    await this.hubConnection.invoke("JoinCitaGroup", medicoId);
    this.hubConnection.on(grupoNombre, callback);
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
