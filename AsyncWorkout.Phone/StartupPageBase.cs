using System;
using System.Diagnostics;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace AsyncWorkout
{
    public class StartupPageBase : PhoneApplicationPage
    {
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                await GlobalServices.ServiceManager.Initializer;

                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
                else
                {
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));

                    NavigationService.RemoveBackEntry();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("StartupPageBase.OnNavigatedTo: " + ex.Message);
            }
        }
    }
}