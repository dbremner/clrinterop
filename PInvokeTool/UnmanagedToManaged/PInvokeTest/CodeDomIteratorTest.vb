﻿' Copyright (c) Microsoft Corporation.  All rights reserved.

'The following code was generated by Microsoft Visual Studio 2005.
'The test owner should check each test for validity.
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System
Imports System.Text
Imports System.Collections.Generic
Imports PInvoke.Parser
Imports PInvoke.Transform
Imports System.CodeDom


'''<summary>
'''This is a test class for PInvoke.Transform.CodeDomIterator and is intended
'''to contain all PInvoke.Transform.CodeDomIterator Unit Tests
'''</summary>
<TestClass()> _
Public Class CodeDomIteratorTest


    Private testContextInstance As TestContext

    '''<summary>
    '''Gets or sets the test context which provides
    '''information about and functionality for the current test run.
    '''</summary>
    Public Property TestContext() As TestContext
        Get
            Return testContextInstance
        End Get
        Set(ByVal value As TestContext)
            testContextInstance = value
        End Set
    End Property
#Region "Additional test attributes"
    '
    'You can use the following additional attributes as you write your tests:
    '
    'Use ClassInitialize to run code before running the first test in the class
    '
    '<ClassInitialize()>  _
    'Public Shared Sub MyClassInitialize(ByVal testContext As TestContext)
    'End Sub
    '
    'Use ClassCleanup to run code after all tests in a class have run
    '
    '<ClassCleanup()>  _
    'Public Shared Sub MyClassCleanup()
    'End Sub
    '
    'Use TestInitialize to run code before running each test
    '
    '<TestInitialize()>  _
    'Public Sub MyTestInitialize()
    'End Sub
    '
    'Use TestCleanup to run code after each test has run
    '
    '<TestCleanup()>  _
    'Public Sub MyTestCleanup()
    'End Sub
    '
#End Region

    Private Function Convert(ByVal code As String) As List(Of Object)
        Dim converter As New BasicConverter()
        Dim ctd As CodeTypeDeclarationCollection = converter.ConvertNativeCodeToCodeDom(code, New PInvoke.ErrorProvider())
        Dim it As New CodeDomIterator()
        Return it.Iterate(ctd)
    End Function

    Private Sub ExpectTypeRef(ByVal name As String, ByVal list As List(Of Object))
        For Each obj As Object In list
            Dim typeRef As CodeTypeReference = TryCast(obj, CodeTypeReference)
            If typeRef IsNot Nothing Then
                If 0 = String.CompareOrdinal(name, typeRef.BaseType) Then
                    Return
                End If
            End If
        Next

        Assert.Fail("Could not find the type reference: " & name)
    End Sub

    Private Sub ExpectField(ByVal name As String, ByVal list As List(Of Object))
        For Each obj As Object In list
            Dim field As CodeMemberField = TryCast(obj, CodeMemberField)
            If field IsNot Nothing AndAlso 0 = String.CompareOrdinal(field.Name, name) Then
                Return
            End If
        Next

        Assert.Fail("Could not find the field: " & name)
    End Sub

    Private Sub ExpectProc(ByVal name As String, ByVal list As List(Of Object))
        For Each obj As Object In list
            Dim field As CodeMemberMethod = TryCast(obj, CodeMemberMethod)
            If field IsNot Nothing AndAlso 0 = String.CompareOrdinal(field.Name, name) Then
                Return
            End If
        Next

        Assert.Fail("Could not find the proc: " & name)
    End Sub
    Private Sub ExpectType(ByVal name As String, ByVal list As List(Of Object))
        For Each obj As Object In list
            Dim ctd As CodeTypeDeclaration = TryCast(obj, CodeTypeDeclaration)
            If ctd IsNot Nothing AndAlso 0 = String.CompareOrdinal(ctd.Name, name) Then
                Return
            End If
        Next

        Assert.Fail("Could not find the type: " & name)
    End Sub
    ''' <summary>
    ''' Normal types
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub Depth1()
        Dim code As String = _
            "struct foo { int i; }; "
        Dim list As List(Of Object) = Convert(code)
        ExpectTypeRef("System.Int32", list)
    End Sub

    <TestMethod()> _
    Public Sub Depth2()
        Dim code As String = _
            "union foo { int i; char j; }"
        Dim list As List(Of Object) = Convert(code)
        ExpectTypeRef("System.Int32", list)
        ExpectTypeRef("System.Byte", list)
        ExpectField("j", list)
        ExpectField("i", list)
    End Sub

    ''' <summary>
    ''' Enumeration members
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub Depth3()
        Dim code As String = _
            "enum foo { v1, v2, };"
        Dim list As List(Of Object) = Convert(code)
        ExpectField("v1", list)
        ExpectField("v2", list)
        ExpectType("foo", list)
    End Sub

    <TestMethod()> _
    Public Sub IterateBitVector()
        Dim code As String = _
            "struct bar { int i: 4; }; "
        Dim list As List(Of Object) = Convert(code)
        ExpectType("bar", list)
    End Sub

    <TestMethod()> _
    Public Sub IterateProc()
        Dim code As String = _
            "int foo(char* y);"
        Dim list As List(Of Object) = Convert(code)
        ExpectTypeRef("System.Int32", list)
        ExpectProc("foo", list)
    End Sub


End Class
