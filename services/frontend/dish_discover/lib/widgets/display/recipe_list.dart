import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:scroll_to_top/scroll_to_top.dart';

import '../../entities/new_recipe.dart';
import '../display_with_input/recipe_card.dart';
import '../style/style.dart';
import 'no_results_card.dart';

class RecipeList extends StatefulWidget {
  // Updated to use RecipeResponse instead of List<Recipe>
  final Future<RecipeResponse> Function() getRecipes;
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
    return FutureBuilder<RecipeResponse>(
        future: widget.getRecipes(),
        builder: (context, snapshot) {
          // Handle loading state
          if (snapshot.connectionState != ConnectionState.done) {
            return const Center(child: Text("Loading..."));
          }
          
          // Handle error state
          if (snapshot.hasError) {
            return const SingleChildScrollView(
                child: NoResultsCard(timedOut: true));
          }
          
          // Handle null or empty data
          if (snapshot.data == null || snapshot.data!.data.isEmpty) {
            return const SingleChildScrollView(
                child: NoResultsCard(timedOut: false));
          }

          // Access the actual recipe list from RecipeResponse.data
          final recipes = snapshot.data!.data;
          final totalElements = snapshot.data!.totalElements;
          final currentPage = snapshot.data!.page;
          final totalPages = snapshot.data!.totalPages;

          return Expanded(
              child: Column(
                children: [
                  // Optional: Show pagination info
                  Padding(
                    padding: const EdgeInsets.all(8.0),
                    child: Text(
                      'Showing ${recipes.length} of $totalElements recipes (Page ${currentPage + 1} of $totalPages)',
                      style: Theme.of(context).textTheme.bodySmall,
                    ),
                  ),
                  // Recipe list
                  Expanded(
                    child: ScrollToTop(
                        btnColor: buttonColor,
                        scrollController: scrollController,
                        child: ListView.builder(
                            controller: scrollController,
                            itemCount: recipes.length,
                            itemBuilder: (context, index) => RecipeCard(
                                recipeProvider:
                                    ChangeNotifierProvider<Recipe>(
                                        (ref) => recipes[index])))),
                  ),
                ],
              ));
        });
  }
}