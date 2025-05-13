import csv
from uuid import UUID

# Example data (you’d replace this with your actual UUIDs + names)
ingredients = [
    {"id": "f2bea0e2-73c0-4d58-89e4-2d61f59424f5", "name": "Tomato", "units": ["pcs", "kg", "g"]},
    {"id": "c0ac9074-6434-422f-87b6-6cb85e853cb4", "name": "Garlic", "units": ["kg", "g", "pcs", "clove"]},
    {"id": "ab70cc29-3c38-4990-aacb-da019ba11dd1", "name": "Olive Oil", "units": ["l", "ml", "tsp", "tbsp", "cup"]},
    {"id": "2aa17ede-a10e-407f-8feb-41d0cc54dc7a", "name": "Chicken Breast", "units": ["kg", "g", "pcs", "slice"]},
    {"id": "1bb34aa2-e426-46e9-a518-d44fb6b419e5", "name": "Basil", "units": ["kg", "g", "pcs"]},
    {"id": "5c7fc23a-188d-4a53-9325-21ccef62d808", "name": "Onion", "units": ["kg", "g", "pcs"]},
    {"id": "2aa17ede-a10e-407f-8feb-41d0cc54dc7a", "name": "Carrot", "units": ["kg", "g", "pcs"]},
    {"id": "b5b94f34-17ee-4324-b5aa-377619ec67fc", "name": "Potato", "units": ["kg", "g", "pcs"]},
    {"id": "8078633e-f337-40ad-828f-f545c3fb9503", "name": "Salt", "units": ["kg", "g", "pcs", "dash","pinch", "tsp", "tbsp"]},
    {"id": "b463be3b-0e4f-4256-8702-6af84474bb96", "name": "Black Pepper", "units": ["kg", "g", "pcs", "dash","pinch", "tsp", "tbsp"]},
    {"id": "8cf37f89-0b9b-409f-9c1c-e0db03666966", "name": "Egg", "units": ["kg", "g", "pcs"]},
    {"id": "83d8df76-37b0-43f9-9f3b-dde96f2ab8c8","name": "Milk", "units": ["l", "ml", "tsp", "tbsp", "cup"]},
    {"id": "7698a6b3-c0a2-48aa-af8c-be363a26023e","name": "Cheddar Cheese", "units": ["kg", "g", "slice"]},
    {"id": "f383e75c-cee2-4445-88de-922311c86037","name": "Butter", "units": ["kg", "g", "pcs"]},
    {"id": "5d6b881a-bcff-4e92-84bc-935578fdb8d9","name": "Flour", "units": ["kg", "g", "pcs", "dash","pinch", "tsp", "tbsp"]},
    {"id": "709364bc-514e-4559-a8ec-ea71468dc891","name": "Sugar", "units": ["kg", "g", "pcs", "dash","pinch", "tsp", "tbsp"]},
    {"id": "ec91d02e-e3f6-485e-b833-736251b98460","name": "Rice", "units": ["kg", "g"]},
    {"id": "71f878bb-a083-41d8-b81c-8b3b2abf6e9d","name": "Pasta", "units": ["kg", "g"]},
    {"id": "0540f760-856d-455b-b8cc-ea7959723c19","name": "Ground Beef", "units": ["kg", "g"]},
    {"id": "f1fe8e4c-494e-42c1-9db0-7ad42a28bfee","name": "Salmon", "units": ["kg", "g", "pcs", "slice"]},
    {"id": "60c5638c-28fa-4b5e-976a-11c6de1ab517","name": "Spinach", "units": ["kg", "g"]},
    {"id": "3f3b0b0f-fcec-4653-a5c7-a7763bae7ec1","name": "Mushroom", "units": ["kg", "g", "pcs", "slice"]},
    {"id": "52b53d42-3782-4a61-972f-046752d553cf","name": "Bell Pepper", "units": ["kg", "g", "pcs", "slice"]},
    {"id": "afb15d83-77bb-42bc-99dd-bc35ee2fd896","name": "Avocado", "units": ["kg", "g", "pcs", "slice"]},
    {"id": "9749559b-1b4b-483c-897b-96bd1aaa26ec","name": "Yogurt", "units": ["kg", "g"]},
    {"id": "4220b200-ef4c-485a-8c07-12d57bdf1688","name": "Quinoa", "units": ["kg", "g", "pcs", "slice"]},
    {"id": "8d86aff1-8f8d-4c9b-8cb4-306b942e283b","name": "Chickpeas", "units": ["kg", "g"]},
    {"id": "5af9acd5-5f52-46f2-b717-01f76a0be253","name": "Coconut Milk", "units": ["l", "ml", "cup"]},
    {"id": "4fa170f1-a8b7-4cd3-9e5c-dc513c1fc25f","name": "Zucchini", "units": ["kg", "g", "pcs", "slice"]},
    {"id": "e0936c15-f17a-41f2-aacb-b91c81107046","name": "Mozzarella Cheese", "units": ["kg", "g", "pcs", "slice"]},
    {"id": "87dccb5b-5262-4b27-b292-0f3bc0ea915a","name": "Cilantro", "units": ["kg", "g"]},
    {"id": "79d351be-2b19-4c35-8a93-0a426832bc23","name": "Lime", "units": ["kg", "g", "pcs", "slice"]},
    {"id": "7b9a0da2-f5f9-45d1-9fc4-8e26ed064936", "name": "Beef Steak", "units": ["kg", "g", "pcs", "slice"]},
    {"id": "d9f3d1c2-309c-417f-9e0f-1d39d62ea314", "name": "Pork Shoulder", "units": ["kg", "g", "pcs", "slice"]},
    {"id": "17015c7b-e74d-4774-afe9-0dd5cb837bed", "name": "Almonds", "units": ["kg", "g"]},
    {"id": "6162fa94-f602-4376-9659-f87433bca77e", "name": "Honey", "units": ["kg", "g", "tsp", "tbsp"]},
    {"id":"0d105985-1449-480f-8a59-a28b1a842f3c","name":"Shrimp", "units": ["kg", "g", "pcs"]},
    {"id":"c88c52ba-2534-40e4-8d49-3b0860df4649","name":"Lentils", "units": ["kg", "g"]},
    {"id":"678754a1-100d-4cef-ace6-2d66f80fe67e","name":"Tofu", "units": ["kg", "g", "pcs", "slice"]},
    {"id":"5c92d401-9ee4-4940-9b73-86fa01228d4a","name":"Arugula", "units": ["kg", "g"]},
    {"id":"4c102109-1ee9-43c5-9e88-bacf0e668d0d","name":"Parmesan Cheese", "units": ["kg", "g"]},
    {"id":"0f6e6b4f-61e7-400f-b8d4-693e399d2b1e","name":"Sweet Potato", "units": ["kg", "g", "pcs", "slice"]},
    {"id":"b224635c-e836-45a3-8d9a-150bcc18b1b7","name":"Red Onion", "units": ["kg", "g", "pcs", "slice"]},
    {"id":"e2e91854-f8ca-427f-9b32-e18fd2a48498","name":"Cucumber", "units": ["kg", "g", "pcs"]},
    {"id":"626c897a-34f6-412e-8f78-9474d056768a","name":"Bell Pepper (Yellow)", "units": ["kg", "g", "pcs", "slice"]},
    {"id":"f2ccb5c7-cc9a-4776-ba35-df2c9bf7da19","name":"Feta Cheese", "units": ["kg", "g", "pcs", "slice"]},
    {"id":"cb232181-c09f-4dc0-90fe-246ec3a8dad3","name":"Pine Nuts", "units": ["kg", "g"]},
    {"id":"1b3391c1-2f8b-49f8-a0d1-f876f1ed15f5","name":"Ginger", "units": ["kg", "g", "slice"]},
    {"id":"9bd72cef-87e4-4108-a047-e3df0529eee9","name":"Soy Sauce", "units": ["l", "ml", "cup", "tsp", "tbsp"]},
    {"id":"008340ad-18b2-4ed8-9eab-da78f798910d","name":"Jalapeno", "units": ["kg", "g"]},
    {"id":"ca0aa585-83cd-496f-b04b-5578e131f293","name":"Mango", "units": ["kg", "g", "pcs", "slice"]},
    {"id":"c261d725-4f92-4bb5-a150-0da24d7a520b","name":"Panko Breadcrumbs", "units": ["kg", "g"]},
    {"id":"a1fc11e8-6c74-4e1a-95a6-654f4c96bcf8","name":"Ricotta Cheese", "units": ["kg", "g"]},
    {"id":"58564747-90f7-4053-bf00-599fe78dfa96","name":"Artichoke Hearts", "units": ["kg", "g"]},
    {"id":"9266dcfa-f2c0-4ea5-a6d4-303297476cc4","name":"Sun-Dried Tomatoes", "units": ["kg", "g"]},
    {"id":"8da402f8-94e1-4387-9468-c6b7cfcbf460","name":"Blueberries", "units": ["kg", "g"]},
    {"id":"c4e88c03-7fef-4d55-b9b9-3a3a79d1ba83","name":"Hazelnuts", "units": ["kg", "g"]},
    {"id":"31d3c9c1-2bca-4db6-8e6e-68a2e079938d","name":"Prosciutto", "units": ["kg", "g"]},
    {"id":"414c6e0a-6b4b-4a47-9424-2589544c0985","name":"Kale", "units": ["kg", "g"]},
    {"id":"397a895b-3fe7-40a7-847c-ff5cebbceb4c","name":"Sriracha Sauce", "units": ["l", "ml", "cup", "tsp", "tbsp"]},
    {"id":"7bd2d809-c73e-4b12-ad13-398b5a0235f8","name":"Tahini", "units": ["kg", "g"]},
    {"id":"21b74ccf-3fa4-4de2-959e-5bed5f5469de","name":"Edamame", "units": ["kg", "g"]},
    {"id":"69a2dbee-7de7-4514-9f3a-4012fea5d5a3","name":"Seaweed Sheets", "units": ["pcs"]},
    {"id":"7ebd8830-e505-4ed4-8fff-0a35d306e377","name":"Butternut Squash", "units": ["kg", "g"]},
    {"id":"42b87fca-1db3-4666-8eff-f622cd40e0f7","name":"Farro", "units": ["kg", "g"]},
    {"id":"d0156778-ad53-4edf-8292-2e114234573b","name":"Halloumi Cheese", "units": ["kg", "g", "pcs", "slice"]},
    {"id":"18d709a8-a642-4ddf-9734-5b22c02cfa3e","name":"Kimchi", "units": ["kg", "g"]},
    {"id":"ee6611b2-a656-4c16-b938-ccede9148216","name":"Gochujang", "units": ["kg", "g"]},
    {"id":"70da1e02-29df-4e19-83ca-e0e29d855544","name":"Black Garlic", "units": ["kg", "g", "pcs", "slice"]},
    {"id":"166b6140-d29b-441a-80f6-c13ce8e846ed","name":"Burrata Cheese", "units": ["kg", "g", "pcs", "slice"]},
    {"id":"f5d56393-a959-49db-a83f-82bb293e3f86","name":"Pea Shoots", "units": ["kg", "g", "pcs", "slice"]},
    {"id":"4f7d0cf6-bf55-4514-9db0-b1f32e09f9ea","name":"Chia Seeds", "units": ["kg", "g", "pcs", "slice"]},
    {"id":"cabab904-8ec4-4d1b-b701-99093a9c8220","name":"Acai", "units": ["kg", "g", "pcs", "slice"]},
    {"id":"a9ce1c00-1b78-4246-82fe-f11c4e17252d","name":"Tempeh", "units": ["kg", "g", "pcs", "slice"]},
    {"id":"ec583eb9-56c3-422a-b34a-fbca1a285766","name":"Pecorino Romano", "units": ["kg", "g", "pcs", "slice"]},
    {"id":"5509cd51-1d9d-4dd4-9c7d-c1c943ca903b","name":"Harissa", "units": ["kg", "g", "pcs", "slice"]},
    {"id":"5e768450-4ec5-41c9-9096-81edd2c010c3","name":"Sumac", "units": ["kg", "g", "pcs", "slice"]},
    {"id":"5c3da6a1-fffd-499f-9a1e-37a1b0706fd4","name":"Miso Paste", "units": ["kg", "g", "pcs", "slice"]},
    {"id":"fb261397-84fc-4182-889b-96de72a6bd80","name":"Soba Noodles", "units": ["kg", "g", "pcs", "slice"]},
    {"id":"88530444-cbec-440d-8f06-bdc74ad46a3f","name":"Nori Sheets", "units": ["kg", "g", "pcs", "slice"]},
    {"id":"e4430f19-f09d-48c6-a096-1a363146de40","name":"Cardamom", "units": ["kg", "g", "pcs", "slice"]},
    {"id":"8ec89983-aa4d-4ef6-83a8-27d7db93eb7c","name":"Lemongrass", "units": ["kg", "g", "pcs", "slice"]},
    {"id":"ec1cbb0d-0975-43de-9aad-33debf9138f9","name":"Matcha Powder", "units": ["kg", "g", "pcs", "slice"]}
]

