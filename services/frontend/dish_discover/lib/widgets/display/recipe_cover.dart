import 'package:flutter/material.dart';

class RecipeCover extends StatelessWidget {
  final Image? cover;
  const RecipeCover({super.key, required this.cover});

  @override
  Widget build(BuildContext context) {
    Image img = cover ?? Image.asset('assets/images/missing_cover.jpg');

    return Container(
        decoration: BoxDecoration(
            image: DecorationImage(
                fit: BoxFit.cover, image: img.image, isAntiAlias: true)));
  }
}
