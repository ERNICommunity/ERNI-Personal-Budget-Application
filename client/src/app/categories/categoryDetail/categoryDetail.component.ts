import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { Category } from '../../model/category';
import { CategoryService } from '../../services/category.service';
import { ActivatedRoute } from '@angular/router';
import { FormControl, NgControl } from '@angular/forms';


@Component({
  selector: 'app-category-detail',
  templateUrl: './categoryDetail.component.html',
  styleUrls: ['./categoryDetail.component.css']
})

export class CategoryDetailComponent implements OnInit {
  category: Category;
  isSubmitted: boolean;

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

  save(): void {
    this.isSubmitted = true;
    this.categoryService.updateCategory(this.category).subscribe(() => this.goBack()).add(() => this.isSubmitted = false);
  }

  addMail(newMail: NgControl): void {
    if (newMail.invalid || (this.category.email.includes(newMail.value))) {
      return;
    }
    this.category.email.push(newMail.value);
    newMail.reset();

  }

  deleteMail(emailsToDelete: NgControl): void {
    this.category.email = this.category.email.filter(element => !emailsToDelete.value.includes(element));
    emailsToDelete.reset();
  }
}
