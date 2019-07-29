import {Injectable} from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { User } from '../_models/user';
import { UserService } from '../Services/user.service';
import { AlertifyService } from '../Services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class  MemberDetailsResolver implements Resolve<User> {

     constructor(private userService: UserService, private router: Router,
        private alertify: AlertifyService) {}

     resolve(route: ActivatedRouteSnapshot): Observable<User> {
          return this.userService.getUser(route.params['id']).pipe(
            catchError(error => {
                this.alertify.error(error);
                this.router.navigate(['/members']);
                return of(null);
            }));
          }
}
