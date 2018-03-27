import { Component } from '@angular/core';
import { AdalService } from './services/adal.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Zeroes app';

  constructor(private adalService: AdalService){

  }
}
