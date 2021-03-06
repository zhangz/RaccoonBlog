using System.Threading.Tasks;
using Elmah;
using RaccoonBlog.Web.Infrastructure.Raven;

namespace RaccoonBlog.Web.Infrastructure.Commands
{
    public static class CommandExecutor
    {
        public static void ExcuteLater(ICommand command)
        {
            Task.Factory.StartNew(() =>
            {
                var succcessfully = false;
                try
                {
                    DocumentStoreHolder.TryAddSession(command);
                    command.Execute();
                    succcessfully = true;
                }
                finally
                {
                    DocumentStoreHolder.TryComplete(command, succcessfully);
                }
            }, TaskCreationOptions.LongRunning)
                .ContinueWith(task =>
                {
                    ErrorLog.GetDefault(null).Log(new Error(task.Exception));
                }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
