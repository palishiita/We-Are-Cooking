import 'package:flutter/material.dart';

import '../style/style.dart';

class CustomDropdown<T> extends StatelessWidget {
  final List<(T, String)> labeledOptions;
  final void Function(T?) onChanged;
  final T currentValue;

  /// Custom DropdownButton with option dense = true.
  const CustomDropdown(
      {super.key,
      required this.currentValue,
      required this.labeledOptions,
      required this.onChanged});

  @override
  Widget build(BuildContext context) {
    return DropdownButton<T>(
        isDense: true,
        focusColor: backgroundColor(context),
        menuMaxHeight: 200,
        value: currentValue,
        style: Theme.of(context).textTheme.labelMedium,
        alignment: Alignment.center,
        items: labeledOptions
            .map((entry) => DropdownMenuItem(
                value: entry.$1,
                child: Text(entry.$2, overflow: TextOverflow.ellipsis)))
            .toList(),
        onChanged: onChanged);
  }
}
