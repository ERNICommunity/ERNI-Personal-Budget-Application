import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params} from '@angular/router';
import { Location } from '@angular/common';
import { Request } from '../../model/request/request';
import { Category } from '../../model/category';
import { RequestService } from '../../services/request.service';
import { CategoryService } from '../../services/category.service';
import { FormGroup,  FormBuilder,  Validators } from '@angular/forms';

@Component({
  selector: 'app-request-edit',
  templateUrl: 'requestEdit.component.html',
  styleUrls: ['requestEdit.component.css']
})
export class RequestEditComponent implements OnInit {
  request: Request;
  categories : Category[];
  selectedCategory: Category;
  selectedDate : Date;
  requestForm: FormGroup;
  httpResponseError : string;
  dirty: boolean;
  
  constructor(private requestService: RequestService,
              private categoryService : CategoryService,
              private route: ActivatedRoute,
              private location: Location,
              private fb: FormBuilder){
                this.createForm();
               }

  ngOnInit() {

    this.onChanges();
    
    this.route.params.subscribe((params: Params) => {
      var idParam = params['id']; 
      
      this.getRequest(idParam);
    });
  }

  createForm() {
    this.requestForm = this.fb.group({
       title: ['', Validators.required ],
       amount: ['', Validators.required ],
       category: ['', Validators.required ],
       url: ['', Validators.required ]
       
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

  getRequest(id: number): void {
    this.requestService.getRequest(id)
      .subscribe(request => 
        { 
          this.request = request;
          this.selectedDate = new Date(request.date);

          this.categoryService.getCategories()
          .subscribe(categories =>{ this.categories = categories.filter(cat => cat.isActive == true);
            this.selectedCategory = categories.find(cat => cat.id == this.request.categoryId);
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
    this.request.date = this.selectedDate;
    this.request.categoryId = this.selectedCategory.id;
   
    this.requestService.updateRequest(this.request)
       .subscribe(() => this.goBack(),
       err => {
        this.httpResponseError = err.error
      })
  }
}
