import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../Services/auth.service';
import { AlertifyService } from '../Services/alertify.service';
import { FormControl, FormGroup, Validators, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap';
import { User } from '../_models/user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Input() valuesFromHome: any;
  @Output() cancelRegister = new EventEmitter();
  bsConfig: Partial<BsDatepickerConfig>;
  colorTheme = 'theme-red';
  cancelMode = false;
  user: User;
  registerForm: FormGroup;
  constructor(private authService: AuthService, private router: Router,
    private alertifyService: AlertifyService, private fb: FormBuilder) { }

  ngOnInit() {
   this.createRegisterForm();
   this.bsConfig = Object.assign({}, { containerClass: this.colorTheme });
  }
  passwordMatchValidator(g: FormControl) {
    return g.get('password').value === g.get('confirmPassword').value ? null : {'mismatch': true};
  }

  createRegisterForm() {
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: [null, Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['',  [Validators.required, Validators.minLength(4), Validators.maxLength(16)]],
      confirmPassword: ['', Validators.required]
    }, {validator: this.passwordMatchValidator});
  }
  Register() {
   if (this.registerForm.valid) {
      this.user = Object.assign({}, this.registerForm.value);
      this.authService.Register(this.user).subscribe(() => {
        this.alertifyService.success('Registration successful');
      }, error => {
        this.alertifyService.error(error);
      }, () => {
         this.authService.login(this.user).subscribe(() => {
          // this.alertifyService.message('Navigating');
           this.router.navigate(['/members']);
         });
      });
   }
  }
  cancel() {
    this.cancelRegister.emit(false);
    this.alertifyService.warning('Canceling');
  }

}
