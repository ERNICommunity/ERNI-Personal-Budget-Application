import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params} from '@angular/router';
import { Location } from '@angular/common';
import { Request } from '../../model/request/request';
import { RequestService } from '../../services/request.service';
import { FormGroup,  FormBuilder,  Validators } from '@angular/forms';
import { PatchRequest } from '../../model/PatchRequest';

@Component({
  selector: 'app-request-edit',
  templateUrl: 'requestEdit.component.html',
  styleUrls: ['requestEdit.component.css']
})
export class RequestEditComponent implements OnInit {
  requestForm: FormGroup;
  httpResponseError : string;
  dirty: boolean;

  requestId: number;
  
  constructor(private requestService: RequestService,
              private route: ActivatedRoute,
              private location: Location,
              private fb: FormBuilder){
                this.createForm();
               }

  ngOnInit() {
    this.route.params.subscribe((params: Params) => {
      var idParam = params['id']; 
      
      this.getRequest(idParam);
    });
  }

  createForm() {
    this.requestForm = this.fb.group({
       title: ['', Validators.required ],
       amount: ['', Validators.required ],
       date: ['', Validators.required]       
    });
  }

  getRequest(id: number): void {
    this.requestService.getRequest(id)
      .subscribe(request => 
        { 
          this.requestId = id;

          // SET VALUES
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
    var requestId = this.requestId;
    // SAVE
   
    this.requestService.updateRequest({ requestId, title, amount, date } as PatchRequest)
       .subscribe(() => this.goBack(),
       err => {
        this.httpResponseError = err.error
      })
  }
}
