// material.module.ts
import { NgModule } from "@angular/core";
import { MatIconModule, MatIconRegistry } from "@angular/material/icon";
@NgModule({ 
imports: [ MatIconModule ], 
exports: [ MatIconModule, MatIconRegistry ], 
providers: [MatIconRegistry] }) 
export class MaterialModule {}