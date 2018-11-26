using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using LinqToDB;
using Microsoft.Extensions.Options;
using Netsphere.Common.Configuration;
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
        public ReactiveProperty<bool> UseSqlite { get; }

        public ConnectViewModel()
        {
            ConnectText = new ReactiveProperty<string>("Connect");

            UseSqlite = new ReactiveProperty<bool>(false);

            Host = new ReactiveProperty<string>("127.0.0.1:3306").SetValidateNotifyError(x =>
                UseSqlite.Value ? null : ConnectViewModelValidation.Host.Validate(x).ErrorMessages.FirstOrDefault());

            Username = new ReactiveProperty<string>("root").SetValidateNotifyError(x =>
                UseSqlite.Value ? null : ConnectViewModelValidation.Username.Validate(x).ErrorMessages.FirstOrDefault());

            Password = new ReactiveProperty<string>().SetValidateNotifyError(x =>
                UseSqlite.Value ? null : ConnectViewModelValidation.Password.Validate(x).ErrorMessages.FirstOrDefault());

            Database = new ReactiveProperty<string>().SetValidateNotifyError(x =>
                UseSqlite.Value ? null : ConnectViewModelValidation.Database.Validate(x).ErrorMessages.FirstOrDefault());

            ResourcePath = new ReactiveProperty<string>()
                .SetValidateNotifyError(x =>
                    UseSqlite.Value ? null : ConnectViewModelValidation.ResourcePath.Validate(x).ErrorMessages.FirstOrDefault());

            var canConnect = this.WhenAnyValue(x => x.Host.Value, x => x.Username.Value, x => x.Password.Value,
                    x => x.Database.Value, x => x.ResourcePath.Value, x => x.UseSqlite.Value)
                .Select(_ => UseSqlite.Value
                    ? ConnectViewModelValidation.Sqlite.Validate(this).Succeeded
                    : ConnectViewModelValidation.MySql.Validate(this).Succeeded);

            Connect = ReactiveCommand.CreateFromTask(ConnectImpl, canConnect);
            Exit = ReactiveCommand.Create(ExitImpl);
            SelectResourceFile = ReactiveCommand.CreateFromTask(SelectResourceFileImpl);

            this.WhenAnyValue(x => x.UseSqlite.Value)
                .Subscribe(_ =>
                {
                    Username.ForceValidate();
                    Password.ForceValidate();
                    Database.ForceValidate();
                });
        }

        private async Task ConnectImpl()
        {
            ConnectText.Value = "Connecting...";

            var split = Host.Value.Split(':');
            var host = split[0];
            var port = ushort.Parse(split[1]);
            IOptions<DatabaseOptions> options = new OptionsWrapper<DatabaseOptions>(new DatabaseOptions
            {
                ConnectionStrings = new ConnectionStrings
                {
                    Auth = "",
                    Game = $"Server={host};Port={port};Database={Database.Value};Uid={Username.Value};Pwd={Password.Value};SslMode=None;",
                },
                RunMigration = false,
                UseSqlite = UseSqlite.Value
            });

            var provider = new DatabaseProvider(options);
            try
            {
                await Task.Run(async () =>
                {
                    using (var db = provider.Open<GameContext>())
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

            AvaloniaLocator.CurrentMutable.Bind<IDatabaseProvider>().ToConstant(provider);
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
