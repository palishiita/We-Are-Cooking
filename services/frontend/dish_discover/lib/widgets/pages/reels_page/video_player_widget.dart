import 'package:flutter/material.dart';
import 'package:video_player/video_player.dart';
import 'package:flutter/foundation.dart';
import 'video_player_web.dart';

class VideoPlayerWidget extends StatefulWidget {
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
  State<VideoPlayerWidget> createState() => _VideoPlayerWidgetState();
}

class _VideoPlayerWidgetState extends State<VideoPlayerWidget> {
  @override
  Widget build(BuildContext context) {
    if (kIsWeb) {
      return VideoPlayerWeb(
        url: widget.url,
        isFullScreen: widget.isFullScreen,
        enableGestures: widget.enableGestures,
      );
    } else {
      return _VideoPlayerMobile(
        url: widget.url,
        isFullScreen: widget.isFullScreen,
        enableGestures: widget.enableGestures,
      );
    }
  }
}

class _VideoPlayerMobile extends StatefulWidget {
  final String url;
  final bool isFullScreen;
  final bool enableGestures;

  const _VideoPlayerMobile({
    required this.url,
    this.isFullScreen = false,
    this.enableGestures = true,
  });

  @override
  State<_VideoPlayerMobile> createState() => _VideoPlayerMobileState();
}

class _VideoPlayerMobileState extends State<_VideoPlayerMobile> {
  late VideoPlayerController _controller;
  bool _initialized = false;
  bool _showControls = false;

  @override
  void initState() {
    super.initState();
    _controller = VideoPlayerController.networkUrl(Uri.parse(widget.url))
      ..initialize().then((_) {
        setState(() {
          _initialized = true;
        });
        _controller.setLooping(true);
        _controller.play();
      });
    
    _controller.addListener(() {
      setState(() {});
    });
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  void _togglePlayPause() {
    setState(() {
      _controller.value.isPlaying ? _controller.pause() : _controller.play();
    });
  }

  void _toggleControls() {
    setState(() {
      _showControls = !_showControls;
    });
    
    if (_showControls) {
      Future.delayed(const Duration(seconds: 3), () {
        if (mounted) {
          setState(() {
            _showControls = false;
          });
        }
      });
    }
  }

  String _formatDuration(Duration duration) {
    String twoDigits(int n) => n.toString().padLeft(2, "0");
    String twoDigitMinutes = twoDigits(duration.inMinutes.remainder(60));
    String twoDigitSeconds = twoDigits(duration.inSeconds.remainder(60));
    return "${twoDigits(duration.inHours)}:$twoDigitMinutes:$twoDigitSeconds";
  }

  @override
  Widget build(BuildContext context) {
    if (!_initialized) {
      return Container(
        color: Colors.black,
        child: const Center(
          child: SizedBox.shrink(),
        ),
      );
    }

    Widget videoPlayer = SizedBox.expand(
      child: FittedBox(
        fit: BoxFit.cover,
        child: SizedBox(
          width: _controller.value.size.width,
          height: _controller.value.size.height,
          child: VideoPlayer(_controller),
        ),
      ),
    );

    Widget content = widget.enableGestures
        ? GestureDetector(
            onTap: _toggleControls,
            child: videoPlayer,
          )
        : videoPlayer;

    return Stack(
      fit: StackFit.expand,
      children: [
        content,
        if (_showControls && widget.enableGestures) ...[
          Container(
            color: Colors.black.withOpacity(0.3),
          ),
          Center(
            child: Container(
              decoration: BoxDecoration(
                color: Colors.black.withOpacity(0.6),
                borderRadius: BorderRadius.circular(50),
              ),
              child: IconButton(
                onPressed: _togglePlayPause,
                icon: Icon(
                  _controller.value.isPlaying ? Icons.pause : Icons.play_arrow,
                  color: Colors.white,
                  size: 50,
                ),
              ),
            ),
          ),
          Positioned(
            bottom: 20,
            left: 16,
            right: 16,
            child: Container(
              padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
              decoration: BoxDecoration(
                color: Colors.black.withOpacity(0.7),
                borderRadius: BorderRadius.circular(20),
              ),
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  SliderTheme(
                    data: SliderTheme.of(context).copyWith(
                      trackHeight: 4.0,
                      thumbShape: const RoundSliderThumbShape(enabledThumbRadius: 8.0),
                      overlayShape: const RoundSliderOverlayShape(overlayRadius: 16.0),
                      activeTrackColor: Colors.white,
                      inactiveTrackColor: Colors.white.withOpacity(0.3),
                      thumbColor: Colors.white,
                      overlayColor: Colors.white.withOpacity(0.2),
                    ),
                    child: Slider(
                      value: _controller.value.position.inMilliseconds.toDouble(),
                      min: 0.0,
                      max: _controller.value.duration.inMilliseconds.toDouble(),
                      onChanged: (value) {
                        _controller.seekTo(Duration(milliseconds: value.round()));
                      },
                    ),
                  ),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Text(
                        _formatDuration(_controller.value.position),
                        style: const TextStyle(
                          color: Colors.white,
                          fontSize: 12,
                        ),
                      ),
                      Text(
                        _formatDuration(_controller.value.duration),
                        style: const TextStyle(
                          color: Colors.white,
                          fontSize: 12,
                        ),
                      ),
                    ],
                  ),
                ],
              ),
            ),
          ),
        ],
      ],
    );
  }
}
