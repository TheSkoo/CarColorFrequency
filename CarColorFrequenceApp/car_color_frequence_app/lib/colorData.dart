class ColorData {
    int colorDictId = 0;
    String color = '';
    int backgroundColorRGB = 0;
    int foregroundColorRGB = 0;
    int count = 0;

    ColorData({
      required this.colorDictId,
      required this.color,
      required this.backgroundColorRGB,
      required this.foregroundColorRGB,
      required this.count,
    });
    
  String getFormattedCount() {
    return count.toString().replaceAllMapped(
      RegExp(r'(\d{1,3})(?=(\d{3})+(?!\d))'),
      (Match m) => '${m[1]},',
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'colorDictId': colorDictId,
      'color': color,
      'backgroundColorRGB': backgroundColorRGB,
      'foregroundColorRGB': foregroundColorRGB,
      'count': count,
    };
  }

  factory ColorData.fromJson(Map<String, dynamic> json) {
    return ColorData(
      colorDictId: json['colorDictId'],
      color: json['color'],
      backgroundColorRGB: json['backgroundColorRGB'],
      foregroundColorRGB: json['foregroundColorRGB'],
      count: json['count'],
    );
  }}