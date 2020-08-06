﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace FFXIVFishingScheduleViewer
{
    class FishDetailViewModel
        : ViewModel
    {
        private bool _isDisposed;
        private string _parentWindowTitle;
        private Fish _fish;
        private ICollection<MenuItemViewModel> _menuItems;
        private ICollection<FishDetailOfFishingSpotViewModel> _fishingSpots;
        private ISettingProvider _settingProvider;
        private int _selectedTabIndex;

        public FishDetailViewModel(string parentWindowTitle, Fish fish, FishingSpot selectedFishingSpot, Func<FishingSpot, string> memoGetter, ISettingProvider settingProvider)
        {
            _isDisposed = false;
            _parentWindowTitle = parentWindowTitle;
            _settingProvider = settingProvider;
            _settingProvider.UserLanguageChanged += _settingProvider_UserLanguageChanged;
            _fish = fish;
            var fishingSpots = _fish.FishingConditions.Select(condition => condition.FishingSpot).OrderBy(spot => spot.Area.AreaGroup.Order).ThenBy(spot => spot.Area.Order).ThenBy(spot => spot.Order);
            var fishingBaits = _fish.FishingConditions.SelectMany(condition => condition.FishingBaits).OrderBy(bait => bait.Order);
            _selectedTabIndex =
                (
                    Enumerable.Range(0, int.MaxValue)
                    .Zip(fishingSpots, (index, spot) => new { index, spot })
                    .Where(item => item.spot == selectedFishingSpot)
                    .FirstOrDefault() ?? new { index = 0, spot = (FishingSpot)null }
                ).index;
            _fishingSpots = fishingSpots.Select(spot => new FishDetailOfFishingSpotViewModel(fish, spot, memoGetter,settingProvider)).ToList();
            var brushConverer = new BrushConverter();
            Background = _fish.DifficultySymbol.GetBackgroundColor();
            _menuItems = new List<MenuItemViewModel>();
            _menuItems.Add(MenuItemViewModel.CreateShowFishInCBHMenuItem(_fish));
            foreach (var spot in fishingSpots)
                _menuItems.Add(MenuItemViewModel.CreateShowSpotInCBHMenuItem(spot));
            foreach (var bait in fishingBaits)
                _menuItems.Add(MenuItemViewModel.CreateShowBaitInCBHMenuItem(bait));
            _menuItems.Add(MenuItemViewModel.CreateSeparatorMenuItem());
            _menuItems.Add(MenuItemViewModel.CreateShowFishInEDBMenuItem(_fish));
            foreach (var bait in fishingBaits)
                _menuItems.Add(MenuItemViewModel.CreateShowBaitInEDBMenuItem(bait));
            _menuItems.Add(MenuItemViewModel.CreateSeparatorMenuItem());
            _menuItems.Add(MenuItemViewModel.CreateCancelMenuItem());
            GUIText = GUITextTranslate.Instance;
            ResetCommand = new SimpleCommand(p =>
            {
                foreach (var spot in _fishingSpots)
                {
                    spot.ResetCommand.Execute(null);
                }
            });
        }

        public string WindowTitleText => string.Format(GUIText["Title.FishDetailWindow"], _fish.Name, _parentWindowTitle);
        public IEnumerable<FishDetailOfFishingSpotViewModel> FishingSpots => _fishingSpots;
        public IEnumerable<MenuItemViewModel> ContextMenuItems => _menuItems;
        public GUITextTranslate GUIText { get; }
        public Brush Background { get; }

        public int SelectedTabIndex 
        {
            get => _selectedTabIndex;

            set
            {
                if (value != _selectedTabIndex)
                {
                    _selectedTabIndex = value;
                    RaisePropertyChangedEvent(nameof(SelectedTabIndex));
                }
            }
        }

        public ICommand OKCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand ResetCommand { get; }

        protected override void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                foreach (var item in _fishingSpots)
                    item.Dispose();
                _fishingSpots.Clear();
                foreach (var menuItem in _menuItems)
                    menuItem.Dispose();
                _menuItems.Clear();
                _settingProvider.UserLanguageChanged -= _settingProvider_UserLanguageChanged;
                _isDisposed = true;
                base.Dispose(disposing);
            }
        }

        private void _settingProvider_UserLanguageChanged(object sender, EventArgs e)
        {
            RaisePropertyChangedEvent(nameof(WindowTitleText));
            RaisePropertyChangedEvent(nameof(ContextMenuItems));
            RaisePropertyChangedEvent(nameof(GUIText));
        }
    }
}