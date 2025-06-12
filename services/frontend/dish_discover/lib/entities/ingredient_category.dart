class IngredientCategory {
  final String id; // UUID stored as String
  final String name;
  final String description;

  IngredientCategory({
    required this.id,
    required this.name,
    required this.description,
  });

  factory IngredientCategory.fromJson(Map<String, dynamic> json) {
    return IngredientCategory(
      id: json['id'],
      name: json['name'],
      description: json['description'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'description': description,
    };
  }
}
