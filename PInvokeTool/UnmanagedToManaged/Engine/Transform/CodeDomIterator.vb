﻿' Copyright (c) Microsoft Corporation.  All rights reserved.
Imports System.CodeDom
Imports System.Collections.Generic

Namespace Transform

    ''' <summary>
    ''' Used to iterate various CodeDom constructions
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CodeDomIterator

        Public Function Iterate(ByVal col As CodeTypeDeclarationCollection) As List(Of Object)
            ThrowIfNull(col, NameOf(col))

            Dim list As New List(Of Object)
            For Each ctd As CodeTypeDeclaration In col
                IterateTypeMemberImpl(ctd, list)
            Next

            Return list
        End Function

        Public Function Iterate(ByVal ctd As CodeTypeDeclaration) As List(Of Object)
            ThrowIfNull(ctd, NameOf(ctd))

            Dim list As New List(Of Object)
            IterateTypeMemberImpl(ctd, list)
            Return list
        End Function

#Region "CodeMember Iteration"

        Private Sub IterateTypeMemberImpl(ByVal ctm As CodeTypeMember, ByVal list As List(Of Object))
            ThrowIfNull(ctm, NameOf(ctm))
            ThrowIfNull(list, NameOf(list))

            list.Add(ctm)
            IterateAttributesImpl(ctm.CustomAttributes, list)

            Dim memEvent = TryCast(ctm, CodeMemberEvent)
            If memEvent IsNot Nothing Then
                IterateMemberEventImpl(memEvent, list)
                Return
            End If

            Dim memField = TryCast(ctm, CodeMemberField)
            If memField IsNot Nothing Then
                IterateMemberFieldImpl(memField, list)
                Return
            End If

            Dim memMethod = TryCast(ctm, CodeMemberMethod)
            If memMethod IsNot Nothing Then
                IterateMemberMethodImpl(memMethod, list)
                Return
            End If

            Dim memProperty = TryCast(ctm, CodeMemberProperty)
            If memProperty IsNot Nothing Then
                IterateMemberPropertyImpl(memProperty, list)
                Return
            End If

            Dim memSnippet = TryCast(ctm, CodeSnippetTypeMember)
            If memSnippet IsNot Nothing Then
                IterateSnippetTypeImpl(memSnippet, list)
                Return
            End If

            Dim typeDecl = TryCast(ctm, CodeTypeDeclaration)
            If typeDecl IsNot Nothing Then
                IterateTypeDeclarationImpl(typeDecl, list)
                Return
            End If

        End Sub

        Private Sub IterateTypeDeclarationImpl(ByVal col As CodeTypeDeclarationCollection, ByVal list As List(Of Object))
            ThrowIfNull(col, NameOf(col))
            ThrowIfNull(list, NameOf(list))

            For Each ctd As CodeTypeDeclaration In col
                IterateTypeMemberImpl(ctd, list)
            Next
        End Sub

        Private Sub IterateTypeDeclarationImpl(ByVal ctd As CodeTypeDeclaration, ByVal list As List(Of Object))
            ThrowIfNull(ctd, NameOf(ctd))
            ThrowIfNull(list, NameOf(list))

            ' Don't add to the list because this is added in the member iterater
            For Each ctdRef As CodeTypeReference In ctd.BaseTypes
                IterateTypeRefImpl(ctdRef, list)
            Next

            For Each member As CodeTypeMember In ctd.Members
                IterateTypeMemberImpl(member, list)
            Next

            IterateTypeParametersImpl(ctd.TypeParameters, list)

            Dim delType = TryCast(ctd, CodeTypeDelegate)
            If delType IsNot Nothing Then
                list.Add(delType.ReturnType)

                IterateParametersImpl(delType.Parameters, list)
            End If

        End Sub

        Private Sub IterateMemberEventImpl(ByVal memEvent As CodeMemberEvent, ByVal list As List(Of Object))
            ThrowIfNull(memEvent, NameOf(memEvent))
            ThrowIfNull(list, NameOf(list))

            For Each typeRef As CodeTypeReference In memEvent.ImplementationTypes
                IterateTypeRefImpl(typeRef, list)
            Next

            IterateTypeRefImpl(memEvent.PrivateImplementationType, list)
            IterateTypeRefImpl(memEvent.Type, list)
        End Sub

        Private Sub IterateMemberFieldImpl(ByVal memField As CodeMemberField, ByVal list As List(Of Object))
            ThrowIfNull(memField, NameOf(memField))
            ThrowIfNull(list, NameOf(list))

            IterateTypeRefImpl(memField.Type, list)
            IterateExprImpl(memField.InitExpression, list)
        End Sub

        Private Sub IterateMemberMethodImpl(ByVal memMethod As CodeMemberMethod, ByVal list As List(Of Object))
            ThrowIfNull(memMethod, NameOf(memMethod))
            ThrowIfNull(list, NameOf(list))

            IterateTypeRefImpl(memMethod.ImplementationTypes, list)
            IterateParametersImpl(memMethod.Parameters, list)
            IterateTypeRefImpl(memMethod.PrivateImplementationType, list)
            IterateTypeRefImpl(memMethod.ReturnType, list)
            IterateAttributesImpl(memMethod.CustomAttributes, list)
            IterateStatementsImpl(memMethod.Statements, list)
            IterateTypeParametersImpl(memMethod.TypeParameters, list)
        End Sub

        Private Sub IterateMemberPropertyImpl(ByVal memProperty As CodeMemberProperty, ByVal list As List(Of Object))
            ThrowIfNull(memProperty, NameOf(memProperty))
            ThrowIfNull(list, NameOf(list))

            IterateTypeRefImpl(memProperty.Type, list)
            IterateTypeRefImpl(memProperty.ImplementationTypes, list)
            IterateParametersImpl(memProperty.Parameters, list)
            IterateTypeRefImpl(memProperty.PrivateImplementationType, list)
            If memProperty.HasGet Then
                IterateStatementsImpl(memProperty.GetStatements, list)
            End If
            If memProperty.HasSet Then
                IterateStatementsImpl(memProperty.SetStatements, list)
            End If

        End Sub

