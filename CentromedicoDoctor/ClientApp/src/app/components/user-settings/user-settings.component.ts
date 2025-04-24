import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { medico } from 'src/app/interfaces/InterfacesDto';
import { AccountService } from 'src/app/services/account.service';
import { MatDialog } from '@angular/material/dialog';
import { ImageCropperComponent } from '../image-cropper/image-cropper.component';
import { base64ToFile } from 'ngx-image-cropper';
import { SnackBarService } from 'src/app/services/snack-bar.service';
import { ResetPasswordComponent } from '../reset-password/reset-password.component';
import { ProgressSpinnerMode } from '@angular/material/progress-spinner';
var mimeDb = require("mime-db");

@Component({
  selector: 'app-user-settings',
  templateUrl: './user-settings.component.html',
  styleUrls: ['./user-settings.component.css']
})
export class UserSettingsComponent implements OnInit {

  userRole: string;
  croppedImage: any;
  userFormGroup: FormGroup;
  _medico: medico;
  isProfiPhotoChanged: boolean;
  ext_tele = [];
  isExtDuplicated: boolean;
  currentUserRole$ = this.accountSvc.currentUserRole;
  mode: ProgressSpinnerMode = 'indeterminate';
  loading: boolean = false;

  constructor(
    private _formBuilder: FormBuilder,
    private accountSvc: AccountService,
    private dialog: MatDialog,
    private openSnackBar: SnackBarService,

  ) {


    this.currentUserRole$.subscribe(r => {
      this.userRole = r;
    });

    this.userFormGroup = this._formBuilder.group({
      nameControl: ["", Validators.required],
      lastNameControl: ["", Validators.required],
      telefono1Control: [""],
      telefono2Control: [""],

      telefono1ReachControl: [],
      telefono2ReachControl: [],
      telefono3ReachControl: [],
      telefono4ReachControl: [],

      ext1Control: [""],
      ext2Control: [""],
      ext3Control: [""],

      emailControl: [""],
      consultingRoomControl: ["", Validators.required],
      urlFacebookControl: [""],
      urlTwitterControl: [""],
      urlInstagramControl: [""],
    });

  }




  ngOnInit(): void {
    this.loading = true;

    this.accountSvc.GetUserInfo().subscribe(
      (r: medico) => {
        this._medico = r;
        this.userFormGroup.get("nameControl").setValue(r?.nombre);
        this.userFormGroup.get("lastNameControl").setValue(r?.apellido);
        this.userFormGroup.get("telefono1Control").setValue(r?.telefono1);
        this.userFormGroup.get("telefono2Control").setValue(r?.telefono2);
        this.userFormGroup.get("telefono1ReachControl").setValue(r.telefono1_contact?.includes("w"));
        this.userFormGroup.get("telefono2ReachControl").setValue(r.telefono1_contact?.includes("t"));
        this.userFormGroup.get("telefono3ReachControl").setValue(r.telefono2_contact?.includes("w"));
        this.userFormGroup.get("telefono4ReachControl").setValue(r.telefono2_contact?.includes("t"));
        this.userFormGroup.get("ext1Control").setValue(r?.extensiones_telefonicas[0]);
        this.userFormGroup.get("ext2Control").setValue(r?.extensiones_telefonicas[1]);
        this.userFormGroup.get("ext3Control").setValue(r?.extensiones_telefonicas[2]);
        this.userFormGroup.get("emailControl").setValue(r?.correo);
        this.userFormGroup.get("consultingRoomControl").setValue(r?.consultorio);
        this.userFormGroup.get("urlFacebookControl").setValue(r?.url_facebook);
        this.userFormGroup.get("urlTwitterControl").setValue(r?.url_twitter);
        this.userFormGroup.get("urlInstagramControl").setValue(r?.url_instagram);
        this.croppedImage = r.profilePhoto + `?time=${Date.now()}`;
      },
      (err) => {
        console.error(err);
        this.openSnackBar.open("Error al tratar de traer los datos 😥", 1);
      },
      () => {
        this.loading = false;
      }
    );

  }

