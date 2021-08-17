import { NgModule } from '@angular/core';
import { PreloadAllModules, RouterModule, Routes } from '@angular/router';
import { AuthGuardService } from './services/api/auth-guard.service';
import { AuthNegateGuardService } from './services/api/auth-negate-guard.service';

const routes: Routes = [
  {
    path: '',
    loadChildren: () => import('./modules/tabs/tabs.module').then( m => m.TabsModule),
    // TODO: add auth guard
    canActivate: [AuthGuardService]
  },
  {
    path: 'welcome',
    canActivate: [AuthNegateGuardService],
    loadChildren: () => import('./modules/authentication/authentication.module').then( m => m.AuthenticationModule)
  }
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes, { preloadingStrategy: PreloadAllModules })
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
