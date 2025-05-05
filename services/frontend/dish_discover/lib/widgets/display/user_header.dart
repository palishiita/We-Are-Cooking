import 'dart:io';

import 'package:dish_discover/widgets/display/user_avatar.dart';
import 'package:dish_discover/widgets/display_with_input/like_save_indicator.dart';
import 'package:dish_discover/widgets/inputs/custom_text_field.dart';
import 'package:file_picker/file_picker.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../entities/app_state.dart';
import '../../entities/user.dart';
import '../dialogs/custom_dialog.dart';
import '../style/style.dart';

class UserHeader extends ConsumerWidget {
  final ChangeNotifierProvider<User> userProvider;
  const UserHeader({super.key, required this.userProvider});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    User user = ref.watch(userProvider);
    TextEditingController descriptionController = TextEditingController();

    return Flex(
        direction: Axis.vertical,
        mainAxisSize: MainAxisSize.min,
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Stack(alignment: AlignmentDirectional.topEnd, children: [
            UserAvatar(username: user.username, image: user.image),
            FloatingActionButton(
                shape: const CircleBorder(),
                mini: true,
                backgroundColor: containerColor(context),
                child: Icon(Icons.edit_outlined, color: buttonColor),
                onPressed: () async {
                  await FilePicker.platform
                      .pickFiles(
                          dialogTitle: 'Please select an image:',
                          type: FileType.image)
                      .then((res) {
                    String? path = res?.files[0].path;
                    if (path != null) {
                      AppState.currentUser!.image = Image.file(File(path));
                      AppState.currentUser!.notifyListeners();
                    }
                  });
                })
          ]),
          Stack(alignment: AlignmentDirectional.topEnd, children: [
            Padding(
                padding: const EdgeInsets.all(15),
                child: Center(
                    child: Text(user.description,
                        textAlign: TextAlign.center,
                        maxLines: 50,
                        softWrap: true,
                        overflow: TextOverflow.ellipsis))),
            Padding(
                padding: EdgeInsets.all(5),
                child: FloatingActionButton(
                    shape: const CircleBorder(),
                    mini: true,
                    backgroundColor: containerColor(context),
                    child: Icon(Icons.edit_outlined, color: buttonColor),
                    onPressed: () => CustomDialog.callDialog(
                            context,
                            "Change description",
                            '',
                            null,
                            CustomTextField(
                                controller: descriptionController,
                                hintText: 'Description'),
                            'Change', () {
                          AppState.currentUser!.description =
                              descriptionController.text;
                          AppState.currentUser!.notifyListeners();
                        })))
          ]),
          LikeSaveIndicator(
              likeButtonEnabled: true,
              likeCount: user.likesTotal,
              onLikePressed: null,
              saveButtonEnabled: true,
              saveCount: user.savesTotal,
              onSavePressed: null,
              center: true),
          const Divider(height: 2)
        ]);
  }
}
