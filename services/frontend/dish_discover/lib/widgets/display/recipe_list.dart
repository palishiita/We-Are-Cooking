import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:scroll_to_top/scroll_to_top.dart';

import '../../entities/recipe.dart';
import '../display_with_input/recipe_card.dart';
import '../style/style.dart';
import 'no_results_card.dart';

class RecipeList extends StatefulWidget {
  final Future<List<Recipe>> Function() getRecipes;
  const RecipeList({super.key, required this.getRecipes});

  @override
  State<StatefulWidget> createState() => _RecipeListState();
}

class _RecipeListState extends State<RecipeList> {
  late ScrollController scrollController;

  @override
  void initState() {
    super.initState();
    scrollController = ScrollController();
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder(
        future: widget.getRecipes(),
        builder: (context, recipes) => recipes.connectionState !=
                ConnectionState.done
            ? const Center(child: Text("Loading..."))
            : recipes.hasError
                ? const SingleChildScrollView(
                    child: NoResultsCard(timedOut: true))
                : recipes.data == null || recipes.data!.isEmpty
                    ? const SingleChildScrollView(
                        child: NoResultsCard(timedOut: false))
                    : Expanded(
                        child: ScrollToTop(
                            // mini: true, // Removed as it is not a valid parameter
                            btnColor: buttonColor,
                            scrollController: scrollController,
                            child: ListView.builder(
                                controller: scrollController,
                                itemCount: recipes.data!.length,
                                itemBuilder: (context, index) => RecipeCard(
                                    recipeProvider:
                                        ChangeNotifierProvider<Recipe>(
                                            (ref) => recipes.data![index]))))));
  }
}
