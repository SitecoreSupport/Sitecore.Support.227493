using Sitecore.Analytics;
using Sitecore.Analytics.Automation.Data;
using Sitecore.Analytics.Automation.MarketingAutomation;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Form.Core.Configuration;
using Sitecore.WFFM.Abstractions.Actions;
using Sitecore.WFFM.Abstractions.Dependencies;
using System;

namespace Sitecore.Support.Form.Submit
{
  public class EnrollInEngagementPlan : Sitecore.Form.Submit.EnrollInEngagementPlan
  {
    public override void Execute(ID formId, AdaptedResultList adaptedFields, ActionCallContext actionCallContext = null, params object[] data)
    {
      if (Tracker.Current != null && Tracker.Current.Session != null && !string.IsNullOrEmpty(this.EngagementState) && adaptedFields.IsTrueStatement(this.EnrollWhen))
      {
        ID iD = default(ID);
        if (!ID.TryParse(this.EngagementState, out iD))
        {
          throw new InvalidOperationException(DependenciesManager.ResourceManager.GetString("NO_STATE_IN_ENGAGEMENT_PLAN_SELECTED"));
        }
        Item planByState = this.GetPlanByState(iD);
        AutomationStateManager automationStateManager = Tracker.Current.Session.CreateAutomationStateManager();
        if (planByState != null && !automationStateManager.IsInEngagementState(iD))
        {
          automationStateManager.EnrollInEngagementPlan(planByState.ID, iD);
        }
        if (automationStateManager.IsInEngagementState(iD))
        {
          return;
        }
        string message = string.Format(DependenciesManager.ResourceManager.GetString("THE_VISITOR_CANNOT_BE_ENROLL"), (planByState != null) ? planByState.DisplayName : iD.ToString());
        throw new InvalidOperationException(message);
      }
    }
    private Item GetPlanByState(ID stateId)
    {
      Assert.ArgumentNotNullOrEmpty(stateId, "stateId");
      Item item = StaticSettings.ContextDatabase.GetItem(stateId);
      if (item == null)
      {
        return null;
      }
      return item.Parent;
    }
  } 
}