#End Region

#Region "CodeParameter Iteration"

        Private Sub IterateParametersImpl(ByVal col As CodeParameterDeclarationExpressionCollection, ByVal list As List(Of Object))
            ThrowIfNull(col, NameOf(col))
            ThrowIfNull(list, NameOf(list))

            For Each param As CodeParameterDeclarationExpression In col
                list.Add(param)
                IterateAttributesImpl(param.CustomAttributes, list)
                IterateTypeRefImpl(param.Type, list)
            Next
        End Sub

#End Region

#Region "CodeAttribute Iteration"
        Private Sub IterateAttributesImpl(ByVal col As CodeAttributeDeclarationCollection, ByVal list As List(Of Object))
            ThrowIfNull(col, NameOf(col))
            ThrowIfNull(list, NameOf(list))

            For Each decl As CodeAttributeDeclaration In col
                IterateAttributesImpl(decl, list)
            Next

        End Sub

        Private Sub IterateAttributesImpl(ByVal attr As CodeAttributeDeclaration, ByVal list As List(Of Object))
            ThrowIfNull(attr, NameOf(attr))
            ThrowIfNull(list, NameOf(list))

            list.Add(attr)
            IterateTypeRefImpl(attr.AttributeType, list)

            For Each arg As CodeAttributeArgument In attr.Arguments
                list.Add(arg)
                IterateExprImpl(arg.Value, list)
            Next
        End Sub

#End Region

#Region "CodeExpression Iteration"
        Private Sub IterateExprImpl(ByVal expr As CodeExpression, ByVal list As List(Of Object))
            ThrowIfNull(list, NameOf(list))
            If expr Is Nothing Then
                Return
            End If

            list.Add(expr)
            Dim primExpr = TryCast(expr, CodePrimitiveExpression)
            If primExpr IsNot Nothing Then
                list.Add(primExpr.Value)
                Return
            End If

            Dim fieldRef = TryCast(expr, CodeFieldReferenceExpression)
            If fieldRef IsNot Nothing Then
                IterateExprImpl(fieldRef.TargetObject, list)
                Return
            End If

            Dim typeRefExpr = TryCast(expr, CodeTypeReferenceExpression)
            If typeRefExpr IsNot Nothing Then
                IterateTypeRefImpl(typeRefExpr.Type, list)
                Return
            End If

            Dim binExpr = TryCast(expr, CodeBinaryOperatorExpression)
            If binExpr IsNot Nothing Then
                IterateExprImpl(binExpr.Left, list)
                IterateExprImpl(binExpr.Right, list)
                Return
            End If

            Dim propSetExpr = TryCast(expr, CodePropertySetValueReferenceExpression)
            If propSetExpr IsNot Nothing Then
                Return
            End If

            Dim thisExpr = TryCast(expr, CodeThisReferenceExpression)
            If thisExpr IsNot Nothing Then
                Return
            End If

            Dim methodInvokeExpr = TryCast(expr, CodeMethodInvokeExpression)
            If methodInvokeExpr IsNot Nothing Then
                For Each arg As CodeExpression In methodInvokeExpr.Parameters
                    IterateExprImpl(arg, list)
                Next
                IterateExprImpl(methodInvokeExpr.Method, list)
                Return
            End If

            Dim methodRefExpr = TryCast(expr, CodeMethodReferenceExpression)
            If methodRefExpr IsNot Nothing Then
                IterateExprImpl(methodRefExpr.TargetObject, list)
                Return
            End If

            Dim createExpr = TryCast(expr, CodeObjectCreateExpression)
            If createExpr IsNot Nothing Then
                IterateTypeRefImpl(createExpr.CreateType, list)
                Return
            End If

            Dim varRefExpr = TryCast(expr, CodeVariableReferenceExpression)
            If varRefExpr IsNot Nothing Then
                Return
            End If

            Dim createArrayExpr = TryCast(expr, CodeArrayCreateExpression)
            If createArrayExpr IsNot Nothing Then
                IterateTypeRefImpl(createArrayExpr.CreateType, list)
                Return
            End If

            Dim castExpr = TryCast(expr, CodeCastExpression)
            If castExpr IsNot Nothing Then
                IterateTypeRefImpl(castExpr.TargetType, list)
                IterateExprImpl(castExpr.Expression, list)
                Return
            End If

            Dim notExpr = TryCast(expr, CodeNotExpression)
            If notExpr IsNot Nothing Then
                IterateExprImpl(notExpr.Expression, list)
                Return
            End If

            Dim negativeExpr = TryCast(expr, CodeNegativeExpression)
            If negativeExpr IsNot Nothing Then
                IterateExprImpl(negativeExpr.Expression, list)
                Return
            End If

            Dim shiftExpr = TryCast(expr, CodeShiftExpression)
            If shiftExpr IsNot Nothing Then
                IterateExprImpl(shiftExpr.Left, list)
                IterateExprImpl(shiftExpr.Right, list)
                Return
            End If

            Dim directionalExpr = TryCast(expr, CodeDirectionalSymbolExpression)
            If directionalExpr IsNot Nothing Then
                IterateExprImpl(directionalExpr.Expression, list)
                return
            End If

            Throw New NotImplementedException("Unrecognized expression")
        End Sub

