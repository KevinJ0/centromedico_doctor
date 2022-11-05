import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { medico, MedicoUserForm } from 'src/app/interfaces/InterfacesDto';
import { AccountService } from 'src/app/services/account.service';
import { base64ToFile, Dimensions, ImageCroppedEvent, ImageTransform, LoadedImage } from 'ngx-image-cropper';

class ImageSnippet {
  constructor(public src: string, public file: File) { }
}
@Component({
  selector: 'app-user-settings',
  templateUrl: './user-settings.component.html',
  styleUrls: ['./user-settings.component.css']
})
export class UserSettingsComponent implements OnInit {

  userFormGroup: FormGroup;
  _medico: medico;

  constructor(private _formBuilder: FormBuilder,
    private accountSvc: AccountService) {

  }

  croppedImage: any;

  ngOnInit(): void {

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
      consultingRoomControl: [""],
      urlFacebookControl: [""],
      urlTwitterControl: [""],
      urlInstagramControl: [""],
    });


    this.accountSvc.GetUserInfo().subscribe(
      (r: medico) => {
        this._medico = r;

        this.userFormGroup.get("nameControl").setValue(r.nombre);
        this.userFormGroup.get("lastNameControl").setValue(r.apellido);
        this.userFormGroup.get("telefono1Control").setValue(r.telefono1);
        this.userFormGroup.get("telefono2Control").setValue(r.telefono2);
        this.userFormGroup.get("telefono1ReachControl").setValue(r.telefono1_contact.includes("w"));
        this.userFormGroup.get("telefono2ReachControl").setValue(r.telefono1_contact.includes("t"));
        this.userFormGroup.get("telefono3ReachControl").setValue(r.telefono2_contact.includes("w"));
        this.userFormGroup.get("telefono4ReachControl").setValue(r.telefono2_contact.includes("t"));
        this.userFormGroup.get("ext1Control").setValue(r.extensiones_telefonicas[0]);
        this.userFormGroup.get("ext2Control").setValue(r.extensiones_telefonicas[1]);
        this.userFormGroup.get("ext3Control").setValue(r.extensiones_telefonicas[2]);
        this.userFormGroup.get("emailControl").setValue(r.correo);
        this.userFormGroup.get("consultingRoomControl").setValue(r.consultorio);
        this.userFormGroup.get("urlFacebookControl").setValue(r.url_facebook);
        this.userFormGroup.get("urlTwitterControl").setValue(r.url_twitter);
        this.userFormGroup.get("urlInstagramControl").setValue(r.url_instagram);


      }, (err) => {
        console.error(err);
      });

  }

  Submit(): void {
    let telefono1_contact = this.userFormGroup.get("telefono1ReachControl").value ? "w" : "" +
      this.userFormGroup.get("telefono2ReachControl").value ? "t" : "";

    let telefono2_contact = this.userFormGroup.get("telefono3ReachControl").value ? "w" : "" +
      this.userFormGroup.get("telefono4ReachControl").value ? "t" : "";

    let ext_tele = [
      this.userFormGroup.get("ext1Control").value,
      this.userFormGroup.get("ext2Control").value,
      this.userFormGroup.get("ext3Control").value
    ];

    let formUser: MedicoUserForm = {
      nombre: this.userFormGroup.get("nameControl").value,
      apellido: this.userFormGroup.get("lastNameControl").value,
      telefono1: this.userFormGroup.get("telefono1Control").value,
      telefono2: this.userFormGroup.get("telefono2Control").value,
      telefono1_contact: telefono1_contact,
      telefono2_contact: telefono2_contact,
      extensiones_telefonicas: ext_tele,
      consultorio: this.userFormGroup.get("consultingRoomControl").value,
      url_facebook: this.userFormGroup.get("urlFacebookControl").value,
      url_twitter: this.userFormGroup.get("urlTwitterControl").value,
      url_instagram: this.userFormGroup.get("urlInstagramControl").value,
      profilePhoto: this.croppedImage
    };

    this.accountSvc.SaveUserInfo(formUser).subscribe(
      (r) => {
        console.log(r);
      }, (err) => {
        console.error(err);
      });
  }

  selectedFile: ImageSnippet;
  imageUrl: any;
  imageChangedEvent: any = '';

  canvasRotation = 0;
  rotation = 0;
  scale = 1;
  showCropper = false;
  containWithinAspectRatio = false;
  transform: ImageTransform = {};

  getCroppedImg(croppedImage) {
    this.croppedImage = croppedImage;
  }


  fileChangeEvent(event: any): void {
    this.imageChangedEvent = event;
  }

  processFile(imageInput: any) {
    this.croppedImage = imageInput.files[0];

    const reader = new FileReader();

    reader.addEventListener('load', (event: any) => {

      //  this.selectedFile = new ImageSnippet(event.target.result, this.croppedImage);

      //this.imageUrl = reader.result;
      this.imageUrl = 'https://cdn-icons-png.flaticon.com/512/3047/3047331.png';
    });

    reader.readAsDataURL(this.croppedImage);

  }

}
