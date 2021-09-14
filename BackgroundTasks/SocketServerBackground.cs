using System.Diagnostics;
using Windows.ApplicationModel.Background;

namespace BackgroundTasks
{
  public sealed class SocketServerBackground : IBackgroundTask
  {
    public void Run(IBackgroundTaskInstance taskInstance)
    {
      Debug.WriteLine("I ran the background task!");

    }
  }
}
