import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TeamBudgetComponent } from './team-budget.component';

describe('TeamBudgetComponent', () => {
  let component: TeamBudgetComponent;
  let fixture: ComponentFixture<TeamBudgetComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TeamBudgetComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TeamBudgetComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
