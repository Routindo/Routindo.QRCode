using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QRCoder;
using Umator.Contract;
using Umator.Plugins.QRCode.Components.Actions;

namespace Umator.Plugins.QRCode.Tests
{
    [TestClass]
    public class QRCodeGenerationTests
    {
        private readonly string qrCodeContent = "Umator is able to QR CODE";

        [TestMethod]
        public void GenerateQRCodeSuccessfulTest()
        {
            string imageFormat = "png";
            string eccl = QRCodeGenerator.ECCLevel.H.ToString("G");
            string output = Path.Combine(Path.GetTempPath(), "output.png");
            const bool eraseExistingFile = true;
            const bool forceUtf8 = true;
            const int pixelPerModule = 20;
            GenerateQRCodeAction action = new GenerateQRCodeAction()
            {
                ImageFormat = imageFormat,
                ErrorCorrectLevel = eccl,
                OutputFilePath = output, 
                EraseExistingFile = eraseExistingFile,
                ForceUtf8 = forceUtf8,
                PixelPerModule = pixelPerModule
            };

            var actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(GenerateQRCodeActionExecutionArgs.PlainText,
                    qrCodeContent));

            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);
            Assert.IsTrue(File.Exists(output));
            File.Delete(output);
        }

        [TestMethod]
        public void GenerateQRCodeFailsOnWrongPixelPerModule()
        {
            string imageFormat = "png";
            string eccl = QRCodeGenerator.ECCLevel.H.ToString("G");
            string output = Path.Combine(Path.GetTempPath(), "output.png");
            const bool eraseExistingFile = true;
            const bool forceUtf8 = true;
            int pixelPerModule = 26;
            GenerateQRCodeAction action = new GenerateQRCodeAction()
            {
                ImageFormat = imageFormat,
                ErrorCorrectLevel = eccl,
                OutputFilePath = output,
                EraseExistingFile = eraseExistingFile,
                ForceUtf8 = forceUtf8,
                PixelPerModule = pixelPerModule
            };

            var actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(GenerateQRCodeActionExecutionArgs.PlainText,
                    "Fails on pixel per module > 25"));

            Assert.IsNotNull(actionResult);
            Assert.IsFalse(actionResult.Result);
            Assert.IsNotNull(actionResult.AttachedException);
            Assert.IsFalse(File.Exists(output));

            pixelPerModule = 0;
            action.PixelPerModule = pixelPerModule;
            actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(GenerateQRCodeActionExecutionArgs.PlainText,
                    "Fails on pixel per module < than 1px"));

            Assert.IsNotNull(actionResult);
            Assert.IsFalse(actionResult.Result);
            Assert.IsNotNull(actionResult.AttachedException);
            Assert.IsFalse(File.Exists(output));
        }

        [TestMethod]
        public void GenerateQRCodeFailsOnWrongOrEmptyImageFormat()
        {
            string imageFormat = "umator";
            string eccl = QRCodeGenerator.ECCLevel.H.ToString("G");
            string output = Path.Combine(Path.GetTempPath(), "output.png");
            const bool eraseExistingFile = true;
            const bool forceUtf8 = true;
            const int pixelPerModule = 20;
            GenerateQRCodeAction action = new GenerateQRCodeAction()
            {
                ImageFormat = imageFormat,
                ErrorCorrectLevel = eccl,
                OutputFilePath = output,
                EraseExistingFile = eraseExistingFile,
                ForceUtf8 = forceUtf8,
                PixelPerModule = pixelPerModule
            };

            var actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(GenerateQRCodeActionExecutionArgs.PlainText,
                    qrCodeContent));

            Assert.IsNotNull(actionResult);
            Assert.IsFalse(actionResult.Result);
            Assert.IsNotNull(actionResult.AttachedException);
            Assert.IsFalse(File.Exists(output));

            imageFormat = string.Empty;
            action.ImageFormat = imageFormat;

            actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(GenerateQRCodeActionExecutionArgs.PlainText,
                    qrCodeContent));

            Assert.IsNotNull(actionResult);
            Assert.IsFalse(actionResult.Result);
            Assert.IsNotNull(actionResult.AttachedException);
            Assert.IsFalse(File.Exists(output));
        }

        [TestMethod]
        public void GenerateQRCodeFailsOnWrongOrEmptyECCL()
        {
            string imageFormat = "png";
            string eccl = "U"; //QRCodeGenerator.ECCLevel.H.ToString("G");
            string output = Path.Combine(Path.GetTempPath(), "output.png");
            const bool eraseExistingFile = true;
            const bool forceUtf8 = true;
            const int pixelPerModule = 20;
            GenerateQRCodeAction action = new GenerateQRCodeAction()
            {
                ImageFormat = imageFormat,
                ErrorCorrectLevel = eccl,
                OutputFilePath = output,
                EraseExistingFile = eraseExistingFile,
                ForceUtf8 = forceUtf8,
                PixelPerModule = pixelPerModule
            };

            var actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(GenerateQRCodeActionExecutionArgs.PlainText,
                    qrCodeContent));

            Assert.IsNotNull(actionResult);
            Assert.IsFalse(actionResult.Result);
            Assert.IsNotNull(actionResult.AttachedException);
            Assert.IsFalse(File.Exists(output));

            eccl = string.Empty;
            action.ErrorCorrectLevel = eccl;

            actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(GenerateQRCodeActionExecutionArgs.PlainText,
                    qrCodeContent));

            Assert.IsNotNull(actionResult);
            Assert.IsFalse(actionResult.Result);
            Assert.IsNotNull(actionResult.AttachedException);
            Assert.IsFalse(File.Exists(output));
        }

        [TestMethod]
        public void GenerateQRCodeFailsNullOrEmptyOutput()
        {
            string imageFormat = "png";
            string eccl = QRCodeGenerator.ECCLevel.H.ToString("G");
            string output = null; //Path.Combine(Path.GetTempPath(), "output.png");
            const bool eraseExistingFile = true;
            const bool forceUtf8 = true;
            const int pixelPerModule = 20;
            GenerateQRCodeAction action = new GenerateQRCodeAction()
            {
                ImageFormat = imageFormat,
                ErrorCorrectLevel = eccl,
                OutputFilePath = output,
                EraseExistingFile = eraseExistingFile,
                ForceUtf8 = forceUtf8,
                PixelPerModule = pixelPerModule
            };

            var actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(GenerateQRCodeActionExecutionArgs.PlainText,
                    qrCodeContent));

            Assert.IsNotNull(actionResult);
            Assert.IsFalse(actionResult.Result);
            Assert.IsNotNull(actionResult.AttachedException);
            Assert.IsFalse(File.Exists(output));

            output = string.Empty;
            action.OutputFilePath = output;

            actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(GenerateQRCodeActionExecutionArgs.PlainText,
                    qrCodeContent));

            Assert.IsNotNull(actionResult);
            Assert.IsFalse(actionResult.Result);
            Assert.IsNotNull(actionResult.AttachedException);
            Assert.IsFalse(File.Exists(output));
        }

        [TestMethod]
        public void GenerateQRCodeFailsOnNoEraseExisting()
        {
            string imageFormat = "png";
            string eccl = QRCodeGenerator.ECCLevel.H.ToString("G");
            string output = Path.Combine(Path.GetTempPath(), "output.png");
            bool eraseExistingFile = true;
            const bool forceUtf8 = true;
            const int pixelPerModule = 20;
            GenerateQRCodeAction action = new GenerateQRCodeAction()
            {
                ImageFormat = imageFormat,
                ErrorCorrectLevel = eccl,
                OutputFilePath = output,
                EraseExistingFile = eraseExistingFile,
                ForceUtf8 = forceUtf8,
                PixelPerModule = pixelPerModule
            };

            var actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(GenerateQRCodeActionExecutionArgs.PlainText,
                    qrCodeContent));

            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);
            Assert.IsTrue(File.Exists(output));

            eraseExistingFile = false;
            action.EraseExistingFile = eraseExistingFile;

            actionResult = action.Execute(ArgumentCollection.New()
                .WithArgument(GenerateQRCodeActionExecutionArgs.PlainText,
                    qrCodeContent));

            Assert.IsNotNull(actionResult);
            Assert.IsFalse(actionResult.Result);
            Assert.IsNotNull(actionResult.AttachedException);
            Assert.IsTrue(File.Exists(output));
            File.Delete(output);
        }
    }
}
