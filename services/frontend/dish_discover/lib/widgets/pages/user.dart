import 'package:dish_discover/widgets/display/loading_indicator.dart';
import 'package:dish_discover/widgets/pages/edit_recipe.dart';
import 'package:dish_discover/widgets/style/style.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../entities/app_state.dart';
import '../../entities/new_recipe.dart';
import '../../entities/user.dart';
import '../dialogs/custom_dialog.dart';
import '../display/tab_title.dart';
import '../display/user_header.dart';
import '../inputs/custom_text_field.dart';
import '../inputs/popup_menu.dart';

class UserPage extends ConsumerStatefulWidget {
  static const routeName = "/user";
  final String username;
  final ChangeNotifierProvider<User>? userProvider;

  const UserPage({super.key, required this.username, this.userProvider});

  @override
  ConsumerState<ConsumerStatefulWidget> createState() => _UserPageState();
}

class _UserPageState extends ConsumerState<UserPage> {
  ChangeNotifierProvider<User>? userProvider;
  late bool isCurrentUser;

  @override
  void initState() {
    super.initState();
    userProvider = widget.userProvider;

    isCurrentUser = (widget.username == AppState.currentUser!.username);
    if (userProvider == null && isCurrentUser) {
      userProvider =
          ChangeNotifierProvider<User>((ref) => AppState.currentUser!);
    }
  }

  @override
  Widget build(BuildContext context) {
    return userProvider == null ? loading() : done();
  }

  Widget loading() {
    return FutureBuilder(
        future: Future<User>(() => User.getUser(widget.username)),
        builder: (context, userData) {
          if (userData.connectionState != ConnectionState.done) {
            return LoadingIndicator(title: widget.username);
          }

          User user;
          if (userData.data == null) {
            if (kDebugMode) {
              user = User(
                  userId: '00000000-0000-0000-0000-000000000000',
                  username: "${widget.username}_debug",
                  description:
                      'Testing testing testing testing testing testing testing',
                  image: Image.asset("assets/images/launcher_icon.jpg"),
                  email: '',
                  password: '');
            } else {
              return LoadingErrorIndicator(title: widget.username);
            }
          } else {
            user = userData.data!;
          }

          userProvider = ChangeNotifierProvider<User>((ref) => user);

          return done();
        });
  }

  Widget done() {
    User user = ref.watch(userProvider!);

    return Scaffold(
        appBar: AppBar(
            toolbarHeight: appBarHeight,
            scrolledUnderElevation: 0.0,
            title: TabTitle(title: user.username),
            centerTitle: true,
            leading: const BackButton(),
            actions: [
              isCurrentUser
                  ? IconButton(
                      icon: const Icon(Icons.settings),
                      onPressed: () =>
                          Navigator.of(context).pushNamed("/settings"))
                  : IconButton(
                      icon: const Icon(Icons.settings),
                      onPressed: () => PopupMenuAction.shareAction(
                        context,
                        "Sharing user",
                        "Have a look at this: ",
                        user.getUrl()))
            ]),
        body: SingleChildScrollView(
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              UserHeader(userProvider: userProvider!),
              // RecipeList can be added here if implemented
            ]),
        ),
        floatingActionButton: isCurrentUser
            ? FloatingActionButton(
                shape: const CircleBorder(),
                mini: true,
                child: const Icon(Icons.add),
                onPressed: () {
                  TextEditingController titleController =
                      TextEditingController();

                  CustomDialog.callDialog(
                      context,
                      'Create recipe',
                      '',
                      null,
                      CustomTextField(
                          controller: titleController, hintText: 'Title'),
                      'Create', () {
                    if (titleController.text.trim().isNotEmpty) {
                      Recipe newRecipe = Recipe(
                          id: '',
                          name: titleController.text,
                          userData: UserData(
                              userId: AppState.currentUser!.userId,
                              username: AppState.currentUser!.username));
                      AppState.currentUser!.addRecipe(newRecipe);
                      Future.microtask(() => Navigator.of(context).push(
                          MaterialPageRoute(
                              builder: (context) =>
                                  EditRecipePage(recipeId: newRecipe.id, name: newRecipe.name))));
                      return null;
                    } else {
                      return "Title cannot be empty";
                    }
                  });
                })
            : null);
  }
}