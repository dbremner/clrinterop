using System;
using System.Collections.Generic;
using System.Text;
using CoreRuleEngine;
using TypeLibTypes.Interop;

namespace TlbImpRuleEngine
{
    public class TypeInfoMatchTarget : IMatchTarget, IGetTypeLibElementCommonInfo
    {
        private TypeLib m_typeLib;

        private readonly TypeInfo m_typeInfo;

        private readonly TYPEKIND m_typeKind;

        private readonly string m_typeString;

        private string m_name;

        private readonly Guid m_guid;

        public TypeInfoMatchTarget(TypeLib typeLib, TypeInfo typeInfo, TYPEKIND typeKind)
        {
            if (typeInfo == null) throw new ArgumentNullException(nameof(typeInfo));
            m_typeLib = typeLib;
            m_typeInfo = typeInfo;
            m_typeKind = typeKind;
            m_typeString = TypeLibUtility.TypeKind2String(m_typeKind);
            m_guid = typeInfo.GetTypeAttr().Guid;
        }

        public TYPEKIND TypeKind
        {
            get
            {
                return m_typeKind;
            }
        }

        public Guid GUID
        {
            get
            {
                return m_guid;
            }
        }

        #region IMatchTarget Members

        public ICategory GetCategory()
        {
            return TypeCategory.GetInstance();
        }

        #endregion

        #region ITypeLibElementCommonInfo Members

        public string Name
        {
            get
            {
                if (m_name == null)
                {
                    m_name = m_typeInfo.GetDocumentation();
                }
                return m_name;
            }
        }

        public string Type
        {
            get
            {
                return m_typeString;
            }
        }

        #endregion
    }
}
