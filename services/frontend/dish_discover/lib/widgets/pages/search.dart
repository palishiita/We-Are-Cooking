import 'package:dish_discover/widgets/inputs/custom_search_bar.dart';
import 'package:dish_discover/widgets/style/style.dart';
import 'package:flutter/material.dart';

import '../../entities/new_recipe.dart';
import '../display/recipe_list.dart';

class SearchPage extends StatefulWidget {
  static const routeName = "/search";
  final String searchPhrase;

  const SearchPage({super.key, required this.searchPhrase});

  @override
  State<StatefulWidget> createState() => _SearchPageState();
}

class _SearchPageState extends State<SearchPage> {
  late String searchPhrase;
  late String currentSearchTerm; // The term being typed
  DateTime lastRefresh = DateTime.now();
  
  @override
  void initState() {
    super.initState();
    searchPhrase = widget.searchPhrase;
    currentSearchTerm = widget.searchPhrase;
  }

  void _onSearchSubmitted(String submittedSearchPhrase) {
    setState(() {
      searchPhrase = submittedSearchPhrase;
    });
  }

 @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        toolbarHeight: appBarHeight,
        scrolledUnderElevation: 0.0,
        title: const Text('Search'),
        centerTitle: true,
        leading: const BackButton(),
      ),
      body: Flex(
          direction: Axis.vertical,
          mainAxisSize: MainAxisSize.min,
          children: [
            CustomSearchBar(
                initialSearchPhrase: searchPhrase, 
                goToSearchPage: false,
                onSearch: _onSearchSubmitted), // Only rebuild on submit
            RecipeList(
                searchQuery: searchPhrase, // Pass search as parameter
                getRecipes: (page) => Recipe.getRecipes(
                  query: searchPhrase.isEmpty ? null : searchPhrase,
                  count: 10,
                  page: page,
                  sortBy: null,
                  orderByAsc: true,
                ))
          ]),
    );
  }
}