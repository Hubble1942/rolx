import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { Phase } from '@app/projects/core';
import { PhaseService } from '@app/projects/core';
import moment from 'moment';
import { BehaviorSubject, combineLatest, Observable, Subscription } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

@Component({
  selector: 'rolx-phase-selector',
  templateUrl: './phase-selector.component.html',
  styleUrls: ['./phase-selector.component.scss'],
})
export class PhaseSelectorComponent implements OnInit, OnDestroy {

  private subscription = new Subscription();
  private phaseShadow: Phase;
  private begin$ = new BehaviorSubject<moment.Moment>(moment('1900-01-01'));
  private allPhases$ = new BehaviorSubject<Phase[]>([]);
  private filterText$ = new BehaviorSubject<string>('');

  @Output()
  selected = new EventEmitter<Phase>();

  @Input()
  excluded: Phase[] = [];

  @Input()
  end = moment('2999-12-31');

  candidates$: Observable<Phase[]>;

  constructor(private phaseService: PhaseService) {}

  @Input()
  get begin(): moment.Moment {
    return this.begin$.value;
  }
  set begin(value: moment.Moment) {
    this.begin$.next(value);
  }

  get phase() {
    return this.phaseShadow;
  }

  set phase(value: any) {
    if (!(value instanceof Phase)) {
      this.filterText$.next(value);
      value = this.allPhases$.value.find(ph => ph.fullName === value);
    }

    if (value) {
      this.phaseShadow = value;
      this.selected.emit(value);
    }
  }

  ngOnInit() {
    const excludedIds = new Set(this.excluded.map(ph => ph.id));
    this.subscription.add(this.begin$.pipe(
      switchMap(b => this.phaseService.getAll(b)),
      map(phs => phs.filter(ph => !excludedIds.has(ph.id))),
      map(phs => phs.sort((a, b) => a.fullName.localeCompare(b.fullName))),
    ).subscribe(this.allPhases$));

    this.candidates$ = combineLatest([this.allPhases$, this.filterText$]).pipe(
      map(([phases, filterText]) => this.filterByEndAndFullName(phases, filterText).slice(0, 5)),
    );
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  fullNameOf(phase: Phase): string {
    return phase ? phase.fullName : '';
  }

  private filterByEndAndFullName(phases: Phase[], filterText: string): Phase[] {
    filterText = filterText.toLocaleLowerCase();
    return phases
      .filter(ph => ph.startDate <= this.end && ph.fullName.toLocaleLowerCase().includes(filterText));
  }

}