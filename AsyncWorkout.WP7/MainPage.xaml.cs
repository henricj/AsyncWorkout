using System;
using System.Windows.Navigation;

namespace AsyncWorkout
{
    public partial class MainPage : ServiceManagerPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var strings = GlobalServices.Services.Strings;

            if (null == strings)
                return;


        }
    }
}