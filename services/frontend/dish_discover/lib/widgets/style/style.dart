import 'package:flutter/material.dart';

/// Theme data, frequently used colors and other layout details.
Color baseColor = Colors.orange[700]!;
Color tabIndicatorColor = Colors.orange[900]!;
Color likeColor = Colors.red[800]!;
Color saveColor = Colors.cyan;
Color imageShadowColor = Colors.transparent;
Color buttonColor = baseColor;
Color checkColor = Colors.green;
Color inactiveColor = Colors.blueGrey;

Color textEditorColor = Colors.white;

Color backgroundColor(BuildContext context) =>
    Theme.of(context).colorScheme.background;
Color primaryColor(BuildContext context) =>
    Theme.of(context).colorScheme.primary;
Color inversePrimaryColor(BuildContext context) =>
    Theme.of(context).colorScheme.inversePrimary;
Color containerColor(BuildContext context) =>
    Theme.of(context).colorScheme.onInverseSurface;
Color outerContainerColor(BuildContext context) =>
    Theme.of(context).colorScheme.surfaceVariant;
Color errorColor(BuildContext context) => Theme.of(context).colorScheme.error;
Color errorContainerColor(BuildContext context) =>
    Theme.of(context).colorScheme.inverseSurface;
Color onErrorContainerColor(BuildContext context) =>
    Theme.of(context).colorScheme.onInverseSurface;

ThemeData appThemeLight = ThemeData(
    colorScheme: ColorScheme.fromSeed(
        seedColor: baseColor, brightness: Brightness.light),
    useMaterial3: true);

ThemeData appThemeDark = ThemeData(
    colorScheme:
        ColorScheme.fromSeed(seedColor: baseColor, brightness: Brightness.dark),
    useMaterial3: true);

TextStyle textStyle = const TextStyle();

double appBarHeight = 75;
