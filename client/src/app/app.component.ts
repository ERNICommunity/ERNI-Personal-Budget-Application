import { Component } from "@angular/core";
import { MsalBroadcastService, MsalService } from "@azure/msal-angular";
import { SharedModule } from "./shared/shared.module";

@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.scss"],
  standalone: true,
  imports: [SharedModule],
})
export class AppComponent {
  constructor(
    private msal: MsalService,
    // !!! DO NOT REMOVE THIS: MsalBroadcastService has to be injected/created, the login won't work otherwise
    private msalBroadcast: MsalBroadcastService
  ) {
    this.msal.handleRedirectObservable().subscribe();
  }
}
