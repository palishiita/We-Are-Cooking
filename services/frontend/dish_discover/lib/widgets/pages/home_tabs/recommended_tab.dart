import 'package:flutter/material.dart';

import '../../../entities/app_state.dart';
import '../../display/recipe_list.dart';
import '../../display/tab_title.dart';

class RecommendedTab extends StatelessWidget {
  const RecommendedTab({super.key});

  @override
  Widget build(BuildContext context) {
    return Flex(
        direction: Axis.vertical,
        mainAxisSize: MainAxisSize.min,
        children: [
          const TabTitle(title: 'Recommended'),
          RecipeList(getRecipes: AppState.currentUser!.getRecommendations)
        ]);
  }
}
