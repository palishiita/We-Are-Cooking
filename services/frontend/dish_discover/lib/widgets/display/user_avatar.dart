import 'package:dish_discover/widgets/pages/user.dart';
import 'package:dish_discover/widgets/style/style.dart';
import 'package:flutter/material.dart';

import '../../entities/app_state.dart';

class UserAvatar extends StatelessWidget {
  final Image? image;
  final double diameter;
  final String username;

  const UserAvatar(
      {super.key,
      required this.username,
      required this.image,
      this.diameter = 70.0});

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
        onTap: username == AppState.currentUser!.username
            ? null
            : () => Navigator.of(context).push(MaterialPageRoute(
                builder: (context) => UserPage(username: username))),
        child: Padding(
            padding: const EdgeInsets.all(5.0),
            child: CircleAvatar(
                radius: diameter / 2,
                foregroundImage: image?.image,
                backgroundImage:
                    Image.asset("assets/images/default_avatar.jpg").image,
                backgroundColor: inactiveColor)));
  }
}
