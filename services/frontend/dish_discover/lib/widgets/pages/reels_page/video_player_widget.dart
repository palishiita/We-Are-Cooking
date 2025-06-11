import 'package:flutter/material.dart';
import 'video_player_web.dart';

class VideoPlayerWidget extends StatelessWidget {
  final String url;
  final bool isFullScreen;
  final bool enableGestures;

  const VideoPlayerWidget({
    super.key,
    required this.url,
    this.isFullScreen = false,
    this.enableGestures = true,
  });

  @override
  Widget build(BuildContext context) {
    return VideoPlayerWeb(
      url: url,
      isFullScreen: isFullScreen,
      enableGestures: enableGestures,
    );
  }
}
