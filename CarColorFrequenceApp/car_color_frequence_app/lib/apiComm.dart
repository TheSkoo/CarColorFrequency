import 'package:http/http.dart' as http;
import 'dart:convert'; // For JSON decoding
import './colorData.dart';

class Apicomm
{
  // url for local api hosted by IIS
  static final String url = "http://localhost.com:779/api/CarColorFrequency/";

  // url for api hosted by local Azure function
  //static final String url = "http://localhost.com:7194/api/CarColorFrequency/";
 
static Future<List<ColorData>> fetchData() async {
 
  try {
    final getUrl = Uri.parse(url);
    final response = await http.get(getUrl);
    
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

static Future<void> postData(List<ColorData> colorData) async {
  try {
    final commitUrl = Uri.parse(url);
    final response = await http.post(
      commitUrl,
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode(colorData.map((item) => item.toJson()).toList()),
    );

    if (response.statusCode != 200) {
      print('Data posted successfully');
    }
  } catch (e) {
    print('Error fetching data: $e');
  }
}
}