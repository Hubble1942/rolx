import { TransformAsIsoDate } from '@app/core/util/iso-date';
import { assertDefined } from '@app/core/util/utils';
import { Exclude, Type } from 'class-transformer';
import * as moment from 'moment';

import { BalanceCorrection } from './balance-correction';
import { PartTimeSetting } from './part-time-setting';
import { Role } from './role';
import { VacationDaysSetting } from './vacation-days-setting';

export class User {
  id!: string;
  firstName!: string;
  lastName!: string;
  email!: string;
  avatarUrl!: string;
  role!: Role;

  @TransformAsIsoDate()
  entryDate!: moment.Moment;

  @TransformAsIsoDate()
  leavingDate?: moment.Moment;

  isConfirmed!: boolean;

  @Type(() => PartTimeSetting)
  partTimeSettings!: PartTimeSetting[];

  @Type(() => VacationDaysSetting)
  vacationDaysSettings!: VacationDaysSetting[];

  @Type(() => BalanceCorrection)
  balanceCorrections!: BalanceCorrection[];

  validateModel(): void {
    assertDefined(this, 'id');
    assertDefined(this, 'firstName');
    assertDefined(this, 'lastName');
    assertDefined(this, 'email');
    assertDefined(this, 'avatarUrl');
    assertDefined(this, 'role');
    assertDefined(this, 'entryDate');
    assertDefined(this, 'isConfirmed');
    assertDefined(this, 'partTimeSettings');
    assertDefined(this, 'balanceCorrections');

    this.partTimeSettings.forEach((e) => e.validateModel());
    this.vacationDaysSettings.forEach((e) => e.validateModel());
    this.balanceCorrections.forEach((e) => e.validateModel());
  }

  @Exclude()
  get fullName(): string {
    return `${this.firstName} ${this.lastName}`;
  }

  @Exclude()
  isActiveAt(date: moment.Moment): boolean {
    return (
      this.entryDate.isSameOrBefore(date, 'day') &&
      (this.leavingDate == null || this.leavingDate.isSameOrAfter(date, 'day'))
    );
  }
}
