# Use the official Dart image with Flutter preinstalled
FROM cirrusci/flutter:latest

# Set working directory inside the container
WORKDIR /app

# Copy pubspec files first to cache dependencies
COPY pubspec.yaml pubspec.lock ./

# Get dependencies
RUN flutter pub get

# Copy the rest of the project files
COPY . .

# Build the Flutter web app (outputs to build/web)
RUN flutter build web --release

# Use a lightweight web server to serve the web app
FROM nginx:alpine

# Remove default nginx website
RUN rm -rf /usr/share/nginx/html/*

# Copy built Flutter web app to nginx html directory
COPY --from=0 /app/build/web /usr/share/nginx/html

# Expose port 80
EXPOSE 80

# Start nginx
CMD ["nginx", "-g", "daemon off;"]