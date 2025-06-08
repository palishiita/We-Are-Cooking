import 'dart:convert';

import 'package:dish_discover/entities/app_state.dart';
import 'package:flutter/foundation.dart';
import 'package:http/http.dart' as http;


class IngredientOption {
  final String id;
  final String name;
  final String? description;

  IngredientOption({required this.id, required this.name, this.description});

  factory IngredientOption.fromJson(Map<String, dynamic> json) {
    return IngredientOption(
      id: json['id'] ?? '',
      name: json['name'] ?? '',
      description: json['description'],
    );
  }
}

class UnitOption {
  final String id;
  final String name;

  UnitOption({required this.id, required this.name});

  factory UnitOption.fromJson(Map<String, dynamic> json) {
    return UnitOption(
      id: json['id'] ?? '',
      name: json['name'] ?? '',
    );
  }
}

// Database service methods for ingredients and units
class IngredientService {
  static Future<List<IngredientOption>> getAvailableIngredients({
    String? query,
    int count = 10,
    int page = 0,
    String? sortBy,
    bool orderByAsc = true,
  }) async {
    try {
      
      // Build query parameters
      final Map<String, String> queryParams = {
        'count': count.toString(),
        'page': page.toString(),
      };

      
      if (query != null && query.isNotEmpty) {
        queryParams['query'] = query;
      }
      if (sortBy != null && sortBy.isNotEmpty) {
        queryParams['sortBy'] = sortBy;
      }
      queryParams['orderByAsc'] = orderByAsc.toString();

      Map<String, String> requestHeaders = {
        'Content-type': 'application/json',
        'Accept': 'application/json',
        'X-Uuid': AppState.currentUser?.userId ?? '00000000-0000-0000-0000-000000000000'
      };

      final uri = Uri.http(
        'localhost:7140',
        '/api/ingredients/ingredients',
        queryParams
      );
      
      final response = await http.get(
        uri, // Replace with your actual endpoint
        headers: requestHeaders,
      );

      if (response.statusCode == 200) {
        final Map<String, dynamic> decoded = jsonDecode(response.body);
      
        // Extract the data array from the response
        final List<dynamic> ingredientsList = decoded['data'] as List<dynamic>;
        
        // Convert to IngredientOption objects
        return ingredientsList
            .map((json) => IngredientOption.fromJson(json as Map<String, dynamic>))
            .toList();
      } else {
        throw Exception('Failed to load ingredients: ${response.statusCode}');
      }
    } catch (e) {
      if (kDebugMode) {
        print('Error fetching ingredients: $e');
      }
      // Return sample data as fallback
      return [
        IngredientOption(id: '00000000-0000-0000-0000-000000000001', name: 'Flour'),
        IngredientOption(id: '00000000-0000-0000-0000-000000000002', name: 'Sugar'),
        IngredientOption(id: '00000000-0000-0000-0000-000000000003', name: 'Salt'),
        IngredientOption(id: '00000000-0000-0000-0000-000000000004', name: 'Butter'),
        IngredientOption(id: '00000000-0000-0000-0000-000000000005', name: 'Milk'),
        IngredientOption(id: '00000000-0000-0000-0000-000000000006', name: 'Eggs'),
        IngredientOption(id: '00000000-0000-0000-0000-000000000007', name: 'Vanilla Extract'),
        IngredientOption(id: '00000000-0000-0000-0000-000000000008', name: 'Baking Powder'),
      ];
    }
  }

  static Future<List<UnitOption>> getAvailableUnits({
    String? query,
    int count = 10,
    int page = 0,
    String? sortBy,
    bool orderByAsc = true,
  }) async {
    try {
      final Map<String, String> queryParams = {
        'count': count.toString(),
        'page': page.toString(),
      };

      if (query != null && query.isNotEmpty) {
        queryParams['query'] = query;
      }
      if (sortBy != null && sortBy.isNotEmpty) {
        queryParams['sortBy'] = sortBy;
      }
      queryParams['orderByAsc'] = orderByAsc.toString();

      Map<String, String> requestHeaders = {
        'Content-type': 'application/json',
        'Accept': 'application/json',
        'X-Uuid': AppState.currentUser?.userId ?? '00000000-0000-0000-0000-000000000000'
      };

      final uri = Uri.http(
        'localhost:7140',
        '/api/ingredients/units',
        queryParams
      );

      final response = await http.get(
        uri, // Replace with your actual endpoint
        headers: requestHeaders,
      );

      if (response.statusCode == 200) {
        final Map<String, dynamic> decoded = jsonDecode(response.body);
        
        // Extract the data array from the response
        final List<dynamic> unitsList = decoded['data'] as List<dynamic>;
        
        // Convert to UnitOption objects
        return unitsList
            .map((json) => UnitOption.fromJson(json as Map<String, dynamic>))
            .toList();
      } else {
        throw Exception('Failed to load units: ${response.statusCode}');
      }
    } catch (e) {
      if (kDebugMode) {
        print('Error fetching units: $e');
      }
      // Return sample data as fallback
      return [
        UnitOption(id: '00000000-0000-0000-0000-000000000001', name: 'grams'),
        UnitOption(id: '00000000-0000-0000-0000-000000000002', name: 'cups'),
        UnitOption(id: '00000000-0000-0000-0000-000000000003', name: 'tablespoons'),
        UnitOption(id: '00000000-0000-0000-0000-000000000004', name: 'teaspoons'),
        UnitOption(id: '00000000-0000-0000-0000-000000000005', name: 'liters'),
        UnitOption(id: '00000000-0000-0000-0000-000000000006', name: 'pieces'),
        UnitOption(id: '00000000-0000-0000-0000-000000000007', name: 'ounces'),
        UnitOption(id: '00000000-0000-0000-0000-000000000008', name: 'pounds'),
      ];
    }
  }
}