#End Region

#Region "CodeStatement iteration"

        Private Sub IterateStatementsImpl(ByVal statements As CodeStatementCollection, ByVal list As List(Of Object))
            For Each statement As CodeStatement In statements
                IterateStatementImpl(statement, list)
            Next
        End Sub

        Private Sub IterateStatementImpl(ByVal state As CodeStatement, ByVal list As List(Of Object))
            ThrowIfNull(state, NameOf(state))
            ThrowIfNull(list, NameOf(list))

            list.Add(state)
            Dim retState = TryCast(state, CodeMethodReturnStatement)
            If retState IsNot Nothing Then
                IterateExprImpl(retState.Expression, list)
                Return
            End If

            Dim asgState = TryCast(state, CodeAssignStatement)
            If asgState IsNot Nothing Then
                IterateExprImpl(asgState.Left, list)
                IterateExprImpl(asgState.Right, list)
                Return
            End If

            Dim varDeclState = TryCast(state, CodeVariableDeclarationStatement)
            If varDeclState IsNot Nothing Then
                IterateExprImpl(varDeclState.InitExpression, list)
                IterateTypeRefImpl(varDeclState.Type, list)
                Return
            End If

            Dim condState = TryCast(state, CodeConditionStatement)
            If condState IsNot Nothing Then
                IterateExprImpl(condState.Condition, list)
                IterateStatementsImpl(condState.TrueStatements, list)
                IterateStatementsImpl(condState.FalseStatements, list)
                Return
            End If

            Dim labelState = TryCast(state, CodeLabeledStatement)
            If labelState IsNot Nothing Then
                Return
            End If

            Dim gotoState = TryCast(state, CodeGotoStatement)
            If gotoState IsNot Nothing Then
                Return
            End If

            Dim exprState = TryCast(state, CodeExpressionStatement)
            If exprState IsNot Nothing Then
                IterateExprImpl(exprState.Expression, list)
                Return
            End If

            Throw New NotImplementedException("Unrecognized statement")
        End Sub
#End Region

        Private Sub IterateTypeRefImpl(ByVal col As CodeTypeReferenceCollection, ByVal list As List(Of Object))
            ThrowIfNull(col, NameOf(col))
            ThrowIfNull(list, NameOf(list))

            For Each typeRef As CodeTypeReference In col
                IterateTypeRefImpl(typeRef, list)
            Next
        End Sub

        Private Sub IterateTypeRefImpl(ByVal typeRef As CodeTypeReference, ByVal list As List(Of Object))
            If typeRef Is Nothing Then
                Return
            End If

            list.Add(typeRef)
            IterateTypeRefImpl(typeRef.ArrayElementType, list)
            IterateTypeRefImpl(typeRef.TypeArguments, list)
        End Sub

#Region "Not Implemented"

        Private Sub IterateSnippetTypeImpl(ByVal memSnippet As CodeSnippetTypeMember, ByVal list As List(Of Object))
            Throw New NotImplementedException()
        End Sub

        Private Sub IterateTypeParametersImpl(ByVal col As CodeTypeParameterCollection, ByVal list As List(Of Object))
            ThrowIfNull(col, NameOf(col))
            ThrowIfNull(list, NameOf(list))

            For Each param As CodeTypeParameter In col
                Throw New NotImplementedException()
            Next

        End Sub

#End Region

    End Class

End Namespace
