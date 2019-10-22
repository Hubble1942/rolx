import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { AppImportModule } from '@app/app-import.module';
import { SignInPageComponent } from '@app/main/sign-in-page/sign-in-page.component';
import { CustomerFormComponent } from './customer-form/customer-form.component';
import { CustomerTableComponent } from './customer-table/customer-table.component';
import { MainPageComponent } from './main-page/main-page.component';
import { MainRoutingModule } from './main-routing.module';
import { MonthPageComponent } from './month-page/month-page.component';
import { MonthTableComponent } from './month-page/month-table/month-table.component';
import { ProjectFormComponent } from './project-form/project-form.component';
import { ProjectTableComponent } from './project-table/project-table.component';

@NgModule({
  declarations: [
    MainPageComponent,
    SignInPageComponent,
    MonthPageComponent,
    MonthTableComponent,
    CustomerTableComponent,
    CustomerFormComponent,
    ProjectTableComponent,
    ProjectFormComponent,
  ],
  imports: [
    CommonModule,
    AppImportModule,
    MainRoutingModule,
  ]
})
export class MainModule { }
