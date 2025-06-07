import '../reels_page.dart';
import 'package:flutter/material.dart';

class RecommendedTab extends StatefulWidget {
  const RecommendedTab({super.key});

  @override
  State<RecommendedTab> createState() => _RecommendedTabState();
}

class _RecommendedTabState extends State<RecommendedTab> {
  final List<Map<String, dynamic>> testRecipes = [
    {
      'id': 1,
      'title': 'My Test Recipe',
      'author': '123_debug',
      'description': 'This is a test recipe for debugging purposes.',
      'image': 'assets/images/logo.png',
      'likes': 0,
      'saved': false,
    },
    {
      'id': 2,
      'title': 'Chocolate Cake',
      'author': '123_debug',
      'description': 'A delicious chocolate cake recipe.',
      'image': 'assets/images/logo.png',
      'likes': 0,
      'saved': false,
    },
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Flex(
        direction: Axis.vertical,
        mainAxisSize: MainAxisSize.min,
        children: [
          Expanded(
            child: ListView.builder(
              itemCount: testRecipes.length,
              itemBuilder: (context, index) {
                final recipe = testRecipes[index];
                return Card(
                  margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      ListTile(
                        leading: const CircleAvatar(
                          backgroundColor: Colors.grey,
                          child: Icon(Icons.person, color: Colors.white),
                        ),
                        title: Text(recipe['title']),
                        subtitle: Text(recipe['author']),
                      ),
                      Image.asset(
                        recipe['image'],
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
                                    color: recipe['likes'] > 0 ? Colors.red : Colors.grey,
                                  ),
                                  onPressed: () {
                                    setState(() {
                                      recipe['likes']++;
                                    });
                                  },
                                ),
                                Text('${recipe['likes']}'),
                              ],
                            ),
                            IconButton(
                              icon: Icon(
                                recipe['saved'] ? Icons.bookmark : Icons.bookmark_border,
                                color: recipe['saved'] ? Colors.blue : Colors.grey,
                              ),
                              onPressed: () {
                                setState(() {
                                  recipe['saved'] = !recipe['saved'];
                                });
                              },
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),
                );
              },
            ),
          ),
        ],
      ),
      floatingActionButton: Column(
        mainAxisAlignment: MainAxisAlignment.end,
        children: [
          FloatingActionButton(
            shape: const CircleBorder(),
            mini: true,
            child: const Icon(Icons.add),
            onPressed: () {
              TextEditingController titleController = TextEditingController();

              showDialog(
                context: context,
                builder: (context) {
                  return AlertDialog(
                    title: const Text('Create Recipe'),
                    content: TextField(
                      controller: titleController,
                      decoration: const InputDecoration(
                        hintText: 'Recipe Title',
                      ),
                    ),
                    actions: [
                      TextButton(
                        onPressed: () => Navigator.of(context).pop(),
                        child: const Text('Cancel'),
                      ),
                      TextButton(
                        onPressed: () {
                          if (titleController.text.trim().isNotEmpty) {
                            setState(() {
                              testRecipes.add({
                                'id': testRecipes.length + 1,
                                'title': titleController.text,
                                'author': '123_debug',
                                'description': 'New recipe description.',
                                'image': 'assets/images/logo.png',
                                'likes': 0,
                                'saved': false,
                              });
                            });
                            Navigator.of(context).pop();
                          }
                        },
                        child: const Text('Create'),
                      ),
                    ],
                  );
                },
              );
            },
          ),
          const SizedBox(height: 8),
          FloatingActionButton(
            shape: const CircleBorder(),
            mini: true,
            child: const Icon(Icons.video_library),
            onPressed: () {
              Navigator.of(context).push(
                MaterialPageRoute(builder: (context) => const ReelsPage()),
              );
            },
          ),
        ],
      ),
    );
  }
}