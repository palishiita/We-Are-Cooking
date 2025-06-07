import 'package:flutter/material.dart';

class UploadDialog {
  static Future<Map<String, String>?> show(BuildContext context) async {
    final reelTitleController = TextEditingController();
    final reelDescriptionController = TextEditingController();
    final videoTitleController = TextEditingController();
    final videoDescriptionController = TextEditingController();
    final userIdController = TextEditingController(text: 'user-123');
    return showDialog<Map<String, String>>(
      context: context,
      builder: (BuildContext context) {
        return AlertDialog(
          title: const Text('Upload Reel'),
          content: SingleChildScrollView(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                TextField(
                  controller: userIdController,
                  decoration: const InputDecoration(
                    labelText: 'User ID',
                    hintText: 'Enter your user ID',
                  ),
                ),
                const SizedBox(height: 16),
                TextField(
                  controller: reelTitleController,
                  decoration: const InputDecoration(
                    labelText: 'Reel Title',
                    hintText: 'Enter reel title',
                  ),
                ),
                const SizedBox(height: 16),
                TextField(
                  controller: reelDescriptionController,
                  decoration: const InputDecoration(
                    labelText: 'Reel Description',
                    hintText: 'Enter reel description',
                  ),
                  maxLines: 3,
                ),
                const SizedBox(height: 16),
                TextField(
                  controller: videoTitleController,
                  decoration: const InputDecoration(
                    labelText: 'Video Title',
                    hintText: 'Enter video title',
                  ),
                ),
                const SizedBox(height: 16),
                TextField(
                  controller: videoDescriptionController,
                  decoration: const InputDecoration(
                    labelText: 'Video Description',
                    hintText: 'Enter video description',
                  ),
                  maxLines: 3,
                ),
              ],
            ),
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.of(context).pop(),
              child: const Text('Cancel'),
            ),
            ElevatedButton(
              onPressed: () {
                if (reelTitleController.text.isNotEmpty &&
                    videoTitleController.text.isNotEmpty &&
                    userIdController.text.isNotEmpty) {
                  Navigator.of(context).pop({
                    'userId': userIdController.text,
                    'reelTitle': reelTitleController.text,
                    'reelDescription': reelDescriptionController.text,
                    'videoTitle': videoTitleController.text,
                    'videoDescription': videoDescriptionController.text,
                  });
                }
              },
              child: const Text('Upload'),
            ),
          ],
        );
      },
    );
  }
}
