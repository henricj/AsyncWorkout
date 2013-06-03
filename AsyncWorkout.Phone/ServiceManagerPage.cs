using System;
using System.Linq;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace AsyncWorkout
{
    public class ServiceManagerPage : PhoneApplicationPage
    {
        static readonly Uri StartupPage = new Uri("/StartupPage.xaml", UriKind.Relative);

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (NavigationService.CanGoBack)
            {
                for (; ; )
                {
                    var first = NavigationService.BackStack.FirstOrDefault();

                    if (null == first || first.Source != StartupPage)
                        break;

                    NavigationService.RemoveBackEntry();
                }
            }

            if (!GlobalServices.ServiceManager.IsReady())
                NavigationService.Navigate(StartupPage);
        }
    }
}
