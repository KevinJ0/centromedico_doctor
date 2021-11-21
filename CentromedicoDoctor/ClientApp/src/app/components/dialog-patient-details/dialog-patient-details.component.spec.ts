import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogPatientDetailsComponent } from './dialog-patient-details.component';

describe('DialogPatientDetailsComponent', () => {
  let component: DialogPatientDetailsComponent;
  let fixture: ComponentFixture<DialogPatientDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DialogPatientDetailsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DialogPatientDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
