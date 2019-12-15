import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FavouritePhaseService, Phase } from '@app/account/core';
import { Record } from '@app/work-record/core';
import { Subscription } from 'rxjs';

@Component({
  selector: 'rolx-week-table',
  templateUrl: './week-table.component.html',
  styleUrls: ['./week-table.component.scss'],
})
export class WeekTableComponent implements OnInit, OnDestroy {

  readonly weekdays = [
    'monday',
    'tuesday',
    'wednesday',
    'thursday',
    'friday',
    'saturday',
    'sunday',
  ];

  readonly displayedColumns: string[] = [
    'phase',
    ...this.weekdays,
  ];

  @Input()
  records: Record[] = [];

  allPhases: Phase[] = [];

  isAddingPhase = false;

  private inputPhases: Phase[] = [];
  private favouritePhases: Phase[] = [];
  private homegrownPhases: Phase[] = [];
  private subscriptions = new Subscription();

  constructor(private favouritePhaseService: FavouritePhaseService) { }

  @Input()
  get phases(): Phase[] {
    return this.inputPhases;
  }
  set phases(value: Phase[]) {
    this.inputPhases = value;
    this.homegrownPhases = [];

    this.update();
  }

  ngOnInit() {
    this.subscriptions.add(
      this.favouritePhaseService.favourites$.subscribe(phs => this.favourites = phs),
    );
  }

  ngOnDestroy() {
    this.subscriptions.unsubscribe();
  }

  startAdding() {
    this.isAddingPhase = true;
    this.update();
  }

  addHomegrown(phase: Phase) {
    this.isAddingPhase = false;
    this.homegrownPhases.push(phase);
    this.update();
  }

  private set favourites(value: Phase[]) {
    this.favouritePhases = value;
    this.update();
  }

  private update() {
    const localPhasesIds = new Set<number>([
      ...this.inputPhases,
      ...this.homegrownPhases,
    ].map(ph => ph.id));

    const nonLocalFavourites = this.favouritePhases.filter(ph => !localPhasesIds.has(ph.id));

    const sortedPhases = [...this.inputPhases, ...nonLocalFavourites]
      .sort((a, b) => a.fullName.localeCompare(b.fullName));

    this.allPhases = [...sortedPhases, ...this.homegrownPhases];
    if (this.isAddingPhase) {
      this.allPhases.push(null);
    }
  }

}