import {Injectable} from '@angular/core';
import {Observable, of} from 'rxjs';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {AdalService} from './adal.service';
import {Category} from '../model/category';
import { ConfigService } from './config.service';

@Injectable()
export class CategoryService {

    url = "RequestCategory";

    constructor(private http: HttpClient, private adalService: AdalService, private configService: ConfigService) {
    }

    public getRequests(): Observable<Category[]> {
        var httpOptions = {
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + this.adalService.accessToken
            })
        };

        return this.http.get<Category[]>(this.configService.apiUrlBase + this.url, httpOptions)
    }

    public getCategory(id): Observable<Category> {
        var httpOptions = {
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + this.adalService.accessToken
            })
        };

        return this.http.get<Category>(this.configService.apiUrlBase + this.url + "/" + id, httpOptions)
    }

    public getCategories(): Observable<Category[]> {
        var httpOptions = {
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + this.adalService.accessToken
            })
        };

        return this.http.get<Category[]>(this.configService.apiUrlBase + this.url, httpOptions)
    }

    public updateCategory(category: Category): Observable<any> {
        var httpOptions = {
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + this.adalService.accessToken
            })
        };

        return this.http.put(this.configService.apiUrlBase + this.url, category, httpOptions);
        /*.pipe(
         tap(_ => this.log(`updated hero id=${hero.id}`)),
         catchError(this.handleError<any>('updateHero'))
         );*/
    }

    public addCategory(category: Category): Observable<Category> {
        var httpOptions = {
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + this.adalService.accessToken
            })
        };
        
        return this.http.post<Category>(this.configService.apiUrlBase + this.url, category, httpOptions);
        /*.pipe(
         tap((hero: Hero) => this.log(`added hero w/ id=${hero.id}`)),
         catchError(this.handleError<Hero>('addHero'))
         );*/
    }

    public deleteCategory(category: Category | number): Observable<Category> {
        const id = typeof category === 'number' ? category : category.id;
        const deleteUrl = `${this.url}?id=${id}`;
        var httpOptions = {
            headers: new HttpHeaders({
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + this.adalService.accessToken
            })
        };
        
        return this.http.delete<Category>(this.configService.apiUrlBase + deleteUrl, httpOptions);
    }
}
