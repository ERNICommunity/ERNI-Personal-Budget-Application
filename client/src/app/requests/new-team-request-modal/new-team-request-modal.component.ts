import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import { TeamRequestAddComponent } from '../team-request-add/team-request-add.component';

@Component({
    selector: 'app-new-team-request-modal',
    template: ''
})
export class NewTeamRequestModalComponent implements OnDestroy {
    destroy = new Subject<any>();
    currentDialog = null;

    constructor(
        private modalService: NgbModal,
        route: ActivatedRoute,
        router: Router) {
        route.params.pipe(takeUntil(this.destroy)).subscribe(params => {

            // When router navigates on this component is takes the params and opens up the photo detail modal
            this.currentDialog = this.modalService.open(TeamRequestAddComponent, { centered: true });
            this.currentDialog.componentInstance.budgetId = params.budgetId;

            // Go back to home page after the modal is closed
            this.currentDialog.result.then(result => {
                //router.navigateByUrl('/');
                router.navigate(['.'], { relativeTo: route.parent });
            }, reason => {
                //router.navigateByUrl('/');
                router.navigate(['.'], { relativeTo: route.parent });
            });
        });
    }

    ngOnDestroy() {
        this.destroy.next();
    }
}
