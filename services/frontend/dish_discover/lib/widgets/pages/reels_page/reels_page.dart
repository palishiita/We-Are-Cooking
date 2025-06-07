import 'package:flutter/material.dart';
import 'package:file_picker/file_picker.dart';
import 'models.dart';
import 'reels_service.dart';
import 'upload_dialog.dart';
import 'reel_video_page.dart';

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
    final result = await FilePicker.platform.pickFiles(type: FileType.video);
    if (result != null && result.files.single.bytes != null) {
      final fileBytes = result.files.single.bytes!;
      final fileName = result.files.single.name;

      final metadata = await UploadDialog.show(context);
      if (metadata == null) return;

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
            _reelsFuture =
                ReelsService.fetchReelsWithVideos(); // Refresh the list
          });
        } else {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text('Upload failed!')),
          );
        }
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Material(
      color: Colors.transparent,
      child: FutureBuilder<List<ReelWithVideo>>(
        future: _reelsFuture,
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return Container(); // No loading indicator
          }
          if (snapshot.hasError) {
            return Center(child: Text('Error: ${snapshot.error}'));
          }
          final reelsWithVideos = snapshot.data ?? [];
          if (reelsWithVideos.isEmpty) {
            return const Center(child: Text('No reels yet. Upload one!'));
          }

          return Stack(
            children: [
              PageView.builder(
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
              ),
              // Floating upload button
              Positioned(
                top: 50,
                right: 20,
                child: SafeArea(
                  child: FloatingActionButton(
                    onPressed: _uploadReel,
                    backgroundColor: Colors.white.withOpacity(0.8),
                    child: const Icon(Icons.upload, color: Colors.black),
                    tooltip: 'Upload Reel',
                  ),
                ),
              ),
            ],
          );
        },
      ),
    );
  }
}