unit_ids = {
    "kg": "fbf200dc-7e74-4d7f-a44a-c341d9b00d5b",
    "g": "88eab83f-6a40-4470-a1bc-2d8d460bb105",
    "pcs": "b338ace5-0f97-4c56-8ac4-615241ae6885",
    "slice": "59bf857c-5a20-4b29-976b-f622822d733a",
    "ml": "e7e4bc58-caec-4a62-b2c7-3fbc4c9b11cc",
    "l": "70b712c6-9a2e-414f-ae1d-bcf7700364f4",
    "tsp": "85893ecf-fa96-4f5c-a6a3-d70371d83269",
    "tbsp": "cc07c68b-c9a0-4de2-b0f0-b0ca4e8994ec",
    "cup": "76c5ef78-b4c1-4048-bc7d-8d3c2a43154b",
    "clove": "a85e6a9e-8c99-410d-a1bc-ee1f6a4b13fe",
    "stick": "4900d77f-afc8-4198-95be-1d5e994ba562",
    "pinch" : "084b380c-7637-4e31-910a-1eefe1d47b8a",
    "dash" : "b04f3df1-b071-412f-bae6-57edc70e1cbc",
    "can" : "d9cddb00-3f69-4212-ab02-5fde0f3b042f",
    "jar" : "7e30798b-2b0a-4e16-a0de-c27424395b1b",
    "drop" : "2a6d7ed1-7fbd-479b-acb2-b4c993c2d3e8",
    "packet" : "1c62b185-6686-42e1-8647-3f3746781f2b"
}

