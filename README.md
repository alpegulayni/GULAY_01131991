# Video Upload & Streaming Application

This repository contains a simple full–stack web application that allows users to upload videos and watch them back in the browser.  The backend API is implemented with **.NET 8** while the frontend is built using **Angular 20.1.3**.  Uploaded videos are persisted to disk on the server and metadata is stored in a SQLite database via Entity Framework Core.  When a new video is uploaded the server generates a small thumbnail image, saves it alongside the video file and records everything in the database.  Categories can be assigned to videos and duplicate category names are prevented on the server.

## Features

- **Home page** — Lists all available videos with thumbnails, truncated descriptions (up to 160 characters) and a comma‑separated list of categories.  Hovering over a thumbnail displays the full video title as a tooltip.  Clicking the “Watch” button navigates to the streaming page.
- **Upload page** — Provides a form to submit a new video.  Users can specify a title, optional description, categories (comma separated) and select a video file.  Only MP4, AVI and MOV files up to 100 MB are accepted.  After a successful upload the app redirects to the streaming page for the newly uploaded video.
- **Streaming page** — Streams the selected video using the browser’s `<video>` element and displays its metadata and categories.
- **Backend API** — Exposes endpoints to list videos, upload new videos, download thumbnails and stream the original video files.  The API validates file size and type, creates thumbnails and ensures category names are unique.
- **Docker support** — Both the backend and frontend can be run in containers using Docker and Docker Compose.

## Project Structure

- `backend/` — Source code for the ASP.NET Core Web API.  The `VideoApi` project contains models, data access, services and controllers.  A `Dockerfile` is provided to build and run the API in a container.
- `frontend/` — Source code for the Angular application.  Components, services and routing are organised under `src/app`.  A `Dockerfile` builds the Angular project and serves it via Nginx.
- `docker-compose.yml` — Defines two services (`backend` and `frontend`) that can be started together.  Video uploads are persisted to `backend/UploadedVideos` on the host machine.
- `README.md` — This file.

## Prerequisites

To build and run the solution locally (without Docker) you’ll need:

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Node.js](https://nodejs.org/) (v18+ recommended) and [npm](https://www.npmjs.com/)
- Angular CLI (`npm install -g @angular/cli@20.1.3`)

## Running Locally

1. **Clone the repository**

   ```bash
   git clone https://github.com/alpegulayni/GULAY_01131991.git
   cd GULAY_01131991
   ```

2. **Backend**

   ```bash
   cd backend/VideoApi
   dotnet restore
   dotnet build
   dotnet run
   ```

   By default the API listens on `https://localhost:5001` and `http://localhost:5000`.  You can change the ports in `launchSettings.json` or via environment variables.

   Once the API is running, you can explore its endpoints using the integrated **Swagger** UI.  Navigate to `http://localhost:5000/swagger` (or the corresponding port) to view and interact with the automatically generated OpenAPI documentation.  The raw JSON specification is also available at `/swagger/v1/swagger.json`.

3. **Frontend**

   In a separate terminal:

   ```bash
   cd ../frontend
   npm install
   ng serve --proxy-config proxy.conf.json
   ```

   The Angular development server runs on `http://localhost:4200` and proxies API requests to `http://localhost:5000`.

4. **Using the application**

   - Navigate to `http://localhost:4200` to view the home page.
   - Use the “Upload” button in the top right to navigate to the upload form.  Select a supported video file (MP4, AVI or MOV), add optional details and submit.  After the upload completes you will be redirected to the streaming page for that video.
   - Return to the home page to see the list of uploaded videos.

## Running with Docker

Ensure that [Docker](https://www.docker.com/) and [Docker Compose](https://docs.docker.com/compose/) are installed on your system.  Then run:

```bash
docker-compose up --build
```

This command builds the backend and frontend images and starts two containers:

- **backend** — ASP.NET Core Web API listening on port 5000.
- **frontend** — Nginx serving the compiled Angular application on port 4200.

Uploaded videos and thumbnails are stored on your host machine under `backend/UploadedVideos` so that data persists across container restarts.

Navigate to `http://localhost:4200` in your browser to access the application.

## Migrations and Database

The application uses SQLite for simplicity.  When the API starts it automatically creates the `app.db` database file if it doesn’t already exist.  You can inspect or reset this file by deleting `backend/VideoApi/app.db`.  If you choose to migrate to a different database provider (e.g. SQL Server or PostgreSQL) update the connection string in `appsettings.json` and install the corresponding EF Core provider in `VideoApi.csproj`.

## Notes

- Thumbnail generation uses the `Xabe.FFmpeg` library, which requires FFmpeg binaries to be available at runtime.  In environments where FFmpeg is not installed the service falls back to writing a blank JPEG image so that a thumbnail file is still created.
- The code here is intended as an illustrative reference implementation.  Before deploying to production you should harden security (e.g. authentication and authorisation), improve error handling and add unit/integration tests.
