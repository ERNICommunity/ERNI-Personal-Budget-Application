import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TeamRequestAddComponent } from './team-request-add.component';

describe('TeamRequestAddComponent', () => {
  let component: TeamRequestAddComponent;
  let fixture: ComponentFixture<TeamRequestAddComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TeamRequestAddComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TeamRequestAddComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
