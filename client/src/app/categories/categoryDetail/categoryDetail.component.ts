import { Component, OnInit} from '@angular/core';
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
       .subscribe(() => this.goBack())
  }
}
