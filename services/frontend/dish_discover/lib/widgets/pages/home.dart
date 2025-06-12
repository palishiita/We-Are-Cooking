import 'dart:convert';

import 'package:dish_discover/services/api_client.dart';
import 'package:dish_discover/widgets/inputs/custom_search_bar.dart';
import 'package:dish_discover/widgets/pages/user.dart';
import 'package:dish_discover/widgets/style/style.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';

import '../../entities/app_state.dart';
import '../../entities/user.dart';
import '../display/loading_indicator.dart';
import 'home_tabs/recommended_tab.dart';
//import 'home_tabs/fridge_tab.dart';
import 'home_tabs/cookbook_tab.dart';
import 'reels_page/reels_page.dart';
import 'login.dart';

// HELPER CLASS
class ProfileDto {
    String userUuid;
    String userName;
    String? imageUrl;
    String? imageSmallUrl;

    bool? isPrivate;
    bool? isBanned;

    String? bio;

    ProfileDto({required this.userUuid, required this.userName, this.imageUrl, this.imageSmallUrl, this.isPrivate, this.isBanned, this.bio});

    factory ProfileDto.fromJson(Map<String, dynamic> json) {
    return ProfileDto(
      userUuid: json['userUuid'] ?? '',
      userName: json['userName'] ?? '',
      imageUrl: json['imageUrl'],
      imageSmallUrl: json['imageSmallUrl'],
      isPrivate: json['isPrivate'],
      isBanned: json['isBanned'],
      bio: json['bio'],
    );
  }
}

class HomePage extends StatefulWidget {
  static const routeName = "/home";

  HomePage({super.key});

  @override
  State<StatefulWidget> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage>
    with SingleTickerProviderStateMixin {
  late TabController tabController;
  int _previousIndex = 0;
  static final ApiClient _apiClient = ApiClient();
  bool _isLoggedIn = false;

  @override
  void initState() {
    super.initState();
    tabController = TabController(length: 3, vsync: this);
    tabController.addListener(_handleTabSelection);
    
    init();
  }

  void init() async {
    var userData = await _apiClient.get('/api/profile/me');

    if(userData.statusCode != 200){
      AppState.currentUser = User(username: 'Not Found', userId: '00000000-0000-0000-0000-000000000000', password: '', email: '');
      _isLoggedIn = true;
    }
    else{
      final Map<String, dynamic> jsonData = userData.data as Map<String, dynamic>;

      // Now use jsonData to create your objects
      final decoded = ProfileDto.fromJson(jsonData);
      AppState.currentUser = User(username: decoded.userName, userId: decoded.userUuid, password: '', email: '');
      _isLoggedIn = true;
    }
  }

  @override
  void dispose() {
    tabController.removeListener(_handleTabSelection);
    tabController.dispose();
    super.dispose();
  }

  void _handleTabSelection() {
    if (tabController.indexIsChanging) {
      if (tabController.index == 1) {
        Future.microtask(() {
          tabController.animateTo(_previousIndex);
        });

        Navigator.of(context).push(
          PageRouteBuilder(
            opaque: false,
            pageBuilder: (context, animation, secondaryAnimation) =>
                const ReelsPage(),
            transitionDuration: const Duration(milliseconds: 300),
            transitionsBuilder:
                (context, animation, secondaryAnimation, child) {
              return FadeTransition(
                opacity: animation,
                child: child,
              );
            },
          ),
        );
      } else {
        _previousIndex = tabController.index;
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    // if (!AppState.userDataLoaded) {
    //   return loading();
    // }


    return done();
  }

  Widget loading() {
    return FutureBuilder(
        future:
            Future<User>(() => User.getUser(AppState.currentUser!.username)),
        builder: (context, userData) {
          if (userData.connectionState != ConnectionState.done) {
            return const LoadingIndicator(
                showBackButton: false, title: "We Are Cooking");
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
        title: "We Are Cooking",
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
            title: Text('We Are Cooking',
                style: Theme.of(context).textTheme.headlineMedium),
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
                children: const [
                  RecommendedTab(),
                  SizedBox.shrink(),
                  CookbookTab(),
                ],
              ))
            ]),
        bottomNavigationBar: TabBar(
          controller: tabController,
          tabs: const [
            Tab(text: 'Recommended'),
            Tab(text: 'Reels'),
            Tab(text: 'Cookbook'),
          ],
          indicatorColor: buttonColor,
          labelColor: buttonColor,
          unselectedLabelColor: inactiveColor,
        ));
  }
}
