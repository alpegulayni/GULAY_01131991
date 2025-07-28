import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { VideoService } from './video.service';
import { Video } from '../models/video';

describe('VideoService', () => {
  let service: VideoService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [VideoService]
    });
    service = TestBed.inject(VideoService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should retrieve videos', () => {
    const mockVideos: Video[] = [
      {
        id: 1,
        title: 'Sample',
        description: 'Desc',
        fileName: 'sample.mp4',
        filePath: '/videos/sample.mp4',
        thumbnailPath: '/thumbnails/sample.jpg',
        categories: []
      }
    ];
    service.getVideos().subscribe(videos => {
      expect(videos).toEqual(mockVideos);
    });
    const req = httpMock.expectOne('/api/videos');
    expect(req.request.method).toBe('GET');
    req.flush(mockVideos);
  });
});