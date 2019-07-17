import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../Services/auth.service';
import { AlertifyService } from '../Services/alertify.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Input() valuesFromHome: any;
  @Output() cancelRegister = new EventEmitter();
  cancelMode = false;
  model: any = {};
  constructor(private authService: AuthService,  private alertifyService: AlertifyService) { }

  ngOnInit() {
  }

  Register() {
   this.authService.Register(this.model).subscribe(() => {
    this.alertifyService.success('Your account was registred successful!');
   }, err => {
    this.alertifyService.error('Something went wrong!');
   });
  }
  cancel() {
    this.cancelRegister.emit(false);
  }

}
