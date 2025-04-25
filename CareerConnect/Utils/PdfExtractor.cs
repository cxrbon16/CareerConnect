using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using System.Text;
using System.Text.RegularExpressions;

public class PdfExtractor
{
    public async Task<string> PdfToTextAsync(byte[] fileBytes)
    {
        if (fileBytes == null)
            throw new ArgumentNullException(nameof(fileBytes));

        using var memoryStream = new MemoryStream(fileBytes);
        using var document = PdfDocument.Open(memoryStream);

        var stringBuilder = new StringBuilder();

        foreach (var page in document.GetPages())
        {
            var lines = page.GetWords()
                            .GroupBy(w => (int)w.BoundingBox.Bottom) // Y ekseninde hizalanmış kelimeleri grupla
                            .OrderByDescending(g => g.Key);           // Yükseklik sırasına göre (üstten aşağıya)

            foreach (var line in lines)
            {
                var sortedWords = line.OrderBy(w => w.BoundingBox.Left); // soldan sağa sıralama
                foreach (var word in sortedWords)
                {
                    stringBuilder.Append(word.Text + " ");
                }
                stringBuilder.AppendLine();
            }

            stringBuilder.AppendLine(); // Sayfa sonu boş satır
        }

        var fullText = stringBuilder.ToString();

        // Çoklu boşluk ve boş satırları düzelt
        fullText = Regex.Replace(fullText, @"[ \t]+", " "); // Fazla boşlukları tek boşluk yap
        fullText = Regex.Replace(fullText, @"[\r\n]{2,}", "\n\n"); // 2'den fazla boş satırı tek boş satıra indir

        Console.WriteLine(fullText);

        return await Task.FromResult(fullText.Trim());
    }
}
