import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthenticationRoutingModule } from './authentication-routing.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { LoginComponent } from './login/login.component';
import { IonicModule } from '@ionic/angular';
import { RegisterComponent } from './register/register.component';

@NgModule({
  declarations: [
    LoginComponent,
    RegisterComponent
  ],
  imports: [
    AuthenticationRoutingModule,
    CommonModule,
    IonicModule,
    TranslateModule.forChild(),
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [
  ]
})
export class AuthenticationModule { }
