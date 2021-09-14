using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Windows.Input;
using UWPSocketServer.Helpers;
using UWPSocketServer.Services;
using UWPSocketServer.Views;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace UWPSocketServer.ViewModels
{
    public class ShellViewModel : ObservableObject
    {
        private readonly KeyboardAccelerator _altLeftKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu);
        private readonly KeyboardAccelerator _backKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.GoBack);
        private IList<KeyboardAccelerator> _keyboardAccelerators;

        private ICommand _loadedCommand;
        private ICommand _MenuViewsSignalRServerCommand;
        private ICommand _menuViewsMainCommand;
        private ICommand _menuViewsBlankCommand;
        private ICommand _menuViewsSockServCommand;
        private ICommand _menuFileExitCommand;
        private ICommand _MenuViewsBackgroundSocketServerCommand;

        public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new RelayCommand(OnLoaded));

        public ICommand MenuViewsMainCommand => _menuViewsMainCommand ?? (_menuViewsMainCommand = new RelayCommand(OnMenuViewsMain));
        public ICommand MenuViewsSignalRServerCommand => _MenuViewsSignalRServerCommand ?? (_MenuViewsSignalRServerCommand = new RelayCommand(OnMenuViewsSignalRServer));
        public ICommand MenuViewsBlankCommand => _menuViewsBlankCommand ?? (_menuViewsBlankCommand = new RelayCommand(OnMenuViewsBlank));
        public ICommand MenuViewsSockServCommand => _menuViewsSockServCommand ?? (_menuViewsSockServCommand = new RelayCommand(OnMenuViewsSockServ));
        public ICommand MenuViewsBackgroundSocketServerCommand => _MenuViewsBackgroundSocketServerCommand ?? (_MenuViewsBackgroundSocketServerCommand = new RelayCommand(OnMenuViewsBackgroundSocketServerCommand));
        

        public ICommand MenuFileExitCommand => _menuFileExitCommand ?? (_menuFileExitCommand = new RelayCommand(OnMenuFileExit));

        public ShellViewModel()
        {
        }

        public void Initialize(Frame shellFrame, SplitView splitView, Frame rightFrame, IList<KeyboardAccelerator> keyboardAccelerators)
        {
            NavigationService.Frame = shellFrame;
            MenuNavigationHelper.Initialize(splitView, rightFrame);
            _keyboardAccelerators = keyboardAccelerators;
        }

        private void OnLoaded()
        {
            // Keyboard accelerators are added here to avoid showing 'Alt + left' tooltip on the page.
            // More info on tracking issue https://github.com/Microsoft/microsoft-ui-xaml/issues/8
            _keyboardAccelerators.Add(_altLeftKeyboardAccelerator);
            _keyboardAccelerators.Add(_backKeyboardAccelerator);
        }

        private void OnMenuViewsMain() => MenuNavigationHelper.UpdateView(typeof(MainPage));
        private void OnMenuViewsSignalRServer() => MenuNavigationHelper.UpdateView(typeof(SignalRServerPage));
        private void OnMenuViewsBlank() => MenuNavigationHelper.UpdateView(typeof(BlankPage));
        private void OnMenuViewsSockServ() => MenuNavigationHelper.UpdateView(typeof(SockServPage));
        private void OnMenuViewsBackgroundSocketServerCommand() => MenuNavigationHelper.UpdateView(typeof(BackgroundSocketServer));

        private void OnMenuFileExit()
        {
            Application.Current.Exit();
        }

        private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
        {
            var keyboardAccelerator = new KeyboardAccelerator() { Key = key };
            if (modifiers.HasValue)
            {
                keyboardAccelerator.Modifiers = modifiers.Value;
            }

            keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;
            return keyboardAccelerator;
        }

        private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var result = NavigationService.GoBack();
            args.Handled = result;
        }
    }
}
