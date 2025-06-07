import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:file_picker/file_picker.dart';
import 'models.dart';

class ReelsService {
  static const String baseUrl = 'http://localhost:7001';

  static Future<List<ReelWithVideo>> fetchReelsWithVideos() async {
    try {
      final response = await http.get(
        Uri.parse('$baseUrl/reel-videos'),
        headers: {'Content-Type': 'application/json'},
      );

      print('Response status: ${response.statusCode}');
      print('Response body: ${response.body}');

      if (response.statusCode == 200) {
        final Map<String, dynamic> responseData = json.decode(response.body);
        final List<dynamic> reelsData = responseData['reels'];
        final List<dynamic> videosData = responseData['videos'];

        List<ReelWithVideo> reelsWithVideos = [];
        for (var reelJson in reelsData) {
          final reel = Reel.fromJson(reelJson);
          final videoJson = videosData.firstWhere(
            (video) => video['id'] == reel.videoId,
            orElse: () => null,
          );

          if (videoJson != null) {
            final video = Video.fromJson(videoJson);
            reelsWithVideos.add(ReelWithVideo(reel: reel, video: video));
          }
        }

        return reelsWithVideos;
      } else {
        throw Exception(
            'Failed to load reels with videos: ${response.statusCode}');
      }
    } catch (e) {
      print('Error fetching reels with videos: $e');
      throw Exception('Failed to load reels with videos: $e');
    }
  }

  static Future<bool> uploadReel({
    required List<int> fileBytes,
    required String fileName,
    required Map<String, String> metadata,
  }) async {
    try {
      final uri = Uri.parse('$baseUrl/reel-video');
      final request = http.MultipartRequest('POST', uri);

      request.files.add(http.MultipartFile.fromBytes(
        'file',
        fileBytes,
        filename: fileName,
      ));

      final videoJson = json.encode({
        'posting_user_id': metadata['userId'],
        'title': metadata['videoTitle'],
        'description': metadata['videoDescription'],
        'video_length_seconds': 0,
      });
      request.fields['video'] = videoJson;

      final reelJson = json.encode({
        'posting_user_id': metadata['userId'],
        'title': metadata['reelTitle'],
        'description': metadata['reelDescription'],
      });
      request.fields['reel'] = reelJson;

      final response = await request.send();
      return response.statusCode == 200;
    } catch (e) {
      print('Upload error: $e');
      return false;
    }
  }
}
