import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RequestFilesComponent } from './request-files.component';

describe('RequestFilesComponent', () => {
  let component: RequestFilesComponent;
  let fixture: ComponentFixture<RequestFilesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RequestFilesComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RequestFilesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
