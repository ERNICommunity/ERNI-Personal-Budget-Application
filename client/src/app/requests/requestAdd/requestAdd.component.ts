import { Component, OnInit, Input } from '@angular/core';
import { CategoryService } from '../../services/category.service';
import { RequestService } from '../../services/request.service';
import { Category } from '../../model/category';
import { Request } from '../../model/request/request';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { BusyIndicatorService } from '../../services/busy-indicator.service';
import { Budget } from '../../model/budget';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AlertService } from '../../services/alert.service';
import { NewRequest } from '../../model/newRequest';
import { Alert, AlertType } from '../../model/alert.model';


@Component({
  selector: 'app-request-add',
  templateUrl: './requestAdd.component.html',
  styleUrls: ['./requestAdd.component.css']
})
export class RequestAddComponent implements OnInit {
  @Input() budget: Budget;
  categories: Category[];
  httpResponseError: string;
  requestForm: FormGroup;
  dirty: boolean;

  budgetId: number;

  constructor(
    public modal: NgbActiveModal,
    private categoryService: CategoryService,
    private requestService: RequestService,
    private fb: FormBuilder,
    private busyIndicatorService: BusyIndicatorService,
    private alertService: AlertService) {
    this.createForm();
  }

  ngOnInit() {
    this.onChanges();
    this.getCategories();
  }

  createForm() {
    this.requestForm = this.fb.group({
      title: ['', Validators.required],
      amount: ['', Validators.required],
      category: ['', Validators.required],
      date: ['', Validators.required],
      url: ['']//, Validators.required]
    });
  }

  onChanges() {
    this.requestForm.get('category').valueChanges
      .subscribe(selectedCategory => {
        if (selectedCategory.isUrlNeeded) {
          this.requestForm.get('url').enable();
        }
        else {
          this.requestForm.get('url').disable();
          this.requestForm.get('url').reset();
        }
      });
  }

  getCategories(): void {

    this.busyIndicatorService.start();
    this.categoryService.getCategories()
      .subscribe(categories => {
        this.categories = categories.filter(cat => cat.isActive == true);
          this.requestForm.patchValue({ category: categories.filter(cat => cat.isActive == true)[0] });
        this.busyIndicatorService.end();
      });
  }

  isDirty(): boolean {
    return this.dirty;
  }

  save(): void {
    var budgetId = this.budgetId;
    var title = this.requestForm.get("title").value;
    var amount = this.requestForm.get("amount").value;
    var categoryId = this.requestForm.get("category").value;
    var ngbDate = this.requestForm.get("date").value;

    var date = new Date(ngbDate.year, ngbDate.month - 1, ngbDate.day);

    var url = "";// this.requestForm.get("url").value;
    this.busyIndicatorService.start();

    this.requestService.addRequest({ budgetId, title, amount, date, categoryId, url } as NewRequest)
      .subscribe(() => {
        this.busyIndicatorService.end();
        this.modal.close();
        this.alertService.alert(new Alert({ message: "Request created successfully", type: AlertType.Success, keepAfterRouteChange: true }));
      },
        err => {
          this.busyIndicatorService.end();
          this.alertService.error("Error while creating request: " + JSON.stringify(err.error), "addRequestError");
        });
  }
}
