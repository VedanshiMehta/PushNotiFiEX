using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Firebase.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PushNotiFiEX
{
    [Service]
    [IntentFilter(new[]{"com.google.firebase.MESSAGING_EVENT"})]
    class NotificationMessagingService: FirebaseMessagingService
    {
        private const string TAG = "NotificationMessagingService";
        private string _body;

        public readonly string channelID = "my_notification_channel";
        public const int pendingIntentId = 1;

        public override void OnNewToken(string token)
        {
            base.OnNewToken(token);
            Log.Info(TAG, "OnNewToken:" + token);
        }

        public override void OnMessageReceived(RemoteMessage message)
        {
            try
            {
                Log.Info(TAG, "From: " + message.From);

                var notification = message.GetNotification();
                if (notification != null && notification.Body != null)
                {
                    _body = notification.Body;
                }
                else {

                    _body = "Push Notification";
                }
                DisplayNotification(notification.Title, _body, message.Data);
            }
            catch(Exception ex)
            {

                Console.WriteLine(ex.Message);
            
            }
            base.OnMessageReceived(message);
        }

        private void DisplayNotification(string title, string body, IDictionary<string, string> data)
        {
            try
            {
                

                Intent intent = new Intent(this, typeof(NotificationActivity));
                intent.PutExtra("key", body);
                PendingIntent pendingIntent = PendingIntent.GetActivity(this, pendingIntentId,intent, PendingIntentFlags.UpdateCurrent);

                if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                {
                    return;                
                }

                string channelName = Resources.GetString(Resource.String.channel_name);
                string channelDescription = Resources.GetString(Resource.String.channel_description);
                NotificationChannel notificationchannel = new NotificationChannel(channelID, channelName, NotificationImportance.High);
                notificationchannel.Description = channelDescription;
                NotificationManager notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager.CreateNotificationChannel(notificationchannel);


                NotificationCompat.Builder builder = new NotificationCompat.Builder(this, channelID)
                                                .SetSmallIcon(Resource.Drawable.ic_newnotification)
                                                .SetColor(ContextCompat.GetColor(ApplicationContext, Resource.Color.my_blue))
                                                .SetColorized(true)
                                                .SetContentTitle(title)
                                                .SetContentText(body)
                                                .SetAutoCancel(true)
                                                .SetDefaults((int)NotificationDefaults.All)
                                                .SetContentIntent(pendingIntent);

                Notification notification = builder.Build();
                NotificationManager manager = GetSystemService(NotificationService) as NotificationManager;

                const int notificationID = 1;

                manager.Notify(notificationID, notification);

            } catch (Exception ecp)
            {
                Console.WriteLine(ecp.Message);

            }
        }
    }
}