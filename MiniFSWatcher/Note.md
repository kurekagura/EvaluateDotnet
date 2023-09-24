# [MiniFSWatcher](https://github.com/CenterDevice/MiniFSWatcher)

[Detecting moved files using FileSystemWatcher](https://stackoverflow.com/questions/1286114/detecting-moved-files-using-filesystemwatcher)

FileSystemWatcherの移動がDeleted->Createdの二つのイベントに分割される課題を解決できるようです。

## 特徴（ほぼ翻訳のサマリ）

MSのminispyドライバのサンプルをベースにしている。

https://github.com/microsoft/Windows-driver-samples/tree/main/filesys/miniFilter/minispy

※ [ビルド環境](./build20230923.md)でビルド成功を確認。

.NETのFileSystemWatcherは「移動」の概念がなく「作成」と「削除」イベントを生成するが、MiniFSWatcherでは、1つのパーティション内でファイルの移動を追跡できる。

変更イベント量の削減。AggregateEventsオプションを使用すると、各ファイルハンドルが閉じられたときに1つのファイルイベントしか受信されないため、連続した変更イベントは発生しない。複数の書き込み操作が実行された場合「変更」イベントは1つだけトリガーされる。ファイルが作成されて変更された場合(コピー操作が原因の場合)、「作成」イベントは1つだけ受信される。

変更の原因となったプロセス情報の取得。例えば、特定のアプリケーションによって実行された変更を無視するために、誰が変更を引き起こしたのかを知ることができる。すべてのイベントの原因となったプロセスIDを知ることができ、さらに自身のプロセスIDによって引き起こされたすべてのイベントを直接フィルタリングできる。

## 動作確認

[Installing the kernel mode driver](https://github.com/CenterDevice/MiniFSWatcher#installing-the-kernel-mode-driver)にあるとおり、カーネルドライバのインストールが必要です。

[こちら](./setup.md)を参照してください。
