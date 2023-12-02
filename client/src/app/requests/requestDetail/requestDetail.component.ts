import { Component, computed, inject } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { RequestService } from "../../services/request.service";
import { InvoiceImageService } from "../../services/invoice-image.service";
import { distinctUntilChanged, map, shareReplay, switchMap } from "rxjs";
import { mapSuccessfulResult, toResultSignal } from "../../utils/signals";
import { SharedModule } from "../../shared/shared.module";

@Component({
  selector: "app-request-detail",
  templateUrl: "requestDetail.component.html",
  styleUrls: ["requestDetail.component.css"],
  standalone: true,
  imports: [SharedModule],
})
export class RequestDetailComponent {
  route = inject(ActivatedRoute);
  requestService = inject(RequestService);
  router = inject(Router);
  invoiceImageService = inject(InvoiceImageService);

  #requestId$ = this.route.params.pipe(
    map((params) => +params["requestId"]),
    distinctUntilChanged(),
    shareReplay(1)
  );

  request = toResultSignal(
    this.#requestId$.pipe(
      switchMap((requestId) => this.requestService.getRequest(requestId))
    )
  );

  title = computed(() => mapSuccessfulResult(this.request(), (r) => r.title));

  images = toResultSignal(
    this.#requestId$.pipe(
      switchMap((requestId) =>
        this.invoiceImageService.getInvoiceImages(requestId)
      )
    )
  );

  isVisible: boolean = true;

  download(imageId: number) {
    this.invoiceImageService.getInvoiceImage(imageId);
  }

  public onHide(): void {
    this.router.navigate(["../../"], { relativeTo: this.route });
  }

  public close(): void {
    this.router.navigate(["../../"], { relativeTo: this.route });
  }
}
