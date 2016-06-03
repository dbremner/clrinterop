﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Linq;

namespace CoreRuleEngine
{
    /// <summary>
    /// The definition of AndCondition.
    /// AndConditionDef is the creator of AndCondition instances.
    /// In this definition, the condition name of AndConditions is set "And".
    /// </summary>
    public class AndConditionDef : AbstractMultipleConditionDef
    {
        private const string s_conditionName = "And";

        private AndConditionDef() { }

        private static readonly AndConditionDef s_andConditionDef = new AndConditionDef();

        public static AndConditionDef GetInstance()
        {
            return s_andConditionDef;
        }

        public override ICondition Create()
        {
            return new AndCondition();
        }

        public override string GetConditionName()
        {
            return s_conditionName;
        }
    }

    /// <summary>
    /// The Condition for And logic.
    /// An AndCondition returns true, if all its subconditions return true. That means all its
    /// subconditions are satisfied.
    /// </summary>
    public class AndCondition : AbstractMultipleCondition
    {
        public AndCondition()
        {
            m_list = new List<ICondition>();
        }

        public AndCondition(List<ICondition> andList)
        {
            Debug.Assert(andList != null);
            m_list = andList;
        }

        public override bool Match(IMatchTarget matchTarget)
        {
            return m_list.All(t => t.Match(matchTarget));
        }

        public override IConditionDef GetConditionDef()
        {
            return AndConditionDef.GetInstance();
        }
    }
}
