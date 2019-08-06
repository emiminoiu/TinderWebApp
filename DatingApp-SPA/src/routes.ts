import {Routes} from '@angular/router';
import { HomeComponent } from './app/home/home.component';
import { MemberListComponent } from './app/members/member-list/member-list.component';
import { MessagesComponent } from './app/messages/messages.component';
import { ListsComponent } from './app/lists/lists.component';
import { AuthGuard } from './app/_guards/auth.guard';
import { MemberDetailComponent } from './app/members/member-detail/member-detail.component';
import { MemberDetailsResolver } from './app/_resolvers/member-details.resolver';
import { MemberListResolver } from './app/_resolvers/member-list.resolver';
import { MemberEditComponent } from './app/members/member-edit/member-edit.component';
import { MemberEditResolver } from './app/_resolvers/member-edit.resolver';
import { PreventUnsavedChanges } from './app/_guards/prevent-unsaved-changes.guard';
import { ListResolver } from './app/_resolvers/list.resolver';



export const appRoutes: Routes = [
    { path: '', component: HomeComponent},
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            { path: 'members', component: MemberListComponent,  resolve: {users: MemberListResolver}},
            { path: 'messages', component: MessagesComponent},
            { path: 'member/edit', component: MemberEditComponent,
             resolve: {user: MemberEditResolver}, canDeactivate: [PreventUnsavedChanges]},
            { path: 'members/:id', component: MemberDetailComponent, resolve: {user: MemberDetailsResolver}},
            { path: 'lists', component: ListsComponent, resolve: {users: ListResolver}},

        ]
    },
    { path: '**', redirectTo: '', pathMatch: 'full'}
];

