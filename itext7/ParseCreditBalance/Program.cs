using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Tagutils;

namespace ParseCreditBalance
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var reader = new PdfReader(@"..\..\..\PrivateJunkData\20250608_syumatsu2025053000.pdf");

            var pdfDoc = new PdfDocument(reader);
            bool isTagged = pdfDoc.GetStructTreeRoot() != null;

            Console.WriteLine($"タグ付きPDFか？: {isTagged}");

            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                var page = pdfDoc.GetPage(i);
                var strategy = new LocationTextExtractionStrategy();
                var text = PdfTextExtractor.GetTextFromPage(page, strategy);

                Console.WriteLine($"--- Page {i} ---");
                Console.WriteLine(text);
            }

            pdfDoc.Close();
        }
    }
}
