import 'package:flutter/material.dart';

class CustomTextField extends StatelessWidget {
  final TextEditingController controller;
  final String hintText;
  final bool obscure;
  final int maxLength;
  final InputDecoration? decoration;
  final void Function(String)? onChanged;
  final IconButton? trailingAction;

  /// Custom padded TextField for user input. Hides the character
  /// count that appears when using max length.
  const CustomTextField(
      {super.key,
      required this.controller,
      required this.hintText,
      this.obscure = false,
      this.maxLength = 45,
      this.decoration,
      this.onChanged,
      this.trailingAction});

  @override
  Widget build(BuildContext context) {
    int lines = obscure || maxLength <= 50 ? 1 : (maxLength / 30).ceil();
    return Padding(
        padding: const EdgeInsets.symmetric(horizontal: 0, vertical: 5.0),
        child: TextField(
          controller: controller,
          textAlignVertical: TextAlignVertical.center,
          focusNode: FocusNode(),
          obscureText: obscure,
          maxLength: maxLength,
          maxLines: lines,
          buildCounter: (BuildContext context,
                  {required int currentLength,
                  required bool isFocused,
                  required int? maxLength}) =>
              null,
          style: Theme.of(context).textTheme.bodyMedium,
          decoration: decoration?.copyWith(hintText: hintText) ??
              InputDecoration(
                  filled: false,
                  border: OutlineInputBorder(
                      borderRadius: BorderRadius.circular(30.0)),
                  hintText: hintText,
                  suffixIcon: trailingAction),
          onChanged: onChanged,
        ));
  }
}
