import 'package:dio/dio.dart';
import 'package:cookie_jar/cookie_jar.dart';
import 'package:dio_cookie_manager/dio_cookie_manager.dart';
import 'package:flutter/foundation.dart';

class ApiRecommender {
  late Dio _dio;
  static const String baseUrl = 'http://localhost:8069/';
  ApiRecommender() {
    _dio = Dio(
      BaseOptions(
        baseUrl: baseUrl,
        connectTimeout: const Duration(milliseconds: 5000),
        receiveTimeout: const Duration(milliseconds: 10000),
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json',
        },
        extra: {
          'withCredentials': true,
        },
      ),
    );

    if (!kIsWeb) {
      final cookieJar = CookieJar();
      _dio.interceptors.add(CookieManager(cookieJar));
    } else {
      _dio.interceptors.add(InterceptorsWrapper(
        onRequest: (options, handler) {
          options.extra['withCredentials'] = true;
          handler.next(options);
        },
      ));
    }

    _dio.interceptors
        .add(LogInterceptor(requestBody: true, responseBody: true));
  }

  Future<Response<T>> get<T>(String path,
      {Map<String, dynamic>? queryParameters}) async {
    try {
      return await _dio.get<T>(path, queryParameters: queryParameters);
    } on DioException catch (e) {
      print('DioError on GET $path: $e');
      rethrow;
    } catch (e) {
      print('Unexpected error on GET $path: $e');
      rethrow;
    }
  }

  Future<Response<T>> post<T>(String path,
      {dynamic data, Map<String, dynamic>? queryParameters}) async {
    try {
      return await _dio.post<T>(path,
          data: data, queryParameters: queryParameters);
    } on DioException catch (e) {
      print('DioError on POST $path: $e');
      rethrow;
    } catch (e) {
      print('Unexpected error on POST $path: $e');
      rethrow;
    }
  }

  Future<Response<T>> put<T>(String path,
      {dynamic data, Map<String, dynamic>? queryParameters}) async {
    try {
      return await _dio.put<T>(path,
          data: data, queryParameters: queryParameters);
    } on DioException catch (e) {
      print('DioError on PUT $path: $e');
      rethrow;
    } catch (e) {
      print('Unexpected error on PUT $path: $e');
      rethrow;
    }
  }

  Future<Response<T>> delete<T>(String path,
      {dynamic data, Map<String, dynamic>? queryParameters}) async {
    try {
      return await _dio.delete<T>(path,
          data: data, queryParameters: queryParameters);
    } on DioException catch (e) {
      print('DioError on DELETE $path: $e');
      rethrow;
    } catch (e) {
      print('Unexpected error on DELETE $path: $e');
      rethrow;
    }
  }

  Future<Response<T>> uploadMultipart<T>(String path,
      {required FormData formData}) async {
    try {
      return await _dio.post<T>(path, data: formData);
    } on DioException catch (e) {
      print('DioError on UPLOAD $path: $e');
      rethrow;
    } catch (e) {
      print('Unexpected error on UPLOAD $path: $e');
      rethrow;
    }
  }
}