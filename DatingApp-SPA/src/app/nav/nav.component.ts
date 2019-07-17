import { Component, OnInit } from '@angular/core';
import { AuthService } from '../Services/auth.service';
import { AlertifyService } from '../Services/alertify.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  constructor(private authService: AuthService, private alertifyService: AlertifyService) { }

  ngOnInit() {
  }
  login() {
    this.authService.login(this.model).subscribe(next => {
       this.alertifyService.success('You logged in successfully!');
    }, err => {
       this.alertifyService.error('Something went wrong!');
    });
  }
  loggedIn() {
   const token = localStorage.getItem('token');
   return !!token;
  }
  logOut() {
    localStorage.removeItem('token');
    this.alertifyService.warning('You logged out successfully!');
  }
  }



