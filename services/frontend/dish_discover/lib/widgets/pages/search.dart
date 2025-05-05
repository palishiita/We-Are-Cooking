import 'package:dish_discover/widgets/dialogs/filter_side_menu.dart';
import 'package:dish_discover/widgets/inputs/custom_search_bar.dart';
import 'package:dish_discover/widgets/style/style.dart';
import 'package:flutter/material.dart';

import '../../entities/recipe.dart';
import '../../entities/tag.dart';
import '../display/recipe_list.dart';

class SearchPage extends StatefulWidget {
  static const routeName = "/search";
  final String searchPhrase;
  final List<Tag>? filter;

  const SearchPage({super.key, required this.searchPhrase, this.filter});

  @override
  State<StatefulWidget> createState() => _SearchPageState();
}

class _SearchPageState extends State<SearchPage> {
  late List<Tag> currentFilter;
  late String searchPhrase;

  @override
  void initState() {
    super.initState();
    currentFilter = widget.filter ?? [];
    searchPhrase = widget.searchPhrase;
  }

  @override
  Widget build(BuildContext context) {
    final scaffoldKey = GlobalKey<ScaffoldState>();

    return Scaffold(
        key: scaffoldKey,
        appBar: AppBar(
          toolbarHeight: appBarHeight,
          scrolledUnderElevation: 0.0,
          title: const Text('Search'),
          centerTitle: true,
          leading: const BackButton(),
          actions: [
            IconButton(
                onPressed: () => scaffoldKey.currentState!.openEndDrawer(),
                icon: const Icon(Icons.tune_rounded))
          ],
        ),
        body: Flex(
            direction: Axis.vertical,
            mainAxisSize: MainAxisSize.min,
            children: [
              CustomSearchBar(
                  initialSearchPhrase: searchPhrase, goToSearchPage: false),
              RecipeList(
                  getRecipes: () => Future<List<Recipe>>(() async =>
                      Recipe.filterRecipes(
                          await Recipe.getRecipes(searchPhrase),
                          currentFilter)))
            ]),
        endDrawer: FilterSideMenu(
            filter: currentFilter,
            onFilter: (newFilter) => setState(() {
                  currentFilter = newFilter;
                })));
  }
}
