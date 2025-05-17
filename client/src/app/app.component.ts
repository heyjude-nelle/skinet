import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './layout/header/header.component';
import { HttpClient } from '@angular/common/http';
import { Product } from './shared/models/products';
import { Pagination } from './shared/models/pagination';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent implements OnInit {
  baseUrl = 'https://localhost:5001/api/';
  private readonly http = inject(HttpClient);
  title = 'Dante SkiStore';
  products: Product[] = [];

  ngOnInit(): void {
    interface GetProductsResponse extends Pagination<Product> {}

    this.http.get<GetProductsResponse>(this.baseUrl + 'products').subscribe({
      next: (response: GetProductsResponse) => (this.products = response.data),
      error: (error: any) => console.log(error),
      complete: () => console.log('complete'),
    });
  }
}
