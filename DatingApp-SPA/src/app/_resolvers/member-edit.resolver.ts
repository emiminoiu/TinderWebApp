import {Injectable} from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { User } from '../_models/user';
import { UserService } from '../Services/user.service';
import { AlertifyService } from '../Services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../Services/auth.service';

@Injectable()
export class  MemberEditResolver implements Resolve<User> {

     constructor(private userService: UserService, private authService: AuthService, private router: Router,
        private alertify: AlertifyService) {}

     resolve(route: ActivatedRouteSnapshot): Observable<User> {
          return this.userService.getUser(this.authService.decodedToken.nameid).pipe(
            catchError(error => {
                this.alertify.error('Problem resolving edit data');
                this.router.navigate(['/members']);
                return of(null);
            }));
          }
}
