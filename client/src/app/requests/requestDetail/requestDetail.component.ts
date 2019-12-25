import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params} from '@angular/router';
import { Location } from '@angular/common';
import { Request } from '../../model/request';
import { Category } from '../../model/category';
import { RequestService } from '../../services/request.service';
import { CategoryService } from '../../services/category.service';
import { FormGroup,  FormBuilder,  Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-request-detail',
  templateUrl: 'requestDetail.component.html',
  styleUrls: ['requestDetail.component.css']
})
export class RequestDetailComponent implements OnInit {
  request: Request;
  categories : Category[];
  category: Category;
  selectedDate : Date;
  requestForm: FormGroup;
  httpResponseError : string;
  
  constructor(private requestService: RequestService,
              private categoryService : CategoryService,
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

          this.categoryService.getCategories()
          .subscribe(categories =>{ this.categories = categories.filter(cat => cat.isActive == true);
            this.category = categories.find(cat => cat.id == this.request.categoryId);
            });
        },err => {
          this.httpResponseError = err.error
        });
    }
 
  goBack(): void {
    this.location.back();
  }
}
