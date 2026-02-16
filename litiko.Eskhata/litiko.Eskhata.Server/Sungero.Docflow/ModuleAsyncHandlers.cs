using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace litiko.Eskhata.Module.Docflow.Server
{
  partial class ModuleAsyncHandlers 
  {

    public virtual void CompleteApprovalAssignmentlitiko(litiko.Eskhata.Module.Docflow.Server.AsyncHandlerInvokeArgs.CompleteApprovalAssignmentlitikoInvokeArgs args)
    {
      if (args == null || args.ApprovalAssignmentId <= 0)
      {
        Logger.ErrorFormat("CompleteApprovalAssignmentlitiko. Not enough agruments.");
        return;
      }
      
      const int maxRetries = 100;
      
      // В лог добавляем все параметры, которые могут помочь в анализе при возникновении ошибок.
      var logPostfix = string.Format("ApprovalAssignmentId = {0}.", args.ApprovalAssignmentId);
      Logger.DebugFormat("CompleteApprovalAssignmentlitiko. Start. {0}", logPostfix);
          
      // Получить сушность
      var approvalAssignment = Sungero.Docflow.ApprovalAssignments.Get(args.ApprovalAssignmentId);
      if (approvalAssignment == null)
      {
        Logger.ErrorFormat("CompleteApprovalAssignmentlitiko. Assignment not found. {0}", logPostfix);
        return;
      }
      
      // Пытаемся заблокировать необходимую сущность.
      if (!Locks.TryLock(approvalAssignment))
      {
        // При неудачной попытке делаем запись в лог и отправляем обработчик на повтор.
        Logger.DebugFormat("CompleteApprovalAssignmentlitiko. Assignment is locked. Sent to retry. {0}", logPostfix);
        if (args.RetryIteration < maxRetries)
        {
          args.Retry = true;  // попросить платформу повторить
        }
        return;
      }
      
      try
      {
        // Логика изменения сущности.
        
        approvalAssignment.Complete(Sungero.Docflow.ApprovalAssignment.Result.Approved);

        Logger.DebugFormat("CompleteApprovalAssignmentlitiko. Assignment completed. {0}", logPostfix);
      }
      catch (Exception ex)
      {
        // В случае возникновения ошибки пишем сообщение в лог с уровнем Error и логируем полный стек ошибки.
        Logger.ErrorFormat("CompleteApprovalAssignmentlitiko. An error occured. {0}. Try = {1}. {2}", logPostfix, args.RetryIteration, ex.Message);
      
        if (args.RetryIteration < maxRetries)
        {
          args.Retry = true;  // попросить платформу повторить
        }
      }
      finally
      {
        // Снимаем блокировку с сущности.
        Locks.Unlock(approvalAssignment);
      }
      
      // Логируем завершение работы обработчика.
      Logger.DebugFormat("CompleteApprovalAssignmentlitiko. Finish. {0}", logPostfix);
    }

  }
}