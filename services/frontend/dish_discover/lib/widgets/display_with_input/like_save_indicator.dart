import 'package:dish_discover/widgets/style/style.dart';
import 'package:flutter/material.dart';

class LikeSaveIndicator extends StatelessWidget {
  final int likeCount;
  final bool likeButtonEnabled;
  final void Function()? onLikePressed;
  final int saveCount;
  final bool saveButtonEnabled;
  final void Function()? onSavePressed;
  final bool center;

  const LikeSaveIndicator(
      {super.key,
      required this.likeButtonEnabled,
      required this.likeCount,
      required this.onLikePressed,
      required this.saveButtonEnabled,
      required this.saveCount,
      required this.onSavePressed,
      this.center = false});

  String intToShortString(int value) {
    if (value < 0) {
      value = 0;
    }

    int thousands = (value / 1000).floor();
    int hundreds = (value / 100).floor() % 10;

    return thousands == 0
        ? value.toString()
        : hundreds == 0
            ? "${thousands}K"
            : "$thousands.${hundreds}K";
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
        padding: const EdgeInsets.all(5.0),
        child: Flex(
            direction: Axis.horizontal,
            mainAxisSize: MainAxisSize.min,
            mainAxisAlignment:
                center ? MainAxisAlignment.center : MainAxisAlignment.start,
            children: [
              IconButton(
                  icon: Icon(Icons.favorite_border_outlined, color: likeColor),
                  selectedIcon: Icon(Icons.favorite, color: likeColor),
                  isSelected: likeButtonEnabled,
                  onPressed: onLikePressed),
              Text(intToShortString(likeCount)),
              IconButton(
                  icon: Icon(Icons.bookmark_border, color: saveColor),
                  selectedIcon: Icon(Icons.bookmark, color: saveColor),
                  isSelected: saveButtonEnabled,
                  onPressed: onSavePressed),
              Text(intToShortString(saveCount))
            ]));
  }
}
