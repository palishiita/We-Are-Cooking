class Unit {
  final String id;
  final String name;
  
  Unit(
    {required this.id,
      required this.name})

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name
    };
  }

  factory Unit.fromJson(Map<String, dynamic> json) {
    return Unit(
      id: json['id'],
      name: json['name'],
    );
}}