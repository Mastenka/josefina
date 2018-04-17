//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using FluentScheduler;
//using System.Threading;
//using System.Web.Hosting;
//using System.Net.Http;

//namespace Josefina.Helpers
//{


//    public class TimerRegistry : Registry
//    {
//        public TimerRegistry()
//        {
//            // Schedule an IJob to run at an interval
//            Schedule<TicketsCallbackJob>().ToRunNow().AndEvery(1).Days();
//        }
//    }

//    public class TicketsCallbackJob : IJob, IRegisteredObject
//    {
//        private readonly object _lock = new object();

//        private bool _shuttingDown;

//        public TicketsCallbackJob()
//        {
//            // Register this job with the hosting environment.
//            // Allows for a more graceful stop of the job, in the case of IIS shutting down.
//            HostingEnvironment.RegisterObject(this);
//        }

//        public void Execute()
//        {
//            lock (_lock)
//            {
//                if (_shuttingDown)
//                    return;

//                string apiUrl = "http://localhost:44301/api/project/tickets/CancelOverDueTickets/";
//                using (var client = new HttpClient())
//                {
//                    var response = client.GetAsync(apiUrl).Result;

//                    if (response.IsSuccessStatusCode)
//                    {
//                        // by calling .Result you are performing a synchronous call
//                        var responseContent = response.Content;

//                        // by calling .Result you are synchronously reading the result
//                        string responseString = responseContent.ReadAsStringAsync().Result;

//                        Console.WriteLine(responseString);
//                    }
//                }
//            }
//        }

//        public void Stop(bool immediate)
//        {
//            // Locking here will wait for the lock in Execute to be released until this code can continue.
//            lock (_lock)
//            {
//                _shuttingDown = true;
//            }

//            HostingEnvironment.UnregisterObject(this);
//        }
//    }

//}