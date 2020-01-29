import { Component, Inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { Phase } from '@app/account/core';
import { Duration } from '@app/core/util';
import { Record, RecordEntry, WorkRecordService } from '@app/work-record/core';
import { FormRow } from './form-row';

export interface MultiEntriesDialogData {
  record: Record;
  phase: Phase;
}

@Component({
  selector: 'rolx-multi-entries-dialog',
  templateUrl: './multi-entries-dialog.component.html',
  styleUrls: ['./multi-entries-dialog.component.scss'],
})
export class MultiEntriesDialogComponent implements OnInit {

  form = this.fb.group({
    entries: this.fb.array([]),
  });

  formRows: FormRow[] = [];
  displayedColumns: string[] = [
    'mode',
    'begin',
    'end',
    'pause',
    'duration',
    'comment',
  ];

  constructor(@Inject(MAT_DIALOG_DATA) public data: MultiEntriesDialogData,
              private dialogRef: MatDialogRef<MultiEntriesDialogComponent>,
              private workRecordService: WorkRecordService,
              private fb: FormBuilder) {
    this.dialogRef.disableClose = true;
  }

  ngOnInit() {
    const entries = this.data.record.entriesOf(this.data.phase);

    if (entries.length) {
      entries
        .filter(e => !e.duration.isZero)
        .forEach(e => this.addRow(e));
    } else {
      this.addRow();
    }
  }

  get totalDuration(): Duration {
    return new Duration(
      this.formRows
        .filter(r => r.duration.value)
        .reduce((sum, r) => sum + r.duration.value.seconds, 0));
  }

  submit() {
    const entries = this.formRows
      .filter(r => r.hasDuration)
      .map(r => r.toEntry());

    const record = this.data.record.replaceEntriesOfPhase(this.data.phase, entries);
    this.workRecordService.update(record)
      .subscribe(() => {
        this.data.record.entries = record.entries;
        this.close();
      });
  }

  close() {
    this.dialogRef.close();
  }

  tryAddRow(index: number) {
    if (index === this.formRows.length - 1) {
      this.addRow();
    }
  }

  addRow(entry?: RecordEntry | null) {
    const row = new FormRow(entry);

    (this.form.controls.entries as FormArray).push(row.group);
    this.formRows = [...this.formRows, row];
  }

}