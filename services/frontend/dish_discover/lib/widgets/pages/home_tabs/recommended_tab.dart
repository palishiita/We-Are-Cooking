import 'package:flutter/material.dart';
import '../../../entities/app_state.dart';
import '../../../services/recommendation_service.dart';
import '../view_recipe.dart';

class RecommendedTab extends StatefulWidget {
  const RecommendedTab({super.key});

  @override
  State<RecommendedTab> createState() => _RecommendedTabState();
}

class _RecommendedTabState extends State<RecommendedTab> {
  late Future<List<Map<String, dynamic>>> _recipesFuture;
  final String userId = AppState.currentUser?.userId ?? '9aff0c98-1b53-4659-97d0-1b15027bde69';
  final Map<String, int> _likes = {};
  final Map<String, bool> _saved = {};

  @override
  void initState() {
    super.initState();
    _recipesFuture = RecommendationService.getRecommendedRecipeDetails(userId: userId, topN: 10);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Recommended Recipes'),
        centerTitle: true,
      ),
      body: FutureBuilder<List<Map<String, dynamic>>>(
        future: _recipesFuture,
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          } else if (snapshot.hasError) {
            return Center(child: Text('Error: [31m${snapshot.error}[0m'));
          } else if (!snapshot.hasData || snapshot.data!.isEmpty) {
            return const Center(child: Text('No recommendations found.'));
          }

          final recipes = snapshot.data!;

          return ListView.builder(
            itemCount: recipes.length,
            itemBuilder: (context, index) {
              final recipe = recipes[index];
              final title = recipe['title'] ?? '';
              final author = recipe['author'] ?? 'Unknown';
              final recipeId = recipe['id'] ?? title;
              _likes.putIfAbsent(title, () => 0);
              _saved.putIfAbsent(title, () => false);
              return Card(
                margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
                child: InkWell(
                  onTap: () {
                    Navigator.of(context).push(
                      MaterialPageRoute(
                        builder: (context) => ViewRecipePage(recipeId: recipeId),
                      ),
                    );
                  },
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      ListTile(
                        leading: const CircleAvatar(
                          backgroundColor: Colors.grey,
                          child: Icon(Icons.person, color: Colors.white),
                        ),
                        title: Text(title),
                        subtitle: Text('Author: $author'),
                      ),
                      Image.asset(
                        'assets/images/image.png',
                        height: 150,
                        width: double.infinity,
                        fit: BoxFit.cover,
                      ),
                      Padding(
                        padding: const EdgeInsets.all(8.0),
                        child: Row(
                          mainAxisAlignment: MainAxisAlignment.spaceBetween,
                          children: [
                            Row(
                              children: [
                                IconButton(
                                  icon: Icon(
                                    Icons.favorite,
                                    color: _likes[title]! > 0 ? Colors.red : Colors.grey,
                                  ),
                                  onPressed: () {
                                    setState(() {
                                      _likes[title] = _likes[title]! + 1;
                                    });
                                  },
                                ),
                                Text('${_likes[title]}'),
                              ],
                            ),
                            IconButton(
                              icon: Icon(
                                _saved[title]! ? Icons.bookmark : Icons.bookmark_border,
                                color: _saved[title]! ? Colors.blue : Colors.grey,
                              ),
                              onPressed: () {
                                setState(() {
                                  _saved[title] = !_saved[title]!;
                                });
                              },
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),
                ),
              );
            },
          );
        },
      ),
    );
  }
}