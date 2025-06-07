//import 'dart:io';
import '../reels_page.dart';
import 'package:flutter/material.dart';

class RecommendedTab extends StatefulWidget {
  const RecommendedTab({super.key});

  @override
  State<RecommendedTab> createState() => _RecommendedTabState();
}

class _RecommendedTabState extends State<RecommendedTab> {
  // Persistent events list
  final List<Map<String, String>> events = [
    {
      'name': 'Italian Cooking Class',
      'description': 'Learn to make authentic Italian pasta!',
      'image': 'assets/images/event_italian.jpg'
    },
    {
      'name': 'Sushi Workshop',
      'description': 'Master the art of sushi rolling.',
      'image': 'assets/images/event_sushi.jpg'
    },
    {
      'name': 'Vegan Feast',
      'description': 'Explore delicious plant-based recipes.',
      'image': 'assets/images/event_vegan.jpg'
    },
  ];

  // Test recipes with like and save state
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
          const Padding(
            padding: EdgeInsets.all(16.0),
            child: Text(
              'Discover New Flavors and Cuisines!',
              style: TextStyle(
                fontSize: 20,
                fontWeight: FontWeight.bold,
              ),
            ),
          ),

          // Horizontal scrollable event cards
          SizedBox(
            height: 220,
            child: ListView.separated(
              scrollDirection: Axis.horizontal,
              padding: const EdgeInsets.symmetric(horizontal: 16),
              itemCount: events.length + 1, // Add 1 for the "Add Event" button
              separatorBuilder: (context, index) => const SizedBox(width: 12),
              itemBuilder: (context, index) {
                if (index == events.length) {
                  // Add Event Button
                  return SizedBox(
                    width: 120,
                    child: Column(
                      children: [
                        FloatingActionButton(
                          shape: const CircleBorder(),
                          backgroundColor: Theme.of(context).primaryColor,
                          child: const Icon(Icons.event, color: Colors.white),
                          onPressed: () {
                            // Logic to add a new event
                            TextEditingController nameController =
                                TextEditingController();
                            TextEditingController descriptionController =
                                TextEditingController();
                            String? imagePath;

                            showDialog(
                              context: context,
                              builder: (context) {
                                return AlertDialog(
                                  title: const Text('Create Event'),
                                  content: SingleChildScrollView(
                                    child: Column(
                                      children: [
                                        TextField(
                                          controller: nameController,
                                          decoration: const InputDecoration(
                                            hintText: 'Event Name',
                                          ),
                                        ),
                                        const SizedBox(height: 8),
                                        TextField(
                                          controller: descriptionController,
                                          decoration: const InputDecoration(
                                            hintText: 'Event Description',
                                          ),
                                        ),
                                        const SizedBox(height: 8),
                                        ElevatedButton.icon(
                                          onPressed: () async {
                                            // Logic to upload an image
                                          },
                                          icon: const Icon(Icons.image),
                                          label: const Text('Upload Image'),
                                        ),
                                      ],
                                    ),
                                  ),
                                  actions: [
                                    TextButton(
                                      onPressed: () =>
                                          Navigator.of(context).pop(),
                                      child: const Text('Cancel'),
                                    ),
                                    TextButton(
                                      onPressed: () {
                                        if (nameController.text
                                                .trim()
                                                .isNotEmpty &&
                                            descriptionController.text
                                                .trim()
                                                .isNotEmpty) {
                                          setState(() {
                                            events.add({
                                              'name': nameController.text,
                                              'description':
                                                  descriptionController.text,
                                              'image': imagePath ??
                                                  'assets/images/event_placeholder.jpg',
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
                        const Text(
                          'Create Event',
                          style: TextStyle(
                            fontSize: 12,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                      ],
                    ),
                  );
                }

                final event = events[index];
                return SizedBox(
                  width: 300, // card width
                  child: Card(
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(15.0),
                    ),
                    elevation: 5,
                    child: InkWell(
                      onTap: () {
                        // Show event details in a popup
                        showDialog(
                          context: context,
                          builder: (context) {
                            return AlertDialog(
                              title: Text(event['name']!),
                              content: SingleChildScrollView(
                                child: Column(
                                  mainAxisSize: MainAxisSize.min,
                                  crossAxisAlignment: CrossAxisAlignment.start,
                                  children: [
                                    Image.asset(
                                      event['image']!,
                                      height: 150,
                                      width: double.infinity,
                                      fit: BoxFit.cover,
                                    ),
                                    const SizedBox(height: 8),
                                    Text(
                                      event['description']!,
                                      style: const TextStyle(fontSize: 14),
                                    ),
                                  ],
                                ),
                              ),
                              actions: [
                                TextButton(
                                  onPressed: () => Navigator.of(context).pop(),
                                  child: const Text('Close'),
                                ),
                              ],
                            );
                          },
                        );
                      },
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          ClipRRect(
                            borderRadius: const BorderRadius.vertical(
                              top: Radius.circular(15.0),
                            ),
                            child: Image.asset(
                              event['image']!,
                              height: 120,
                              width: double.infinity,
                              fit: BoxFit.cover,
                            ),
                          ),
                          Padding(
                            padding: const EdgeInsets.all(8.0),
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text(
                                  event['name']!,
                                  style: const TextStyle(
                                    fontSize: 16,
                                    fontWeight: FontWeight.bold,
                                  ),
                                ),
                                const SizedBox(height: 4),
                                Text(
                                  event['description']!,
                                  style: const TextStyle(
                                    fontSize: 12,
                                    color: Colors.grey,
                                  ),
                                ),
                              ],
                            ),
                          ),
                        ],
                      ),
                    ),
                  ),
                );
              },
            ),
          ),

          // Recipe list below the event cards
          Expanded(
            child: ListView.builder(
              itemCount: testRecipes.length,
              itemBuilder: (context, index) {
                final recipe = testRecipes[index];
                return Card(
                  margin:
                      const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
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
                                    color: recipe['likes'] > 0
                                        ? Colors.red
                                        : Colors.grey,
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
                                recipe['saved']
                                    ? Icons.bookmark
                                    : Icons.bookmark_border,
                                color:
                                    recipe['saved'] ? Colors.blue : Colors.grey,
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
          // FloatingActionButton(
          //   shape: const CircleBorder(),
          //   mini: true,
          //   child: const Icon(Icons.video_library),
          //   onPressed: () {
          //     showDialog(
          //       context: context,
          //       barrierDismissible: false,
          //       barrierColor: Colors.transparent,
          //       builder: (context) => const ReelsPage(),
          //     );
          //   },
          // ),
        ],
      ),
    );
  }
}
