import { Component, inject, OnInit } from '@angular/core';
import { RegisterComponent } from "../register/register.component";
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RegisterComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
registerMode = false;

users: any;
http = inject(HttpClient);
registerToggle() {
    this.registerMode = !this.registerMode
}
ngOnInit(){
  this.getUsers();
}

cancelRegisterMode(event: boolean) {
this.registerMode = event;
}

getUsers() {
  this.http.get('https://localhost:7165/api/users').subscribe({
    next: response => this.users = response,
    error: error => console.log(error),
    complete: () => console.log('request has been completed')
  })
}

}
