# Use the Flutter "stable" tag (includes Dart SDK 3.3.3 as of Flutter 3.19.5)
FROM ghcr.io/cirruslabs/flutter:stable

# Set working directory inside the container
WORKDIR /app

COPY dish_discover .

# Get dependencies
RUN flutter pub get

# Build the Flutter web app
RUN flutter build web --release

# Use a lightweight web server to serve the app
FROM nginx:alpine

# Remove default nginx content
RUN rm -rf /usr/share/nginx/html/*

# Copy built app to nginx
COPY --from=0 /app/build/web /usr/share/nginx/html

# Expose port 80
EXPOSE 80

# Start nginx
CMD ["nginx", "-g", "daemon off;"]