  setExtensions() {

    this.ext_tele = [
      this.userFormGroup.get("ext1Control").value,
      this.userFormGroup.get("ext2Control").value,
      this.userFormGroup.get("ext3Control").value
    ];

    this.isExtDuplicated = false;
    const duplicates = this.ext_tele.filter((item, index) => index !== this.ext_tele.indexOf(item));

    if (duplicates.length != 0) {
      this.isExtDuplicated = true;

      this.userFormGroup.get("ext1Control").setErrors({
        notUnique: true
      });

      this.userFormGroup.get("ext2Control").setErrors({
        notUnique: true
      });

      this.userFormGroup.get("ext3Control").setErrors({
        notUnique: true
      });

      this.userFormGroup.get("ext1Control").markAsTouched();
      this.userFormGroup.get("ext2Control").markAsTouched();
      this.userFormGroup.get("ext3Control").markAsTouched();
    } else {

      this.userFormGroup.get("ext1Control").setErrors(null);
      this.userFormGroup.get("ext2Control").setErrors(null);
      this.userFormGroup.get("ext3Control").setErrors(null);

      this.userFormGroup.get("ext1Control").markAsUntouched();
      this.userFormGroup.get("ext2Control").markAsUntouched();
      this.userFormGroup.get("ext3Control").markAsUntouched();
    }

    console.log('ha cambiado la extension')

  }

  Submit(): void {


    if (this.userFormGroup.invalid) {
      console.error("Formulario incompleto");
      console.log(this.userFormGroup.get("ext1Control").errors)
      this.openSnackBar.open("Hay campos que necesitan ser completados.", 1);

      return;
    }

    let telefono1_contact = (this.userFormGroup.get("telefono1ReachControl").value ? "w" : "") +
      (this.userFormGroup.get("telefono2ReachControl").value ? "t" : "");

    let telefono2_contact = (this.userFormGroup.get("telefono3ReachControl").value ? "w" : "") +
      (this.userFormGroup.get("telefono4ReachControl").value ? "t" : "");

    if (this.isExtDuplicated) {
      return;
    }

    const formData = new FormData()

    if (this.isProfiPhotoChanged) {
      const imagePath = base64ToFile(this.croppedImage);

      const imgFile = new File([imagePath], 'profilePhoto.' + mimeDb[imagePath.type].extensions[0]);

      formData.append('ProfilePhoto', imgFile, imgFile.name);
    }

    const _form = { ... this.userFormGroup };

    formData.append('nombre', _form.value.nameControl);
    formData.append('apellido', _form.value.lastNameControl ?? "");
    formData.append('telefono1', _form.value.telefono1Control ?? "");
    formData.append('telefono2', _form.value.telefono2Control ?? "");
    formData.append('telefono1_contact', telefono1_contact);
    formData.append('telefono2_contact', telefono2_contact);
    formData.append('exten_tel_arrstr', this.ext_tele.toString());
    formData.append('consultorio', _form.value.consultingRoomControl ?? "");
    formData.append('url_facebook', _form.value.urlFacebookControl ?? "");
    formData.append('url_twitter', _form.value.urlTwitterControl ?? "");
    formData.append('url_instagram', _form.value.urlInstagramControl ?? "");

    this.loading = true;

    this.accountSvc.SaveUserInfo(formData).subscribe(
      (r) => {
        console.log(r);
        if (r)
          this.openSnackBar.open("Actualizado correctamente 👌", 0);

      }, (err) => {
        this.openSnackBar.open("Ha ocurrido un error 😥", 1);
        console.error(err);
      },
      () => this.loading = false
    );

  }


  fileChangeEvent(event: any): void {
    this.openImageCropper(event);
  }

  openImageCropper(event: any) {

    const dialogRef = this.dialog.open(ImageCropperComponent, {
      data: event,
    });

    dialogRef.afterClosed().subscribe((croppedImage: any) => {
      console.log(croppedImage)
      if (croppedImage) {
        this.croppedImage = croppedImage.data;
        this.isProfiPhotoChanged = true;
      }
    });
  }


  openResetPassword() {
    this.dialog.open(ResetPasswordComponent);
  }
}
