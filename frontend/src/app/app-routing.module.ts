import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { UploadComponent } from './upload/upload.component';
import { StreamComponent } from './stream/stream.component';
import { CategoriesComponent } from './categories/categories.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'upload', component: UploadComponent },
  { path: 'categories', component: CategoriesComponent },
  { path: 'stream/:id', component: StreamComponent },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes),
    // Import standalone components referenced by the routes so the router can resolve them
    HomeComponent,
    UploadComponent,
    StreamComponent,
    CategoriesComponent
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }