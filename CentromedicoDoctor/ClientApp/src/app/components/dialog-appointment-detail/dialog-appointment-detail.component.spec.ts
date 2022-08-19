import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogAppointmentDetailComponent } from './dialog-appointment-detail.component';

describe('DialogAppointmentDetailComponent', () => {
  let component: DialogAppointmentDetailComponent;
  let fixture: ComponentFixture<DialogAppointmentDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DialogAppointmentDetailComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DialogAppointmentDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
