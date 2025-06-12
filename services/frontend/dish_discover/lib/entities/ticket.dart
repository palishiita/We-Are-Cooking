//import 'dart:convert';
//
//import 'package:flutter/foundation.dart';
//import 'package:flutter/material.dart';
//import 'package:http/http.dart' as http;
//
//import 'app_state.dart';
//// not implemented
//class Ticket extends ChangeNotifier {
//  final int reportId;
//  int? recipeId;
//  String? violatorId;
//  final String issuerId;
//  Image? issuerAvatar;
//  int? commentId;
//  final String reason;
//  bool accepted;
//
//  Ticket(
//      {required this.reportId,
//      this.recipeId,
//      this.violatorId,
//      required this.issuerId,
//      this.issuerAvatar,
//      this.commentId,
//      required this.reason,
//      this.accepted = false}) {
//    assert(recipeId != null || violatorId != null || commentId != null);
//  }
//
//  factory Ticket.fromJson(Map<String, dynamic> json) {
//    return Ticket(
//      reportId: json['reportId'],
//      recipeId: json['recipeId'],
//      violatorId: json['violatorId'],
//      issuerId: json['issuerId'],
//      commentId: json['commentId'],
//      reason: json['reason'],
//      accepted: json['accepted'],
//    );
//  }
//
//  Map<String, dynamic> toJson() {
//    return {
//      'reportId': reportId,
//      'recipeId': recipeId,
//      'violatorId': violatorId,
//      'issuerId': issuerId,
//      'commentId': commentId,
//      'reason': reason,
//      'accepted': accepted,
//    };
//  }
//
//  Map<String, dynamic>? getIdentifierAndViolator() {
//    if (commentId != null) {
//      return {'identifier': commentId, 'violatorId': violatorId};
//    } else if (recipeId != null) {
//      return {'identifier': recipeId, 'violatorId': violatorId};
//    } else {
//      return null;
//    }
//  }
//
//  static Future<Ticket?> getAssignedTicket() async {
//    // TODO get active or get next one from queue
//    return null;
//  }
//
//  static Future<void> addTicket(Ticket ticket) async {
//    final response = await http.post(
//      Uri.parse('http://${AppState.serverDomain}/api/tickets'),
//      headers: {'Content-Type': 'application/json'},
//      body: jsonEncode(ticket.toJson()),
//    );
//
//    if (response.statusCode == 201) {
//      if (kDebugMode) {
//        print('Ticket added successfully');
//      }
//    } else {
//      throw Exception(
//          'Failed to add ticket, status code: ${response.statusCode}');
//    }
//  }
//
//  static Future<List<Ticket>> getTickets() async {
//    final response = await http
//        .get(Uri.parse('http://${AppState.serverDomain}/api/tickets'));
//
//    if (response.statusCode == 200) {
//      final List data = json.decode(response.body)['tickets'];
//      return data.map((item) => Ticket.fromJson(item)).toList();
//    } else {
//      throw Exception(
//          'Failed to load tickets, status code: ${response.statusCode}');
//    }
//  }
//}
//