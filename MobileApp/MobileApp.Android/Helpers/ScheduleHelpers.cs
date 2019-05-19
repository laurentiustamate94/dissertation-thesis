using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MobileApp.Droid.Helpers
{
    public class ScheduleHelpers
    {
        private const int TriggerIntervalInMiliseconds = 3 * 1000;

        private ScheduleHelpers()
        {
            // noop
        }

        public static void Schedule(Context context)
        {
            var alarmIntent = new Intent(context, typeof(BackgroundReceiver));
            var pending = PendingIntent.GetBroadcast(context, 0, alarmIntent, PendingIntentFlags.UpdateCurrent);
            var alarmManager = context.GetSystemService(Context.AlarmService).JavaCast<AlarmManager>();
            var triggerAtMilis = SystemClock.ElapsedRealtime() + TriggerIntervalInMiliseconds;

            alarmManager.Set(AlarmType.ElapsedRealtime, triggerAtMilis, pending);
        }
    }
}