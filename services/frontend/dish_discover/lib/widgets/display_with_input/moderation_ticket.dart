import 'package:dish_discover/widgets/pages/view_recipe.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../entities/ticket.dart';
import '../display/tab_title.dart';
import '../display/user_avatar.dart';
import '../pages/user.dart';

class ModerationTicket extends ConsumerStatefulWidget {
  const ModerationTicket({super.key});

  @override
  ConsumerState<ConsumerStatefulWidget> createState() =>
      _ModerationTicketState();
}

class _ModerationTicketState extends ConsumerState<ModerationTicket> {
  ChangeNotifierProvider<Ticket>? ticketProvider;

  @override
  Widget build(BuildContext context) {
    return FutureBuilder(
        future: Ticket.getAssignedTicket(),
        builder: (context, ticketData) {
          if (ticketProvider != null) {
            return done();
          }

          if (ticketData.connectionState != ConnectionState.done) {
            return loading();
          }

          Ticket? ticket = ticketData.data;

          if (ticket == null) {
            if (kDebugMode) {
              ticket = Ticket(
                  reportId: 12345,
                  recipeId: null, //1,
                  violatorId: 'hells_kitchen_666',
                  issuerId: 'upstanding_citizen',
                  issuerAvatar: Image.asset("assets/images/launcher_icon.jpg"),
                  reason: 'Spreading satanism with their username >:(');
            } else {
              return none();
            }
          }

          ticketProvider = ChangeNotifierProvider<Ticket>((ref) => ticket!);
          return done();
        });
  }

  Widget none() {
    return const Padding(
        padding: EdgeInsets.symmetric(vertical: 5.0, horizontal: 20.0),
        child: Card(
            child: Padding(
                padding: EdgeInsets.all(15),
                child: Flex(
                    direction: Axis.vertical,
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Padding(
                          padding: EdgeInsets.only(bottom: 5.0),
                          child: TabTitle(
                              title: 'Complaint tickets', small: true)),
                      Center(child: Text('No complaints found'))
                    ]))));
  }

  Widget loading() {
    return const Padding(
        padding: EdgeInsets.symmetric(vertical: 5.0, horizontal: 20.0),
        child: Card(child: Center(child: Text('Loading...'))));
  }

  Widget done() {
    if (ticketProvider == null) {
      return none();
    }

    Ticket ticket = ref.watch(ticketProvider!);

    return Padding(
        padding: const EdgeInsets.symmetric(vertical: 5.0, horizontal: 20.0),
        child: Card(
            child: Flex(
                direction: Axis.vertical,
                mainAxisSize: MainAxisSize.min,
                children: [
              ListTile(
                  dense: true,
                  leading: UserAvatar(
                      username: ticket.issuerId,
                      image: ticket.issuerAvatar,
                      diameter: 30),
                  title: Text('Complaint #${ticket.reportId}',
                      softWrap: true, overflow: TextOverflow.ellipsis),
                  subtitle: GestureDetector(
                      onTap: () => Navigator.of(context).push(MaterialPageRoute(
                          builder: (context) =>
                              UserPage(username: ticket.issuerId))),
                      child: Text(ticket.issuerId,
                          softWrap: true, overflow: TextOverflow.ellipsis)),
                  trailing: AspectRatio(
                      aspectRatio: 2.0,
                      child: Flex(
                        direction: Axis.horizontal,
                        mainAxisSize: MainAxisSize.min,
                        mainAxisAlignment: MainAxisAlignment.end,
                        children: [
                          IconButton(
                              icon: const Icon(Icons.close),
                              onPressed: () {
                                setState(() {
                                  ticketProvider = null;
                                });
                              }),
                          IconButton(
                              icon: Icon(ticket.accepted
                                  ? Icons.delete
                                  : Icons.check_rounded),
                              onPressed: ticket.accepted
                                  ? () => setState(() {
                                        ticketProvider = null;
                                      })
                                  : () {
                                      ref.read(ticketProvider!).accepted = true;
                                      ref
                                          .read(ticketProvider!)
                                          .notifyListeners();
                                    })
                        ],
                      ))),
              const Divider(height: 1.0),
              AspectRatio(
                  aspectRatio: 1.9,
                  child: Align(
                      alignment: Alignment.topLeft,
                      child: Padding(
                          padding: const EdgeInsets.all(10.0),
                          child: Text("Reason: ${ticket.reason}",
                              maxLines: 50,
                              softWrap: true,
                              overflow: TextOverflow.ellipsis)))),
              const Divider(height: 1.0),
              Padding(
                  padding: const EdgeInsets.symmetric(
                      vertical: 5.0, horizontal: 10.0),
                  child: Flex(
                      direction: Axis.horizontal,
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        Expanded(
                            child: Text(
                                "${ticket.violatorId != null ? 'User' : ticket.commentId != null ? 'Comment' : 'Recipe'} : ${ticket.violatorId}",
                                softWrap: true,
                                overflow: TextOverflow.ellipsis)),
                        IconButton(
                            icon: const Icon(Icons.arrow_right_alt_rounded),
                            onPressed: () {
                              Navigator.of(context).push(MaterialPageRoute(
                                  builder: (context) => ticket.violatorId !=
                                          null
                                      ? UserPage(username: ticket.violatorId!)
                                      : ViewRecipePage(
                                          recipeId: ticket.recipeId!)));
                            })
                      ]))
            ])));
  }
}
