import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CategoryService } from '../services/category.service';
import { Category } from '../models/category';

/**
 * A component that lists all existing categories and allows the user to create new ones.
 */
@Component({
  selector: 'app-categories',
  templateUrl: './categories.component.html',
  standalone: true,
  imports: [CommonModule, FormsModule]
})
export class CategoriesComponent implements OnInit {
  categories: Category[] = [];
  newCategoryName = '';
  message = '';

  constructor(private categoryService: CategoryService) {}

  ngOnInit(): void {
    this.loadCategories();
  }

  /**
   * Refresh the list of categories from the API.
   */
  loadCategories(): void {
    this.categoryService.getCategories().subscribe(categories => {
      this.categories = categories;
    });
  }

  /**
   * Submit a new category to the API.
   */
  addCategory(): void {
    const name = this.newCategoryName.trim();
    if (!name) {
      this.message = 'Category name is required.';
      return;
    }
    this.categoryService.addCategory({ name }).subscribe({
      next: () => {
        this.message = 'Category added successfully.';
        this.newCategoryName = '';
        this.loadCategories();
      },
      error: (error) => {
        // Display an error message returned from the API, or a generic message.
        this.message = error?.error || 'Unable to add category.';
      }
    });
  }
}