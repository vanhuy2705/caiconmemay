using QRCoder;

namespace QuanLyThueSanTheThao.Helpers;

public static class QrImageHelper
{
    public static Bitmap Create(string payload, int pixelsPerModule = 8)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.M);
        using var qr = new QRCode(data);
        return qr.GetGraphic(pixelsPerModule, Color.Black, Color.White, true);
    }
}
