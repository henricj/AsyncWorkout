using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace AsyncWorkout
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Url required");
                return;
            }

            try
            {
                if (ServicePointManager.DefaultConnectionLimit < 8)
                    ServicePointManager.DefaultConnectionLimit = 8;

                var asyncWorkoutServices = new AsyncWorkoutServices(token =>
                {
                    Debug.WriteLine("Starting workout");

                    var taskWorkout = new TaskWorkout(new Uri(args[0]), token);

                    var polling = new RunUpdatePolling<IEnumerable<string>>(taskWorkout.LoadAsync(), token, taskWorkout.ToString);

                    return polling.RunAsync();
                }, new CryptographyService());

                GlobalServices.ServiceManager = asyncWorkoutServices;
                GlobalServices.Services = asyncWorkoutServices;

                GlobalServices.ServiceManager.Launching();

                GlobalServices.ServiceManager.Initializer.Wait();

                var strings = GlobalServices.Services.Strings;

                GlobalServices.ServiceManager.Closing();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
