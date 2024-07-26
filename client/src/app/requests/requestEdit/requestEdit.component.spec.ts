import { waitForAsync, ComponentFixture, TestBed } from '@angular/core/testing';

import { RequestEditComponent } from './requestEdit.component';

describe('RequestEditComponent', () => {
  let component: RequestEditComponent;
  let fixture: ComponentFixture<RequestEditComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ RequestEditComponent ]
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
