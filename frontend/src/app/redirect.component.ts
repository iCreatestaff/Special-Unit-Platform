import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-redirect',
  standalone: true,
  template: '', // No UI needed
  imports: [CommonModule],
})
export class RedirectComponent implements OnInit {
  constructor(private router: Router) {}

  ngOnInit(): void {
    const accountId = localStorage.getItem('accountId');
    if (accountId) {
      this.router.navigate(['/dashboard']);
    } else {
      this.router.navigate(['/authentication/login']);
    }
  }
}
