import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MatDialogContentAppointmentComponent } from './mat-dialog-content-appointment.component';

describe('MatDialogContentAppointmentComponent', () => {
  let component: MatDialogContentAppointmentComponent;
  let fixture: ComponentFixture<MatDialogContentAppointmentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MatDialogContentAppointmentComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MatDialogContentAppointmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
