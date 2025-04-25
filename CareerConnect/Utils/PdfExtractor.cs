using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using CareerConnect.Models.DTOs;
using System.Text;

public class PdfExtractor
{
    public string ExtractTextFromPdf(byte[] fileBytes)
    {
        using (var memoryStream = new MemoryStream(fileBytes))
        using (var document = PdfDocument.Open(memoryStream))
        {
            StringBuilder text = new StringBuilder();
            foreach (var page in document.GetPages())
            {
                text.AppendLine(page.Text);
            }
            return text.ToString();
        }
    }
}
