import 'package:flutter/material.dart';
import 'package:file_picker/file_picker.dart';
import 'dart:typed_data';

class UploadDialog {
  static Future<Map<String, dynamic>?> show(BuildContext context) async {
    final reelTitleController = TextEditingController();
    final reelDescriptionController = TextEditingController();
    final videoTitleController = TextEditingController();
    final videoDescriptionController = TextEditingController();

    Uint8List? selectedFileBytes;
    String? selectedFileName;
    String? errorMessage;

    void refresh() => (context as Element).markNeedsBuild();
    reelTitleController.addListener(refresh);
    videoTitleController.addListener(refresh);
    reelDescriptionController.addListener(refresh);
    videoDescriptionController.addListener(refresh);

    return showDialog<Map<String, dynamic>>(
      context: context,
      builder: (BuildContext context) {
        return StatefulBuilder(
          builder: (context, setState) {
            return AlertDialog(
              title: const Text('Upload Reel'),
              content: SizedBox(
                width: MediaQuery.of(context).size.width * 0.8,
                height: MediaQuery.of(context).size.height * 0.7,
                child: SingleChildScrollView(
                  child: Column(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Container(
                        width: double.infinity,
                        padding: const EdgeInsets.all(16),
                        decoration: BoxDecoration(
                          border: Border.all(
                            color: errorMessage != null
                                ? Colors.red.shade300
                                : Colors.grey.shade300,
                          ),
                          borderRadius: BorderRadius.circular(8),
                          color: selectedFileBytes != null
                              ? Colors.green.shade50
                              : Colors.grey.shade50,
                        ),
                        child: Column(
                          children: [
                            Icon(
                              selectedFileBytes != null
                                  ? Icons.video_file
                                  : Icons.video_library_outlined,
                              size: 48,
                              color: selectedFileBytes != null
                                  ? Colors.green
                                  : Colors.grey,
                            ),
                            const SizedBox(height: 8),
                            Text(
                              selectedFileName ?? 'No file selected',
                              style: TextStyle(
                                fontSize: 14,
                                fontWeight: selectedFileBytes != null
                                    ? FontWeight.bold
                                    : FontWeight.normal,
                                color: selectedFileBytes != null
                                    ? Colors.green.shade700
                                    : Colors.grey.shade600,
                              ),
                              textAlign: TextAlign.center,
                            ),
                            const SizedBox(height: 12),
                            ElevatedButton.icon(
                              onPressed: () async {
                                await _selectVideoFile(context,
                                    (bytes, fileName, error) {
                                  setState(() {
                                    if (error != null) {
                                      errorMessage = error;
                                      selectedFileBytes = null;
                                      selectedFileName = null;
                                    } else {
                                      selectedFileBytes = bytes;
                                      selectedFileName = fileName;
                                      errorMessage = null;
                                    }
                                  });
                                });
                              },
                              icon: const Icon(Icons.folder_open),
                              label: const Text('Select video file'),
                              style: ElevatedButton.styleFrom(
                                backgroundColor: selectedFileBytes != null
                                    ? Colors.green
                                    : null,
                                foregroundColor: selectedFileBytes != null
                                    ? Colors.white
                                    : null,
                              ),
                            ),
                            if (errorMessage != null) ...[
                              const SizedBox(height: 8),
                              Text(
                                errorMessage!,
                                style: TextStyle(
                                  fontSize: 12,
                                  color: Colors.red.shade700,
                                  fontWeight: FontWeight.w500,
                                ),
                                textAlign: TextAlign.center,
                              ),
                            ],
                            if (selectedFileBytes == null) ...[
                              const SizedBox(height: 8),
                              Text(
                                'Supported formats: MP4, WebM, OGV, MOV, AVI, MKV\nMaximum size: 100MB',
                                style: TextStyle(
                                  fontSize: 12,
                                  color: Colors.grey.shade600,
                                ),
                                textAlign: TextAlign.center,
                              ),
                            ] else ...[
                              const SizedBox(height: 8),
                              Text(
                                'Size: ${_formatFileSize(selectedFileBytes!.length)}',
                                style: TextStyle(
                                  fontSize: 12,
                                  color: Colors.green.shade600,
                                  fontWeight: FontWeight.w500,
                                ),
                              ),
                            ],
                          ],
                        ),
                      ),
                      const SizedBox(height: 16),
                      TextField(
                        controller: reelTitleController,
                        decoration: const InputDecoration(
                          labelText: 'Reel Title *',
                          hintText: 'Enter reel title',
                          border: OutlineInputBorder(),
                        ),
                      ),
                      const SizedBox(height: 16),
                      TextField(
                        controller: reelDescriptionController,
                        decoration: const InputDecoration(
                          labelText: 'Reel Description',
                          hintText: 'Enter reel description',
                          border: OutlineInputBorder(),
                        ),
                        maxLines: 3,
                      ),
                      const SizedBox(height: 16),
                      TextField(
                        controller: videoTitleController,
                        decoration: const InputDecoration(
                          labelText: 'Video Title *',
                          hintText: 'Enter video title',
                          border: OutlineInputBorder(),
                        ),
                      ),
                      const SizedBox(height: 16),
                      TextField(
                        controller: videoDescriptionController,
                        decoration: const InputDecoration(
                          labelText: 'Video Description',
                          hintText: 'Enter video description',
                          border: OutlineInputBorder(),
                        ),
                        maxLines: 3,
                      ),
                    ],
                  ),
                ),
              ),
              actions: [
                TextButton(
                  onPressed: () => Navigator.of(context).pop(),
                  child: const Text('Cancel'),
                ),
                ElevatedButton(
                  onPressed: selectedFileBytes != null &&
                          reelTitleController.text.isNotEmpty &&
                          videoTitleController.text.isNotEmpty
                      ? () {
                          Navigator.of(context).pop({
                            'reelTitle': reelTitleController.text,
                            'reelDescription': reelDescriptionController.text,
                            'videoTitle': videoTitleController.text,
                            'videoDescription': videoDescriptionController.text,
                            'fileBytes': selectedFileBytes,
                            'fileName': selectedFileName,
                          });
                        }
                      : null,
                  child: const Text('Upload'),
                ),
              ],
            );
          },
        );
      },
    );
  }

  static Future<void> _selectVideoFile(BuildContext context,
      Function(Uint8List?, String?, String?) onFileSelected) async {
    try {
      final result = await FilePicker.platform.pickFiles(
        type: FileType.custom,
        allowedExtensions: ['mp4', 'webm', 'ogv', 'mov', 'avi', 'mkv'],
        allowMultiple: false,
        withData: true,
      );

      if (result != null && result.files.single.bytes != null) {
        final file = result.files.single;
        final fileName = file.name;
        final fileBytes = file.bytes!;

        if (fileBytes.length > 100 * 1024 * 1024) {
          onFileSelected(
              null, null, 'File is too large. Maximum size is 100MB.');
          return;
        }

        final extension = fileName.toLowerCase().split('.').last;
        final supportedFormats = ['mp4', 'webm', 'ogv', 'mov', 'avi', 'mkv'];

        if (!supportedFormats.contains(extension)) {
          onFileSelected(null, null,
              'Unsupported file format. Supported formats: ${supportedFormats.join(', ')}');
          return;
        }

        onFileSelected(fileBytes, fileName, null);
      }
    } catch (e) {
      onFileSelected(null, null, 'Error selecting file: ${e.toString()}');
    }
  }

  static String _formatFileSize(int bytes) {
    if (bytes < 1024) return '$bytes B';
    if (bytes < 1024 * 1024) return '${(bytes / 1024).toStringAsFixed(1)} KB';
    return '${(bytes / (1024 * 1024)).toStringAsFixed(1)} MB';
  }
}
