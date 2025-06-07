import 'dart:ui';
import 'package:flutter/material.dart';
import 'models.dart';
import 'reels_service.dart';
import 'upload_dialog.dart';
import 'reel_video_page.dart';
import '../../style/style.dart';
import '../../../entities/app_state.dart';

class ReelsPage extends StatefulWidget {
  const ReelsPage({super.key});

  @override
  State<ReelsPage> createState() => _ReelsPageState();
}

class _ReelsPageState extends State<ReelsPage> {
  late Future<List<ReelWithVideo>> _reelsFuture;

  @override
  void initState() {
    super.initState();
    _reelsFuture = ReelsService.fetchReelsWithVideos();
  }

  Future<void> _uploadReel() async {
    final result = await UploadDialog.show(context);
    if (result == null) return;

    final fileBytes = result['fileBytes'] as List<int>;
    final fileName = result['fileName'] as String;

    String userId = '3fa85f64-5717-4562-b3fc-2c963f66afa6';
    if (AppState.currentUser != null) {
      userId =
          '3fa85f64-5717-4562-b3fc-2c963f66afa6';
    }

    final metadata = <String, String>{
      'userId': userId,
      'reelTitle': result['reelTitle'] as String,
      'reelDescription': result['reelDescription'] as String,
      'videoTitle': result['videoTitle'] as String,
      'videoDescription': result['videoDescription'] as String,
    };

    final success = await ReelsService.uploadReel(
      fileBytes: fileBytes,
      fileName: fileName,
      metadata: metadata,
    );

    if (mounted) {
      if (success) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Upload successful!')),
        );
        setState(() {
          _reelsFuture = ReelsService.fetchReelsWithVideos();
        });
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Upload failed!')),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Material(
      color: Colors.transparent,
      child: Stack(
        fit: StackFit.expand,
        children: [
          Positioned.fill(
            child: BackdropFilter(
              filter: ImageFilter.blur(sigmaX: 15, sigmaY: 15),
              child: Container(
                color: Colors.black.withOpacity(0.3),
              ),
            ),
          ),
          FutureBuilder<List<ReelWithVideo>>(
            future: _reelsFuture,
            builder: (context, snapshot) {
              if (snapshot.connectionState == ConnectionState.waiting) {
                return Container();
              }
              if (snapshot.hasError) {
                return Center(child: Text('Error: ${snapshot.error}'));
              }
              final reelsWithVideos = snapshot.data ?? [];
              if (reelsWithVideos.isEmpty) {
                return const Center(child: Text('No reels yet. Upload one!'));
              }

              return PageView.builder(
                scrollDirection: Axis.vertical,
                itemCount: reelsWithVideos.length,
                itemBuilder: (context, index) {
                  final reelWithVideo = reelsWithVideos[index];
                  final reel = reelWithVideo.reel;
                  final video = reelWithVideo.video;

                  return ReelVideoPage(
                    reel: reel,
                    video: video,
                  );
                },
              );
            },
          ),
          Positioned(
            top: 50,
            left: 20,
            child: SafeArea(
              child: Container(
                decoration: BoxDecoration(
                  color: baseColor.withOpacity(0.9),
                  borderRadius: BorderRadius.circular(20),
                  border: Border.all(
                    color: baseColor.withOpacity(0.3),
                    width: 1,
                  ),
                  boxShadow: [
                    BoxShadow(
                      color: Colors.black.withOpacity(0.3),
                      blurRadius: 8,
                      spreadRadius: 1,
                    ),
                  ],
                ),
                child: IconButton(
                  onPressed: () => Navigator.of(context).pop(),
                  icon: const Icon(
                    Icons.close,
                    color: Colors.white,
                    size: 24,
                  ),
                ),
              ),
            ),
          ),
          Positioned(
            top: 50,
            right: 20,
            child: SafeArea(
              child: Container(
                decoration: BoxDecoration(
                  color: baseColor.withOpacity(0.9),
                  borderRadius: BorderRadius.circular(20),
                  border: Border.all(
                    color: baseColor.withOpacity(0.3),
                    width: 1,
                  ),
                  boxShadow: [
                    BoxShadow(
                      color: Colors.black.withOpacity(0.3),
                      blurRadius: 8,
                      spreadRadius: 1,
                    ),
                  ],
                ),
                child: IconButton(
                  onPressed: _uploadReel,
                  icon: const Icon(Icons.upload, color: Colors.white),
                  tooltip: 'Upload Reel',
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }
}
