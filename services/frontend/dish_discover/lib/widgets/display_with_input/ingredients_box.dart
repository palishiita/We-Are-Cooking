import 'dart:math';

import 'package:dish_discover/entities/ingredient.dart';
import 'package:dish_discover/widgets/dialogs/custom_dialog.dart';
import 'package:dish_discover/widgets/display/tab_title.dart';
import 'package:dish_discover/widgets/inputs/custom_text_field.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../entities/new_recipe.dart';
import '../../entities/ingredient_helpers.dart';

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

  
  // Database-driven options
  List<IngredientOption> availableIngredients = [];
  List<UnitOption> availableUnits = [];
  
  // Selected values for the dialog
  IngredientOption? selectedIngredient;
  UnitOption? selectedUnit;
  double selectedQuantity = 1.0;

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
    
    _loadDatabaseOptions();
  }
  
  // Load ingredients and units from database
  Future<void> _loadDatabaseOptions() async {
    try {
      availableIngredients = await IngredientService.getAvailableIngredients();
      // this should be constant, unit count stays the same
      availableUnits = await IngredientService.getAvailableUnits(count: 20, page: 1);
      setState(() {});
    } catch (e) {
      //if (kDebugMode) {
        print('Error loading database options: $e');
      //}
    }
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

  String ingredientToStringOld(Ingredient ingredient) {
    String amount = doubleToString(ingredient.quantity * multiplier);
    String units = ingredient.unit == null || ingredient.unit!.isEmpty
        ? ''
        : ' ${ingredient.unit}';
    return "${ingredient.name}: $amount$units";
  }

  String ingredientToString(RecipeIngredient ingredient, int index) {
    String amount = doubleToString(ingredient.quantity * multiplier);
    String units = ingredient.unit.isEmpty ? '' : ' ${ingredient.unit}';
    return "${ingredient.name} #${index + 1} - $amount$units";
  }

  void _showQuantityPicker() {
    showDialog(
      context: context,
      builder: (BuildContext context) {
        double tempQuantity = selectedQuantity;
        return AlertDialog(
          title: const Text('Select Quantity'),
          content: SizedBox(
            height: 250,
            child: Column(
              children: [
                Text(
                  tempQuantity.toStringAsFixed(1),
                  style: Theme.of(context).textTheme.headlineMedium,
                ),
                const SizedBox(height: 20),
                Expanded(
                  child: ListWheelScrollView.useDelegate(
                    itemExtent: 50,
                    physics: const FixedExtentScrollPhysics(),
                    controller: FixedExtentScrollController(
                      initialItem: ((selectedQuantity - 0.1) / 0.1).round(),
                    ),
                    onSelectedItemChanged: (index) {
                      tempQuantity = (index * 0.1) + 0.1;
                    },
                    childDelegate: ListWheelChildBuilderDelegate(
                      builder: (context, index) {
                        final value = (index * 0.1) + 0.1;
                        return Center(
                          child: Text(
                            value.toStringAsFixed(1),
                            style: const TextStyle(fontSize: 18),
                          ),
                        );
                      },
                      childCount: 1000, // Allows up to 100.0
                    ),
                  ),
                ),
              ],
            ),
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.of(context).pop(),
              child: const Text('Cancel'),
            ),
            TextButton(
              onPressed: () {
                setState(() {
                  selectedQuantity = tempQuantity;
                });
                Navigator.of(context).pop();
              },
              child: const Text('OK'),
            ),
          ],
        );
      },
    );
  }

  // Updated ingredient dialog with database-driven dropdowns and quantity picker
  void callIngredientDialog(bool add, Recipe recipe, int? index) {
    if (add) {
      selectedIngredient = null;
      selectedUnit = null;
      selectedQuantity = 1.0;
    } else {
      // Find the existing ingredient and unit from the database options
      selectedIngredient = availableIngredients.firstWhere(
        (ing) => ing.id == recipe.ingredients[index!].ingredientId,
        orElse: () => availableIngredients.isNotEmpty ? availableIngredients.first : IngredientOption(id: '', name: ''),
      );
      selectedUnit = availableUnits.firstWhere(
        (unit) => unit.id == recipe.ingredients[index!].unitId,
        orElse: () => availableUnits.isNotEmpty ? availableUnits.first : UnitOption(id: '', name: ''),
      );
      print('Selected unit: ${selectedUnit!.name}');
      print('Selected ingredient: ${selectedIngredient!.name}');
      selectedQuantity = recipe.ingredients[index!].quantity;
    }

    CustomDialog.callDialog(
        context,
        add ? 'Add ingredient' : 'Edit ingredient',
        '',
        null,
        StatefulBuilder(
          builder: (context, setDialogState) {
            return Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                // Ingredient Dropdown
                DropdownButtonFormField<IngredientOption>(
                  value: selectedIngredient,
                  decoration: const InputDecoration(
                    labelText: 'Ingredient',
                    border: OutlineInputBorder(),
                  ),
                  items: availableIngredients.map((ingredient) {
                    return DropdownMenuItem<IngredientOption>(
                      value: ingredient,
                      child: Text(ingredient.name),
                    );
                  }).toList(),
                  onChanged: (IngredientOption? newValue) {
                    setDialogState(() {
                      selectedIngredient = newValue;
                    });
                  },
                ),
                
                const SizedBox(height: 16),
                
                // Quantity Picker
                InkWell(
                  onTap: () {
                    _showQuantityPicker();
                    // Refresh dialog state after quantity picker closes
                    Future.delayed(const Duration(milliseconds: 100), () {
                      setDialogState(() {});
                    });
                  },
                  child: Container(
                    padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 16),
                    decoration: BoxDecoration(
                      border: Border.all(color: Colors.grey),
                      borderRadius: BorderRadius.circular(4),
                    ),
                    child: Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        Text('Quantity: ${selectedQuantity.toStringAsFixed(1)}'),
                        const Icon(Icons.arrow_drop_down),
                      ],
                    ),
                  ),
                ),
                
                const SizedBox(height: 16),
                
                // Unit Dropdown
                DropdownButtonFormField<UnitOption>(
                  value: selectedUnit,
                  decoration: const InputDecoration(
                    labelText: 'Unit',
                    border: OutlineInputBorder(),
                  ),
                  items: availableUnits.map((unit) {
                    return DropdownMenuItem<UnitOption>(
                      value: unit,
                      child: Text(unit.name),
                    );
                  }).toList(),
                  onChanged: (UnitOption? newValue) {
                    setDialogState(() {
                      selectedUnit = newValue;
                    });
                  },
                ),
              ],
            );
          },
        ),
        add ? 'Add' : 'Save', () {
      if (selectedIngredient == null || selectedUnit == null) {
        // Show error or return without saving
        return 'Please select both ingredient and unit';
      }

      RecipeIngredient newIngredient = RecipeIngredient(
          ingredientId: selectedIngredient!.id,
          name: selectedIngredient!.name,
          description: 'default description',
          quantity: selectedQuantity,
          unit: selectedUnit!.name,
          unitId: selectedUnit!.id);

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
                      : Row(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            Text(
                              "Quantity multiplier:",
                              style: Theme.of(context).textTheme.bodyMedium,
                            ),
                            const SizedBox(width: 8),
                            MultiplierField(
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
                                ))
                          ],))),
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
                                                Expanded(
                                                    child: InkWell(
                                                        onTap: () =>
                                                            callIngredientDialog(
                                                                false,
                                                                recipe,
                                                                index),
                                                        child: Text(
                                                            "  ${recipe.ingredients[index].name} #${index + 1} - ${doubleToString(recipe.ingredients[index].quantity)} ${recipe.ingredients[index].unit}",
                                                            overflow: TextOverflow
                                                                .ellipsis)))
                                              ])
                                        : Text(
                                            "\u2022  ${recipe.ingredients[index].name} #${index + 1} - ${doubleToString(recipe.ingredients[index].quantity * multiplier)} ${recipe.ingredients[index].unit}",
                                            overflow: TextOverflow.ellipsis)))))
                  ]))
        ])));
  }
}
