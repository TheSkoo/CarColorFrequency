import 'dart:ui';

import 'package:flutter/material.dart';
import 'package:complete_timer/complete_timer.dart';
import './apiComm.dart';
import './colorData.dart';

void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  // This widget is the root of your application.
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Flutter Demo',
      theme: ThemeData(
        // This is the theme of your application.
        //
        // TRY THIS: Try running your application with "flutter run". You'll see
        // the application has a purple toolbar. Then, without quitting the app,
        // try changing the seedColor in the colorScheme below to Colors.green
        // and then invoke "hot reload" (save your changes or press the "hot
        // reload" button in a Flutter-supported IDE, or press "r" if you used
        // the command line to start the app).
        //
        // Notice that the counter didn't reset back to zero; the application
        // state is not lost during the reload. To reset the state, use hot
        // restart instead.
        //
        // This works for code too, not just values: Most code changes can be
        // tested with just a hot reload.
        colorScheme: .fromSeed(seedColor: Colors.deepPurple),
      ),
      home: const MyHomePage(title: 'Flutter Demo Home Page'),
    );
  }
}

class MyHomePage extends StatefulWidget {
  const MyHomePage({super.key, required this.title});

  // This widget is the home page of your application. It is stateful, meaning
  // that it has a State object (defined below) that contains fields that affect
  // how it looks.

  // This class is the configuration for the state. It holds the values (in this
  // case the title) provided by the parent (in this case the App widget) and
  // used by the build method of the State. Fields in a Widget subclass are
  // always marked "final".

  final String title;

  @override
  State<MyHomePage> createState() => _MyHomePageState();
}

class _MyHomePageState extends State<MyHomePage> {
  late final AppLifecycleListener _listener;
  
    // list of car colors to track, will be populated from API call
  List<ColorData> _colorData = [];

  // The following two variables are for bundling updates top the server to avoid making a web api call for every button click, 
  // which would be inefficient and could cause performance issues. 
  // Instead, we will track how many button clicks have been made that have not yet been committed to the server, 
  // and once that count reaches a certain threshold, we will make a web api call to 
  // update the server with the new counts for all colors at once, and reset the pending count back to 0. 
  // This way we can reduce the number of web api calls and improve performance while still keeping the server 
  // updated with the latest counts in a reasonable timeframe.
  int _pendingCount = 0;

  // threshold of uncommitted button clicks before triggering a web api call to update the server with the new counts
  static const int _commitThreshold = 50;

  CompleteTimer get _staleDataTimer => CompleteTimer(
    duration: Duration(seconds: 30),
    periodic: false, // Set to true to make the timer repeat periodically
    autoStart: false, // Set to true to start the timer immediately
    // The callback function is invoked after the given duration.
    callback: (timer) {
      if (_pendingCount > 0) {
        _staleDataTimer.cancel(); // Cancel the timer to free up resources
        _commitData();
      }
    },
  );

  @override
  void initState() {
    super.initState();
    // Initialize the listener with your callback handlers
    _listener = AppLifecycleListener(
      onExitRequested: _handleExitRequest,
    );
    Apicomm.fetchData().then((value) {
        setState(() {
          _colorData = value;
        });
  });
  }

  Future<AppExitResponse> _handleExitRequest() async {
    _commitData();
    return AppExitResponse.exit; // Exit the app with a success code
  }

  @override
  void dispose() {
    // Clean up the listener to prevent memory leaks
    _listener.dispose();
    super.dispose();
  }

  void _commitData() {
    Apicomm.postData(_colorData);
    _pendingCount = 0;
  }

  void _incrementCounter (int colorDictId) {
    var colorDataItem = _colorData.firstWhere((item) => item.colorDictId == colorDictId);
    colorDataItem.count++;
    _pendingCount++;
  
    if (_pendingCount >= _commitThreshold) {  
      _commitData();
    } else {
      if (!_staleDataTimer.isRunning) {
        _staleDataTimer.start();
      }
    }
    setState(() {
      // This call to setState tells the Flutter framework that something has
      // changed in this State, which causes it to rerun the build method below
      // so that the display can reflect the updated values. If we changed
      // _counter without calling setState(), then the build method would not be
      // called again, and so nothing would appear to happen.
    });
  }

  var _displayCounts = true;
  var _totalCounts = 0;
  void _toggleDisplayMode() {
      _displayCounts = !_displayCounts;
    setState(() {
    });
  }

  @override
  Widget build(BuildContext context) {
      _totalCounts = _colorData.fold(0, (sum, item) => sum + item.count);

    // This method is rerun every time setState is called, for instance as done
    // by the _incrementCounter method above.
    //
    // The Flutter framework has been optimized to make rerunning build methods
    // fast, so that you can just rebuild anything that needs updating rather
    // than having to individually change instances of widgets.
    return Container (
      padding: const EdgeInsets.all(16.0),
      color: Colors.white70,
      child: Wrap(
        spacing: 8.0, // Space between buttons
        runSpacing: 8.0, // gap between lines
        children: <Widget> [
          ..._colorData.map((label) {
            return ElevatedButton(
              onPressed: () => _incrementCounter(label.colorDictId),
              style: ElevatedButton.styleFrom(
                backgroundColor: Color(label.backgroundColorRGB + 4278190080), // convert to ARGB by adding alpha value
                foregroundColor: Color(label.foregroundColorRGB + 4278190080),
              ),
              child: Text(_displayCounts ? label.getFormattedCount() : "${(label.count / _totalCounts * 100).toStringAsFixed(1)}%"),
            );
          }),
          ElevatedButton(
            onPressed: () => _toggleDisplayMode(),
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.black,
              foregroundColor: Colors.white,
            ),
            child: Text("Toggle Display Mode"),
          ),
        ]
        )
    );
  }
}
