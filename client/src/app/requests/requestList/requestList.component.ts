import {Component, OnInit} from '@angular/core';
import {Request} from '../../model/request';
import {RequestService} from '../../services/request.service';

@Component({
    selector: 'app-users',
    templateUrl: 'requestList.component.html',
    styleUrls: ['requestList.component.css']
})
export class RequestListComponent implements OnInit {
    requests: Request[];

    constructor(private valueService: RequestService) {
    }

    ngOnInit() {
        this.getHeroes();
    }

    getHeroes(): void {
        this.valueService.getPendingRequests()
            .subscribe(requests => this.requests = requests);
    }

}
