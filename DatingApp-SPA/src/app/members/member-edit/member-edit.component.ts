import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { User } from 'src/app/_models/user';
import { NgForm } from '@angular/forms';
import { AlertifyService } from 'src/app/Services/alertify.service';
import { AuthService } from 'src/app/Services/auth.service';
import { UserService } from 'src/app/Services/user.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
@ViewChild('editForm') editForm: NgForm;
user: User;
photoUrl: string;
@HostListener('window:beforeunload', ['$event'])
unloadNotification($event: any) {
  if (this.editForm.dirty) {
    $event.returnValue = true;
  }
}
constructor(private route: ActivatedRoute, private alertify: AlertifyService,
    private authService: AuthService, private userService: UserService ) { }

ngOnInit() {
    this.route.data.subscribe(data => {
      this.user = data['user'];
    });
    this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl =  photoUrl);
  }
  updateUser() {
  this.userService.updateUser(this.authService.decodedToken.nameid, this.user).subscribe(next => {
    this.alertify.success('Profile Updated Successfully');
    this.editForm.reset(this.user );
  }, error => {
    console.log(this.user);
  });
  }
  updateMainPhoto(photoUrl) {
    this.user.photoUrl = photoUrl;
}
}
