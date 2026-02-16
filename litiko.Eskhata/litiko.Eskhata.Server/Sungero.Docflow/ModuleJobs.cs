using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace litiko.Eskhata.Module.Docflow.Server
{
  partial class ModuleJobs 
  {

    /// <summary>
    /// 
    /// </summary>
    public virtual void ApprovalAssignmentsAutocompletionlitiko()
    {
      // Поиск всех етапов согласования с включенным автозавершением
      var stages = Sungero.Docflow.ApprovalStages.GetAll(s =>
        s.StageType == Sungero.Docflow.ApprovalStage.StageType.Approvers
        && litiko.Eskhata.ApprovalStages.As(s) != null
        && litiko.Eskhata.ApprovalStages.As(s).AutoCompletionlitiko != null
        && litiko.Eskhata.ApprovalStages.As(s).AutoCompletionlitiko == true);
      
      if (stages != null)
      {
        // Поиск активных заданий на согласования, где используеться етап согласования с включенным автозавершением
        var assignments = Sungero.Docflow.ApprovalAssignments.GetAll(a => a.Status == Sungero.Workflow.AssignmentBase.Status.InProcess
                                                                       && a.Stage != null
                                                                       && stages.Contains(a.Stage)
                                                                       && (litiko.Eskhata.ApprovalStages.As(a.Stage).AutoCompletionInDayslitiko != null
                                                                        || litiko.Eskhata.ApprovalStages.As(a.Stage).AutoCompletionInHourslitiko != null)
                                                                      ).ToList();
        
        foreach (var assignment in assignments)
        {
          var stage = litiko.Eskhata.ApprovalStages.As(assignment.Stage);
          if (stage != null)
          {
            var days = stage.AutoCompletionInDayslitiko;
            var hours = stage.AutoCompletionInHourslitiko;
            var autocompletionStartDate = assignment.Created.Value;
            
            // Вычисляем дату автомического завершения
            if (days != null)
              autocompletionStartDate.AddWorkingDays(days.Value);
            if (hours != null)
              autocompletionStartDate.AddWorkingHours(hours.Value);
            
            // Проверяем если текущая дата больше даты автоматического завершения то завершаем задание                  
            if ((days != null || hours != null) && autocompletionStartDate < Calendar.Now)
            {
              // запускаем асинхронный обработчик по завершению задания "Согласование"
              var completeApprovalAssignmentHandler = litiko.Eskhata.Module.Docflow.AsyncHandlers.CompleteApprovalAssignmentlitiko.Create();
              completeApprovalAssignmentHandler.ApprovalAssignmentId = assignment.Id;
              completeApprovalAssignmentHandler.ExecuteAsync();
            }
          }
        }
      }
    }

  }
}