import 'package:flutter/material.dart';

import '../../display/tab_title.dart';
import '../../display_with_input/moderation_ticket.dart';
import '../../display_with_input/tag_management_box.dart';

class ModerationTab extends StatelessWidget {
  const ModerationTab({super.key});
  @override
  Widget build(BuildContext context) {
    return Flex(
        direction: Axis.vertical,
        mainAxisSize: MainAxisSize.min,
        children: [
          const TabTitle(title: 'Moderation'),
          Expanded(
              child: ListView(
                  children: const [ModerationTicket(), TagManagementBox()]))
        ]);
  }
}
