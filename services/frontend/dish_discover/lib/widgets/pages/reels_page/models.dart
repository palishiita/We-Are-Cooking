class Video {
  final String id;
  final String title;
  final String description;
  final int videoLengthSeconds;
  final String videoUrl;

  Video({
    required this.id,
    required this.title,
    required this.description,
    required this.videoLengthSeconds,
    required this.videoUrl,
  });

  factory Video.fromJson(Map<String, dynamic> json) {
    return Video(
      id: json['id'],
      title: json['title'],
      description: json['description'],
      videoLengthSeconds: json['video_length_seconds'],
      videoUrl: json['video_url'],
    );
  }
}

class Reel {
  final String id;
  final String videoId;
  final String title;
  final String description;
  final String creationTimestamp;

  Reel({
    required this.id,
    required this.videoId,
    required this.title,
    required this.description,
    required this.creationTimestamp,
  });

  factory Reel.fromJson(Map<String, dynamic> json) {
    return Reel(
      id: json['id'],
      videoId: json['video_id'],
      title: json['title'],
      description: json['description'],
      creationTimestamp: json['creation_timestamp'],
    );
  }
}

class ReelWithVideo {
  final Reel reel;
  final Video video;

  ReelWithVideo({
    required this.reel,
    required this.video,
  });
}
