using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace Sandwich
{
    public partial class App : Application
    {
        public App()
        {
            /*		
			<ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml" />
			<ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Colors.xaml" />
			<ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml" />
			<ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml" />*/

            ResourceDictionary dic = new ResourceDictionary();

            //Metro theme
            dic.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml", UriKind.Absolute) });
            dic.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml", UriKind.Absolute) });
            dic.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Colors.xaml", UriKind.Absolute) });

            dic.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml", UriKind.Absolute) });
            dic.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml", UriKind.Absolute) });

            ////dic.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/Sandwich;component/Resources/icons/Icons.xaml", UriKind.Absolute) });
            ////dic.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/Sandwich;component/Resources/Metro.MSControls.Core.Implicit.xaml", UriKind.Absolute) });
            ////dic.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/Sandwich;component/Resources/Metro.MSControls.Toolkit.Implicit.xaml", UriKind.Absolute) });

            this.Resources = dic;

            //ArchiveExtensions.ArchiveDataProvider.init();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.MainWindow = new Sandwich.WPF.MainWindow();
            this.MainWindow.Show();
        }
    }
}
