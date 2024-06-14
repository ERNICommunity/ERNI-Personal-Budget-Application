import { Component, Input } from '@angular/core';
import { Request } from '../../../model/request/request';

@Component({
  selector: 'app-basic-request-info-editor',
  templateUrl: './basic-request-info-editor.component.html',
  styleUrls: ['./basic-request-info-editor.component.css'],
})
export class BasicRequestInfoEditorComponent {
  @Input() isReadonly: boolean;
  @Input() request: Request;

  constructor() {
  }


}
