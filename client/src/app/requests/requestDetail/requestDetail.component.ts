import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params} from '@angular/router';
import { Location } from '@angular/common';
import { Request } from '../../model/request/request';
import { RequestService } from '../../services/request.service';
import { FormGroup,  FormBuilder,  Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-request-detail',
  templateUrl: 'requestDetail.component.html',
  styleUrls: ['requestDetail.component.css']
})
export class RequestDetailComponent implements OnInit {
  request: Request;
  selectedDate : Date;
  requestForm: FormGroup;
  httpResponseError : string;
  
  constructor(private requestService: RequestService,
              private route: ActivatedRoute,
              private location: Location,
              public modal: NgbActiveModal){
               }

  ngOnInit() {
    // this.route.params.subscribe((params: Params) => {
    //   var idParam = params['requestId']; 
      
    //   this.getRequest(idParam);
    // });
  }

  public getRequest(id: number): void {
    this.requestService.getRequest(id)
      .subscribe(request => 
        { 
          this.request = request;
          this.selectedDate = new Date(request.date);
        },err => {
          this.httpResponseError = err.error
        });
    }
 
  goBack(): void {
    this.location.back();
  }
}
