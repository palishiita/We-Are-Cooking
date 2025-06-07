import 'package:flutter/material.dart';
import 'dart:html' as html;
import 'dart:ui_web' as ui_web;

class VideoPlayerWeb extends StatefulWidget {
  final String url;
  final bool isFullScreen;
  final bool enableGestures;

  const VideoPlayerWeb({
    super.key,
    required this.url,
    this.isFullScreen = false,
    this.enableGestures = true,
  });

  @override
  State<VideoPlayerWeb> createState() => _VideoPlayerWebState();
}

class _VideoPlayerWebState extends State<VideoPlayerWeb> {
  html.VideoElement? _videoElement;
  bool _initialized = false;
  bool _showControls = false;
  bool _isPlaying = false;
  Duration _position = Duration.zero;
  Duration _duration = Duration.zero;
  String _viewId = '';

  @override
  void initState() {
    super.initState();
    _initializeVideo();
  }

  void _initializeVideo() {
    _viewId = 'video-${DateTime.now().millisecondsSinceEpoch}';
    
    _videoElement = html.VideoElement()
      ..src = widget.url
      ..autoplay = true
      ..loop = true
      ..muted = false
      ..controls = false
      ..style.width = '100%'
      ..style.height = '100%'
      ..style.objectFit = 'cover'
      ..style.backgroundColor = 'black';

    _videoElement!.onLoadedData.listen((_) {
      setState(() {
        _initialized = true;
        _duration = Duration(seconds: _videoElement!.duration.toInt());
      });
    });

    _videoElement!.onTimeUpdate.listen((_) {
      setState(() {
        _position = Duration(seconds: _videoElement!.currentTime.toInt());
      });
    });

    _videoElement!.onPlay.listen((_) {
      setState(() {
        _isPlaying = true;
      });
    });

    _videoElement!.onPause.listen((_) {
      setState(() {
        _isPlaying = false;
      });
    });    // Register the view
    ui_web.platformViewRegistry.registerViewFactory(_viewId, (int viewId) {
      return _videoElement!;
    });
  }

  @override
  void dispose() {
    _videoElement?.remove();
    super.dispose();
  }

  void _togglePlayPause() {
    if (_videoElement != null) {
      if (_isPlaying) {
        _videoElement!.pause();
      } else {
        _videoElement!.play();
      }
    }
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

  void _seekTo(double value) {
    if (_videoElement != null) {
      _videoElement!.currentTime = value;
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

    return Stack(
      fit: StackFit.expand,
      children: [
        GestureDetector(
          onTap: widget.enableGestures ? _toggleControls : null,
          child: HtmlElementView(viewType: _viewId),
        ),
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
                  _isPlaying ? Icons.pause : Icons.play_arrow,
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
                      value: _position.inSeconds.toDouble(),
                      min: 0.0,
                      max: _duration.inSeconds.toDouble(),
                      onChanged: (value) {
                        _seekTo(value);
                      },
                    ),
                  ),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Text(
                        _formatDuration(_position),
                        style: const TextStyle(
                          color: Colors.white,
                          fontSize: 12,
                        ),
                      ),
                      Text(
                        _formatDuration(_duration),
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
