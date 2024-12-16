import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
// import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  httpClient = inject(HttpClient)
  title = 'Dating App';
  users: any;
  ngOnInit(): void {
    this.httpClient.get('https://localhost:7165/api/users')
    .subscribe({
      next: response => this.users = response,
      error: error => console.log(error),
      complete: () => console.log('completed')
    })
  }
}
