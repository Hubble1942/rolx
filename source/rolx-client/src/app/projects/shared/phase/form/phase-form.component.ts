import { Component, Inject, Input, LOCALE_ID, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ErrorResponse } from '@app/core/error/error-response';
import { ErrorService } from '@app/core/error/error.service';
import { Duration } from '@app/core/util/duration';
import { assertDefined } from '@app/core/util/utils';
import { Phase } from '@app/projects/core/phase';
import { Project } from '@app/projects/core/project';
import { ProjectService } from '@app/projects/core/project.service';

@Component({
  selector: 'rolx-phase-form',
  templateUrl: './phase-form.component.html',
  styleUrls: ['./phase-form.component.scss'],
})
export class PhaseFormComponent implements OnInit {
  @Input() project!: Project;
  @Input() phase!: Phase;

  form = this.fb.group({
    name: ['', Validators.required],
    startDate: ['', Validators.required],
    endDate: [''],
    budget: [null, Validators.min(0)],
    isBillable: [false],
  });

  constructor(
    private router: Router,
    private fb: FormBuilder,
    private projectService: ProjectService,
    private errorService: ErrorService,
    @Inject(LOCALE_ID) private locale: string,
  ) {}

  ngOnInit() {
    assertDefined(this, 'project');
    assertDefined(this, 'phase');

    this.form.patchValue(this.phase);
    this.formBudget = this.phase.budget;
  }

  get formBudget(): Duration {
    const budget = Number.parseFloat(this.form.controls['budget'].value);
    return !Number.isNaN(budget) ? Duration.fromHours(budget) : Duration.Zero;
  }

  set formBudget(value: Duration) {
    const formValue =
      value && !value.isZero
        ? value.hours.toLocaleString(this.locale, { maximumFractionDigits: 1, useGrouping: false })
        : null;
    this.form.controls['budget'].setValue(formValue);
  }

  hasError(controlName: string, errorName: string) {
    return this.form.controls[controlName].hasError(errorName);
  }

  submit() {
    Object.assign(this.phase, this.form.value);
    this.phase.budget = this.formBudget;

    this.projectService.update(this.project).subscribe(
      () => this.cancel(),
      (err) => this.handleError(err),
    );
  }

  cancel() {
    // noinspection JSIgnoredPromiseFromCall
    this.router.navigate(['/project', this.project.id]);
  }

  private handleError(errorResponse: ErrorResponse) {
    if (!errorResponse.tryToHandleWith(this.form)) {
      this.errorService.notifyGeneralError();
    }
  }
}
