import { Component, Input, OnInit } from "@angular/core";
import { Request } from "../../../model/request/request";
import { InputNumberModule } from "primeng/inputnumber";
import { InputTextModule } from "primeng/inputtext";
import { FormsModule } from "@angular/forms";
import { NgIf } from "@angular/common";

@Component({
  selector: "app-basic-request-info-editor",
  templateUrl: "./basic-request-info-editor.component.html",
  styleUrls: ["./basic-request-info-editor.component.css"],
  standalone: true,
  imports: [NgIf, FormsModule, InputTextModule, InputNumberModule],
})
export class BasicRequestInfoEditorComponent implements OnInit {
  @Input({ required: true }) isReadonly!: boolean;
  @Input({ required: true }) request!: Request;

  constructor() {}

  ngOnInit(): void {}
}
