import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { VideoService } from '../services/video.service';
import { Video } from '../models/video';

@Component({
  selector: 'app-stream',
  templateUrl: './stream.component.html',
  standalone: true,
  imports: [CommonModule, RouterModule]
})
export class StreamComponent implements OnInit {
  video?: Video;
  loading = true;

  constructor(private route: ActivatedRoute, private videoService: VideoService) { }

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    const id = idParam ? parseInt(idParam, 10) : NaN;
    if (!isNaN(id)) {
      this.videoService.getVideo(id).subscribe({
        next: v => {
          this.video = v;
          this.loading = false;
        },
        error: () => {
          this.loading = false;
        }
      });
    } else {
      this.loading = false;
    }
  }
}