unit_conversion_map = {
    ("kg", "g"): 1000,
    ("g", "kg"): 0.001,
    ("l", "ml"): 1000,
    ("ml", "l"): 0.001,
    ("tsp", "ml"): 5,
    ("ml", "tsp"): 1/5,
    ("tbsp", "ml"): 15,
    ("ml", "tbsp"): 1/15,
    ("cup", "ml"): 240,
    ("ml", "cup"): 1/240,
    ("tbsp", "tsp"): 3,
    ("tsp", "tbsp"): 1/3,
    ("cup", "tbsp"): 16,
    ("tbsp", "cup"): 1/16,
    ("pcs", "slice"): 8,
    ("slice", "pcs"): 1/8,
    ("pcs", "g"): 200,
    ("g", "pcs"): 1/200,
    ("slice", "g"): 25,
    ("g", "slice"): 1/25,
    ("stick", "g") : 10,
    ("g", "stick") : 0.1,
    ("jar", "g") : 400,
    ("g", "jar") : 0.0025,
    ("can", "g") : 400,
    ("g", "can") : 0.0025,
    ("pinch", "ml") : 5/16,
    ("ml", "pinch") : 3.2,
    ("pinch", "tsp") : 1/16,
    ("tsp", "pinch") : 16,
    ("dash", "ml") : 5/8,
    ("ml", "dash") : 1.6,
    ("dash", "tsp") : 0.125,
    ("tsp", "dash") : 8,
    ("clove" : "g") : 5,
    ("g" : "clove") : 0.2,
    ("pcs":"clove") : 10,
    ("clove":"pcs") : 0.1
}

rows = []

for ingredient in ingredients:
    ingredient_id = ingredient["id"]
    applicable_units = ingredient["units"]

    for unit1 in applicable_units:
        for unit2 in applicable_units:
            if unit1 == unit2:
                continue
            if (unit1, unit2) in unit_conversion_map:
                ratio = unit_conversion_map[(unit1, unit2)]
                rows.append([
                    ingredient_id,
                    unit_ids[unit1],
                    unit_ids[unit2],
                    ratio
                ])

# Write to CSV
with open("ingredient_unit_ratios.csv", "w", newline="") as f:
    writer = csv.writer(f)
    writer.writerow(["ingredient_id", "unit_one_id", "unit_two_id", "ratio"])
    writer.writerows(rows)