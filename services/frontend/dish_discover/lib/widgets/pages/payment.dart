import 'package:dish_discover/widgets/display_with_input/paypal_webview.dart';
import 'package:dish_discover/widgets/style/style.dart';
import 'package:flutter/material.dart';

class PaymentPage extends StatelessWidget {
  final bool buyingPremium;
  const PaymentPage({super.key, required this.buyingPremium});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        resizeToAvoidBottomInset: true,
        appBar: AppBar(
            toolbarHeight: appBarHeight,
            scrolledUnderElevation: 0.0,
            title: Text('Buy ${buyingPremium ? 'Premium' : 'recipe boost'}'),
            centerTitle: true,
            leading: const BackButton()),
        //body: const PayPalWebView(),
        bottomNavigationBar: const Padding(
            padding: EdgeInsets.symmetric(horizontal: 20, vertical: 15),
            child: Text('Reminder: DishDiscover does not offer refunds!',
                softWrap: true,
                overflow: TextOverflow.ellipsis,
                textAlign: TextAlign.center,
                style: TextStyle(fontSize: 13))));
  }
}
