import 'dart:math';

import 'package:dish_discover/entities/ingredient.dart';
import 'package:dish_discover/widgets/dialogs/custom_dialog.dart';
import 'package:dish_discover/widgets/display/tab_title.dart';
import 'package:dish_discover/widgets/inputs/custom_text_field.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../entities/recipe.dart';

class MultiplierField extends StatelessWidget {
  final TextEditingController controller;
  final FocusNode focusNode;
  final String hintText;
  final int maxLength;
  final double width;
  final double height;
  final void Function(String)? onChanged;
  final Icon? leadingIcon;

  /// Custom padded TextField for user input. Hides the character
  /// count that appears when using max length.
  const MultiplierField(
      {super.key,
      required this.controller,
      required this.focusNode,
      required this.hintText,
      this.maxLength = 45,
      this.width = 40,
      this.height = 30,
      this.onChanged,
      this.leadingIcon});

  @override
  Widget build(BuildContext context) {
    return SizedBox(
        width: width,
        height: height,
        child: TextField(
          controller: controller,
          textAlignVertical: TextAlignVertical.center,
          keyboardType: TextInputType.number,
          focusNode: FocusNode(), // TODO fix focus jumping
          maxLength: maxLength,
          buildCounter: (BuildContext context,
                  {required int currentLength,
                  required bool isFocused,
                  required int? maxLength}) =>
              null,
          style: Theme.of(context).textTheme.bodyMedium,
          decoration: InputDecoration(
              filled: false,
              border:
                  OutlineInputBorder(borderRadius: BorderRadius.circular(30.0)),
              hintText: hintText,
              prefixIcon: leadingIcon),
          onChanged: onChanged,
        ));
  }
}

class IngredientsBox extends ConsumerStatefulWidget {
  final ChangeNotifierProvider<Recipe> recipeProvider;
  final bool forEditing;
  const IngredientsBox(
      {super.key, required this.recipeProvider, this.forEditing = false});

  @override
  ConsumerState<ConsumerStatefulWidget> createState() => _IngredientsBoxState();
}

class _IngredientsBoxState extends ConsumerState<IngredientsBox> {
  late FocusNode focusNode;
  late TextEditingController multiplierController;
  late double multiplier;
  late TextEditingController nameController;
  late TextEditingController quantityController;
  late TextEditingController unitController;
  late TextEditingController caloricDensityController;

  @override
  void initState() {
    super.initState();
    multiplier = 1.0;
    multiplierController =
        TextEditingController(text: doubleToString(multiplier));
    focusNode = FocusNode();

    nameController = TextEditingController();
    quantityController = TextEditingController();
    unitController = TextEditingController();
    caloricDensityController = TextEditingController();
  }

  String doubleToString(double d) {
    int rounded = d.round();
    String s = d == rounded ? '$rounded.0' : '$d';
    return s.substring(0, min(s.length, 4));
  }

  double stringToDouble(String s) {
    double d = min(9999, max(0, double.tryParse(s) ?? 0));
    s = d.toString();
    s = s.substring(0, min(s.length, 4));
    return double.tryParse(s) ?? 0;
  }

  String ingredientToString(Ingredient ingredient) {
    String amount = doubleToString(ingredient.quantity * multiplier);
    String units = ingredient.unit == null || ingredient.unit!.isEmpty
        ? ''
        : ' ${ingredient.unit}';
    return "${ingredient.name}: $amount$units";
  }

  void callIngredientDialog(bool add, Recipe recipe, int? index) {
    if (add) {
      nameController.text = '';
      quantityController.text = '';
      unitController.text = '';
      caloricDensityController.text = '';
    } else {
      nameController.text = recipe.ingredients[index!].name;
      quantityController.text = recipe.ingredients[index].quantity.toString();
      unitController.text = recipe.ingredients[index].unit?.toString() ?? '';
      caloricDensityController.text =
          recipe.ingredients[index].caloricDensity?.toString() ?? '';
    }

    CustomDialog.callDialog(
        context,
        add ? 'Add ingredient' : 'Edit ingredient',
        '',
        null,
        Flex(
          direction: Axis.vertical,
          children: [
            CustomTextField(controller: nameController, hintText: 'Name'),
            CustomTextField(
                controller: quantityController, hintText: 'Quantity'),
            CustomTextField(controller: unitController, hintText: 'Units'),
            CustomTextField(
                controller: caloricDensityController,
                hintText: 'Caloric density'),
          ],
        ),
        add ? 'Add' : 'Save', () {
      Ingredient newIngredient = Ingredient(
          id: index == null ? 0 : recipe.ingredients[index].id,
          name: nameController.text,
          quantity: double.tryParse(quantityController.text) ??
              (index == null ? 1.0 : recipe.ingredients[index].quantity),
          unit: unitController.text,
          caloricDensity: int.tryParse(caloricDensityController.text));

      if (add) {
        recipe.addIngredient(newIngredient);
      } else {
        recipe.updateIngredient(index!, newIngredient);
      }

      return null;
    });
  }

  @override
  Widget build(BuildContext context) {
    Recipe recipe = ref.watch(widget.recipeProvider);

    return Padding(
        padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 5.0),
        child: Card(
            child: Stack(children: [
          Padding(
              padding: const EdgeInsets.all(20.0),
              child: Align(
                  alignment: Alignment.topRight,
                  child: widget.forEditing
                      ? IconButton(
                          onPressed: () =>
                              callIngredientDialog(true, recipe, null),
                          icon: const Icon(Icons.add))
                      : MultiplierField(
                          controller: multiplierController,
                          focusNode: focusNode,
                          hintText: 'N',
                          maxLength: 4,
                          width: 90,
                          height: 45,
                          onChanged: (value) => setState(() {
                                multiplier = stringToDouble(value);
                                multiplierController.text =
                                    doubleToString(multiplier);
                              }),
                          leadingIcon: const Icon(
                            Icons.clear,
                            size: 16,
                          )))),
          Padding(
              padding:
                  const EdgeInsets.symmetric(vertical: 20.0, horizontal: 10.0),
              child: Flex(
                  direction: Axis.vertical,
                  mainAxisSize: MainAxisSize.min,
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const Padding(
                        padding: EdgeInsets.only(bottom: 5.0),
                        child: TabTitle(title: "Ingredients")),
                    (recipe.ingredients.isEmpty
                        ? const Padding(
                            padding: EdgeInsets.all(10.0), child: Text("   -"))
                        : Flex(
                            direction: Axis.vertical,
                            mainAxisSize: MainAxisSize.min,
                            crossAxisAlignment: CrossAxisAlignment.stretch,
                            children: List.generate(
                                recipe.ingredients.length,
                                (index) => Padding(
                                    padding: EdgeInsets.all(
                                        widget.forEditing ? 5.0 : 10.0),
                                    child: widget.forEditing
                                        ? Flex(
                                            direction: Axis.horizontal,
                                            children: [
                                                IconButton(
                                                    onPressed: () =>
                                                        recipe.removeIngredient(
                                                            recipe.ingredients[
                                                                index]),
                                                    icon: const Icon(
                                                        Icons.close)),
                                                InkWell(
                                                    onTap: () =>
                                                        callIngredientDialog(
                                                            false,
                                                            recipe,
                                                            index),
                                                    child: Text(
                                                        "  ${ingredientToString(recipe.ingredients[index])}",
                                                        overflow: TextOverflow
                                                            .ellipsis))
                                              ])
                                        : Text(
                                            "\u2022  ${ingredientToString(recipe.ingredients[index])}",
                                            overflow: TextOverflow.ellipsis)))))
                  ]))
        ])));
  }
}
