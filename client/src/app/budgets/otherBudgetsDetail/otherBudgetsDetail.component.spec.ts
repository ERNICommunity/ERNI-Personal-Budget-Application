import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { OtherBudgetsDetailComponent } from './otherBudgetsDetail.component';

describe('OtherBudgetsDetailComponent', () => {
  let component: OtherBudgetsDetailComponent;
  let fixture: ComponentFixture<OtherBudgetsDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OtherBudgetsDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OtherBudgetsDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
