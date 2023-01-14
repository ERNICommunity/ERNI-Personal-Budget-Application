import {
    Directive,
    TemplateRef,
    ViewContainerRef,
    Input,
    OnDestroy
} from '@angular/core';
import { BehaviorSubject, combineLatest, Subscription } from 'rxjs';
import { distinctUntilChanged, map } from 'rxjs/operators';
import { AuthenticationService } from '../../services/authentication.service';
import {
    AuthorizationPolicy,
    PolicyNames
} from '../../services/authorization-policy';

@Directive({ selector: '[auth]' })
export class AuthDirective implements OnDestroy {
    subscription: Subscription;

    constructor(
        private templateRef: TemplateRef<any>,
        private viewContainer: ViewContainerRef,
        authService: AuthenticationService
    ) {
        this.subscription = combineLatest([this.policy, authService.userInfo$])
            .pipe(
                distinctUntilChanged(),
                map(([policy, userInfo]) =>
                    !policy
                        ? true
                        : AuthorizationPolicy.evaluate(policy, userInfo)
                ),
                distinctUntilChanged()
            )
            .subscribe((visible) => {
                if (visible) {
                    this.viewContainer.createEmbeddedView(this.templateRef);
                } else {
                    this.viewContainer.clear();
                }
            });
    }

    ngOnDestroy(): void {
        this.subscription.unsubscribe();
    }

    policy = new BehaviorSubject<PolicyNames | null>(undefined);

    @Input() set auth(name: PolicyNames | null | undefined) {
        this.policy.next(name);
    }
}
