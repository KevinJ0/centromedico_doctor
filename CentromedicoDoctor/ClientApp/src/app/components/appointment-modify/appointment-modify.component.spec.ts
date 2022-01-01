import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AppointmentModifyComponent } from './appointment-modify.component';

describe('AppointmentModifyComponent', () => {
  let component: AppointmentModifyComponent;
  let fixture: ComponentFixture<AppointmentModifyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AppointmentModifyComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AppointmentModifyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
