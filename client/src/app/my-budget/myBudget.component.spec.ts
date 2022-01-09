import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MyBudgetComponent } from './myBudget.component';

describe('MyBudgetsComponent', () => {
  let component: MyBudgetComponent;
  let fixture: ComponentFixture<MyBudgetComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MyBudgetComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MyBudgetComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
