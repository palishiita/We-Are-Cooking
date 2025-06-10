import 'package:dish_discover/widgets/display/validation_message.dart';
import 'package:dish_discover/widgets/inputs/custom_text_field.dart';
import 'package:dish_discover/widgets/pages/home.dart';
import 'package:dish_discover/widgets/pages/register.dart';
import 'package:dish_discover/widgets/style/style.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';

import '../../entities/app_state.dart';
import '../../entities/user.dart';
import '../dialogs/custom_dialog.dart';

class LoginPage extends StatefulWidget {
  static const routeName = "/login";

  const LoginPage({super.key});

  @override
  State<StatefulWidget> createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {
  late TextEditingController usernameController;
  late TextEditingController passwordController;
  String? errorMessage;

  @override
  void initState() {
    super.initState();
    usernameController = TextEditingController();
    passwordController = TextEditingController();
  }

  void login() async {
    setState(() {
      errorMessage = null;
    });

    ScaffoldMessenger.of(context)
        .showSnackBar(const SnackBar(content: Text('Logging in...')));

    String? error;

    if (usernameController.text.isEmpty || passwordController.text.isEmpty) {
      error = "Both fields are required";
    } else {
      error =
          await User.login(usernameController.text, passwordController.text);

      if (error != null && kDebugMode) {
        error = null;

        AppState.currentUser = User(
            userId: '00000000-0000-0000-0000-000000000000',
            username: "${usernameController.text}_debug",
            password: passwordController.text,
            email: '',
            isModerator: true);
        AppState.loginToken = 'FAKE';
      }
    }

    ScaffoldMessenger.of(context).clearSnackBars();

    if (error == null) {
      Navigator.of(context).pushNamedAndRemoveUntil(
          HomePage.routeName, (route) => route.isFirst);
    } else {
      setState(() {
        errorMessage = "Error: ${error!.trim()}!";
      });
    }
  }

  void showRecoverPassword() {
    CustomDialog.callDialog(
        context,
        "Recover password",
        "",
        null,
        CustomTextField(controller: TextEditingController(), hintText: "Email"),
        "Send email", () {
      // TODO  recover password
      return null;
    });
  }

  @override
  Widget build(BuildContext context) {
    if (AppState.currentUser != null) {
      Future.microtask(
          () => Navigator.of(context).pushReplacementNamed(HomePage.routeName));
    }

    return Scaffold(
        appBar:
            AppBar(toolbarHeight: appBarHeight, scrolledUnderElevation: 0.0),
        body: Center(
            child: Padding(
                padding: const EdgeInsets.symmetric(horizontal: 24.0, vertical: 8.0),
                child: ConstrainedBox(
                  constraints: BoxConstraints(maxWidth: 400),
                  child: Column(
                      mainAxisAlignment: MainAxisAlignment.center,
                      mainAxisSize: MainAxisSize.min,
                      crossAxisAlignment: CrossAxisAlignment.stretch,
                      children: [
                        Padding(
                          padding: const EdgeInsets.only(bottom: 8.0),
                          child: Image.asset('assets/images/logo.png', height: 200),
                        ),
                        if (errorMessage != null)
                          ValidationMessage(message: errorMessage!),
                        CustomTextField(
                            controller: usernameController, hintText: 'Username'),
                        CustomTextField(
                            controller: passwordController,
                            hintText: 'Password',
                            obscure: true),
                        Align(
                            alignment: Alignment.centerLeft,
                            child: TextButton(
                                onPressed: showRecoverPassword,
                                child: Text('Recover password',
                                    style: textStyle.merge(const TextStyle(
                                        decoration: TextDecoration.underline))))),
                        Align(
                            alignment: Alignment.bottomRight,
                            child: OutlinedButton(
                                onPressed: login,
                                child: Text('Login', style: textStyle))),
                      ]),
                ),
              ),
            ),
        bottomNavigationBar: Padding(
            padding: const EdgeInsets.only(bottom: 20.0),
            child: Column(
                mainAxisSize: MainAxisSize.min,
                crossAxisAlignment: CrossAxisAlignment.center,
                children: [
                  const Padding(
                      padding: EdgeInsets.symmetric(vertical: 5.0),
                      child: Text("Don't have an account?")),
                  OutlinedButton(
                      child: const Text("Register"),
                      onPressed: () => Navigator.of(context)
                          .pushNamed(RegisterPage.routeName))
                ])));
  }
}