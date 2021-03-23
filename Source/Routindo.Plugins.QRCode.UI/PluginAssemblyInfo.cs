using Routindo.Contract;
using Routindo.Contract.Attributes;
using Routindo.Plugins.QRCode.Components.Actions;
using Routindo.Plugins.QRCode.UI.Views;

[assembly: ComponentConfigurator(typeof(GenerateQRCodeActionConfiguratorView), GenerateQRCodeAction.ComponentUniqueId, "Configurator for QR Code Generator")]