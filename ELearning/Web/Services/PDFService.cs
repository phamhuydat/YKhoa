using DinkToPdf;
using DinkToPdf.Contracts;

namespace Web.Services
{
    public class PDFService : IPDFService
    {
        public readonly IConverter _converter;
        public PDFService(IConverter converter)
        {
            _converter = converter;
        }


        public byte[] GeneratePDF(string html,
                Orientation orientation = Orientation.Portrait,
                PaperKind paperKind = PaperKind.A4)
        {
            // Generate PDF
            var GlobalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = orientation,
                PaperSize = paperKind,
                Margins = new MarginSettings { Top = 10 },
            };

            var ObjectSetting = new ObjectSettings()
            {
                PagesCount = true,
                HtmlContent = html,
                //WebSettings = { DefaultEncoding = "utf-8" },
                //HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 }
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = GlobalSettings,
                Objects = { ObjectSetting }
            };

            return _converter.Convert(pdf);

        }
    }
}
