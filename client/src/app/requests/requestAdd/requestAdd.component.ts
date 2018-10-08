import { Component, OnInit } from '@angular/core';
import { CategoryService } from '../../services/category.service';
import { RequestService } from '../../services/request.service';
import { Category } from '../../model/category';
import { Request } from '../../model/request';
import { Location } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { FormGroup,  FormBuilder,  Validators } from '@angular/forms';


@Component({
  selector: 'app-request-add',
  templateUrl: './requestAdd.component.html',
  styleUrls: ['./requestAdd.component.css']
})
export class RequestAddComponent implements OnInit {
  categories: Category[];
  selectedCategory : Category;
  httpResponseError : string;
  selectedDate : Date;
  requestForm: FormGroup;
  requestUrl: string;

  constructor (private categoryService: CategoryService,
              private requestService: RequestService,
              private location: Location,
              private route: ActivatedRoute,
              private fb: FormBuilder){
              this.createForm();
              }

  ngOnInit() {
    this.onChanges();
    this.getCategories();
    this.selectedDate = new Date();
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

  getCategories(): void {

    this.categoryService.getCategories()
    .subscribe(categories => {
       this.categories = categories.filter(cat => cat.isActive == true),
       this.selectedCategory =categories.filter(cat => cat.isActive == true)[0];
      });
  }

  goBack(): void {
    this.location.back();
  }
  
  save(title: string, amount: number) : void {
    var category = this.selectedCategory;
    var date = this.selectedDate;
    var url = this.requestUrl;

    this.requestService.addRequest({ title, amount, date, category, url} as Request)
       .subscribe(() =>{this.goBack()},
       err => {
        this.httpResponseError = err.error
      })
  }
}
