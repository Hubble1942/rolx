import { Component } from '@angular/core';
import { AuthService } from '@app/auth/core/auth.service';
import { BalanceService } from '@app/records/core/balance.service';
import { WorkRecordService } from '@app/records/core/work-record.service';
import * as moment from 'moment';
import { of } from 'rxjs';
import { catchError, filter, startWith, switchMap, throttleTime } from 'rxjs/operators';

@Component({
  selector: 'rolx-balance-indicator',
  templateUrl: './balance-indicator.component.html',
  styleUrls: ['./balance-indicator.component.scss'],
})
export class BalanceIndicatorComponent {
  readonly balance$ = this.workRecordService.userUpdated$.pipe(
    filter((u) => this.authService.currentApproval?.user.id === u),
    throttleTime(1000),
    startWith(1),
    switchMap(() => this.balanceService.getByDate(moment()).pipe(catchError(() => of(null)))),
  );

  constructor(
    private balanceService: BalanceService,
    private workRecordService: WorkRecordService,
    private authService: AuthService,
  ) {}
}
