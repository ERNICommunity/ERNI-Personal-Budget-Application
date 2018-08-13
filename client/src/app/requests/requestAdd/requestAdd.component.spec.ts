import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RequestAddComponent } from './requestAdd.component';

describe('RequestAddComponent', () => {
  let component: RequestAddComponent;
  let fixture: ComponentFixture<RequestAddComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RequestAddComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RequestAddComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
