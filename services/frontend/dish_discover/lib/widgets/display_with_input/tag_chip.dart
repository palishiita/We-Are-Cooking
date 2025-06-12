import 'package:dish_discover/widgets/pages/search.dart';
import 'package:flutter/material.dart';

import '../../entities/tag.dart';
import '../style/style.dart';

class TagChip extends StatelessWidget {
  final Tag tag;
  final int? occurrences;
  final IconData? icon;
  final void Function()? onPressed;

  const TagChip(
      {super.key,
      required this.tag,
      this.occurrences,
      this.icon = Icons.close,
      this.onPressed});

  String occurrencesShortString(int value) {
    if (value < 0) {
      value = 0;
    }

    int thousands = (value / 1000).floor();
    int hundreds = (value / 100).floor() % 10;

    return thousands == 0
        ? value.toString()
        : hundreds == 0
            ? "${thousands}K"
            : "$thousands.${hundreds}K";
  }

  @override
  Widget build(BuildContext context) {
    Widget label = occurrences != null
        ? Text('#${tag.name} ${occurrencesShortString(occurrences!)}',
            softWrap: true, maxLines: 3, overflow: TextOverflow.ellipsis)
        : Text('#${tag.name}', overflow: TextOverflow.ellipsis);

    return onPressed == null
        ? // searchable
        FilterChip(
            selected: true,
            showCheckmark: false,
            selectedColor: outerContainerColor(context),
            label: label,
            onSelected: (value) => Navigator.of(context).push(MaterialPageRoute(
                builder: (context) => SearchPage(
                      searchPhrase: "",
                      //filter: [tag],
                    ))))
        // deletable
        : InputChip(
            selected: true,
            showCheckmark: false,
            selectedColor: outerContainerColor(context),
            label: label,
            deleteIcon: Icon(icon),
            onDeleted: onPressed);
  }
}
