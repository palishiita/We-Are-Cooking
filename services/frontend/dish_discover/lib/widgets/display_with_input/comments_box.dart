//import 'package:dish_discover/widgets/dialogs/custom_dialog.dart';
//import 'package:dish_discover/widgets/display/tab_title.dart';
//import 'package:dish_discover/widgets/inputs/custom_text_field.dart';
//import 'package:dish_discover/widgets/inputs/popup_menu.dart';
//import 'package:dish_discover/widgets/style/style.dart';
//import 'package:flutter/foundation.dart';
//import 'package:flutter/material.dart';
//import 'package:flutter_riverpod/flutter_riverpod.dart';
//
//import '../../entities/app_state.dart';
//import '../../entities/comment.dart';
//import '../../entities/new_recipe.dart';
//import '../../entities/user.dart';
//import '../display/user_avatar.dart';
//import '../pages/user.dart';
//
// unused, not touched, could/should be removed
//class CommentTile extends ConsumerWidget {
//  final ChangeNotifierProvider<Comment> commentProvider;
//  final TextEditingController passwordController = TextEditingController();
//  final void Function() onDelete;
//
//  CommentTile(
//      {super.key, required this.commentProvider, required this.onDelete});
//
//  @override
//  Widget build(BuildContext context, WidgetRef ref) {
//    Comment comment = ref.watch(commentProvider);
//    User author = User(
//        userId: '',
//        username: comment.author,
//        isModerator: false,
//        password: '',
//        email: '',
//        //isPremium: false,
//        description: '');
//
//    return Padding(
//        padding: const EdgeInsets.all(8),
//        child: Card(
//            child: Flex(direction: Axis.vertical, children: [
//          ListTile(
//              leading: UserAvatar(
//                  username: comment.author, image: author.image, diameter: 30),
//              title: GestureDetector(
//                  onTap: () => Navigator.of(context).push(MaterialPageRoute(
//                      builder: (context) =>
//                          UserPage(username: comment.author))),
//                  child: Text(author.username,
//                      softWrap: true, overflow: TextOverflow.ellipsis)),
//              trailing: author.username.compareTo(AppState.currentUser!.username) ==
//                      0
//                  ? IconButton(
//                      onPressed: () => CustomDialog.callDialog(
//                              context,
//                              'Delete comment',
//                              '',
//                              null,
//                              CustomTextField(
//                                  controller: passwordController,
//                                  hintText: 'Password',
//                                  obscure: true),
//                              'Delete', () {
//                            if (User.checkPassword(passwordController.text)!) {
//                              onDelete();
//                              return null;
//                            } else {
//                              return 'Passwords do not match';
//                            }
//                          }),
//                      icon: const Icon(Icons.delete))
//                  : IconButton(
//                      onPressed: () => AppState.currentUser!.isModerator
//                          ? PopupMenuAction.banAction(context, comment.recipeId,
//                              comment.content, comment.id, null, onDelete)
//                          : PopupMenuAction.reportAction(context, comment.recipeId, comment.content, comment.id, null),
//                      icon: const Icon(Icons.flag))),
//          const Divider(height: 1.0),
//          Align(
//              alignment: Alignment.topLeft,
//              child: Padding(
//                  padding: const EdgeInsets.all(15.0),
//                  child: Text(comment.content,
//                      softWrap: true, overflow: TextOverflow.ellipsis)))
//        ])));
//  }
//}
//
//class CommentsBox extends ConsumerStatefulWidget {
//  final ChangeNotifierProvider<Recipe> recipeProvider;
//
//  const CommentsBox({super.key, required this.recipeProvider});
//
//  @override
//  ConsumerState<ConsumerStatefulWidget> createState() => _CommentsBoxState();
//}
//
//class _CommentsBoxState extends ConsumerState<CommentsBox> {
//  final TextEditingController commentController = TextEditingController();
//
//  @override
//  Widget build(BuildContext context) {
//    Recipe recipe = ref.watch(widget.recipeProvider);
//    List<Comment> comments = recipe.comments;
//    if (kDebugMode && comments.isEmpty) {
//      comments = comments +
//          [
//            Comment(
//                author: 'Test_user', recipeId: 0, id: 0, content: '[Testing]')
//          ];
//    }
//
//    return Padding(
//        padding: const EdgeInsets.symmetric(vertical: 5.0, horizontal: 20.0),
//        child: Card(
//            child: Padding(
//                padding: const EdgeInsets.all(20),
//                child: Flex(
//                    direction: Axis.vertical,
//                    mainAxisAlignment: MainAxisAlignment.start,
//                    crossAxisAlignment: CrossAxisAlignment.start,
//                    children: [
//                      const Padding(
//                          padding: EdgeInsets.only(bottom: 5),
//                          child: TabTitle(title: "Comments")),
//                      Padding(
//                          padding: const EdgeInsets.symmetric(horizontal: 5),
//                          child: CustomTextField(
//                              controller: commentController,
//                              hintText: 'Comment',
//                              trailingAction: IconButton(
//                                  icon: const Icon(
//                                    Icons.send_rounded,
//                                    size: 18,
//                                  ),
//                                  onPressed: () {
//                                    recipe.addComment(Comment(
//                                        id: 0,
//                                        author: AppState.currentUser!.username,
//                                        recipeId: recipe.id,
//                                        content: commentController.text));
//                                    commentController.text = '';
//                                  }))),
//                      Card(
//                          color: outerContainerColor(context),
//                          child: Flex(
//                              direction: Axis.vertical,
//                              mainAxisSize: MainAxisSize.max,
//                              children: List.generate(
//                                  comments.length,
//                                  (index) => CommentTile(
//                                      onDelete: () =>
//                                          recipe.removeComment(comments[index]),
//                                      commentProvider:
//                                          ChangeNotifierProvider<Comment>(
//                                              (ref) => comments[index])))))
//                    ]))));
//  }
//}
