# Build and run the Angular frontend

# Stage 1: build the application
FROM node:20 AS build
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm install
COPY . .
# Build the production assets.  The --configuration=production flag enables optimizations.
RUN npx ng build --configuration=production --output-path=dist

# Stage 2: serve with nginx
FROM nginx:alpine
COPY --from=build /app/dist/ /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]