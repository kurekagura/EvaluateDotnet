# セットアップと動作確認

まず、ビルドしたカーネルモードドライバのインストールを行います。管理者としてコンソールを実行し、x64ビルド成果物をインストールします。

```console
cd O:\src\CenterDevice\MiniFSWatcher\Debug\x64\MiniFSWatcherPackage
RUNDLL32.EXE SETUPAPI.DLL,InstallHinfSection DefaultInstall 132 ./minifswatcher.inf
echo %errorlevel%
```

Windows(x64)の既定では、テスト署名されたドライバは無効となっているので（[テスト署名されたドライバーの読み込みの有効化](https://learn.microsoft.com/ja-jp/windows-hardware/drivers/install/the-testsigning-boot-configuration-option)
）、これを有効化します。

管理者としてコンソールを実行します。

```console
Bcdedit.exe -set TESTSIGNING ON
```

PCを再起動します。再起動後、デスクトップ右下に「テストモード」で実行中の旨が透かし表示されています。

ユーザモード側の.NETライブラリ（MiniFSWatcher）とそれを呼び出すサンプルアプリ（MiniFSWatcherApp）をビルド・実行します（ともに.NET Framework4.5.2）。監視対象のディレクトリをMiniFSWatcherAppの第一引数に与えて実行します。

```console
FileSystemEventFilter Demo Application
======================================

Listing events in "O:\mytmp\test"
To watch a different path, run: minispyApp.exe <pattern>

Press <ESC> to stop exit...
Moved: O:\mytmp\test\file1.txt -> O:\mytmp\test\dir1\file1.txt
```

以上のようにファイルの移動が１イベントとなっていることを確認できました。

## 後片付け

安全のため確認が終わったらテスト署名のドライバを無効化しておきます。管理者としてコンソールを実行します。

```console
Bcdedit.exe -set TESTSIGNING OFF
```

アンインストール

```console
cd O:\src\CenterDevice\MiniFSWatcher\Debug\x64\MiniFSWatcherPackage
RUNDLL32.EXE SETUPAPI.DLL,InstallHinfSection DefaultUninstall 132 ./minifswatcher.inf
echo %errorlevel%
```
