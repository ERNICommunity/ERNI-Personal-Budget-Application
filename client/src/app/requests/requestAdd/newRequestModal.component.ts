import { Component, OnDestroy, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

import { ActivatedRoute, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { RequestDetailComponent } from '../../requests/requestDetail/requestDetail.component';
import { RequestAddComponent } from './requestAdd.component';

@Component({
    selector: 'app-modal-container',
    template: ''
})
export class NewRequestModalComponent implements OnDestroy {
    destroy = new Subject<any>();
    currentDialog = null;

    constructor(
        private modalService: NgbModal,
        route: ActivatedRoute,
        router: Router
    ) {
        route.params.pipe(takeUntil(this.destroy)).subscribe(params => {
            let data = router.getCurrentNavigation();
            if (data == null || data.extras.state === undefined) {
                router.navigate(['.'], { relativeTo: route.parent });
                return;
            }

            // When router navigates on this component is takes the params and opens up the photo detail modal
            this.currentDialog = this.modalService.open(RequestAddComponent, { centered: true });
            this.currentDialog.componentInstance.budgetId = params.budgetId;
            this.currentDialog.componentInstance.budgetType = data.extras.state.budget.type;

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