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
  array: string[] = [];
  
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
  
  addmail(value: string): void {
    if ((value.trim() === '') || (value.search('@') === -1) || (this.category.email.search(value) !== -1)) {
      return;
    }
    if ((this.category.email.length === 0)) {
      this.category.email += value;
      return;
    }
    this.category.email += ',' + value;
  }

  
  onSelected(objs) {
    this.array = objs;
  }

  deletemail(): void {
    let list: string[] = [];
    this.array.forEach(e1 => {
      this.category.email.split(',').forEach(e2 => {
        if (e1 !== e2) {
          list.push(e2);
        }
      });
    });
    this.category.email = list.join(',');
    console.log(this.category.email);
  }
}
