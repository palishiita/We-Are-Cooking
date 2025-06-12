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

    return Scaffold(
        appBar: AppBar(
            toolbarHeight: appBarHeight,
            scrolledUnderElevation: 0.0,
            leading: const BackButton()),
        body: ListView(children: [
          ListTile(
              title: const Text("Log out"),
              onTap: () {
                User.logout();
                Navigator.of(context).pushNamedAndRemoveUntil('/logout', (route) => false);
              }),
        ]));
  }
}
