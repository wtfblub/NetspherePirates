using System;
using System.IO;
using Netsphere.Resource;
using Valit;

namespace Netsphere.Tools.ShopEditor.Validations
{
    public static class ConnectViewModelValidation
    {
        public static IValitator<string> Host { get; }
            = ValitRules<string>.Create()
                .Ensure(x => x, _ => _
                    .Required()
                    .Satisfies(ValidateHost).WithMessage("Invalid host(Format: 'localhost:3306')"))
                .CreateValitator();

        public static IValitator<string> Username { get; }
            = ValitRules<string>.Create()
                .Ensure(x => x, _ => _
                    .Required())
                .CreateValitator();

        public static IValitator<string> Password { get; }
            = ValitRules<string>.Create()
                .Ensure(x => x, _ => _
                    .Required())
                .CreateValitator();

        public static IValitator<string> Database { get; }
            = ValitRules<string>.Create()
                .Ensure(x => x, _ => _
                    .Required())
                .CreateValitator();

        public static IValitator<string> ResourcePath { get; }
            = ValitRules<string>.Create()
                .Ensure(x => x, _ => _
                    .Required()
                    .Satisfies(ValidateResourceFile).WithMessage("Invalid S4 League resource file"))
                .CreateValitator();

        public static IValitator<ViewModels.ConnectViewModel> MySql { get; }
            = ValitRules<ViewModels.ConnectViewModel>.Create()
                .Ensure(x => x.Host.Value, Host)
                .Ensure(x => x.Username.Value, Username)
                .Ensure(x => x.Password.Value, Password)
                .Ensure(x => x.Database.Value, Database)
                .Ensure(x => x.ResourcePath.Value, ResourcePath)
                .CreateValitator();

        public static IValitator<ViewModels.ConnectViewModel> Sqlite { get; }
            = ValitRules<ViewModels.ConnectViewModel>.Create()
                .Ensure(x => x.Host.Value, Host)
                .Ensure(x => x.ResourcePath.Value, ResourcePath)
                .CreateValitator();

        private static bool ValidateHost(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            var split = value.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length != 2)
                return false;

            return !string.IsNullOrEmpty(split[0]) && ushort.TryParse(split[1], out _);
        }

        private static (string value, bool result) s_lastResourceFile;

        private static bool ValidateResourceFile(string value)
        {
            if (s_lastResourceFile.value == value)
                return s_lastResourceFile.result;

            if (string.IsNullOrWhiteSpace(value) || !File.Exists(value))
                return false;

            try
            {
                S4Zip.OpenZip(value);
                s_lastResourceFile = (value, true);
                return true;
            }
            catch
            {
                s_lastResourceFile = (value, false);
                return false;
            }
        }
    }
}
