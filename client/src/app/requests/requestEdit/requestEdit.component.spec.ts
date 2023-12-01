import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RequestEditComponent } from './requestEdit.component';

describe('RequestEditComponent', () => {
  let component: RequestEditComponent;
  let fixture: ComponentFixture<RequestEditComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
    imports: [RequestEditComponent]
})
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RequestEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
