﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CoreRuleEngine
{
    public class AppendSubconditionException : Exception
    {
        private readonly AbstractCompositeCondition m_parentCondition;

        private readonly ICondition m_subCondition;

        public AppendSubconditionException(AbstractCompositeCondition condition, ICondition subCondition)
        {
            m_parentCondition = condition;
            m_subCondition = subCondition;
        }

        public AbstractCompositeCondition ParentCondition
        {
            get
            {
                return m_parentCondition;
            }
        }

        public ICondition SubCondition
        {
            get
            {
                return m_subCondition;
            }
        }
    }

    public abstract class ParseRuleFileException : Exception
    {
        private readonly string m_ruleName;

        protected ParseRuleFileException(string ruleName)
        {
            m_ruleName = ruleName;
        }

        public string RuleName
        {
            get
            {
                return m_ruleName;
            }
        }
    }

    public class NoCategoryAttributeException : ParseRuleFileException
    {
        public NoCategoryAttributeException(string ruleName)
            : base(ruleName) { }
    }

    public class ActionNodeNumberException : ParseRuleFileException
    {
        public ActionNodeNumberException(string ruleName) : base(ruleName) { }
    }

    /// <summary>
    /// Only one 'Condition' element is expected under the 'Rule' element.
    /// Throw when there is no or more than one 'Condition' element.
    /// </summary>
    public class ConditionNodeNumberException : ParseRuleFileException
    {
        public ConditionNodeNumberException(string ruleName) : base(ruleName) { }
    }

    /// <summary>
    /// Only one root condition is expected under the 'Condition' element.
    /// Throw when there is no or more than one root condition nodes.
    /// </summary>
    public class RootConditionNumberException : ParseRuleFileException
    {
        public RootConditionNumberException(string ruleName) : base(ruleName) { }
    }

    public abstract class WriteRuleFileException : Exception
    {
        private readonly string m_ruleName;

        protected WriteRuleFileException(string ruleName)
        {
            m_ruleName = ruleName;
        }

        public string RuleName
        {
            get
            {
                return m_ruleName;
            }
        }
    }

    public class ActionUninitializedException : WriteRuleFileException
    {
        public ActionUninitializedException(string ruleName) : base(ruleName) { }
    }
}
