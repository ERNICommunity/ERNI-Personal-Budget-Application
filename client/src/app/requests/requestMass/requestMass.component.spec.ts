import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RequestMassComponent } from './requestMass.component';

describe('RequestMassComponent', () => {
  let component: RequestMassComponent;
  let fixture: ComponentFixture<RequestMassComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RequestMassComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RequestMassComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
