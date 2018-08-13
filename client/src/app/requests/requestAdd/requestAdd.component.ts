import { Component, OnInit } from '@angular/core';
import { CategoryService } from '../../services/category.service';
import { RequestService } from '../../services/request.service';
import { AdalService } from '../../services/adal.service';
import { Category } from '../../model/category';
import { Request } from '../../model/request';
import { Location } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-request-add',
  templateUrl: './requestAdd.component.html',
  styleUrls: ['./requestAdd.component.css']
})
export class RequestAddComponent implements OnInit {
  categories: Category[];
  selectedValue :number;

  constructor(private categoryService: CategoryService, private requestService: RequestService, private location: Location, private route: ActivatedRoute, private adalService: AdalService) { }

  ngOnInit() {
    this.getCategories();
  }

  getCategories(): void {

    this.categoryService.getCategories()
    .subscribe(categories => this.categories = categories);
  }

  goBack(): void {
    this.location.back();
  }
  
  save(title: string, amount: number, date: Date) : void {
    var categoryId = this.selectedValue;
 
    this.requestService.addRequest({ title, amount, date, categoryId} as Request)
       .subscribe(() => this.goBack())
  }

}
