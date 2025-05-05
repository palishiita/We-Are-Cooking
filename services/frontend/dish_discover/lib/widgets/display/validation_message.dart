import 'package:dish_discover/widgets/style/style.dart';
import 'package:flutter/material.dart';

class ValidationMessage extends StatelessWidget {
  final String message;
  final bool isError;
  const ValidationMessage(
      {super.key, required this.message, this.isError = true});

  @override
  Widget build(BuildContext context) {
    return Padding(
        padding: const EdgeInsets.symmetric(vertical: 10),
        child: RawChip(
            visualDensity: VisualDensity.compact,
            tapEnabled: false,
            backgroundColor: errorContainerColor(context),
            label: Text(message,
                softWrap: true,
                style: TextStyle(color: onErrorContainerColor(context)),
                overflow: TextOverflow.ellipsis),
            avatar: Icon(isError ? Icons.error : Icons.check_circle_rounded,
                color: isError ? errorColor(context) : checkColor, size: 18)));
  }
}
