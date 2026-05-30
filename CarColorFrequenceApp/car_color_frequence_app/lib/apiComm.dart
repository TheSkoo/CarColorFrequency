import 'package:http/http.dart' as http;
import 'dart:convert'; // For JSON decoding
import './colorData.dart';

class Apicomm
{
static Future<List<ColorData>> fetchData() async {
  final url = Uri.parse('http://192.168.50.89:779/CarColorFrequency');
  
  try {
    final response = await http.get(url);
    
    if (response.statusCode == 200) {
      final List<dynamic> parsedJson = jsonDecode(response.body);

      List<ColorData> colorData = parsedJson.map((json) => ColorData.fromJson(json)).toList();    
      return colorData; 
    } else {
      print('Request failed with status: ${response.statusCode}');
    }
  } catch (e) {
    print('Error fetching data: $e');
  }
  return [];
}

static Future<void> postData(ColorData colorData) async {
  final url = Uri.parse('http://192.168.50.89:779/CarColorFrequency');
  
  try {
    final response = await http.post(
      url,
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode(colorData.toJson()),
    );

    if (response.statusCode == 200) {
      print('Data posted successfully');
    } else {
      print('Request failed with status: ${response.statusCode}');
    }
  } catch (e) {
    print('Error fetching data: $e');
  }
}
}