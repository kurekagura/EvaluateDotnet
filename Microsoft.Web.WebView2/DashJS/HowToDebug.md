# デバッグ方法

## 方法①：Developer Toolを起動

通常のChrome/Edgeを同じようにWPF画面上でF12（またはCtrl+Shift+I）を押すと起動します。

## 方法②：VSの中でデバッグ

ちなみにmyapp.jsを「新しい場合はコピーする」にしている。

「DashJS」のデバッグプロパティに「WebView2デバッグを有効にする」という項目に「Microsoft Edge(Chromium)ベース」のWebView2のJavaScriptデバッガーを有効にします。JavaScript診断コンポーネントが必要です。」にチェックをする。

→個別コンポーネント「JavaScript diagnostics」をインストール。

VSからF5デバッグ実行すると、ソリューションエキスプローラ―にスクリプトドキュメント、appasets/myapp.jsやindex.htmlが現れるので、そこにブレークポイントを設定するとブレークする。ただ、デバッグを終了されるとブレークポイントは忘れてしまうようです。もっと良い方法があるのかもしれません。
