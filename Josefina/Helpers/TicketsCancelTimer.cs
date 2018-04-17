using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Josefina.DAL;
using System.Collections;
using Josefina.Entities;
using Josefina.Mailers;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using Josefina.Models.TicketsViewModel;
using System.Net.Http;
using System.Configuration;

namespace Josefina.Helpers
{
    public static class TicketsCancelTimer
    {
        public static Timer ticketsCancelationTimer;

        public static void InitializeTimer()
        {
            //// Create an event to signal the timeout count threshold in the
            //// timer callback.
            //AutoResetEvent autoEvent = new AutoResetEvent(false);

            ////Callbeck delegate for Cancelation
            //TimerCallback tcb = TimerCallback;

            //DateTime current = DateTime.Now;
            //TimeSpan timeToGo = new TimeSpan(4, 0, 0) - current.TimeOfDay; // Timer se poprvé spustí v 04:00:00
            //TimeSpan dayInterval = new TimeSpan(24, 0, 0); // A poté každých 24h

            ////Hack start
            ////TimeSpan timeToGo = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.AddSeconds(30).Second) - current.TimeOfDay; // Timer se poprvé spustí v 04:20:00
            ////TimeSpan dayInterval = new TimeSpan(0, 30, 0); // A poté každých 24h
            ////Hack end


            //if (timeToGo.TotalMilliseconds < 0) //Je-li již po 04:00:00 -> přepočet na zbývající čas do dalšího dne
            //{
            //    timeToGo = timeToGo + dayInterval;
            //}

            //ticketsCancelationTimer = new Timer(tcb, autoEvent, Convert.ToInt32(timeToGo.TotalMilliseconds), Convert.ToInt32(dayInterval.TotalMilliseconds));
        }

        private static void TimerCallback(Object stateInfo)
        {
            //string apiUrl = "http://localhost:44301/api/project/tickets/CancelOverDueTickets/";
            //using (var client = new HttpClient())
            //{
            //    var response = client.GetAsync(apiUrl).Result;

            //    if (response.IsSuccessStatusCode)
            //    {
            //        // by calling .Result you are performing a synchronous call
            //        var responseContent = response.Content;

            //        // by calling .Result you are synchronously reading the result
            //        string responseString = responseContent.ReadAsStringAsync().Result;

            //        Console.WriteLine(responseString);
            //    }
            //}
        }
    }
}