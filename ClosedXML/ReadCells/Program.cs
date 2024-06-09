using ClosedXML.Excel;

namespace ReadCells
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Excelファイルのパスを組み立て
            string xlsxPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MyStockDB.xlsx");

            //// Excelファイルを開く
            //using var wb = new XLWorkbook(xlsxPath);

            //// "銘柄"シートを取得
            //var ws = wb.Worksheet("銘柄");

            //// A列の値を取得。１行目のヘッダはスキップ。
            //var columnA = ws.Column("A").CellsUsed().Skip(1);
            //// A列の値を表示
            //foreach (var cell in columnA)
            //{
            //    Console.WriteLine(cell.Value);
            //}

            //　エクセルを開いている場合でもエラーにならないように読み取り専用で開く
            using (FileStream fs = new FileStream(xlsxPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var workbook = new XLWorkbook(fs))
            {
                var ws = workbook.Worksheet("銘柄");
                var columnA = ws.Column("A").CellsUsed().Skip(1);

                // A列の値を表示
                foreach (var cell in columnA)
                {
                    Console.WriteLine(cell.Value);
                }

            }

        }
    }
}
