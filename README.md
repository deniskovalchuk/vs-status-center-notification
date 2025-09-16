# Visual Studio Report

<h2>Problem description</h2>

This project uses [`IVsTaskStatusCenterService`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.taskstatuscenter.ivstaskstatuscenterservice?view=visualstudiosdk-2022)
to show progress in the Visual Studio status bar and demonstrates that [`CompletionActions.RetainAndNotifyOnRanToCompletion`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.taskstatuscenter.completionactions?view=visualstudiosdk-2022)
does not provide an additional visual notification upon task completion.

1. `CompletionActions.RetainOnRanToCompletion`

    Documentation:  
    `After the task associated with this flag runs to completion it is retained in the Task Status Center UI.`

    ![RetainOnRanToCompletion](https://github.com/deniskovalchuk/vs-status-center-notification/blob/main/Images/RetainOnRanToCompletion.gif)

2. `CompletionActions.RetainAndNotifyOnRanToCompletion`

    Documentation:  
    `After the task associated with this flag runs to completion it is retained in the Task Status Center UI and a visual notification is provided to the user indicating the task completed.`

    **There is no additional visual notification upon task completion:**
   
    ![RetainAndNotifyOnRanToCompletion](https://github.com/deniskovalchuk/vs-status-center-notification/blob/main/Images/RetainAndNotifyOnRanToCompletion.gif)

See [RunTaskCommand.Execute()](https://github.com/deniskovalchuk/vs-status-center-notification/blob/0dcf6bdd53c13bc1a415a6a848b4bf33dc69e030/StatusCenterNotification/RunTaskCommand.cs#L90) for more details.

<h2>Steps to reproduce</h2>

1. Download the project.
2. Open the solution in Visual Studio.
3. `Debug -> Start Debugging (F5)`.
4. In the opened Visual Studio Experimental Instance:
    - `Tools -> Invoke RunTaskCommand`.
5. Modify [RunTaskCommand.Execute()](https://github.com/deniskovalchuk/vs-status-center-notification/blob/0dcf6bdd53c13bc1a415a6a848b4bf33dc69e030/StatusCenterNotification/RunTaskCommand.cs#L97) to compare different values.

<h2>Environment</h2>

Microsoft Visual Studio Professional 2022 Version 17.14.13 (August 2025)  
Windows 11 Version 23H2 OS Build 22631.5624
