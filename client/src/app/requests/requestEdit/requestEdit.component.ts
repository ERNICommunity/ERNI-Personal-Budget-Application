import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params} from '@angular/router';
import { Location } from '@angular/common';
import { Request } from '../../model/request/request';
import { RequestService } from '../../services/request.service';
import { FormGroup,  FormBuilder,  Validators } from '@angular/forms';
import { PatchRequest } from '../../model/PatchRequest';
import { NgbDateStruct, NgbDate, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { AlertService } from '../../services/alert.service';
import { Alert, AlertType } from '../../model/alert.model';

@Component({
  selector: 'app-request-edit',
  templateUrl: 'requestEdit.component.html',
  styleUrls: ['requestEdit.component.css']
})
export class RequestEditComponent {
  requestForm: FormGroup;
  httpResponseError : string;
  dirty: boolean;

  requestId: number;
  
  constructor(private requestService: RequestService,
              public modal: NgbActiveModal,
              private location: Location,
              private fb: FormBuilder,
              private alertService: AlertService){
                this.createForm();
               }

  createForm() {
    this.requestForm = this.fb.group({
       title: ['', Validators.required ],
       amount: ['', Validators.required ],
       date: ['', Validators.required]       
    });
  }

  public showRequest(id: number): void {
    this.requestService.getRequest(id)
      .subscribe(request => 
        { 
          this.requestId = id;

          var date = new Date(request.date);

          var ngbDate = new NgbDate(date.getFullYear(), date.getMonth() + 1, date.getDate());  

          this.requestForm.setValue({
            title: request.title,
            amount: request.amount,
            date: ngbDate
          });
        },err => {
          this.httpResponseError = err.error
        });
    }
 
    setDirty(): void{
      this.dirty = true;
    }
  
    isDirty(): boolean{
      return this.dirty;
    }

  goBack(): void {
    this.location.back();
  }

  save() : void {
    var title = this.requestForm.get("title").value;
    var amount = this.requestForm.get("amount").value;
    var ngbDate = this.requestForm.get("date").value;
    var date = new Date(ngbDate.year, ngbDate.month - 1, ngbDate.day);
    var id = this.requestId;
    // SAVE
   
    this.requestService.updateRequest({ id, title, amount, date } as PatchRequest)
       .subscribe(() => {
        this.alertService.alert(new Alert({ message: "Request updated", type: AlertType.Success, keepAfterRouteChange: true }));
        this.modal.close();
       },
       err => {
        this.alertService.error("Error while creating request: " + JSON.stringify(err.error), "addRequestError");
      })
  }
}
