import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../entities/new_recipe.dart';
import '../display_with_input/recipe_card.dart';
import 'no_results_card.dart';

class RecipeList extends StatefulWidget {
  // Updated to use RecipeResponse instead of List<Recipe>
  final Future<RecipeResponse> Function(int page) getRecipes;
  final String? searchQuery;
  const RecipeList({super.key, required this.getRecipes, this.searchQuery});
  

  @override
  State<StatefulWidget> createState() => _RecipeListState();
}

class _RecipeListState extends State<RecipeList> {
  late ScrollController scrollController;
  late Future<RecipeResponse> _recipesFuture;
  bool _isDisposed = false;
  int currentPage = 1;

  @override
  void initState() {
    super.initState();
    scrollController = ScrollController();
    _recipesFuture = widget.getRecipes(currentPage);
  }

  @override
  void didUpdateWidget(RecipeList oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (!_isDisposed && oldWidget.searchQuery != widget.searchQuery) {
      // Refresh the recipes when search query changes
      currentPage = 1;
      _refreshRecipes();
    }
  }

  void _refreshRecipes() {
    if (!_isDisposed) {
      setState(() {
        _recipesFuture = widget.getRecipes(currentPage);
      });
    }
  }
  
  void _goToNextPage() {
    setState(() {
      currentPage++;
      _refreshRecipes();
      // Scroll to top when changing pages
      if (scrollController.hasClients) {
        scrollController.animateTo(
          0,
          duration: const Duration(milliseconds: 300),
          curve: Curves.easeOut,
        );
      }
    });
  }

  void _goToPreviousPage() {
    if (currentPage > 1) {
      setState(() {
        currentPage--;
        _refreshRecipes();
        // Scroll to top when changing pages
        if (scrollController.hasClients) {
          scrollController.animateTo(
            0,
            duration: const Duration(milliseconds: 300),
            curve: Curves.easeOut,
          );
        }
      });
    }
  }

  @override
  void dispose() {
    _isDisposed = true;
    if (scrollController.hasClients) {
      scrollController.dispose();
    }
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder<RecipeResponse>(
        future: _recipesFuture, // Use the stored future instead of calling getRecipes() again
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
                      'Showing ${recipes.length} of $totalElements recipes (Page $currentPage of $totalPages)',
                      style: Theme.of(context).textTheme.bodySmall,
                    ),
                  ),
                  // Recipe list
                  Expanded(
                    child: ListView.builder(
                        controller: scrollController,
                        itemCount: recipes.length,
                        itemBuilder: (context, index) => RecipeCard(
                            recipeProvider:
                                ChangeNotifierProvider<Recipe>(
                                    (ref) => recipes[index]))),
                  ),
                  // Pagination controls
                  if (totalPages > 1)
                    Container(
                      padding: const EdgeInsets.all(16.0),
                      decoration: BoxDecoration(
                        color: Theme.of(context).scaffoldBackgroundColor,
                        boxShadow: [
                          BoxShadow(
                            color: Colors.grey.withOpacity(0.2),
                            spreadRadius: 1,
                            blurRadius: 4,
                            offset: const Offset(0, -2),
                          ),
                        ],
                      ),
                      child: Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          ElevatedButton.icon(
                            onPressed: currentPage > 1 ? _goToPreviousPage : null,
                            icon: const Icon(Icons.arrow_back),
                            label: const Text('Previous'),
                          ),
                          Text(
                            'Page $currentPage of $totalPages',
                            style: Theme.of(context).textTheme.titleMedium,
                          ),
                          ElevatedButton.icon(
                            onPressed: currentPage < totalPages ? _goToNextPage : null,
                            icon: const Icon(Icons.arrow_forward),
                            label: const Text('Next'),
                          ),
                        ],
                      ),
                      )
                ],
              )
            );
        });
  }
}