import 'package:flutter/material.dart';

class TabTitle extends StatelessWidget {
  final String title;
  final bool small;

  /// Padded text with context's titleLarge text style.
  const TabTitle({super.key, required this.title, this.small = false});

  @override
  Widget build(BuildContext context) {
    return Padding(
        padding: const EdgeInsets.symmetric(horizontal: 25.0),
        child: Text(title,
            softWrap: true,
            textAlign: TextAlign.center,
            overflow: TextOverflow.ellipsis,
            style: small
                ? Theme.of(context).textTheme.titleSmall
                : Theme.of(context).textTheme.titleMedium));
  }
}
