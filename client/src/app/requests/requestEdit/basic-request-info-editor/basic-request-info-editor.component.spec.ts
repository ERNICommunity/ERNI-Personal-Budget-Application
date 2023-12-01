import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BasicRequestInfoEditorComponent } from './basic-request-info-editor.component';

describe('BasicRequestInfoEditorComponent', () => {
  let component: BasicRequestInfoEditorComponent;
  let fixture: ComponentFixture<BasicRequestInfoEditorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
    imports: [BasicRequestInfoEditorComponent]
})
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BasicRequestInfoEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
