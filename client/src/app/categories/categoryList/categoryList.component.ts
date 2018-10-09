import { Component, OnInit } from '@angular/core';
import { CategoryService } from '../../services/category.service';
import { Category } from "../../model/category";

@Component({
    selector: 'app-category-list',
    templateUrl: './CategoryList.component.html',
    styleUrls: ['./CategoryList.component.css']
})
export class CategoryListComponent implements OnInit {
    categories: Category[];

    constructor(private categoryService: CategoryService) {
    }

    ngOnInit() {
        this.getCategories();
    }

    getCategories(): void {
        this.categoryService.getCategories()
            .subscribe(categories => this.categories = categories);
    }

    add(title: string): void {
        title = title.trim();
        if (!title) { return; }
        this.categoryService.addCategory({ title } as Category)
            .subscribe(() => this.getCategories());
    }

    delete(id: number): void {
        this.categoryService.deleteCategory(id).subscribe(() => this.categories = this.categories.filter(cat => cat.id !== id));
    }
}
