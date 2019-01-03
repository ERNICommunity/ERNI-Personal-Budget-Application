import { AdalService } from '../services/adal.service';
import { ConfigService } from '../services/config.service';
import {Component} from '@angular/core';

@Component({
  selector: 'home',
  styleUrls: ['./home.component.css'],
  templateUrl: './home.component.html'
})
export class HomeComponent {
   
  constructor(public adalService: AdalService,public config: ConfigService){
    console.log('User info from JWT');
    console.log(this.adalService.userInfo);
    console.log('JWT Token');
    console.log(this.adalService.accessToken);
  }

   logout() {
        this.adalService.logout();
    }

    public get isLoggedIn() {
        return this.adalService.isAuthenticated;
    }
    
}
