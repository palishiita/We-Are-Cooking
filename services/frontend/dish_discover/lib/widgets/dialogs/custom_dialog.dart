import 'package:dish_discover/widgets/display/validation_message.dart';
import 'package:flutter/material.dart';

class CustomDialog extends StatefulWidget {
  final String title;
  final String? subtitle;
  final Widget? message;
  final Widget child;
  final String buttonLabel;
  final String? Function() onPressed;

  /// Simple Material dialog for confirmation with custom message.
  /// On cancel or click away, closes the dialog. On confirm, executes onConfirm.
  const CustomDialog(
      {super.key,
      required this.title,
      required this.subtitle,
      required this.message,
      required this.child,
      required this.buttonLabel,
      required this.onPressed});

  /// Calls a simple Material dialog for confirmation with custom message.
  /// On cancel or click away, closes the dialog. On confirm, executes onConfirm.
  static void callDialog(
      BuildContext context,
      String title,
      String? subtitle,
      Widget? message,
      Widget child,
      String buttonLabel,
      String? Function() onPressed) {
    Navigator.of(context).push(DialogRoute(
        context: context,
        builder: (context) => CustomDialog(
            title: title,
            subtitle: subtitle,
            message: message,
            buttonLabel: buttonLabel,
            onPressed: onPressed,
            child: child)));
  }

  @override
  State<StatefulWidget> createState() => _CustomDialogState();
}

class _CustomDialogState extends State<CustomDialog> {
  String? errorMessage;

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
        insetPadding: const EdgeInsets.all(8),
        titleTextStyle: Theme.of(context).textTheme.labelLarge,
        title: Text(widget.title, textAlign: TextAlign.center, softWrap: true),
        content: Flex(
            direction: Axis.vertical,
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.center,
            children: [
              widget.subtitle == null
                  ? Container()
                  : Text(widget.subtitle!,
                      softWrap: true, overflow: TextOverflow.ellipsis),
              errorMessage == null
                  ? Container()
                  : ValidationMessage(message: errorMessage!),
              widget.child
            ]),
        actions: [
          Center(
              child: OutlinedButton(
                  child: Text(widget.buttonLabel),
                  onPressed: () {
                    String? error = widget.onPressed();

                    if (error != null) {
                      setState(() {
                        errorMessage = '$error!';
                      });
                    } else {
                      Navigator.of(context).pop();
                    }
                  }))
        ]);
  }
}
