import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import {BehaviorSubject} from 'rxjs';
import { User } from '../_models/user';
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl = 'http://localhost:5000/api/auth';
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  currentUser: User;
  photoUrl = new BehaviorSubject<string>('../../user.png');
  currentPhotoUrl = this.photoUrl.asObservable();

  constructor(private http: HttpClient) {}
  login(model: any) {
    return this.http.post(this.baseUrl + '/login', model).pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem('token', user.token);
          localStorage.setItem('user', JSON.stringify(user.user));
          this.currentUser = user.user;
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          this.changeMemberPhoto(this.currentUser.photoUrl);
        }
      })
    );
  }

  changeMemberPhoto(photoUrl) {
    this.photoUrl.next(photoUrl);
  }
  Register(model: any) {
    return this.http.post(this.baseUrl + '/register', model);
  }
  loggedIn() {
    const token = localStorage.getItem('token');
    // return this.jwtHelper.isTokenExpired(token);
    return !!token;
  }

}
