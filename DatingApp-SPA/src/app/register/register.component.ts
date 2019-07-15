import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../Services/auth.service';

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
  constructor(private authService: AuthService) { }

  ngOnInit() {
  }

  Register() {
   this.authService.Register(this.model).subscribe(() => {
     console.log('Registration Successful');
   }, err => {
     console.log(err);
   });
  }
  cancel() {
    this.cancelRegister.emit(false);
  }

}
