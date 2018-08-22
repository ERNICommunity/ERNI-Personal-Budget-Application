import {Injectable} from '@angular/core';
import {Observable, of} from 'rxjs';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {Category} from '../model/category';
import { ConfigService } from './config.service';
import { ServiceHelper } from './service.helper';

@Injectable()
export class CategoryService {

    url = "RequestCategory";

    constructor(private http: HttpClient, private configService: ConfigService, private serviceHelper: ServiceHelper) {
    }

    public getRequests(): Observable<Category[]> {
        return this.http.get<Category[]>(this.configService.apiUrlBase + this.url, this.serviceHelper.getHttpOptions());
    }

    public getCategory(id): Observable<Category> {
        return this.http.get<Category>(this.configService.apiUrlBase + this.url + "/" + id, this.serviceHelper.getHttpOptions())
    }

    public getCategories(): Observable<Category[]> {
        return this.http.get<Category[]>(this.configService.apiUrlBase + this.url, this.serviceHelper.getHttpOptions())
    }

    public updateCategory(category: Category): Observable<any> {
        return this.http.put(this.configService.apiUrlBase + this.url, category, this.serviceHelper.getHttpOptions());
    }

    public addCategory(category: Category): Observable<Category> {
        return this.http.post<Category>(this.configService.apiUrlBase + this.url, category, this.serviceHelper.getHttpOptions());
    }

    public deleteCategory(id: number): Observable<Category> {
        return this.http.delete<Category>(this.configService.apiUrlBase + this.url + '/' + id, this.serviceHelper.getHttpOptions());
    }
}
