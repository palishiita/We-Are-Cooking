import 'package:dish_discover/widgets/display/tab_title.dart';
import 'package:dish_discover/widgets/inputs/custom_text_field.dart';
import 'package:dish_discover/widgets/pages/user.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../entities/new_recipe.dart';

class RecipeHeader extends ConsumerStatefulWidget {
  final ChangeNotifierProvider<Recipe> recipeProvider;
  final bool forEditing;

  const RecipeHeader(
      {super.key, required this.recipeProvider, this.forEditing = false});

  @override
  ConsumerState<ConsumerStatefulWidget> createState() => _RecipeHeaderState();
}

class _RecipeHeaderState extends ConsumerState<RecipeHeader> {
  late TextEditingController titleController;
  late TextEditingController descriptionController;

  @override
  void initState() {
    super.initState();

    titleController = TextEditingController();
    descriptionController = TextEditingController();
  }

  @override
  Widget build(BuildContext context) {
    return widget.forEditing
        ? editable(context, ref.read(widget.recipeProvider))
        : notEditable(context, ref);
  }

  Widget notEditable(BuildContext context, WidgetRef ref) {
    Recipe recipe = ref.watch(widget.recipeProvider);

    return Flex(
        direction: Axis.vertical,
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          TabTitle(title: recipe.title),
          GestureDetector(
              child: Padding(
                  padding: const EdgeInsets.all(15),
                  child: Text(recipe.author,
                      overflow: TextOverflow.ellipsis,
                      style: Theme.of(context)
                          .textTheme
                          .titleSmall
                          ?.copyWith(color: Colors.black54))),
              onTap: () => Navigator.of(context).push(MaterialPageRoute(
                  builder: (context) => UserPage(username: recipe.author)))),
          Padding(
              padding: const EdgeInsets.all(15),
              child: Center(
                  child: Text(recipe.description,
                      textAlign: TextAlign.center,
                      maxLines: 50,
                      softWrap: true,
                      overflow: TextOverflow.ellipsis))),
          const Divider(height: 2)
        ]);
  }

  Widget editable(BuildContext context, Recipe recipe) {
    titleController.text = recipe.title;
    descriptionController.text = recipe.description;

    return Flex(
        direction: Axis.vertical,
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Padding(
              padding: const EdgeInsets.all(15),
              child: CustomTextField(
                  controller: titleController,
                  hintText: 'Title',
                  maxLength: 50,
                  onChanged: (value) => recipe.editRecipe(title: value))),
          Padding(
              padding: const EdgeInsets.all(15),
              child: Center(
                  child: CustomTextField(
                controller: descriptionController,
                hintText: 'Description',
                maxLength: 150,
                onChanged: (value) => recipe.editRecipe(description: value),
              ))),
          const Divider(height: 2)
        ]);
  }
}
