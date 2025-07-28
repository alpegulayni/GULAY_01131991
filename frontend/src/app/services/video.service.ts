import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { Video } from '../models/video';
import { environment } from '../environments/environment';

@Injectable({ providedIn: 'root' })
export class VideoService {
  private readonly baseUrl = `${environment.apiUrl}/api/videos`;
  constructor(private http: HttpClient) { }

  getVideos(): Observable<Video[]> {
    return this.http.get<Video[]>(this.baseUrl);
  }

  uploadVideo(formData: FormData): Observable<Video> {
    return this.http.post<Video>(`${this.baseUrl}/upload`, formData);
  }

  getVideo(id: number): Observable<Video> {
    return this.getVideos().pipe(
      map(videos => videos.find(v => v.id === id) as Video)
    );
  }
}