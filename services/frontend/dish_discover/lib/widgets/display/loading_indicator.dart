import 'package:dish_discover/widgets/display/no_results_card.dart';
import 'package:dish_discover/widgets/display/tab_title.dart';
import 'package:flutter/material.dart';

import '../style/style.dart';

class LoadingIndicator extends StatelessWidget {
  final String? title;
  final bool showBackButton;
  const LoadingIndicator({super.key, this.title, this.showBackButton = true});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        appBar: AppBar(
            toolbarHeight: appBarHeight,
            scrolledUnderElevation: 0.0,
            title: TabTitle(title: title ?? ''),
            centerTitle: true,
            leading: showBackButton ? const BackButton() : null),
        body: SingleChildScrollView(
            child: Center(
                child: Text('Loading...',
                    style: Theme.of(context).textTheme.labelLarge))));
  }
}

class LoadingErrorIndicator extends StatelessWidget {
  final String? title;
  final bool showBackButton;
  final bool timedOut;
  final Widget? child;
  const LoadingErrorIndicator(
      {super.key,
      this.title,
      this.child,
      this.showBackButton = true,
      this.timedOut = true});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        appBar: AppBar(
            toolbarHeight: appBarHeight,
            scrolledUnderElevation: 0.0,
            title: TabTitle(title: title ?? ''),
            centerTitle: true,
            leading: showBackButton ? const BackButton() : null),
        body: SingleChildScrollView(
            child: Center(child: child ?? NoResultsCard(timedOut: timedOut))));
  }
}
