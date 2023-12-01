import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OtherBudgetsComponent } from './otherBudgets.component';

describe('OtherBudgetsComponent', () => {
  let component: OtherBudgetsComponent;
  let fixture: ComponentFixture<OtherBudgetsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
    imports: [OtherBudgetsComponent]
})
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OtherBudgetsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
