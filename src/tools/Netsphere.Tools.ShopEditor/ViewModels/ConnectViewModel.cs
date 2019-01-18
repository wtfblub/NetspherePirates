using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using Netsphere.Database;
using Netsphere.Resource;
using Netsphere.Tools.ShopEditor.Services;
using Netsphere.Tools.ShopEditor.Validations;
using Netsphere.Tools.ShopEditor.Views;
using Reactive.Bindings;
using ReactiveUI;
using ReactiveCommand = ReactiveUI.ReactiveCommand;

namespace Netsphere.Tools.ShopEditor.ViewModels
{
    public class ConnectViewModel : ReactiveObject
    {
        public ReactiveCommand Connect { get; }
        public ReactiveCommand Exit { get; }
        public ReactiveCommand SelectResourceFile { get; }
        public ReactiveProperty<string> ConnectText { get; }
        public ReactiveProperty<string> Host { get; }
        public ReactiveProperty<string> Username { get; }
        public ReactiveProperty<string> Password { get; }
        public ReactiveProperty<string> Database { get; }
        public ReactiveProperty<string> ResourcePath { get; }

        public ConnectViewModel()
        {
            ConnectText = new ReactiveProperty<string>("Connect");

            Host = new ReactiveProperty<string>("127.0.0.1:3306").SetValidateNotifyError(x =>
                ConnectViewModelValidation.Host.Validate(x).ErrorMessages.FirstOrDefault());

            Username = new ReactiveProperty<string>("root").SetValidateNotifyError(x =>
                ConnectViewModelValidation.Username.Validate(x).ErrorMessages.FirstOrDefault());

            Password = new ReactiveProperty<string>("").SetValidateNotifyError(x =>
                ConnectViewModelValidation.Password.Validate(x).ErrorMessages.FirstOrDefault());

            Database = new ReactiveProperty<string>("").SetValidateNotifyError(x =>
                ConnectViewModelValidation.Database.Validate(x).ErrorMessages.FirstOrDefault());

            ResourcePath = new ReactiveProperty<string>("")
                .SetValidateNotifyError(x =>
                    ConnectViewModelValidation.ResourcePath.Validate(x).ErrorMessages.FirstOrDefault());

            var canConnect = this.WhenAnyValue(x => x.Host.Value, x => x.Username.Value, x => x.Password.Value,
                    x => x.Database.Value, x => x.ResourcePath.Value)
                .Select(_ => ConnectViewModelValidation.MySql.Validate(this).Succeeded);

            Connect = ReactiveCommand.CreateFromTask(ConnectImpl, canConnect);
            Exit = ReactiveCommand.Create(ExitImpl);
            SelectResourceFile = ReactiveCommand.CreateFromTask(SelectResourceFileImpl);
        }

        private async Task ConnectImpl()
        {
            ConnectText.Value = "Connecting...";

            var split = Host.Value.Split(':');
            var host = split[0];
            var port = ushort.Parse(split[1]);

            var connectionStringBuilder = new MySqlConnectionStringBuilder
            {
                Server = host,
                Port = port,
                Database = Database.Value,
                UserID = Username.Value,
                Password = Password.Value,
                SslMode = MySqlSslMode.None
            };

            var serviceProvider = new ServiceCollection()
                .AddSingleton<DatabaseService>()
                .AddDbContext<GameContext>(x => x.UseMySql(connectionStringBuilder.ConnectionString))
                .BuildServiceProvider();
            var databaseService = serviceProvider.GetRequiredService<DatabaseService>();

            try
            {
                await Task.Run(async () =>
                {
                    using (var db = databaseService.Open<GameContext>())
                        await db.ShopVersion.FirstOrDefaultAsync();
                });
            }
            catch (Exception ex)
            {
                await new MessageView("Error", "Unable to connect to database", ex).ShowDialog();
                return;
            }
            finally
            {
                ConnectText.Value = "Connect";
            }

            AvaloniaLocator.CurrentMutable.Bind<DatabaseService>().ToConstant(databaseService);
            AvaloniaLocator.CurrentMutable.Bind<S4Zip>().ToConstant(S4Zip.OpenZip(ResourcePath.Value));
            ResourceService.Instance.Load();
            await ShopService.Instance.LoadFromDatabase();
            var window = Application.Current.MainWindow;
            new MainView().Show();
            window.Close();
        }

        private void ExitImpl()
        {
            Application.Current.Exit();
        }

        private async Task SelectResourceFileImpl()
        {
            var dialog = new OpenFileDialog
            {
                AllowMultiple = false,
                Filters = new List<FileDialogFilter>(new[]
                {
                    new FileDialogFilter
                    {
                        Name = "S4 League archive",
                        Extensions = { "s4hd" }
                    }
                }),
                Title = "Select S4 League resource file"
            };

            var selectedFiles = await dialog.ShowAsync(Application.Current.MainWindow);
            if (selectedFiles != null && selectedFiles.Length > 0)
                ResourcePath.Value = selectedFiles.First();
        }
    }
}
