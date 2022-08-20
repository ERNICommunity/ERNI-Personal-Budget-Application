import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateBudgetsComponent } from './create-budgets.component';

describe('CreateBudgetsComponent', () => {
  let component: CreateBudgetsComponent;
  let fixture: ComponentFixture<CreateBudgetsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateBudgetsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateBudgetsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
