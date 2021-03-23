namespace Routindo.Plugins.QRCode.Components.Actions
{
    public static class GenerateQRCodeActionInstanceArgs
    {
        public const string ErrorCorrectionLevel = nameof(ErrorCorrectionLevel);
        public const string ImageFormat = nameof(ImageFormat);
        public const string ForceUtf8 = nameof(ForceUtf8); 
        public const string PixelPerModule = nameof(PixelPerModule);
        public const string OutputFilePath = nameof(OutputFilePath);
        public const string EraseExistingFile = nameof(EraseExistingFile); 
    }
}