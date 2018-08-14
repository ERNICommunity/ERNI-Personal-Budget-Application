import {Injectable} from '@angular/core';
import {Observable, of} from 'rxjs';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {AdalService} from './adal.service';
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
        /*.pipe(
         tap(_ => this.log(`updated hero id=${hero.id}`)),
         catchError(this.handleError<any>('updateHero'))
         );*/
    }

    public addCategory(category: Category): Observable<Category> {
        return this.http.post<Category>(this.configService.apiUrlBase + this.url, category, this.serviceHelper.getHttpOptions());
        /*.pipe(
         tap((hero: Hero) => this.log(`added hero w/ id=${hero.id}`)),
         catchError(this.handleError<Hero>('addHero'))
         );*/
    }

    public deleteCategory(category: Category | number): Observable<Category> {
        const id = typeof category === 'number' ? category : category.id;
        const deleteUrl = `${this.url}?id=${id}`;
        
        return this.http.delete<Category>(this.configService.apiUrlBase + deleteUrl, this.serviceHelper.getHttpOptions());
    }
}
