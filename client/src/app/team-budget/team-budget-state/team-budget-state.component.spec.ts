import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TeamBudgetStateComponent } from './team-budget-state.component';

describe('TeamBudgetStateComponent', () => {
  let component: TeamBudgetStateComponent;
  let fixture: ComponentFixture<TeamBudgetStateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TeamBudgetStateComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TeamBudgetStateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
