import 'package:flutter/material.dart';
import 'models.dart';
import 'video_player_widget.dart';
import '../../style/style.dart';

class ReelVideoPage extends StatelessWidget {
  final Reel reel;
  final Video video;

  const ReelVideoPage({
    super.key,
    required this.reel,
    required this.video,
  });

  @override
  Widget build(BuildContext context) {
    final screenWidth = MediaQuery.of(context).size.width;
    final screenHeight = MediaQuery.of(context).size.height;
    final videoWidth = screenWidth * 0.7;
    final verticalPadding = 40.0;

    return Stack(
      fit: StackFit.expand,
      children: [
        Center(
          child: Container(
            width: videoWidth,
            height: screenHeight - (verticalPadding * 2),
            margin: EdgeInsets.symmetric(vertical: verticalPadding),
            decoration: BoxDecoration(
              borderRadius: BorderRadius.circular(12),
              boxShadow: [
                BoxShadow(
                  color: Colors.black.withOpacity(0.5),
                  blurRadius: 20,
                  spreadRadius: 5,
                ),
              ],
            ),
            child: ClipRRect(
              borderRadius: BorderRadius.circular(12),
              child: VideoPlayerWidget(
                url: 'http://localhost:7001${video.videoUrl}',
                isFullScreen: false,
                enableGestures: true,
              ),
            ),
          ),
        ),
        Positioned(
          bottom: verticalPadding + 20,
          left: (screenWidth - videoWidth) / 2 + 16,
          right: (screenWidth - videoWidth) / 2 + 80,
          child: Container(
            padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
            decoration: BoxDecoration(
              color: baseColor.withOpacity(0.7),
              borderRadius: BorderRadius.circular(16),
              border: Border.all(
                color: baseColor.withOpacity(0.2),
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
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  reel.title,
                  style: const TextStyle(
                    color: Colors.white,
                    fontSize: 16,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                const SizedBox(height: 6),
                Text(
                  reel.description,
                  style: const TextStyle(
                    color: Colors.white,
                    fontSize: 14,
                  ),
                  maxLines: 3,
                  overflow: TextOverflow.ellipsis,
                ),
              ],
            ),
          ),
        ),
        Positioned(
          bottom: verticalPadding + 20,
          right: (screenWidth - videoWidth) / 2 + 16,
          child: Column(
            children: [
              _buildActionButton(
                icon: Icons.favorite_border,
                color: likeColor,
                onPressed: () {},
              ),
              const SizedBox(height: 12),
              _buildActionButton(
                icon: Icons.comment,
                color: baseColor,
                onPressed: () {},
              ),
              const SizedBox(height: 12),
              _buildActionButton(
                icon: Icons.share,
                color: saveColor,
                onPressed: () {},
              ),
            ],
          ),
        ),
      ],
    );
  }

  Widget _buildActionButton({
    required IconData icon,
    required VoidCallback onPressed,
    Color? color,
  }) {
    final buttonBgColor = color ?? baseColor;
    return Container(
      decoration: BoxDecoration(
        color: buttonBgColor.withOpacity(0.8),
        borderRadius: BorderRadius.circular(25),
        border: Border.all(
          color: buttonBgColor.withOpacity(0.2),
          width: 1,
        ),
        boxShadow: [
          BoxShadow(
            color: Colors.black.withOpacity(0.2),
            blurRadius: 6,
            spreadRadius: 1,
          ),
        ],
      ),
      child: IconButton(
        onPressed: onPressed,
        icon: Icon(
          icon,
          color: Colors.white,
          size: 26,
        ),
      ),
    );
  }
}
