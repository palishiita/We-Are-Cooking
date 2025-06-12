//import 'package:dish_discover/widgets/display/tab_title.dart';
//import 'package:dish_discover/widgets/inputs/custom_text_field.dart';
//import 'package:dish_discover/widgets/inputs/popup_menu.dart';
//import 'package:dish_discover/widgets/style/style.dart';
//import 'package:flutter/material.dart';
//import 'package:flutter_markdown/flutter_markdown.dart';
//import 'package:flutter_riverpod/flutter_riverpod.dart';
//
//import '../../entities/new_recipe.dart';

//class StepsBox extends ConsumerStatefulWidget {
//  final ChangeNotifierProvider<Recipe> recipeProvider;
//  final bool forEditing;
//
//  const StepsBox(
//      {super.key, required this.recipeProvider, this.forEditing = false});
//
//  @override
//  ConsumerState<ConsumerStatefulWidget> createState() => _StepsBoxState();
//}
//
//class _StepsBoxState extends ConsumerState<StepsBox> {
//  late TextEditingController stepsController;
//
//  @override
//  void initState() {
//    super.initState();
//    stepsController =
//        TextEditingController(text: ref.read(widget.recipeProvider).steps);
//  }
//
//  @override
//  Widget build(BuildContext context) {
//    Recipe recipe = ref.watch(widget.recipeProvider);
//
//    return Padding(
//        padding: const EdgeInsets.symmetric(vertical: 5.0, horizontal: 20.0),
//        child: Card(
//            child: Padding(
//                padding: const EdgeInsets.symmetric(
//                    vertical: 20.0, horizontal: 10.0),
//                child: Flex(
//                    direction: Axis.vertical,
//                    mainAxisSize: MainAxisSize.min,
//                    crossAxisAlignment: CrossAxisAlignment.start,
//                    children: [
//                      const Padding(
//                          padding: EdgeInsets.only(bottom: 5.0),
//                          child: TabTitle(title: "Steps")),
//                      Card(
//                          color: textEditorColor,
//                          shadowColor: Colors.transparent,
//                          child: widget.forEditing
//                              ? CustomTextField(
//                                  controller:
//                                      stepsController, // TODO pass edited text back
//                                  hintText: 'Markdown text',
//                                  maxLength: 3000,
//                                  onChanged: (value) => recipe.editRecipe(
//                                      steps: stepsController.text))
//                              : Markdown(
//                                  physics: const NeverScrollableScrollPhysics(),
//                                  data: recipe.steps,
//                                  onTapLink: (alias, url, _) =>
//                                      PopupMenuAction.shareAction(
//                                          context,
//                                          alias,
//                                          'Check this out: ',
//                                          url ?? 'null'),
//                                  selectable: true,
//                                  shrinkWrap: true,
//                                  softLineBreak: true))
//                    ]))));
//  }
//}
//