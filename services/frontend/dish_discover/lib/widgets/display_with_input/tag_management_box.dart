import 'package:dish_discover/widgets/display_with_input/tag_chip.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';

import '../../entities/tag.dart';
import '../dialogs/custom_dialog.dart';
import '../display/tab_title.dart';

class TagManagementBox extends StatefulWidget {
  const TagManagementBox({super.key});

  @override
  State<StatefulWidget> createState() => _TagManagementBoxState();
}

class _TagManagementBoxState extends State<TagManagementBox> {
  List<(Tag, int)>? popularTags;

  @override
  Widget build(BuildContext context) {
    return FutureBuilder(
        future: Tag.getPopularNotPredefinedTags(),
        builder: (context, tagList) {
          if (popularTags != null) {
            return done();
          }

          if (tagList.connectionState != ConnectionState.done) {
            return loading();
          }

          popularTags = tagList.data;

          if (popularTags == null || popularTags!.isEmpty) {
            if (kDebugMode) {
              popularTags = [
                (
                  Tag(
                      isPredefined: false,
                      name: 'high in protein',
                      category: TagCategory.diet),
                  10020
                ),
                (
                  Tag(
                      isPredefined: false,
                      name: 'with calories',
                      category: TagCategory.diet),
                  1670
                ),
                (
                  Tag(
                      isPredefined: false,
                      name: 'for students',
                      category: TagCategory.time),
                  390
                ),
                (
                  Tag(
                      isPredefined: false,
                      name: 'kuchnia slaska',
                      category: TagCategory.cuisine),
                  78
                )
              ];
            } else {
              popularTags = [];
            }
          }

          return done();
        });
  }

  Widget loading() {
    return const Padding(
        padding: EdgeInsets.symmetric(vertical: 5.0, horizontal: 20.0),
        child: Card(child: Center(child: Text('Loading...'))));
  }

  Widget done() {
    popularTags!.sort((a, b) => b.$2.compareTo(a.$2));

    return Padding(
        padding: const EdgeInsets.symmetric(vertical: 5.0, horizontal: 20.0),
        child: Card(
            child: Padding(
                padding: const EdgeInsets.all(15),
                child: Flex(
                    direction: Axis.vertical,
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      const Padding(
                          padding: EdgeInsets.only(bottom: 5.0),
                          child: TabTitle(
                              title: 'Add predefined tag', small: true)),
                      Flex(
                          direction: Axis.vertical,
                          mainAxisSize: MainAxisSize.min,
                          children: popularTags!.isEmpty
                              ? [
                                  const Padding(
                                      padding: EdgeInsets.all(5.0),
                                      child: Center(
                                          child: Text("No candidates found")))
                                ]
                              : List.generate(
                                  popularTags!.length,
                                  (index) => Flex(
                                          direction: Axis.horizontal,
                                          mainAxisSize: MainAxisSize.max,
                                          mainAxisAlignment:
                                              MainAxisAlignment.spaceBetween,
                                          children: [
                                            TagChip(
                                                tag: popularTags![index].$1,
                                                occurrences:
                                                    popularTags![index].$2),
                                            IconButton(
                                                icon: const Icon(Icons.add),
                                                onPressed: () =>
                                                    CustomDialog.callDialog(
                                                        context,
                                                        'Add predefined tag',
                                                        '#${popularTags![index].$1.name} - ${popularTags![index].$1.category?.name ?? 'null'}',
                                                        null,
                                                        Container(),
                                                        'Add', () {
                                                      setState(() {
                                                        popularTags!
                                                            .removeAt(index);
                                                      });
                                                      return null;
                                                    }))
                                          ])))
                    ]))));
  }
}
