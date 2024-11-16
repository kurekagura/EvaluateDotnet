# README

## EF Core Power Tools（VS拡張）

DB FirstでC#コードを自動生成。

EF Core Power Toolsは、マイグレーションコードの生成には対応してない？ので、dotnet-efをインストールして対応。

ローカルツールとしてEFのツールを入れる

```cmd
dotnet new tool-manifest

dotnet tool install --local dotnet-ef

dotnet ef --version
```


Microsoft.EntityFrameworkCore.Designをnuget

マイグレーションコードを自動生成

```cmd
dotnet ef migrations add InitialCreate
```

Consoleアプリでは次のようなエラーが出るので、デザインタイムコードを作成

```text
Unable to create a 'DbContext' of type 'RuntimeType'.
The exception 'Unable to resolve service for type 'Microsoft.EntityFrameworkCore.DbContextOptions`1[ConsoleEF.Models.SampleDbContext]'
while attempting to activate 'ConsoleEF.Models.SampleDbContext'.' was thrown
while attempting to create an instance.
For the different patterns supported at design time,
see https://go.microsoft.com/fwlink/?linkid=851728
```

次のコードでテーブルを作成できる（MSSQLデータベースファイル(SampleLocalDB.mdf)の自動作成は出来ない？）

```C#
await db.Database.MigrateAsync();
```
