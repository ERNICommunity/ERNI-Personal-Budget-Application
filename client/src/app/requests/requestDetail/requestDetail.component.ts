import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import { Request } from '../../model/request';
import { Category } from '../../model/category';
import { RequestService } from '../../services/request.service';
import { CategoryService } from '../../services/category.service';
import { NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-request-detail',
  templateUrl: 'requestDetail.component.html',
  styleUrls: ['requestDetail.component.css']
})
export class RequestDetailComponent implements OnInit {
  request: Request;
  categories : Category[];
  ngbModel;

  constructor(private requestService: RequestService,
              private categoryService : CategoryService,
              private route: ActivatedRoute,
              private location: Location,
              private ngbDateParserFormatter: NgbDateParserFormatter){ }

  ngOnInit() {
    this.getRequest();
  }

  getRequest(): void {

    const id = this.route.snapshot.paramMap.get('id');

    this.requestService.getRequest(id)
      .subscribe(request => 
        { 
          this.request = request;
          this.getServerDate(request.date); 
        });

    this.categoryService.getCategories()
      .subscribe(categories => this.categories = categories);
  }

  getServerDate(dateStruct) : void {
    this.ngbModel = this.ngbDateParserFormatter.parse(dateStruct);
  }

  goBack(): void {
    this.location.back();
  }

  save(date: Date) : void {
    this.request.date = date; 
   
    this.requestService.updateRequest(this.request)
       .subscribe(() => this.goBack())
  }
}
