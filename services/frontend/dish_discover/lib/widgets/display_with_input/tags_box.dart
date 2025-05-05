import 'package:dish_discover/widgets/display/tab_title.dart';
import 'package:dish_discover/widgets/display_with_input/tag_chip.dart';
import 'package:dish_discover/widgets/inputs/tag_input_box.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../entities/recipe.dart';
import '../../entities/tag.dart';

class TagsBox extends ConsumerWidget {
  final ChangeNotifierProvider<Recipe> recipeProvider;
  final bool forEditing;
  const TagsBox(
      {super.key, required this.recipeProvider, this.forEditing = false});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    Recipe recipe = ref.watch(recipeProvider);
    List<Tag> tags = recipe.tags;

    return Padding(
        padding: const EdgeInsets.symmetric(vertical: 5.0, horizontal: 20.0),
        child: Card(
            child: Padding(
                padding: const EdgeInsets.all(20),
                child: Flex(
                    direction: Axis.vertical,
                    mainAxisAlignment: MainAxisAlignment.start,
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      const Padding(
                          padding: EdgeInsets.only(bottom: 15),
                          child: TabTitle(title: "Tags")),
                      forEditing
                          ? TagInputBox(
                              onAdd: (name) => recipe.addTag(Tag(
                                  isPredefined: false,
                                  name: name,
                                  category: null)))
                          : Container(),
                      Wrap(
                          children: List.generate(
                              tags.length,
                              (index) => Padding(
                                  padding: const EdgeInsets.all(5),
                                  child: TagChip(
                                      tag: tags[index],
                                      onPressed: forEditing
                                          ? () => ref
                                              .watch(recipeProvider)
                                              .removeTag(tags[index])
                                          : null))))
                    ]))));
  }
}
