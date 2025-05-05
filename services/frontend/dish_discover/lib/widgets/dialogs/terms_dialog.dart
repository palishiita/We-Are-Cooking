import 'package:flutter/material.dart';

import '../display/tab_title.dart';
import '../style/style.dart';

class TermsDialog extends StatelessWidget {
  /// Simple Material dialog for confirmation with custom message.
  /// On cancel or click away, closes the dialog. On confirm, executes onConfirm.
  const TermsDialog({super.key});

  /// Calls a simple Material dialog for confirmation with custom message.
  /// On cancel or click away, closes the dialog. On confirm, executes onConfirm.
  static void callDialog(BuildContext context) {
    showModalBottomSheet(
        context: context,
        enableDrag: false,
        backgroundColor: containerColor(context),
        useSafeArea: true,
        builder: (context) => const TermsDialog());
  }

  @override
  Widget build(BuildContext context) {
    return Flex(
        direction: Axis.vertical,
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          const Padding(
              padding: EdgeInsets.symmetric(vertical: 15.0),
              child: TabTitle(title: "Terms & Conditions")),
          Expanded(
              child: Padding(
                  padding: const EdgeInsets.fromLTRB(20.0, 0.0, 20.0, 25.0),
                  child: Card(
                      color: backgroundColor(context),
                      elevation: 0,
                      shape: const RoundedRectangleBorder(
                          side: BorderSide(),
                          borderRadius:
                              BorderRadius.all(Radius.circular(20.0))),
                      child: const Padding(
                          padding: EdgeInsets.all(10.0),
                          child:
                              SingleChildScrollView(child: Text('Contents'))))))
        ]);
  }
}
