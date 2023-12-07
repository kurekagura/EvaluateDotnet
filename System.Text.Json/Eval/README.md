# System.Text.Json

json1.jsonはキー(Key:値)が重複しているように見えるが、配列内のためJSON構文として不正はない。
但し、この重複をどのように扱うかはデシリアライザによる）。

例えば、上位を優先するか、下位を優先するか、など。System.Text.Jsonに`List<KeyValuePair<string, JsonElement>`にデシリアライズしたところ、意図した通り複数アイテムとしてきちんとデシリアライズされた。

但し、Valueの型が異なる為、JsonElementをさらに個別の型へデシリアライズする必要がある。一発で行いたい場合、おそらくConverterを書く必要がある。

```json
[
    {
        "Key": "MyPluginA",
        "Value": {
            "name_SelectedFigureItem": "a1",
            "name_MyInteger": "1",
            "name_MyDouble": "1.1"
        }
    },
    {
        "Key": "MyPluginB",
        "Value": {
            "name_MyCheckbox": "true"
        }
    },
    {
        "Key": "MyPluginA",
        "Value": {
            "name_SelectedFigureItem": "a2",
            "name_MyInteger": "2",
            "name_MyDouble": "2.2"
        }
    }
]
```
