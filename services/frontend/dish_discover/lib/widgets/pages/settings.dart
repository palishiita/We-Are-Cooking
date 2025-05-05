import 'package:dish_discover/widgets/dialogs/terms_dialog.dart';
import 'package:dish_discover/widgets/inputs/custom_text_field.dart';
import 'package:dish_discover/widgets/pages/login.dart';
import 'package:dish_discover/widgets/pages/payment.dart';
import 'package:dish_discover/widgets/style/style.dart';
import 'package:flutter/material.dart';

import '../../entities/app_state.dart';
import '../../entities/user.dart';
import '../dialogs/custom_dialog.dart';

class SettingsPage extends StatelessWidget {
  static const routeName = "/settings";
  const SettingsPage({super.key});

  @override
  Widget build(BuildContext context) {
    TextEditingController textController = TextEditingController();
    TextEditingController textController2 = TextEditingController();
    TextEditingController textController3 = TextEditingController();

    return Scaffold(
        appBar: AppBar(
            toolbarHeight: appBarHeight,
            scrolledUnderElevation: 0.0,
            leading: const BackButton()),
        body: ListView(children: [
          ListTile(
              title: const Text("Buy Premium"),
              onTap: () => Navigator.of(context).push(MaterialPageRoute(
                  builder: (context) =>
                      const PaymentPage(buyingPremium: true)))),
          ListTile(
              title: const Text("Change email"),
              onTap: () {
                textController.text = AppState.currentUser!.email;
                CustomDialog.callDialog(
                    context,
                    "Change email",
                    "",
                    null,
                    CustomTextField(
                        controller: textController, hintText: 'Email'),
                    "Change", () {
                  if (textController.text.isNotEmpty) {
                    // TODO email validation
                    AppState.currentUser!
                        .editProfile(email: textController.text);
                    return null;
                  } else {
                    return "Invalid email format";
                  }
                });
              }),
          ListTile(
              title: const Text("Change password"),
              onTap: () {
                textController.text = '';
                textController2.text = '';
                textController3.text = '';
                CustomDialog.callDialog(
                    context,
                    "Change password",
                    "",
                    null,
                    Flex(direction: Axis.vertical, children: [
                      CustomTextField(
                          controller: textController,
                          hintText: 'Old password',
                          obscure: true),
                      CustomTextField(
                          controller: textController2,
                          hintText: 'New password',
                          obscure: true),
                      CustomTextField(
                          controller: textController3,
                          hintText: 'Repeat new password',
                          obscure: true)
                    ]),
                    "Change", () {
                  if (!User.checkPassword(textController.text)!) {
                    return "Wrong password";
                  } else if (textController2.text != textController3.text) {
                    return "New passwords don't match";
                  } else if (textController2.text.isEmpty) {
                    return "Password cannot be empty";
                  } else {
                    return null;
                  }
                });
              }),
          ListTile(
              title: const Text("Terms & Conditions"),
              onTap: () => TermsDialog.callDialog(context)),
          ListTile(
              title: const Text("Log out"),
              onTap: () {
                User.logout();
                Navigator.of(context).pushNamedAndRemoveUntil(
                    LoginPage.routeName, (route) => route.isFirst);
              }),
          ListTile(
              title: const Text("Delete account"),
              onTap: () => CustomDialog.callDialog(
                      context,
                      "Delete account",
                      "",
                      null,
                      CustomTextField(
                          controller: textController,
                          hintText: 'Password',
                          obscure: true),
                      "Delete", () {
                    if (textController.text == AppState.currentUser!.password) {
                      User.logout();
                      Future.microtask(() => Navigator.of(context)
                          .pushNamedAndRemoveUntil(
                              LoginPage.routeName, (route) => route.isFirst));
                      User.removeUser(AppState.currentUser!);
                    } else {
                      return "Wrong password";
                    }
                  })),
        ]));
  }
}
