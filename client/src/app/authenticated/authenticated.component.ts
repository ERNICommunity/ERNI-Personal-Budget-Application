import { Component } from "@angular/core";
import { Router } from "@angular/router";
import { AuthenticationService } from "../services/authentication.service";
import { SharedModule } from "../shared/shared.module";

@Component({
  selector: "app-authenticated",
  templateUrl: "./authenticated.component.html",
  styleUrls: ["./authenticated.component.scss"],
  standalone: true,
  imports: [SharedModule],
})
export class AuthenticatedComponent {
  constructor(
    public authService: AuthenticationService,
    public router: Router
  ) {}

  async logout(): Promise<void> {
    await this.authService.logout();
    this.router.navigate(["/login"]);
  }
}
