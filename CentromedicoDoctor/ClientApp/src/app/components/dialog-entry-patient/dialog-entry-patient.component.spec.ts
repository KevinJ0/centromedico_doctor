import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogEntryPatientComponent } from './dialog-entry-patient.component';

describe('DialogEntryPatientComponent', () => {
  let component: DialogEntryPatientComponent;
  let fixture: ComponentFixture<DialogEntryPatientComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DialogEntryPatientComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DialogEntryPatientComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
