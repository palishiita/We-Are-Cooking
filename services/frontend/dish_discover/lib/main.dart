import 'package:dish_discover/widgets/pages/edit_recipe.dart';
import 'package:dish_discover/widgets/pages/home.dart';
import 'package:dish_discover/widgets/pages/login.dart';
import 'package:dish_discover/widgets/pages/register.dart';
import 'package:dish_discover/widgets/pages/search.dart';
import 'package:dish_discover/widgets/pages/settings.dart';
import 'package:dish_discover/widgets/pages/tutorial.dart';
import 'package:dish_discover/widgets/pages/user.dart';
import 'package:dish_discover/widgets/pages/view_recipe.dart';
import 'package:dish_discover/widgets/style/style.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

void main() {
  WidgetsFlutterBinding.ensureInitialized();
  runApp(const ProviderScope(child: DishDiscoverApp()));
}

class DishDiscoverApp extends StatelessWidget {
  const DishDiscoverApp({super.key});

  // This widget is the root of your application.
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
        debugShowCheckedModeBanner: false,
        title: 'DishDiscover',
        theme: appThemeLight,
        home: HomePage(), 
        routes: {
          LoginPage.routeName: (context) => const LoginPage(),
          RegisterPage.routeName: (context) => const RegisterPage(),
          TutorialPage.routeName: (context) => const TutorialPage(),
          HomePage.routeName: (context) => HomePage(),
          SettingsPage.routeName: (context) => const SettingsPage(),
          SearchPage.routeName: (context) => SearchPage(
              searchPhrase:
                  (ModalRoute.of(context)?.settings.arguments ?? '') as String),
          UserPage.routeName: (context) => UserPage(
              username:
                  (ModalRoute.of(context)?.settings.arguments ?? '') as String),
          ViewRecipePage.routeName: (context) => ViewRecipePage(
              recipeId:
                  (ModalRoute.of(context)?.settings.arguments ?? -1) as String),
          EditRecipePage.routeName: (context) => EditRecipePage(
              recipeId:
                  (ModalRoute.of(context)?.settings.arguments ?? -1) as String)
        });
  }
}
