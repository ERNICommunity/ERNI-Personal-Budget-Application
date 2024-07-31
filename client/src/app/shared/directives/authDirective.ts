import {
  Directive,
  TemplateRef,
  ViewContainerRef,
  Input,
} from "@angular/core";
import { BehaviorSubject, combineLatest } from "rxjs";
import { distinctUntilChanged, map } from "rxjs/operators";
import { AuthenticationService } from "../../services/authentication.service";
import {
  AuthorizationPolicy,
  PolicyNames,
} from "../../services/authorization-policy";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";

@Directive({ selector: "[pbaAuth]" })
export class AuthDirective {
  constructor(
    private templateRef: TemplateRef<unknown>,
    private viewContainer: ViewContainerRef,
    private authService: AuthenticationService,
  ) {
    combineLatest([this.policy, this.authService.userInfo$])
      .pipe(
        distinctUntilChanged(),
        map(([policy, userInfo]) =>
          !policy ? true : AuthorizationPolicy.evaluate(policy, userInfo)
        ),
        distinctUntilChanged(),
        takeUntilDestroyed()
      )
      .subscribe((visible) => {
        if (visible) {
          this.viewContainer.createEmbeddedView(this.templateRef);
        } else {
          this.viewContainer.clear();
        }
      });
  }

  policy = new BehaviorSubject<PolicyNames | null | undefined>(undefined);

  @Input() set pbaAuth(name: PolicyNames | null | undefined) {
    this.policy.next(name);
  }
}
