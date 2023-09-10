import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'noImage',
})
export class NoImagePipe implements PipeTransform {

    transform(url: string): String {
        return url ? url : "https://centromedico-assets.s3.us-east-2.amazonaws.com/noProfileImage.png";
    }
}