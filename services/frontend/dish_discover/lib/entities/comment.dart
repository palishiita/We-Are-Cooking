import "dart:convert";

import "package:dish_discover/entities/user.dart";
import "package:flutter/foundation.dart";
import "package:flutter/material.dart";
import "package:http/http.dart";

import "app_state.dart";

class Comment extends ChangeNotifier {
  final int id;
  final String author;
  Image? authorAvatar;
  final int recipeId;
  String content;
  User? user;

  Comment(
      {required this.author,
      this.authorAvatar,
      required this.recipeId,
      required this.id,
      this.content = '',
      this.user});

  factory Comment.fromJson(Map<String, dynamic> json) {
    return Comment(
      id: json['commentId'],
      author: json['authorId'],
      recipeId: json['recipeId'],
      content: json['content'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'commentId': id,
      'authorId': author,
      'recipeId': recipeId,
      'content': content,
    };
  }

  static Future<void> addComment(Comment comment) async {
    final Response response = await post(
      Uri.parse('http://${AppState.serverDomain}/api/comments'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode(comment.toJson()),
    );

    if (response.statusCode == 201) {
      if (kDebugMode) {
        print('Comment added successfully');
      }
    } else {
      throw Exception(
          'Failed to add comment, status code: ${response.statusCode}');
    }
  }

  static Future<List<Comment>> getComments(int recipeId) async {
    final Response response = await get(Uri.parse(
        'http://${AppState.serverDomain}/api/recipes/$recipeId/comments'));

    if (response.statusCode == 200) {
      final List data = json.decode(response.body)['comments'];
      return data.map((item) => Comment.fromJson(item)).toList();
    } else {
      throw Exception(
          'Failed to load comments, status code: ${response.statusCode}');
    }
  }

  Map<String, dynamic> getAuthor() {
    return {
      'authorId': user?.username,
      'image': user?.image,
    };
  }
}
