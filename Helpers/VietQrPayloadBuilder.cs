using System.Text;

namespace QuanLyThueSanTheThao.Helpers;

public static class VietQrPayloadBuilder
{
    private static string Tlv(string id, string value)
    {
        value ??= string.Empty;
        var len = Encoding.UTF8.GetByteCount(value);
        return $"{id}{len:00}{value}";
    }

    public static string Build(string bankBin, string accountNumber, decimal amount, string content)
    {
        if (string.IsNullOrWhiteSpace(bankBin)) throw new ArgumentException("Bank BIN chưa được cấu hình.");
        if (string.IsNullOrWhiteSpace(accountNumber)) throw new ArgumentException("Số tài khoản chưa được cấu hình.");

        var beneficiary = Tlv("00", bankBin.Trim()) + Tlv("01", accountNumber.Trim());
        var merchantInfo = Tlv("00", "A000000727") + Tlv("01", beneficiary) + Tlv("02", "QRIBFTTA");

        var payload =
            Tlv("00", "01") +          // Payload format indicator
            Tlv("01", "12") +          // Dynamic QR
            Tlv("38", merchantInfo) +   // VietQR merchant account info
            Tlv("53", "704") +         // VND
            Tlv("54", amount.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture)) +
            Tlv("58", "VN") +
            Tlv("62", Tlv("08", Sanitize(content, 25))) +
            "6304";

        return payload + Crc16Ccitt(payload);
    }

    private static string Sanitize(string value, int maxChars)
    {
        if (string.IsNullOrWhiteSpace(value)) return "THANH TOAN";
        var normalized = value.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var ch in normalized)
        {
            if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(ch) != System.Globalization.UnicodeCategory.NonSpacingMark)
                sb.Append(ch);
        }
        var plain = sb.ToString().Normalize(NormalizationForm.FormC).ToUpperInvariant();
        plain = new string(plain.Where(c => char.IsLetterOrDigit(c) || c == ' ' || c == '-' || c == '_').ToArray());
        return plain.Length <= maxChars ? plain : plain[..maxChars];
    }

    private static string Crc16Ccitt(string input)
    {
        ushort crc = 0xFFFF;
        foreach (var b in Encoding.UTF8.GetBytes(input))
        {
            crc ^= (ushort)(b << 8);
            for (var i = 0; i < 8; i++)
                crc = (ushort)((crc & 0x8000) != 0 ? (crc << 1) ^ 0x1021 : crc << 1);
        }
        return crc.ToString("X4");
    }
}
