import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NewTeamRequestModalComponent } from './new-team-request-modal.component';

describe('NewTeamRequestModalComponent', () => {
  let component: NewTeamRequestModalComponent;
  let fixture: ComponentFixture<NewTeamRequestModalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NewTeamRequestModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NewTeamRequestModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
