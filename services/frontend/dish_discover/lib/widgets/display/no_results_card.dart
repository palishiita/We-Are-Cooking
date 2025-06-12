import 'package:flutter/material.dart';

class NoResultsCard extends StatelessWidget {
  final bool timedOut;
  const NoResultsCard({super.key, this.timedOut = false});

  @override
  Widget build(BuildContext context) {
    return Padding(
        padding: const EdgeInsets.symmetric(vertical: 5.0, horizontal: 20.0),
        child: AspectRatio(
            aspectRatio: 1.2,
            child: Card(
                child: Center(
                    child: Text(timedOut
                        ? "Connection timed out :(\nYou might be offline"
                        : "Sorry, nothing to see here :(\n")))));
  }
}
