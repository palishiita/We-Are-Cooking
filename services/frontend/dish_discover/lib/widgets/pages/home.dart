import 'package:dish_discover/widgets/inputs/custom_search_bar.dart';
import 'package:dish_discover/widgets/pages/user.dart';
import 'package:dish_discover/widgets/style/style.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';

import '../../entities/app_state.dart';
import '../../entities/user.dart';
import '../display/loading_indicator.dart';
import 'home_tabs/moderation_tab.dart';
import 'home_tabs/recommended_tab.dart';
import 'home_tabs/saved_tab.dart';
import 'login.dart';

class HomePage extends StatefulWidget {
  static const routeName = "/home";

  HomePage({super.key}) {
    assert(AppState.currentUser != null);
  }

  @override
  State<StatefulWidget> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage>
    with SingleTickerProviderStateMixin {
  late TabController tabController;

  @override
  void initState() {
    super.initState();
    tabController = TabController(
        length: AppState.currentUser!.isModerator ? 3 : 2, vsync: this);
  }

  @override
  Widget build(BuildContext context) {
    if (!AppState.userDataLoaded) {
      return loading();
    }

    return done();
  }

  Widget loading() {
    return FutureBuilder(
        future:
            Future<User>(() => User.getUser(AppState.currentUser!.username)),
        builder: (context, userData) {
          if (userData.connectionState != ConnectionState.done) {
            return const LoadingIndicator(
                showBackButton: false, title: "DishDiscover");
          } else if (userData.data == null) {
            if (kDebugMode) {
              AppState.userDataLoaded = true;
              return done();
            } else {
              return loadError();
            }
          }

          AppState.currentUser = userData.data!;
          AppState.userDataLoaded = true;

          return done();
        });
  }

  Widget loadError() {
    return LoadingErrorIndicator(
        showBackButton: false,
        title: "DishDiscover",
        child: Center(
            child: Flex(
          direction: Axis.vertical,
          mainAxisSize: MainAxisSize.min,
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            const Padding(
                padding: EdgeInsets.all(10),
                child: Text("Could not load user data.")),
            Padding(
                padding: const EdgeInsets.all(10),
                child: FilledButton(
                    onPressed: () {
                      User.logout();
                      Navigator.of(context).pushNamedAndRemoveUntil(
                          LoginPage.routeName, (route) => route.isFirst);
                    },
                    child: const Text("Log out"))),
            Padding(
                padding: const EdgeInsets.all(10),
                child: OutlinedButton(
                    onPressed: () => setState(() {}),
                    child: const Text("Reload")))
          ],
        )));
  }

  Widget done() {
    return Scaffold(
        appBar: AppBar(
            toolbarHeight: appBarHeight,
            scrolledUnderElevation: 0.0,
            title: Text('DishDiscover',
                style: Theme.of(context)
                    .textTheme
                    .headlineMedium), //Image.asset('assets/images/logo.png')),
            centerTitle: true,
            actions: [
              IconButton(
                  onPressed: () => Navigator.of(context).push(MaterialPageRoute(
                      builder: (context) =>
                          UserPage(username: AppState.currentUser!.username))),
                  icon: const Icon(Icons.account_circle_rounded))
            ]),
        body: Flex(
            direction: Axis.vertical,
            mainAxisSize: MainAxisSize.min,
            children: [
              const CustomSearchBar(),
              Expanded(
                  child: TabBarView(
                controller: tabController,
                children: [const RecommendedTab(), const SavedTab()] +
                    (AppState.currentUser!.isModerator
                        ? [const ModerationTab()]
                        : []),
              ))
            ]),
        bottomNavigationBar: TabBar(
          padding: const EdgeInsets.symmetric(horizontal: 120),
          controller: tabController,
          tabs: List.filled(
              AppState.currentUser!.isModerator ? 3 : 2,
              const Padding(
                  padding: EdgeInsets.symmetric(horizontal: 4, vertical: 15),
                  child: Icon(Icons.circle, size: 13))),
          indicatorColor: baseColor.withAlpha(0),
          labelColor: buttonColor,
          unselectedLabelColor: inactiveColor.withAlpha(0x52),
        ));
  }
}
