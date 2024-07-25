import { Injectable } from '@angular/core';
import { Observable, shareReplay } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Budget } from '../model/budget';
import { ConfigService } from './config.service';
import { ServiceHelper } from './service.helper';
import { BudgetType } from '../model/budgetType';
import { User } from '../model/user';
import { CurrentUsersBudget } from '../model/current-users-budget';

@Injectable({ providedIn: 'root' })
export class BudgetService {
    url = 'Budget/';

    constructor(
        private http: HttpClient,
        private serviceHelper: ServiceHelper,
        private configService: ConfigService
    ) {}

    public getActiveUsersBudgets(year: number): Observable<Budget[]> {
        return this.http.get<Budget[]>(
            this.configService.apiUrlBase +
                this.url +
                'users/active/year/' +
                year,
            this.serviceHelper.getHttpOptions()
        );
    }

    public getCurrentUserBudgets(
        year: number
    ): Observable<CurrentUsersBudget[]> {
        return this.http.get<CurrentUsersBudget[]>(
            this.configService.apiUrlBase +
                this.url +
                'user/current/year/' +
                year,
            this.serviceHelper.getHttpOptions()
        );
    }

    public getBudget(budgetId: number): Observable<Budget> {
        return this.http.get<Budget>(
            this.configService.apiUrlBase + this.url + budgetId,
            this.serviceHelper.getHttpOptions()
        );
    }

    public updateBudget(id: number, amount: number): Observable<unknown> {
        return this.http.put(
            this.configService.apiUrlBase + this.url,
            { id, amount },
            this.serviceHelper.getHttpOptions()
        );
    }

    public transferBudget(budgetId: number, userId: number): Observable<unknown> {
        return this.http.put(
            this.configService.apiUrlBase +
                this.url +
                budgetId +
                '/transfer/' +
                userId,
            this.serviceHelper.getHttpOptions()
        );
    }

    public createBudget(
        title: string,
        amount: number,
        userId: number,
        budgetType: number
    ): Observable<unknown> {
        return this.http.post(
            this.configService.apiUrlBase + this.url,
            { userId, title, amount, budgetType },
            this.serviceHelper.getHttpOptions()
        );
    }

    public createBudgetsForAllActiveUsers(
        title: string,
        amount: number,
        budgetType: number
    ): Observable<unknown> {
        return this.http.post(
            this.configService.apiUrlBase + this.url + 'users/all',
            { title, amount, budgetType },
            this.serviceHelper.getHttpOptions()
        );
    }

    #budgetTypes: Observable<BudgetType[]> | null = null;
    public getBudgetsTypes(): Observable<BudgetType[]> {
        if (!this.#budgetTypes) {
            this.#budgetTypes = this.http.get<BudgetType[]>(
                this.configService.apiUrlBase + this.url + 'types',
                this.serviceHelper.getHttpOptions()
            ).pipe(shareReplay(1));
        }
        return this.#budgetTypes;
    }

    public getUsersAvailableForBudgetType(budgetType: number): Observable<User[]> {
        return this.http.get<User[]>(
            this.configService.apiUrlBase +
                this.url +
                'usersAvailableForBudgetType/' +
                budgetType,
            this.serviceHelper.getHttpOptions()
        );
    }
}
