import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Phase } from '@app/projects/core/phase';
import { Project } from '@app/projects/core/project';
import { ProjectService } from '@app/projects/core/project.service';
import { Observable, throwError } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';

@Component({
  selector: 'rolx-phase-list-page',
  templateUrl: './project-detail-page.component.html',
  styleUrls: ['./project-detail-page.component.scss'],
})
export class ProjectDetailPageComponent {
  readonly project$ = this.route.paramMap.pipe(
    switchMap((params) => this.initializeProject(params.get('id'))),
    catchError((e) => {
      if (e.status === 404) {
        // noinspection JSIgnoredPromiseFromCall
        this.router.navigate(['/four-oh-four']);
      }

      return throwError(e);
    }),
  );

  editedPhase?: Phase;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private projectService: ProjectService,
  ) {}

  private initializeProject(idText: string | null): Observable<Project> {
    return this.projectService.getById(Number(idText));
  }
}
