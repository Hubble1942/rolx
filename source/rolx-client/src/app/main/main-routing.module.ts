import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from '@app/auth/core/auth.guard';
import { RoleGuard } from '@app/auth/core/role.guard';
import { ProjectsRoutes } from '@app/projects/projects.routes';
import { RecordsRoutes } from '@app/records/records.routes';
import { ReportsRoutes } from '@app/reports/reports.routes';
import { Role } from '@app/users/core/role';
import { UsersRoutes } from '@app/users/users.routes';

import { ForbiddenPageComponent } from './forbidden-page/forbidden-page.component';
import { MainPageComponent } from './main-page/main-page.component';
import { NotFoundPageComponent } from './not-found-page/not-found-page.component';
import { SettingsPageComponent } from './settings-page/settings-page.component';
import { SignInPageComponent } from './sign-in-page/sign-in-page.component';
import { SomethingWentWrongPageComponent } from './something-went-wrong-page/something-went-wrong-page.component';

const routes: Routes = [
  {
    path: '',
    component: MainPageComponent,
    canActivate: [AuthGuard],
    children: [
      ...ProjectsRoutes,
      ...UsersRoutes,
      ...RecordsRoutes,
      ...ReportsRoutes,
      {
        path: 'settings',
        component: SettingsPageComponent,
        canActivate: [RoleGuard],
        data: { minRole: Role.Backoffice },
      },
      {
        path: 'four-oh-four',
        component: NotFoundPageComponent,
      },
      {
        path: 'server-error',
        component: SomethingWentWrongPageComponent,
      },
      {
        path: '',
        redirectTo: '/week/',
        pathMatch: 'full',
      },
    ],
  },
  {
    path: 'sign-in',
    component: SignInPageComponent,
  },
  {
    path: 'forbidden',
    component: ForbiddenPageComponent,
  },
  {
    path: '**',
    redirectTo: '/four-oh-four',
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class MainRoutingModule {}
