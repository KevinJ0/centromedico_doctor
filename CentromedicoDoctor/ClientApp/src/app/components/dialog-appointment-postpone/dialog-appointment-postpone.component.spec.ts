import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogAppointmentPostponeComponent } from './dialog-appointment-postpone.component';

describe('DialogAppointmentPostponeComponent', () => {
  let component: DialogAppointmentPostponeComponent;
  let fixture: ComponentFixture<DialogAppointmentPostponeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DialogAppointmentPostponeComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DialogAppointmentPostponeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
