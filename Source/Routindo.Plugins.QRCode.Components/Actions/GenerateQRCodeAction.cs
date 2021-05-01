using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QRCoder;
using Routindo.Contract.Actions;
using Routindo.Contract.Arguments;
using Routindo.Contract.Attributes;
using Routindo.Contract.Services;

namespace Routindo.Plugins.QRCode.Components.Actions
{
    [PluginItemInfo(ComponentUniqueId, nameof(GenerateQRCodeAction),
         "Generate a QR Code from a given plaintext to output image", Category = "QRCode", FriendlyName = "Generate QRCode"),
     ExecutionArgumentsClass(typeof(GenerateQRCodeActionExecutionArgs))]
    public class GenerateQRCodeAction : IAction
    {
        public const string ComponentUniqueId = "88D396B4-5F07-4FDE-A097-B3E352956B5E";

        /// <summary>
        ///     Gets or sets the error correct level.
        /// </summary>
        /// <value>
        ///     The error correct level.
        /// </value>
        [Argument(GenerateQRCodeActionInstanceArgs.ErrorCorrectionLevel, true)]
        public string ErrorCorrectLevel { get; set; }

        /// <summary>
        ///     Gets or sets the image format.
        /// </summary>
        /// <value>
        ///     The image format.
        /// </value>
        [Argument(GenerateQRCodeActionInstanceArgs.ImageFormat, true)]
        public string ImageFormat { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [force UTF8].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [force UTF8]; otherwise, <c>false</c>.
        /// </value>
        [Argument(GenerateQRCodeActionInstanceArgs.ForceUtf8)]
        public bool ForceUtf8 { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [erase existing file].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [erase existing file]; otherwise, <c>false</c>.
        /// </value>
        [Argument(GenerateQRCodeActionInstanceArgs.EraseExistingFile, true)]
        public bool EraseExistingFile { get; set; }

        /// <summary>
        ///     Gets or sets the pixel per module.
        /// </summary>
        /// <value>
        ///     The pixel per module.
        /// </value>
        [Argument(GenerateQRCodeActionInstanceArgs.PixelPerModule)]
        public int PixelPerModule { get; set; } = 20;

        /// <summary>
        ///     Gets or sets the output file path.
        /// </summary>
        /// <value>
        ///     The output file path.
        /// </value>
        [Argument(GenerateQRCodeActionInstanceArgs.OutputFilePath)]
        public string OutputFilePath { get; set; }

        public string Id { get; set; }
        public ILoggingService LoggingService { get; set; }


        /// <summary>
        ///     Executes the specified arguments.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        /// <exception cref="Exception">
        ///     Missing mandatory execution argument: {GenerateQRCodeActionExecutionArgs.PlainText}
        ///     or
        ///     Mandatory instance argument is not set : {GenerateQRCodeActionInstanceArgs.ErrorCorrectionLevel}
        ///     or
        ///     Wrong value for argument: {GenerateQRCodeActionInstanceArgs.PixelPerModule} set to {PixelPerModule}
        ///     or
        ///     Mandatory instance argument is not set : {GenerateQRCodeActionInstanceArgs.ImageFormat}
        ///     or
        ///     Missing mandatory argument: {GenerateQRCodeActionExecutionArgs.OutputFilePath}
        ///     or
        ///     Another file with the same name exist: {outputFilePath}
        ///     or
        ///     Failed to get the QR Code image.
        /// </exception>
        public ActionResult Execute(ArgumentCollection arguments)
        {
            try
            {
                var plainText = string.Empty;
                if (arguments.HasArgument(GenerateQRCodeActionExecutionArgs.PlainText))
                    plainText = arguments.GetValue<string>(GenerateQRCodeActionExecutionArgs.PlainText);

                if (string.IsNullOrWhiteSpace(plainText))
                    throw new Exception(
                        $"Missing mandatory execution argument: {GenerateQRCodeActionExecutionArgs.PlainText}");

                QRCodeGenerator.ECCLevel? errorCorrectLevel = null;

                if (!string.IsNullOrWhiteSpace(ErrorCorrectLevel))
                    if (Enum.TryParse<QRCodeGenerator.ECCLevel>(ErrorCorrectLevel, true, out var eccl))
                        errorCorrectLevel = eccl;

                if (!errorCorrectLevel.HasValue)
                    throw new Exception(
                        $"Mandatory instance argument is not set : {GenerateQRCodeActionInstanceArgs.ErrorCorrectionLevel}");

                var pixelPerModule = 0;

                if (PixelPerModule >= MinPixelPerModule && PixelPerModule <= MaxPixelPerModule) pixelPerModule = PixelPerModule;

                if (pixelPerModule == 0)
                    throw new Exception(
                        $"Wrong value for argument: {GenerateQRCodeActionInstanceArgs.PixelPerModule} set to {PixelPerModule}");


                string imageFormat = null;

                if (!string.IsNullOrWhiteSpace(ImageFormat) && AllowedImageFormats()
                        .Any(f => string.Equals(f, ImageFormat, StringComparison.CurrentCultureIgnoreCase)))
                    imageFormat = ImageFormat;

                if (imageFormat == null)
                    throw new Exception(
                        $"Mandatory instance argument is not set : {GenerateQRCodeActionInstanceArgs.ImageFormat}");

                var outputFilePath = OutputFilePath;

                if (string.IsNullOrWhiteSpace(outputFilePath))
                    if (arguments.HasArgument(GenerateQRCodeActionExecutionArgs.OutputFilePath))
                        outputFilePath = arguments.GetValue<string>(GenerateQRCodeActionExecutionArgs.OutputFilePath);

                if (string.IsNullOrWhiteSpace(outputFilePath))
                    throw new Exception(
                        $"Missing mandatory argument: {GenerateQRCodeActionExecutionArgs.OutputFilePath}");

                if (EraseExistingFile)
                    if (File.Exists(outputFilePath))
                        File.Delete(outputFilePath);

                if (File.Exists(outputFilePath))
                    throw new Exception($"Another file with the same name exist: {outputFilePath}");

                byte[] qrCodeBytes = null;
                using (var qrCodeGenerator = new QRCodeGenerator())
                {
                    var qrCodeData = qrCodeGenerator.CreateQrCode(plainText, errorCorrectLevel.Value, ForceUtf8);
                    if (qrCodeData != null)
                    {
                        qrCodeBytes = GetByteQrCode(imageFormat, pixelPerModule, qrCodeData);
                    }
                }

                if (qrCodeBytes == null)
                    throw new Exception("Failed to get the QR Code image.");
                File.WriteAllBytes(outputFilePath, qrCodeBytes);
                // qrCodeImage.Save(outputFilePath, imageFormat);

                return ActionResult.Succeeded();
            }
            catch (Exception exception)
            {
                LoggingService.Error(exception);
                return ActionResult.Failed().WithException(exception);
            }
        }

        private static byte[] GetByteQrCode(string imageFormat, int pixelPerModule, QRCodeData qrCodeData)
        {
            if (imageFormat.ToLower() == "png")
                return new PngByteQRCode(qrCodeData).GetGraphic(pixelPerModule);

            if (imageFormat.ToLower() == "bmp")
                return new BitmapByteQRCode(qrCodeData).GetGraphic(pixelPerModule);

            return null;
        }

        public const int MaxPixelPerModule = 25;
        public const int MinPixelPerModule = 1;
        public static IEnumerable<string> AllowedImageFormats()
        {
            yield return "png";
            yield return "bmp";
            //yield return nameof(System.Drawing.Imaging.ImageFormat.Gif);
            //yield return nameof(System.Drawing.Imaging.ImageFormat.Jpeg);
            //yield return nameof(System.Drawing.Imaging.ImageFormat.Tiff);
        }
    }
}