import 'package:dish_discover/widgets/inputs/tag_input_box.dart';
import 'package:flutter/material.dart';

import '../../entities/tag.dart';
import '../display/tab_title.dart';
import '../display_with_input/tag_chip.dart';

class FilterSideMenu extends StatefulWidget {
  final List<Tag> filter;
  final void Function(List<Tag>) onFilter;
  const FilterSideMenu(
      {super.key, required this.filter, required this.onFilter});

  @override
  State<StatefulWidget> createState() => _FilterSideMenuState();
}

class _FilterSideMenuState extends State<FilterSideMenu> {
  late List<Tag> currentFilter;

  @override
  void initState() {
    super.initState();
    currentFilter = widget.filter;
  }

  @override
  Widget build(BuildContext context) {
    return Drawer(
        width: MediaQuery.of(context).size.width * 0.85,
        shape: const RoundedRectangleBorder(
            borderRadius: BorderRadius.horizontal(left: Radius.circular(30.0))),
        child: Padding(
            padding: const EdgeInsets.all(20),
            child: Flex(
                direction: Axis.vertical,
                mainAxisSize: MainAxisSize.min,
                children: [
                  const TabTitle(title: 'Filter'),
                  TagInputBox(
                      onAdd: (tagName) => setState(() {
                            if (!currentFilter.any((e) => e.name == tagName)) {
                              currentFilter
                                  .add(Tag(name: tagName, isPredefined: false));
                            }
                          })),
                  Align(
                      alignment: Alignment.centerLeft,
                      child: Wrap(
                          children: List.generate(
                              currentFilter.length,
                              (index) => Padding(
                                  padding: const EdgeInsets.all(5),
                                  child: TagChip(
                                      tag: currentFilter[index],
                                      onPressed: () => setState(() {
                                            currentFilter
                                                .remove(currentFilter[index]);
                                          })))))),
                  Expanded(
                      child: Align(
                          alignment: Alignment.bottomCenter,
                          child: OutlinedButton(
                              onPressed: () => widget.onFilter(currentFilter),
                              child: const Text('Filter'))))
                ])));
  }
}
