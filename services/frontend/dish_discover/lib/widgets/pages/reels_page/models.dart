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
    if (json['id'] == null || json['id'].toString().isEmpty) {
      throw ArgumentError('Video id cannot be null or empty');
    }

    return Video(
      id: json['id'].toString(),
      title: json['title']?.toString() ?? '',
      description: json['description']?.toString() ?? '',
      videoLengthSeconds: json['video_length_seconds'] is int
          ? json['video_length_seconds']
          : int.tryParse(json['video_length_seconds']?.toString() ?? '0') ?? 0,
      videoUrl: json['video_url']?.toString() ?? '',
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'title': title,
      'description': description,
      'video_length_seconds': videoLengthSeconds,
      'video_url': videoUrl,
    };
  }

  bool get isValid => id.isNotEmpty && videoUrl.isNotEmpty;
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
    if (json['id'] == null || json['id'].toString().isEmpty) {
      throw ArgumentError('Reel id cannot be null or empty');
    }

    if (json['video_id'] == null || json['video_id'].toString().isEmpty) {
      throw ArgumentError('Reel video_id cannot be null or empty');
    }

    return Reel(
      id: json['id'].toString(),
      videoId: json['video_id'].toString(),
      title: json['title']?.toString() ?? '',
      description: json['description']?.toString() ?? '',
      creationTimestamp: json['creation_timestamp']?.toString() ?? '',
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'video_id': videoId,
      'title': title,
      'description': description,
      'creation_timestamp': creationTimestamp,
    };
  }

  bool get isValid => id.isNotEmpty && videoId.isNotEmpty;
}

class ReelWithVideo {
  final Reel reel;
  final Video video;
  ReelWithVideo({
    required this.reel,
    required this.video,
  });

  bool get isValid => reel.isValid && video.isValid;

  Map<String, dynamic> toJson() {
    return {
      'reel': reel.toJson(),
      'video': video.toJson(),
    };
  }
}
