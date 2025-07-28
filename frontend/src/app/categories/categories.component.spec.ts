import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { CategoriesComponent } from './categories.component';
import { CategoryService } from '../services/category.service';
import { Category } from '../models/category';

describe('CategoriesComponent', () => {
  let component: CategoriesComponent;
  let fixture: ComponentFixture<CategoriesComponent>;
  let categoryServiceSpy: jasmine.SpyObj<CategoryService>;

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('CategoryService', ['getCategories', 'addCategory']);
    await TestBed.configureTestingModule({
      declarations: [ CategoriesComponent ],
      imports: [ FormsModule ],
      providers: [ { provide: CategoryService, useValue: spy } ]
    }).compileComponents();

    categoryServiceSpy = TestBed.inject(CategoryService) as jasmine.SpyObj<CategoryService>;
  });

  it('should load categories on init', () => {
    const mockCategories: Category[] = [ { id: 1, name: 'Test' } ];
    categoryServiceSpy.getCategories.and.returnValue(of(mockCategories));
    fixture = TestBed.createComponent(CategoriesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    expect(component.categories).toEqual(mockCategories);
  });

  it('should call addCategory on the service when addCategory is invoked', () => {
    fixture = TestBed.createComponent(CategoriesComponent);
    component = fixture.componentInstance;
    categoryServiceSpy.getCategories.and.returnValue(of([]));
    categoryServiceSpy.addCategory.and.returnValue(of({ id: 2, name: 'New' } as Category));
    fixture.detectChanges();
    component.newCategoryName = 'New';
    component.addCategory();
    expect(categoryServiceSpy.addCategory).toHaveBeenCalledWith({ name: 'New' });
  });
});