import { Component, inject, signal } from '@angular/core';
import{FormsModule} from '@angular/forms';
import { AccountService } from '../../core/services/account-service';
import { Router, RouterLink, RouterLinkActive } from "@angular/router";
import { ToastService } from '../../core/services/toast-service';

@Component({
  selector: 'app-nav',
  imports: [FormsModule, RouterLink,RouterLinkActive],
  templateUrl: './nav.html',
  styleUrl: './nav.css'
})
export class Nav {
  protected accountService = inject(AccountService)
  
  protected creds: any = {}
  //protected loggedIn =signal(false)
  private router=inject(Router);
  private toast=inject(ToastService);

  login() {
    this.accountService.login(this.creds).subscribe({
      next:result=>{        
      this.router.navigateByUrl('/members');
      //this.loggedIn.set(true); already doing in app.ts
      this.toast.success("Logged in Successfully");
      this.creds={};
    },
      error:error=>{
       this.toast.error(error.error);
      }
    })
  }

  logout(){
   // this.loggedIn.set(false);
   this.accountService.logout();
   this.router.navigateByUrl('/');
  }
}


