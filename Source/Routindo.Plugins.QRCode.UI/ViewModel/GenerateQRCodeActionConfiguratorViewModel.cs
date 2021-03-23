using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using QRCoder;
using Routindo.Contract;
using Routindo.Contract.Arguments;
using Routindo.Contract.UI;
using Routindo.Plugins.QRCode.Components.Actions;

namespace Routindo.Plugins.QRCode.UI.ViewModel
{
    public class GenerateQRCodeActionConfiguratorViewModel: PluginConfiguratorViewModelBase
    {

        private bool _eraseExistingFile = true;
        private int _pixelPerModule = 5;
        private bool _forceUtf8;
        private string _imageFormat = "Png";
        private QRCodeGenerator.ECCLevel _errorCorrectionLevel = QRCodeGenerator.ECCLevel.M;
        private string _outputFilePath;
        public ObservableCollection<QRCodeGenerator.ECCLevel> EccLevels { get; set; }
        public ObservableCollection<string> ImageFormats { get; set; }

        public ObservableCollection<int> PixelPerModuleList { get; set; }

        public GenerateQRCodeActionConfiguratorViewModel()
        {
            EccLevels = new ObservableCollection<QRCodeGenerator.ECCLevel>(Enum.GetValues<QRCodeGenerator.ECCLevel>());
            ImageFormats = new ObservableCollection<string>(GenerateQRCodeAction.AllowedImageFormats());

            PixelPerModuleList = new ObservableCollection<int>(Enumerable.Range(GenerateQRCodeAction.MinPixelPerModule,
                GenerateQRCodeAction.MaxPixelPerModule));

            SelectOutputPathCommand = new RelayCommand(SelectOutputPath);
        }

        private void SelectOutputPath()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                DefaultExt = $"*.{ImageFormat.ToLower()}",
                Filter = $"Image (.{ImageFormat.ToLower()})|*.{ImageFormat.ToLower()}",
                CheckFileExists = false,
                CheckPathExists = false,
                FileName = OutputFilePath,
                ValidateNames = false
            };

            var result= saveFileDialog.ShowDialog(Application.Current.MainWindow);
            if (result.Value)
            {
                OutputFilePath = saveFileDialog.FileName;
            }
        }

        public ICommand SelectOutputPathCommand { get; set; }

        public QRCodeGenerator.ECCLevel ErrorCorrectionLevel
        {
            get => _errorCorrectionLevel;
            set
            {
                _errorCorrectionLevel = value;
                OnPropertyChanged();
            }
        }

        public string ImageFormat
        {
            get => _imageFormat;
            set
            {
                _imageFormat = value;
                OnPropertyChanged();
                OutputFilePath = _outputFilePath;
            }
        }

        public bool ForceUtf8
        {
            get => _forceUtf8;
            set
            {
                _forceUtf8 = value;
                OnPropertyChanged();
            }
        }

        public int PixelPerModule
        {
            get => _pixelPerModule;
            set
            {
                _pixelPerModule = value;
                OnPropertyChanged();
            }
        }

        public string OutputFilePath
        {
            get => _outputFilePath;
            set
            {
                _outputFilePath = value;
                ClearPropertyErrors();
                ValidateOutputFile();
                OnPropertyChanged();
            }
        }

        public bool EraseExistingFile
        {
            get => _eraseExistingFile;
            set
            {
                _eraseExistingFile = value;
                OnPropertyChanged();
            }
        }

        public override void Configure()
        {
            this.InstanceArguments = ArgumentCollection.New()
                .WithArgument(GenerateQRCodeActionInstanceArgs.ImageFormat, ImageFormat)
                .WithArgument(GenerateQRCodeActionInstanceArgs.EraseExistingFile, EraseExistingFile)
                .WithArgument(GenerateQRCodeActionInstanceArgs.ErrorCorrectionLevel, ErrorCorrectionLevel.ToString("G"))
                .WithArgument(GenerateQRCodeActionInstanceArgs.ForceUtf8, ForceUtf8)
                .WithArgument(GenerateQRCodeActionInstanceArgs.OutputFilePath, OutputFilePath)
                .WithArgument(GenerateQRCodeActionInstanceArgs.PixelPerModule, PixelPerModule);
        }

        public override void SetArguments(ArgumentCollection arguments)
        {
            if (arguments.HasArgument(GenerateQRCodeActionInstanceArgs.ImageFormat))
                ImageFormat = arguments.GetValue<string>(GenerateQRCodeActionInstanceArgs.ImageFormat);

            if (arguments.HasArgument(GenerateQRCodeActionInstanceArgs.EraseExistingFile))
                EraseExistingFile = arguments.GetValue<bool>(GenerateQRCodeActionInstanceArgs.EraseExistingFile);

            if (arguments.HasArgument(GenerateQRCodeActionInstanceArgs.ErrorCorrectionLevel))
            {
                var errorCorrectionLevel =
                    arguments.GetValue<string>(GenerateQRCodeActionInstanceArgs.ErrorCorrectionLevel);

                if (Enum.TryParse<QRCodeGenerator.ECCLevel>(errorCorrectionLevel, true, out var eccl))
                {
                    this.ErrorCorrectionLevel = eccl;
                }
            }

            ;

            if (arguments.HasArgument(GenerateQRCodeActionInstanceArgs.ForceUtf8))
                ForceUtf8 = arguments.GetValue<bool>(GenerateQRCodeActionInstanceArgs.ForceUtf8);

            if (arguments.HasArgument(GenerateQRCodeActionInstanceArgs.OutputFilePath))
                OutputFilePath = arguments.GetValue<string>(GenerateQRCodeActionInstanceArgs.OutputFilePath);

            if (arguments.HasArgument(GenerateQRCodeActionInstanceArgs.PixelPerModule))
                PixelPerModule = arguments.GetValue<int>(GenerateQRCodeActionInstanceArgs.PixelPerModule);
        }

        private void ValidateOutputFile()
        {
            if(string.IsNullOrWhiteSpace(OutputFilePath))
                return;

            if (!Path.GetExtension(OutputFilePath).ToLower().EndsWith(ImageFormat.ToLower()))
            {
                AddPropertyError(nameof(OutputFilePath), "Select another file with a correct extension");
            }
        }

        protected override void ValidateProperties()
        {
            // Output file
            ClearPropertyErrors(nameof(OutputFilePath));
            ValidateOutputFile();
            OnPropertyChanged(nameof(OutputFilePath));
        }
    }
}
