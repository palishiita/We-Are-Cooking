import 'dart:convert';

import 'package:dio/dio.dart';
import 'package:http_parser/http_parser.dart';
import 'models.dart';
import '../../../services/api_client.dart';

class ReelsService {
  static final ApiClient _apiClient = ApiClient();

  static Map<String, Map<String, dynamic>> _createVideoMap(
      List<dynamic> videosData) {
    final Map<String, Map<String, dynamic>> videoMap = {};

    for (final videoData in videosData) {
      if (videoData != null &&
          videoData is Map<String, dynamic> &&
          videoData.containsKey('id')) {
        videoMap[videoData['id'].toString()] = videoData;
      }
    }

    return videoMap;
  }

  static Future<List<ReelWithVideo>> fetchReelsWithVideos() async {
    try {
      final response = await _apiClient.get('/api/reels/reel-videos');

      if (response.statusCode == 200) {
        if (response.data is! Map<String, dynamic>) {
          throw Exception(
              'Invalid response format: expected Map but got ${response.data.runtimeType}');
        }

        final Map<String, dynamic> responseData = response.data;
        if (!responseData.containsKey('reels') ||
            responseData['reels'] is! List) {
          throw Exception('Missing or invalid "reels" key in response');
        }

        if (!responseData.containsKey('videos') ||
            responseData['videos'] is! List) {
          throw Exception('Missing or invalid "videos" key in response');
        }
        final List<dynamic> reelsData = responseData['reels'];
        final List<dynamic> videosData = responseData['videos'];

        final videoMap = _createVideoMap(videosData);
        List<ReelWithVideo> reelsWithVideos = [];
        for (int i = 0; i < reelsData.length; i++) {
          final reelJson = reelsData[i];

          if (reelJson == null || reelJson is! Map<String, dynamic>) {
            continue;
          }

          try {
            final reel = Reel.fromJson(reelJson);
            final videoJson = videoMap[reel.videoId];

            if (videoJson != null) {
              try {
                final video = Video.fromJson(videoJson);
                final reelWithVideo = ReelWithVideo(reel: reel, video: video);

                if (reelWithVideo.isValid) {
                  reelsWithVideos.add(reelWithVideo);
                }
              } catch (videoError) {
                // Skip invalid videos
              }
            }
          } catch (reelError) {
            // Skip invalid reels
          }
        }

        return reelsWithVideos;
      } else {
        throw Exception(
            'Failed to load reels with videos: ${response.statusCode}');
      }
    } catch (e) {
      throw Exception('Failed to load reels with videos: $e');
    }
  }

  static Future<bool> uploadReel({
    required List<int> fileBytes,
    required String fileName,
    required Map<String, String> metadata,
  }) async {
    try {
      if (fileBytes.isEmpty) {
        return false;
      }

      if (fileName.isEmpty) {
        return false;
      }

      final requiredFields = [
        'videoTitle',
        'videoDescription',
        'reelTitle',
        'reelDescription'
      ];
      for (final field in requiredFields) {
        if (!metadata.containsKey(field) || metadata[field]!.isEmpty) {
          return false;
        }
      }

      final contentType = _getContentType(fileName);
      if (contentType == null) {
        return false;
      }

      final formData = FormData.fromMap({
        'file': MultipartFile.fromStream(
          () => Stream.fromIterable([fileBytes]),
          fileBytes.length,
          filename: fileName,
          contentType: contentType,
        ),
        'video': json.encode({
          'title': metadata['videoTitle'],
          'description': metadata['videoDescription'],
          'video_length_seconds': 0,
        }),
        'reel': json.encode({
          'title': metadata['reelTitle'],
          'description': metadata['reelDescription'],
        }),
      });

      final response = await _apiClient.uploadMultipart('/api/reels/reel-video',
          formData: formData);

      return response.statusCode == 200;
    } catch (e) {
      return false;
    }
  }

  static MediaType? _getContentType(String fileName) {
    if (fileName.isEmpty) return null;

    final extension = fileName.split('.').last.toLowerCase();
    switch (extension) {
      case 'mp4':
        return MediaType('video', 'mp4');
      case 'mov':
        return MediaType('video', 'quicktime');
      case 'avi':
        return MediaType('video', 'x-msvideo');
      case 'webm':
        return MediaType('video', 'webm');
      case 'mkv':
        return MediaType('video', 'x-matroska');
      default:
        return null;
    }
  }
}
