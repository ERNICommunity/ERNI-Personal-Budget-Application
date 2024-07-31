import { ChangeDetectionStrategy, Component } from "@angular/core";
import { SharedModule } from "../shared/shared.module";

@Component({
  selector: "pba-authenticated",
  templateUrl: "./authenticated.component.html",
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [SharedModule],
})
export class AuthenticatedComponent {}
