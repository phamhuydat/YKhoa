using DinkToPdf;
namespace Web.Services
{
    public interface IPDFService
    {
        byte[] GeneratePDF(string html,
                  Orientation orientation = Orientation.Portrait,
                  PaperKind paperKind = PaperKind.A4);
    }
}