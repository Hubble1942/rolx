import { Transform } from 'class-transformer';
import { duration, Moment } from 'moment';

import { DurationBase } from './duration.base';

export class Duration extends DurationBase<Duration> {
  static formatAsDecimal = false;

  static readonly Zero = new Duration();
  static readonly Pattern = /^(-?)(?:(\d+)(?::|\.\.|,,)([0-5]?\d)|(\d*[.,]?\d*))$/;
  static readonly PatternGroups = {
    Sign: 1,
    Hours: 2,
    Minutes: 3,
    DecimalHours: 4,
  };

  private static readonly HoursPerPersonDay = 8;

  static fromMoments(from: Moment, to: Moment) {
    const momentDuration = duration({ from, to });
    return Duration.fromHours(momentDuration.asHours());
  }

  static fromHours(hours: number) {
    return new Duration(Math.round(hours * DurationBase.SecondsPerHour));
  }

  static fromPersonDays(personDays: number) {
    return Duration.fromHours(personDays * Duration.HoursPerPersonDay);
  }

  static parse(time: string | any, zeroIfEmpty: boolean = false): Duration {
    if (time instanceof Duration) {
      return time as Duration;
    }

    if (typeof time === 'string') {
      time = time.trim();
    }

    if (zeroIfEmpty && (time == null || time === '')) {
      return Duration.Zero;
    }

    const match = this.Pattern.exec(time);
    if (!match) {
      return new Duration(NaN);
    }

    if (match[Duration.PatternGroups.DecimalHours]) {
      return Duration.fromHours(Number.parseFloat(time.replace(',', '.')));
    }

    const hours = Number.parseInt(match[Duration.PatternGroups.Hours], 10);
    const minutes = Number.parseInt(match[Duration.PatternGroups.Minutes], 10);

    const seconds = hours * DurationBase.SecondsPerHour + minutes * DurationBase.SecondsPerMinute;
    return new Duration(match[Duration.PatternGroups.Sign] ? -seconds : seconds);
  }

  get isZero(): boolean {
    return this.isSame(Duration.Zero);
  }

  get isPositive(): boolean {
    return this.seconds > 0;
  }

  get isNegative(): boolean {
    return this.seconds < 0;
  }

  get personDays(): number {
    return this.hours / Duration.HoursPerPersonDay;
  }

  add(other: Duration): Duration {
    return new Duration(this.seconds + other.seconds);
  }

  sub(other: Duration): Duration {
    return new Duration(this.seconds - other.seconds);
  }

  isGreaterThanOrEqualTo(other: Duration): boolean {
    return this.seconds >= other.seconds;
  }

  override toString(forcePlusSign = false): string {
    if (!this.isValid) {
      return '-';
    }

    const minutes = Math.round(this.seconds / 60);
    const wholeHours = Math.abs(Math.trunc(minutes / 60));
    const wholeMinutes = Math.abs(minutes % 60);

    let sign = '';
    if (minutes !== 0) {
      if (this.seconds > 0 && forcePlusSign) {
        sign = '+';
      } else if (this.seconds < 0) {
        sign = '-';
      }
    }

    if (Duration.formatAsDecimal) {
      return `${sign}${Number(Math.abs(this.hours).toFixed(2))}`;
    }

    return `${sign}${wholeHours}:${wholeMinutes.toString(10).padStart(2, '0')}`;
  }
}

export const TransformAsDuration = (): ((target: any, key: string) => void) => {
  const toClass = Transform(({ value }) => (value != null ? new Duration(value) : undefined), {
    toClassOnly: true,
  });
  const toPlain = Transform(({ value }) => value?.seconds, {
    toPlainOnly: true,
  });

  return (target: any, key: string) => {
    toClass(target, key);
    toPlain(target, key);
  };
};
