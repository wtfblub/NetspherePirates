﻿using Netsphere.Tools.ShopEditor.ViewModels;

namespace Netsphere.Tools.ShopEditor.Views
{
    public sealed class SelectEffectView : View<SelectEffectViewModel>
    {
        public SelectEffectView()
        {
            DataContext = ViewModel = new SelectEffectViewModel();
            InitializeComponent();
        }
    }
}
