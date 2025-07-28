import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Category } from '../models/category';
import { environment } from '../environments/environment';

/**
 * Service for retrieving and creating categories.
 */
@Injectable({ providedIn: 'root' })
export class CategoryService {
  private readonly baseUrl = `${environment.apiUrl}/api/categories`;

  constructor(private http: HttpClient) {}

  /**
   * Fetch the list of all categories from the API.
   */
  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(this.baseUrl);
  }

  /**
   * Add a new category by sending its name to the API.
   * @param category Partial category containing at least a name property.
   */
  addCategory(category: { name: string }): Observable<Category> {
    return this.http.post<Category>(this.baseUrl, category);
  }
}