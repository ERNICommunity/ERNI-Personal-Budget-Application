import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Request } from '../../model/request/request';
import { RequestService } from '../../services/request.service';
import { UntypedFormGroup } from '@angular/forms';
import { InvoiceImageService } from '../../services/invoice-image.service';

@Component({
    selector: 'app-request-detail',
    templateUrl: 'requestDetail.component.html',
    styleUrls: ['requestDetail.component.css']
})
export class RequestDetailComponent implements OnInit {
    @ViewChild('downloadLink', { static: false }) downloadLink: ElementRef;
    request: Request;
    createDate: Date;
    requestForm: UntypedFormGroup;
    httpResponseError: string;
    images: { id: number; name: string}[];
    isVisible: boolean;

    constructor(
        private requestService: RequestService,
        private route: ActivatedRoute,
        private router: Router,
        private invoiceImageService: InvoiceImageService
    ) {
        this.isVisible = true;
    }

    ngOnInit() {
        this.route.params.subscribe((params) => {
            this.getRequest(params['requestId']);
        });
    }

    download(imageId: number) {
        this.invoiceImageService.getInvoiceImage(imageId);
    }

    public getRequest(id: number): void {
        this.requestService.getRequest(id).subscribe(
            (request) => {
                this.request = request;
                this.createDate = new Date(request.createDate);
                this.invoiceImageService
                    .getInvoiceImages(id)
                    .subscribe((names) => {
                        this.images = names;
                    });
            },
            (err) => {
                this.httpResponseError = err.error;
            }
        );
    }

    public onHide(): void {
        this.router.navigate(['../../'], { relativeTo: this.route });
    }

    public close(): void {
        this.router.navigate(['../../'], { relativeTo: this.route });
    }
}
