using DocumentFormat.OpenXml.Packaging;
using System.Text;

namespace BriefQuest.Services;

public static class DocumentTextExtractor
{
    public static string ExtractDocx(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        using var doc = WordprocessingDocument.Open(stream, false);
        var body = doc.MainDocumentPart?.Document?.Body;
        if (body == null) return string.Empty;

        var sb = new StringBuilder();
        foreach(var para in body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Paragraph>()){
            sb.AppendLine(para.InnerText);
        }

        return sb.ToString().Trim();
    }

}
