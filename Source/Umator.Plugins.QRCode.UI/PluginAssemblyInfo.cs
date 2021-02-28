using Umator.Contract;
using Umator.Plugins.QRCode.Components.Actions;
using Umator.Plugins.QRCode.UI.Views;

[assembly: ComponentConfigurator(typeof(GenerateQRCodeActionConfiguratorView), GenerateQRCodeAction.ComponentUniqueId, "Configurator for QR Code Generator")]