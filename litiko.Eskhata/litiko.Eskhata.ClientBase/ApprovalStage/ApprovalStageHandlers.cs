using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using litiko.Eskhata.ApprovalStage;

namespace litiko.Eskhata
{
  partial class ApprovalStageClientHandlers
  {

    public virtual void AutoCompletionlitikoValueInput(Sungero.Presentation.BooleanValueInputEventArgs e)
    {
      litiko.Eskhata.Functions.ApprovalStage.SetAutoCompletionFieldVisibilityAndMandatory(_obj);
    }

    public virtual void AutoCompletionInHourslitikoValueInput(Sungero.Presentation.DoubleValueInputEventArgs e)
    {
      litiko.Eskhata.Functions.ApprovalStage.SetAutoCompletionFieldVisibilityAndMandatory(_obj);
    }

    public virtual void AutoCompletionInDayslitikoValueInput(Sungero.Presentation.IntegerValueInputEventArgs e)
    {
      litiko.Eskhata.Functions.ApprovalStage.SetAutoCompletionFieldVisibilityAndMandatory(_obj);
    }

    public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
    {
      base.Refresh(e);
      
      litiko.Eskhata.Functions.ApprovalStage.SetAutoCompletionFieldVisibilityAndMandatory(_obj);
    }

    public override void AllowSendToReworkValueInput(Sungero.Presentation.BooleanValueInputEventArgs e)
    {
      base.AllowSendToReworkValueInput(e);
      
      if (e.NewValue.GetValueOrDefault() && _obj.CustomStageTypelitiko == ApprovalStage.CustomStageTypelitiko.Pause)
        _obj.CustomStageTypelitiko = null;
    }

    public virtual IEnumerable<Enumeration> CustomStageTypelitikoFiltering(IEnumerable<Enumeration> query)
    {     
      #region Согласование
      if (_obj.StageType == StageType.Approvers)
      {        
        var allowedValues = new List<Enumeration>
        {
          CustomStageTypelitiko.BudgetCheck,
          CustomStageTypelitiko.BudgetCheckCont,
          CustomStageTypelitiko.AccountantAppr
        };        
        
        return query.Where(q => allowedValues.Contains(q));
      }      
      #endregion

      #region Задание
      if (_obj.StageType == StageType.SimpleAgr)
      {
        var allowedValues = new List<Enumeration>
        {
          CustomStageTypelitiko.Voting,
          CustomStageTypelitiko.IncludeInMeet,
          CustomStageTypelitiko.ControlIRD,
          CustomStageTypelitiko.ScanReceivedCon,
          CustomStageTypelitiko.SubmitIssueKou,
          CustomStageTypelitiko.Pause
        };        

        if (_obj.AllowSendToRework.GetValueOrDefault())
          allowedValues.Remove(CustomStageTypelitiko.Pause);
        
        return query.Where(q => allowedValues.Contains(q));
      }      
      #endregion

      #region Контроль возврата
      if (_obj.StageType == StageType.CheckReturn)
      {
        var allowedValues = new List<Enumeration>
        {
          CustomStageTypelitiko.OrigReceivedCon
        };        

        return query.Where(q => allowedValues.Contains(q));
      }      
      #endregion
      
      // Для всех остальных — недоступны никакие значения
      return Enumerable.Empty<Enumeration>();
    }
  }

}