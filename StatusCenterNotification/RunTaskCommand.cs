using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TaskStatusCenter;
using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace StatusCenterNotification
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class RunTaskCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("d9f9f2d0-bc09-44a3-b4cd-c50120d76d24");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="RunTaskCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private RunTaskCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static RunTaskCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in RunTaskCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new RunTaskCommand(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            TaskHandlerOptions options = default;
            options.Title = "Task";

            // Uncomment the following line to compare the behavior of 'RetainOnRanToCompletion'
            // and 'RetainAndNotifyOnRanToCompletion':
            //
            // options.ActionsAfterCompletion = CompletionActions.RetainOnRanToCompletion;
            options.ActionsAfterCompletion = CompletionActions.RetainAndNotifyOnRanToCompletion;

            options.DisplayTaskDetails = (t) => {
                VsShellUtilities.ShowMessageBox(this.package,
                                                "Done",
                                                "Task",
                                                OLEMSGICON.OLEMSGICON_INFO,
                                                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                                                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            };

            TaskProgressData data = default;

            IVsTaskStatusCenterService service = package.GetService<SVsTaskStatusCenterService, IVsTaskStatusCenterService>();
            ITaskHandler handler = service.PreRegister(options, data);

            Task task = RunTaskAsync(data, handler);

            handler.RegisterTask(task);
        }

        private async Task RunTaskAsync(TaskProgressData data, ITaskHandler handler)
        {
            float totalSteps = 3;

            for (float currentStep = 1; currentStep <= totalSteps; currentStep++)
            {
                await Task.Delay(1000);

                data.PercentComplete = (int)(currentStep / totalSteps * 100);
                data.ProgressText = $"Step {currentStep} of {totalSteps} completed";
                handler.Progress.Report(data);
            }
        }
    }
}
