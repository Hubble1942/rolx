import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { User, UserService } from '@app/user-management/core';
import { Observable, throwError } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';

@Component({
  selector: 'rolx-user-edit-page',
  templateUrl: './user-edit-page.component.html',
  styleUrls: ['./user-edit-page.component.scss'],
})
export class UserEditPageComponent implements OnInit {

  user$: Observable<User>;

  constructor(private route: ActivatedRoute,
              private router: Router,
              private userService: UserService) { }

  ngOnInit() {
    this.user$ = this.route.paramMap.pipe(
      switchMap(params => this.userService.getById(params.get('id'))),
      catchError(e => {
        if (e.status === 404) {
          // noinspection JSIgnoredPromiseFromCall
          this.router.navigate(['/four-oh-four']);
        }

        return throwError(e);
      }),
    );
  }

}
