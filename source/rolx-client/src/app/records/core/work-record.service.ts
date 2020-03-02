import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ErrorResponse } from '@app/core/error';
import { IsoDate, mapPlainToClassArray } from '@app/core/util';
import { environment } from '@env/environment';
import { classToPlain } from 'class-transformer';
import moment from 'moment';
import { Observable, ReplaySubject, Subject, throwError } from 'rxjs';
import { catchError, mapTo, switchMap, tap } from 'rxjs/operators';
import { Record } from './record';

const WorkRecordUrl = environment.apiBaseUrl + '/v1/workrecord';

@Injectable({
  providedIn: 'root',
})
export class WorkRecordService {
  private readonly userUpdatedSubject = new Subject<string>();
  private updateSequence = new ReplaySubject<number>(1);

  userUpdated$ = this.userUpdatedSubject.asObservable();

  constructor(private httpClient: HttpClient) {
    console.log('--- WorkRecordService.ctor()');

    this.updateSequence.next(1);
  }

  getMonth(month: moment.Moment): Observable<Record[]> {
    const url = WorkRecordUrl + '/month/' + month.format('YYYY-MM');
    return this.httpClient.get(url).pipe(
      mapPlainToClassArray(Record),
    );
  }

  getRange(begin: moment.Moment, end: moment.Moment): Observable<Record[]> {
    const url = WorkRecordUrl
      + '/range/'
      + IsoDate.fromMoment(begin)
      + '..'
      + IsoDate.fromMoment(end);

    return this.httpClient.get<any[]>(url).pipe(
      mapPlainToClassArray(Record),
    );
  }

  update(record: Record): Observable<Record> {
    const currentSequence = this.updateSequence;
    const nextSequence = new ReplaySubject<number>(1);
    this.updateSequence = nextSequence;

    return currentSequence.pipe(
      switchMap(() => this.internalUpdate(record)),
      tap(() => nextSequence.next(0)),
      catchError(e => {
        nextSequence.next(0);
        return throwError(e);
      }),
    );
  }

  private internalUpdate(record: Record): Observable<Record> {
    const url = WorkRecordUrl + '/' + IsoDate.fromMoment(record.date);
    return this.httpClient.put(url, classToPlain(record)).pipe(
      mapTo(record),
      tap(r => this.userUpdatedSubject.next(r.userId)),
      catchError(e => throwError(new ErrorResponse(e))),
    );
  }
}