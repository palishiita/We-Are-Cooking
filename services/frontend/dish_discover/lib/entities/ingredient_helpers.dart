import 'dart:convert';

import 'package:dish_discover/entities/app_state.dart';
import 'package:dish_discover/services/api_client.dart';
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
  static final ApiClient _apiClient = ApiClient();

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

      var response = await _apiClient.get('/api/ingredients/ingredients', queryParameters: queryParams);
      
      if (response.statusCode == 200) {      
        // Extract the data array from the response
        final List<dynamic> ingredientsList = response.data['data'] as List<dynamic>;
        
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
      return [];
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

      var response = await _apiClient.get('/api/ingredients/units', queryParameters: queryParams);
      
      if (response.statusCode == 200) {
        final List<dynamic> unitsList = response.data['data'] as List<dynamic>;        
        // Convert to IngredientOption objects
        return unitsList
            .map((json) => UnitOption.fromJson(json as Map<String, dynamic>))
            .toList();
      } else {
        throw Exception('Failed to load ingredients: ${response.statusCode}');
      }
    } catch (e) {
      if (kDebugMode) {
        print('Error fetching units: $e');
      }
      // Return sample data as fallback
      return [];
    }
  }
}