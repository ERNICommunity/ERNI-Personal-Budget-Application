import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params} from '@angular/router';
import { Location } from '@angular/common';
import { Request } from '../../model/request';
import { Category } from '../../model/category';
import { RequestService } from '../../services/request.service';
import { CategoryService } from '../../services/category.service';
import { FormGroup,  FormBuilder,  Validators } from '@angular/forms';

@Component({
  selector: 'app-request-detail',
  templateUrl: 'requestDetail.component.html',
  styleUrls: ['requestDetail.component.css']
})
export class RequestDetailComponent implements OnInit {
  request: Request;
  categories : Category[];
  selectedDate : Date;
  requestForm: FormGroup;
  
  constructor(private requestService: RequestService,
              private categoryService : CategoryService,
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
       occasion: ['', Validators.required ],
       dateOfOccasion: ['', Validators.required ]
    });
  }

  getRequest(id: number): void {
    this.requestService.getRequest(id)
      .subscribe(request => 
        { 
          this.request = request;
          this.selectedDate = new Date(request.date);
        });

    this.categoryService.getCategories()
     .subscribe(categories => this.categories = categories.filter(cat => cat.isActive == true));
  }
 
  goBack(): void {
    this.location.back();
  }

  save() : void {
    this.request.date = this.selectedDate; 
   
    this.requestService.updateRequest(this.request)
       .subscribe(() => this.goBack())
  }
}
