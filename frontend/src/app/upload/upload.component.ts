import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { VideoService } from '../services/video.service';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule]
})
export class UploadComponent {
  uploadError = '';

  uploadForm = this.fb.group({
    title: ['', Validators.required],
    description: [''],
    categories: [''],
    file: [null as File | null, Validators.required]
  });

  constructor(private fb: FormBuilder, private videoService: VideoService, private router: Router) { }

  onFileChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      const allowed = ['video/mp4', 'video/avi', 'video/x-msvideo', 'video/quicktime', 'video/mov'];
      if (!allowed.includes(file.type)) {
        this.uploadError = 'Only MP4, AVI and MOV files are allowed.';
        this.uploadForm.patchValue({ file: null });
        return;
      }
      if (file.size > 100 * 1024 * 1024) {
        this.uploadError = 'Maximum file size is 100MB.';
        this.uploadForm.patchValue({ file: null });
        return;
      }
      this.uploadError = '';
      this.uploadForm.patchValue({ file: file });
    }
  }

  onSubmit(): void {
    if (this.uploadForm.invalid || this.uploadError) {
      return;
    }
    const formData = new FormData();
    formData.append('Title', this.uploadForm.get('title')?.value || '');
    formData.append('Description', this.uploadForm.get('description')?.value || '');
    const catsRaw = this.uploadForm.get('categories')?.value || '';
    const cats = catsRaw
      .split(',')
      .map((c: string) => c.trim())
      .filter((c: string) => c.length);
    for (const c of cats) {
      formData.append('Categories', c);
    }
    const file = this.uploadForm.get('file')?.value as File;
    formData.append('File', file);
    this.videoService.uploadVideo(formData).subscribe({
      next: (video) => {
        this.router.navigate(['/stream', video.id]);
      },
      error: (err) => {
        this.uploadError = err.error?.error || 'Upload failed.';
      }
    });
  }
}