import 'package:dish_discover/widgets/style/style.dart';
import 'package:flutter/material.dart';

class TutorialPage extends StatelessWidget {
  static const routeName = "/tutorial";
  const TutorialPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        appBar: AppBar(
            toolbarHeight: appBarHeight,
            scrolledUnderElevation: 0.0,
            title: const Text('Tutorial'),
            centerTitle: true,
            leading: Container(),
            actions: [
              IconButton(
                  icon: const Icon(Icons.close),
                  onPressed: () => Navigator.of(context).pop())
            ]),
        body: Padding(
            padding: const EdgeInsets.fromLTRB(30.0, 10.0, 30.0, 35.0),
            child: Card(
                color: backgroundColor(context),
                elevation: 0,
                shape: const RoundedRectangleBorder(
                    side: BorderSide(),
                    borderRadius: BorderRadius.all(Radius.circular(20.0))),
                child: DecoratedBox(
                    decoration: BoxDecoration(
                        borderRadius:
                            const BorderRadius.all(Radius.circular(20.0)),
                        image: DecorationImage(
                            fit: BoxFit.fill,
                            image: Image.asset("assets/images/tutorial_1.png")
                                .image)),
                    child: Container()))));
  }
}
