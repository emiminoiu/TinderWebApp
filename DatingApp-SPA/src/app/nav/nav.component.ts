import { Component, OnInit } from '@angular/core';
import { AuthService } from '../Services/auth.service';
import { AlertifyService } from '../Services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  photoUrl: string;
  constructor(public authService: AuthService, private alertifyService: AlertifyService, private router: Router) { }

  ngOnInit() {
    this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
  }
  login() {
    this.authService.login(this.model).subscribe(next => {
       this.alertifyService.success('You logged in successfully!');
    }, err => {
       this.alertifyService.error('Something went wrong!');
    }, () => {
      this.router.navigate(['/members']);
    });
  }
  loggedIn() {
  //  return this.authService.loggedIn();
  const token = localStorage.getItem('token');
  const user = localStorage.getItem('user');
  this.authService.currentUser = JSON.parse(user);
   return !!token;
  }
  logOut() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.authService.decodedToken = null;
    this.authService.currentUser = null;
    this.alertifyService.warning('You logged out successfully!');
    this.router.navigate(['/home']);
  }
  }



