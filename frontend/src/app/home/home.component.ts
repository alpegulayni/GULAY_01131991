import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { VideoService } from '../services/video.service';
import { Video } from '../models/video';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  standalone: true,
  imports: [CommonModule, RouterModule]
})
export class HomeComponent implements OnInit {
  videos: Video[] = [];
  loading = true;

  constructor(private videoService: VideoService) { }

  ngOnInit(): void {
    this.videoService.getVideos().subscribe({
      next: (data) => {
        this.videos = data;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }
}