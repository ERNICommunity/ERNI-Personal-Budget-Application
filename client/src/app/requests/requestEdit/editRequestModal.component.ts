import { Component, OnDestroy } from '@angular/core';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';

import { ActivatedRoute, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { RequestEditComponent } from './requestEdit.component';

@Component({
    selector: 'app-modal-container',
    template: ''
})
export class EditRequestModalComponent implements OnDestroy {
    destroy = new Subject<any>();
    currentDialog: NgbModalRef = null;
    currentDialogInstance: RequestEditComponent = null;

    constructor(
        private modalService: NgbModal,
        route: ActivatedRoute,
        router: Router
    ) {
        route.params.pipe(takeUntil(this.destroy)).subscribe(params => {

            let data = router.getCurrentNavigation();
            this.currentDialog = this.modalService.open(RequestEditComponent, { centered: true });
            this.currentDialogInstance = this.currentDialog.componentInstance;

            // Go back to home page after the modal is closed
            this.currentDialog.result.then(_ => {
                router.navigate(['.'], { relativeTo: route.parent });
            }, _ => {
                router.navigate(['.'], { relativeTo: route.parent });
            });
            
            if (params.state == 'create') {
                if (data == null || data.extras.state === undefined) {
                    router.navigate(['.'], { relativeTo: route.parent });
                    return;
                }

                this.currentDialogInstance.openRequest(Number(params.id), Number(data.extras.state.budget.type));
                return;
            }

            if (params.state == 'pending') {
                this.currentDialogInstance.openPending(Number(params.id));
                return;
            }

            if (params.state == 'invoice') {
                this.currentDialogInstance.openInvoice(Number(params.id));
                return;
            }

            if (params.state == 'closed') {
                this.currentDialogInstance.openClosed(Number(params.id));
            }
            
        });
    }

    ngOnDestroy() {
        this.destroy.next();
    }
}