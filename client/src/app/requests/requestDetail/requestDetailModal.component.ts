import { Component, OnDestroy } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

import { ActivatedRoute, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { RequestDetailComponent } from './requestDetail.component';

@Component({
    selector: 'app-modal-container',
    template: ''
})
export class RequestDetailModalComponent implements OnDestroy {
    destroy = new Subject<any>();
    currentDialog = null;

    constructor(
        private modalService: NgbModal,
        route: ActivatedRoute,
        router: Router
    ) {
        route.params.pipe(takeUntil(this.destroy)).subscribe(params => {

            // When router navigates on this component is takes the params and opens up the photo detail modal
            this.currentDialog = this.modalService.open(RequestDetailComponent, { centered: true });
            this.currentDialog.componentInstance.requestType = params.type;
            this.currentDialog.componentInstance.getRequest(params['requestId']);

            // Go back to home page after the modal is closed
            this.currentDialog.result.then(result => {
                //router.navigateByUrl('/');
                //router.navigate(["../../parent"], {relativeTo: route, skipLocationChange: false});
                router.navigate(['.'], { relativeTo: route.parent });
            }, reason => {
                //router.navigateByUrl('/');
                //router.navigate(["../../parent"], {relativeTo: route, skipLocationChange: false});
                router.navigate(['.'], { relativeTo: route.parent });
            });
        });
    }

    ngOnDestroy() {
        this.destroy.next();
    }
}