import 'package:dish_discover/widgets/inputs/custom_text_field.dart';
import 'package:flutter/material.dart';

class TagInputBox extends StatefulWidget {
  final void Function(String) onAdd;

  const TagInputBox({
    super.key,
    required this.onAdd,
  });

  @override
  State<StatefulWidget> createState() => _TagInputBoxState();
}

class _TagInputBoxState extends State<TagInputBox> {
  late TextEditingController inputController;
  @override
  void initState() {
    super.initState();
    inputController = TextEditingController();
  }

  @override
  Widget build(BuildContext context) {
    return CustomTextField(
        controller: inputController,
        hintText: 'Tag',
        trailingAction: IconButton(
            icon: const Icon(Icons.add),
            onPressed: () {
              if (inputController.text.isNotEmpty) {
                widget.onAdd(inputController.text);
              }
              inputController.text = '';
            }));
  }
}
