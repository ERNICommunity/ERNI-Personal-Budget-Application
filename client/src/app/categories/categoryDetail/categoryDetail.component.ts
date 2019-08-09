import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { Category } from '../../model/category';
import {CategoryService} from '../../services/category.service';
import { ActivatedRoute } from '@angular/router';


@Component({
  selector: 'app-category-detail',
  templateUrl: './categoryDetail.component.html',
  styleUrls: ['./categoryDetail.component.css']
})
export class CategoryDetailComponent implements OnInit {
  category: Category;
  emailsToDelete: string[] = [];
  
  constructor(private categoryService: CategoryService, private route: ActivatedRoute, private location: Location) { }

  ngOnInit() {
    this.getCategory();
  }

  getCategory(): void {

    const id = this.route.snapshot.paramMap.get('id');

    this.categoryService.getCategory(id)
      .subscribe(category => this.category = category);
  }

  goBack(): void {
    this.location.back();
  }

  save() : void {
    this.categoryService.updateCategory(this.category)
       .subscribe(() => this.goBack());
  }
  
  addMail(value: string): void {
    if (this.category.email === undefined || this.category.email === null) {
      this.category.email = [];
    }
    if ((value.trim() === '') || (!this.validateEmail(value)) || (this.category.email.includes(value))) {
      return;
    }
    this.category.email.push(value);
  }

  
  onSelected(objs) {
    this.emailsToDelete = objs;
  }

  deleteMail(): void {
    this.category.email = this.category.email.filter(element => !this.emailsToDelete.includes(element));
    this.emailsToDelete = [];
  }

  validateEmail(email) {
    var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(String(email).toLowerCase());
}
}
