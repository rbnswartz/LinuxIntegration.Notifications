# LinuxIntegration.Notifications
Native Linux desktop notifications

## Building
Standard dotnet build process, `dotnet build`

## Usage
### Basic
Create an instance of NotificationManager and call ShowNotificationAsync
```c#
NotificationManager manager = new NotificationManager("test app");
var summary = "This is a summary";
var body = "This is the body of the notification";
await manager.ShowNotificationAsync(summary, body, expiration: 5000);
```
### Closing an existing notification
Get the returned Id when you created the notification and then call HideNotificationAsync
```c#
NotificationManager manager = new NotificationManager("test app");
var summary = "This is a summary";
var body = "This is the body of the notification";
var id = await manager.ShowNotificationAsync(summary, body, expiration: 5000);
await manager.HideNotificationAsync(id);
```

### Showing actions
Custom actions can be added to a notification if the notification system supports them.

You can check if it is supported by looking at the SupportsActions property on NotificationManager

If actions are supported you can use them like this. The key of the dictionary is the displayed value and the value is an action that will be called
when the action is clicked
```c#

NotificationManager manager = new NotificationManager("test app");
var summary = "This is a summary";
var body = "This is the body of the notification";
var notificationId = await manager.ShowNotificationAsync(summary, body, actions: new Dictionary<string, Action>()
    {
        ["Press me"] = () => Console.WriteLine("Press me action called"),
        ["Do other important thing"] = () => Console.WriteLine("Other important thing")
    },
    expiration: 5000);
```