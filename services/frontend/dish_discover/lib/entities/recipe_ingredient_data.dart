
class RecipeIngredientData {
  final String id; // guid = string
  final String name;
  final String description
  double quantity;
  String unitId
  String unit;

  RecipeIngredientData({
    required this.id,
    required this.name,
    this.description,
    required this.quantity,
    required this.unitId,
    required this.unit
    });

  factory Ingredient.fromJson(Map<String, dynamic> json) {
    return Ingredient(
      id: json['id'],
      name: json['name'],
      description : json['description']
      quantity: json['amount'],
      unitId: json['unitId']
      unit: json['unit'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'description': description,
      'quantity': quantity,
      'unitId': unitId,
      'unit': unit,
    };
  }
}