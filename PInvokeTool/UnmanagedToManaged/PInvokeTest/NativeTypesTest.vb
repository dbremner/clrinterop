﻿' Copyright (c) Microsoft Corporation.  All rights reserved.
'The following code was generated by Microsoft Visual Studio 2005.
'The test owner should check each test for validity.
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System
Imports System.Text
Imports System.Collections.Generic
Imports PInvoke
Imports PInvoke.Parser

'''<summary>
'''This is a test class for PInvoke.Parser.NativeType and is intended
'''to contain all PInvoke.Parser.NativeType Unit Tests
'''</summary>
<TestClass()> _
Public Class NativeSymbolTest

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

    Private Sub VerifyReachable(ByVal nt As NativeType, ByVal ParamArray names As String())
        Dim bag As New NativeSymbolBag()

        Dim definedNt = TryCast(nt, NativeDefinedType)
        Dim typedefNt = TryCast(nt, NativeTypeDef)
        If definedNt IsNot Nothing Then
            bag.AddDefinedType(DirectCast(nt, NativeDefinedType))
        ElseIf typedefNt IsNot Nothing Then
            bag.AddTypedef(DirectCast(nt, NativeTypeDef))
        Else
            Assert.Fail("Not a searchable type")
        End If

        VerifyReachable(bag, names)
    End Sub

    Private Sub VerifyReachable(ByVal bag As NativeSymbolBag, ByVal ParamArray names As String())
        Assert.IsNotNull(bag)
        Dim map As New Dictionary(Of String, NativeType)
        For Each curSym As NativeSymbol In bag.FindAllReachableNativeSymbols()
            Dim cur = TryCast(curSym, NativeType)
            If cur IsNot Nothing Then
                Dim nt = TryCast(cur, NativeType)
                If nt IsNot Nothing Then
                    map.Add(cur.DisplayName, nt)
                End If
            End If
        Next

        Assert.AreEqual(names.Length, map.Count)
        For Each name As String In names
            Assert.IsTrue(map.ContainsKey(name), "NativeType with name " & name & " not reachable")
        Next

    End Sub

    Private Function Print(ByVal ns As NativeSymbol) As String
        If ns Is Nothing Then
            Return "<Nothing>"
        End If

        Dim str As String = ns.Name
        For Each child As NativeSymbol In ns.GetChildren()
            str &= "(" & Print(child) & ")"
        Next

        Return str
    End Function

    Private Sub VerifyTree(ByVal ns As NativeSymbol, ByVal str As String)
        Dim realStr As String = Print(ns)
        Assert.AreEqual(str, realStr)
    End Sub

    ''' <summary>
    ''' simple test with no children
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub Iterate1()
        Dim nt1 As New NativeStruct()
        nt1.Name = "s1"
        VerifyReachable(nt1, "s1")
    End Sub

    <TestMethod()> _
    Public Sub Iterate2()
        Dim nt1 As New NativeStruct()
        nt1.Name = "s1"
        nt1.Members.Add(New NativeMember("f", New NativeBuiltinType(BuiltinType.NativeInt32)))
        VerifyReachable(nt1, "s1", "int")
    End Sub


    ''' <summary>
    ''' Simple test with a couple of structs
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub Iterate3()
        Dim nt1 As New NativeStruct()
        nt1.Name = "s1"

        Dim nt2 As New NativeStruct()
        nt2.Name = "s2"

        Dim bag As New NativeSymbolBag()
        bag.AddDefinedType(nt1)
        bag.AddDefinedType(nt2)
        VerifyReachable(bag, "s1", "s2")
    End Sub

    ''' <summary>
    ''' Test a simple proxy type
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub Iterate4()
        Dim nt1 As New NativeTypeDef("td1")
        Dim nt2 As New NativeNamedType("n1")
        nt1.RealType = nt2
        VerifyReachable(nt1, "td1", "n1")
    End Sub

    ''' <summary>
    ''' Proxy within a container
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub Iterate5()
        Dim nt1 As New NativeStruct("s1")
        Dim nt2 As New NativeTypeDef("td1")
        Dim nt3 As New NativeNamedType("n1")
        nt2.RealType = nt3
        nt1.Members.Add(New NativeMember("foo", nt2))
        VerifyReachable(nt1, "s1", "td1", "n1")
    End Sub

    ''' <summary>
    ''' Play around with structs
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub Child1()
        Dim s1 As New NativeStruct("s1")
        VerifyTree(s1, "s1")
        s1.Members.Add(New NativeMember("m1", New NativeBuiltinType(BuiltinType.NativeChar)))
        VerifyTree(s1, "s1(m1(char))")
        s1.Members.Add(New NativeMember("m2", New NativeBuiltinType(BuiltinType.NativeByte)))
        VerifyTree(s1, "s1(m1(char))(m2(byte))")
    End Sub

    ''' <summary>
    ''' Replace the children of a struct
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub Child2()
        Dim s1 As New NativeStruct("s1")
        s1.Members.Add(New NativeMember("m1", New NativeBuiltinType(BuiltinType.NativeByte)))
        s1.ReplaceChild(s1.Members(0), New NativeMember("m2", New NativeBuiltinType(BuiltinType.NativeDouble)))
        VerifyTree(s1, "s1(m2(double))")
    End Sub

    ''' <summary>
    ''' Children of an enumeration
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub Child3()
        Dim e1 As New NativeEnum("e1")
        e1.Values.Add(New NativeEnumValue("n1", "v1"))
        VerifyTree(e1, "e1(n1(Value(v1)))")
        e1.Values.Add(New NativeEnumValue("n2", "v2"))
        VerifyTree(e1, "e1(n1(Value(v1)))(n2(Value(v2)))")

    End Sub

    ''' <summary>
    ''' adding a member to an enum shouldn't be part of the enumeration
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub Child4()
        Dim e1 As New NativeEnum("e1")
        e1.Members.Add(New NativeMember("m1", New NativeBuiltinType(BuiltinType.NativeByte)))
        VerifyTree(e1, "e1")
    End Sub

    ''' <summary>
    ''' Verify an enum replace 
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub Child5()
        Dim e1 As New NativeEnum("e1")
        e1.Values.Add(New NativeEnumValue("n1", "v1"))
        e1.ReplaceChild(e1.Values(0), New NativeEnumValue("n2", "v2"))
        VerifyTree(e1, "e1(n2(Value(v2)))")
    End Sub

    ''' <summary>
    ''' Verify a proc
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub Child6()
        Dim proc As New NativeProcedure("proc")
        proc.Signature.ReturnType = New NativeBuiltinType(BuiltinType.NativeByte)
        VerifyTree(proc, "proc(Sig(byte)(Sal))")
        proc.Signature.Parameters.Add(New NativeParameter("p1", New NativeBuiltinType(BuiltinType.NativeChar)))
        VerifyTree(proc, "proc(Sig(byte)(Sal)(p1(char)(Sal)))")
    End Sub

    ''' <summary>
    ''' Replace the parameters of a proc
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub Child7()
        Dim proc As New NativeProcedure("proc")
        proc.Signature.ReturnType = New NativeBuiltinType(BuiltinType.NativeByte)
        proc.Signature.Parameters.Add(New NativeParameter("p1", New NativeBuiltinType(BuiltinType.NativeChar)))
        proc.Signature.ReplaceChild(proc.Signature.ReturnType, New NativeBuiltinType(BuiltinType.NativeFloat))
        VerifyTree(proc, "proc(Sig(float)(Sal)(p1(char)(Sal)))")
        proc.Signature.ReplaceChild(proc.Signature.Parameters(0), New NativeParameter("p2", New NativeBuiltinType(BuiltinType.NativeChar)))
        VerifyTree(proc, "proc(Sig(float)(Sal)(p2(char)(Sal)))")
    End Sub

End Class

<TestClass()> _
Public Class NativeBuiltinTypeTest

    <TestMethod()> _
    Public Sub TestAll()
        For Each bt As BuiltinType In System.Enum.GetValues(GetType(BuiltinType))
            If bt <> BuiltinType.NativeUnknown Then
                Dim nt As New NativeBuiltinType(bt)
                Assert.AreEqual(nt.Name, NativeBuiltinType.BuiltinTypeToName(bt))
                Assert.AreEqual(bt, nt.BuiltinType)
                Assert.IsNotNull(nt.ManagedType)
                Assert.AreNotEqual(0, CInt(nt.UnmanagedType))
            End If
        Next
    End Sub

    ''' <summary>
    ''' Unknown type is used to handle situations where we just don't know what's going
    ''' on so we add the unknown type.  Typically meant for use by third parties
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub Unknown1()
        Dim nt As New NativeBuiltinType("foo")
        Assert.AreEqual(BuiltinType.NativeUnknown, nt.BuiltinType)
        Assert.AreEqual("unknown", nt.Name)
        Assert.AreEqual("unknown", nt.DisplayName)
    End Sub

    <TestMethod()> _
    Public Sub EnsureTypeKeywordToBuiltin()
        For Each cur As TokenType In EnumUtil.GetAllValues(Of TokenType)()
            If TokenHelper.IsTypeKeyword(cur) Then
                Dim bt As NativeBuiltinType = Nothing
                Assert.IsTrue(NativeBuiltinType.TryConvertToBuiltinType(cur, bt))
            End If
        Next
    End Sub


End Class

<TestClass()> _
Public Class NativeProxyTypeTest

    ''' <summary>
    ''' Basic typedef cases
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub Dig1()
        Dim td As New NativeTypeDef("foo")
        td.RealType = New NativeBuiltinType(BuiltinType.NativeByte)
        Assert.AreSame(td.RealType, td.DigThroughTypedefAndNamedTypes())

        Dim outerTd As New NativeTypeDef("bar")
        outerTd.RealType = td
        Assert.AreSame(td.RealType, outerTd.DigThroughTypedefAndNamedTypes())
    End Sub

    ''' <summary>
    ''' Simple tests with named types
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub Dig2()
        Dim named As New NativeNamedType("foo")
        named.RealType = New NativeBuiltinType(BuiltinType.NativeByte)
        Assert.AreSame(named.RealType, named.DigThroughTypedefAndNamedTypes())

        Dim outerNamed As New NativeNamedType("bar")
        outerNamed.RealType = named
        Assert.AreSame(named.RealType, outerNamed.DigThroughTypedefAndNamedTypes())
    End Sub

    ''' <summary>
    ''' Hit some null blocks
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub Dig3()
        Dim named As New NativeNamedType("foo")
        Assert.IsNull(named.DigThroughTypedefAndNamedTypes())
    End Sub

    ''' <summary>
    ''' Don't dig through pointers
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub Dig4()
        Dim pt As New NativePointer(BuiltinType.NativeByte)
        Assert.AreSame(pt, pt.DigThroughTypedefAndNamedTypes())

        Dim td As New NativeTypeDef("foo", pt)
        Assert.AreSame(pt, td.DigThroughTypedefAndNamedTypes())
    End Sub

    ''' <summary>
    ''' Dig and search
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub Dig5()
        Dim pt1 As New NativePointer(New NativeTypeDef("foo", BuiltinType.NativeFloat))
        Assert.AreEqual(NativeSymbolKind.BuiltinType, pt1.RealType.DigThroughTypedefAndNamedTypes().Kind)
        Assert.AreEqual(NativeSymbolKind.TypedefType, pt1.RealType.DigThroughTypedefAndNamedTypesFor("foo").Kind)
        Assert.IsNull(pt1.RealType.DigThroughTypedefAndNamedTypesFor("bar"))
    End Sub

    ''' <summary>
    ''' Dig and search again
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub Dig6()
        Dim named As New NativeNamedType("bar", New NativeTypeDef("td1", BuiltinType.NativeFloat))
        Dim td As New NativeTypeDef("foo", named)

        Assert.AreEqual(NativeSymbolKind.TypedefType, td.DigThroughTypedefAndNamedTypesFor("foo").Kind)
        Assert.AreSame(td, td.DigThroughTypedefAndNamedTypesFor("foo"))
        Assert.AreEqual(NativeSymbolKind.BuiltinType, td.DigThroughTypedefAndNamedTypes().Kind)
        Assert.AreEqual(NativeSymbolKind.NamedType, td.DigThroughTypedefAndNamedTypesFor("bar").Kind)

        Dim named2 As New NativeNamedType("named2", td)
        Assert.AreEqual(NativeSymbolKind.TypedefType, named2.DigThroughNamedTypesFor("foo").Kind)
        Assert.IsNull(named2.DigThroughNamedTypesFor("bar"))
    End Sub

    ''' <summary>
    ''' Parameters should have sal attributes
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub Sal1()
        Dim param As New NativeParameter()
        Assert.IsNotNull(param.SalAttribute)
    End Sub

    ''' <summary>
    ''' The return type should have a sal attribute by default
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub Sal2()
        Dim proc As New NativeProcedure()
        Assert.IsNotNull(proc.Signature.ReturnTypeSalAttribute)
    End Sub

    ''' <summary>
    ''' Make sure each sal entry has a directive
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub Sal3()
        For Each e As SalEntryType In System.Enum.GetValues(GetType(SalEntryType))
            Assert.IsFalse(String.IsNullOrEmpty(NativeSalEntry.GetDirectiveForEntry(SalEntryType.ElemReadableTo)))
        Next
    End Sub
End Class

<TestClass()> _
Public Class NativeParameterTest

    <TestMethod()> _
    Public Sub Pre()
        Dim param As New NativeParameter()
        Assert.IsNotNull(param.Name)
    End Sub

    ''' <summary>
    ''' To be resolved we only need a type.  Function pointer parameters don't have to have names
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub Resolved()
        Dim param As New NativeParameter()
        Assert.IsFalse(param.IsImmediateResolved)
        param.NativeType = New NativeBuiltinType(BuiltinType.NativeByte)
        Assert.IsTrue(param.IsImmediateResolved)
        param.Name = "foo"
        Assert.IsTrue(param.IsImmediateResolved)
    End Sub
End Class

<TestClass()> _
Public Class NativeProcedureTest

    <TestMethod()> _
    Public Sub Pre()
        Dim proc As New NativeProcedure()
        Assert.IsNotNull(proc.Signature)
    End Sub

End Class

<TestClass()> _
Public Class NativeFunctionPointerTest

    <TestMethod()> _
    Public Sub Pre()
        Dim ptr As New NativeFunctionPointer("foo")
        Assert.IsNotNull(ptr.Signature)
    End Sub

End Class

<TestClass()> _
Public Class NativeValueExpressionTest

    <TestMethod()> _
    Public Sub Value1()
        Dim expr As New NativeValueExpression("1+1")
        Assert.AreEqual(2, expr.Values.Count)
        Assert.AreEqual(NativeValueKind.Number, expr.Values(0).ValueKind)
        Assert.AreEqual("1", expr.Values(0).DisplayValue)
        Assert.AreEqual(1, expr.Values(0).Value)
    End Sub

    <TestMethod()> _
    Public Sub Value2()
        Dim expr As New NativeValueExpression("FOO+1")
        Assert.AreEqual(2, expr.Values.Count)
        Assert.AreEqual("FOO", expr.Values(0).DisplayValue)
        Assert.AreEqual("FOO", expr.Values(0).Name)
        Assert.IsNull(expr.Values(0).SymbolValue)
    End Sub

    <TestMethod()> _
    Public Sub Value3()
        Dim expr As New NativeValueExpression("FOO+BAR")
        Assert.AreEqual(2, expr.Values.Count)
        Assert.AreEqual("FOO", expr.Values(0).DisplayValue)
        Assert.AreEqual("BAR", expr.Values(1).DisplayValue)
    End Sub

    <TestMethod()> _
    Public Sub Value4()
        Dim expr As New NativeValueExpression("""bar""+1")
        Assert.AreEqual(2, expr.Values.Count)
        Assert.AreEqual(NativeValueKind.String, expr.Values(0).ValueKind)
        Assert.AreEqual("bar", expr.Values(0).DisplayValue)
    End Sub

    ''' <summary>
    ''' Test the parsing of cast operations
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub Value5()
        Dim expr As New NativeValueExpression("(DWORD)5")
        Assert.AreEqual(2, expr.Values.Count)

        Dim val As NativeValue = expr.Values(0)
        Assert.AreEqual(NativeValueKind.SymbolType, val.ValueKind)
        Assert.AreEqual("DWORD", val.DisplayValue)

        val = expr.Values(1)
        Assert.AreEqual(NativeValueKind.Number, val.ValueKind)
        Assert.AreEqual(5, val.Value)
    End Sub

    <TestMethod()> _
    Public Sub Value6()
        Dim expr As New NativeValueExpression("(DWORD)(5+6)")
        Assert.AreEqual(3, expr.Values.Count)

        Dim val As NativeValue = expr.Values(0)
        Assert.AreEqual(NativeValueKind.SymbolType, val.ValueKind)
        Assert.AreEqual("DWORD", val.DisplayValue)

        val = expr.Values(1)
        Assert.AreEqual(NativeValueKind.Number, val.ValueKind)
        Assert.AreEqual(5, val.Value)

        val = expr.Values(2)
        Assert.AreEqual(NativeValueKind.Number, val.ValueKind)
        Assert.AreEqual(6, val.Value)
    End Sub

    ''' <summary>
    ''' Make sure than bad value expressions are marked as resolvable
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub BadValue1()
        Dim expr As New NativeValueExpression("&&&")
        Assert.IsTrue(expr.IsImmediateResolved)
        Assert.IsFalse(expr.IsParsable)
    End Sub

    ''' <summary>
    ''' Reseting the value should cause a re-parse 
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub BadValue2()
        Dim expr As New NativeValueExpression("&&&")
        Assert.IsTrue(expr.IsImmediateResolved)
        Assert.IsFalse(expr.IsParsable)
        Assert.AreEqual(0, expr.Values.Count)
        expr.Expression = "1+1"
        Assert.IsTrue(expr.IsImmediateResolved)
        Assert.IsTrue(expr.IsParsable)
        Assert.AreEqual(2, expr.Values.Count)
    End Sub

End Class

<TestClass()> _
Public Class NativeValueTest

    <TestMethod()> _
    Public Sub Resolve1()
        Dim val As NativeValue = NativeValue.CreateNumber(1)
        Assert.AreEqual(1, val.Value)
        Assert.AreEqual(NativeValueKind.Number, val.ValueKind)
        Assert.IsTrue(val.IsImmediateResolved)
    End Sub

    <TestMethod()> _
    Public Sub Resolve2()
        Dim val As NativeValue = NativeValue.CreateString("foo")
        Assert.AreEqual("foo", val.Value)
        Assert.AreEqual(NativeValueKind.String, val.ValueKind)
        Assert.IsTrue(val.IsImmediateResolved)
    End Sub

    <TestMethod()> _
    Public Sub Resolve3()
        Dim val As NativeValue = NativeValue.CreateSymbolType("foo")
        Assert.AreEqual("foo", val.Name)
        Assert.AreEqual(NativeValueKind.SymbolType, val.ValueKind)
        Assert.IsTrue(val.IsImmediateResolved)
        val.Value = New NativeBuiltinType(BuiltinType.NativeByte)
        Assert.IsTrue(val.IsImmediateResolved)
    End Sub

    <TestMethod()> _
    Public Sub Resolve4()
        Dim val As NativeValue = NativeValue.CreateSymbolValue("bar")
        Assert.AreEqual("bar", val.Name)
        Assert.AreEqual(NativeValueKind.SymbolValue, val.ValueKind)
        Assert.IsTrue(val.IsImmediateResolved)
        val.Value = New NativeBuiltinType(BuiltinType.NativeByte)
        Assert.IsTrue(val.IsImmediateResolved)
    End Sub

    <TestMethod()> _
    Public Sub Resolve5()
        Dim val As NativeValue = NativeValue.CreateSymbolValue("foo", New NativeBuiltinType(BuiltinType.NativeBoolean))
        Assert.AreEqual("foo", val.Name)
        Assert.AreEqual(NativeValueKind.SymbolValue, val.ValueKind)
        Assert.IsNotNull(val.SymbolValue)
        Assert.IsNull(val.SymbolType)
        Assert.IsTrue(val.IsImmediateResolved)
    End Sub

    <TestMethod()> _
    Public Sub Resolve6()
        Dim val As NativeValue = NativeValue.CreateSymbolType("foo", New NativeBuiltinType(BuiltinType.NativeBoolean))
        Assert.AreEqual("foo", val.Name)
        Assert.AreEqual(NativeValueKind.SymbolType, val.ValueKind)
        Assert.IsNull(val.SymbolValue)
        Assert.IsNotNull(val.SymbolType)
        Assert.IsTrue(val.IsImmediateResolved)
    End Sub

    <TestMethod()> _
    Public Sub Resolve7()
        Dim val As NativeValue = NativeValue.CreateCharacter("c"c)
        Assert.AreEqual("c"c, val.Value)
        Assert.AreEqual(NativeValueKind.Character, val.ValueKind)
        Assert.IsTrue(val.IsImmediateResolved)
    End Sub

    ''' <summary>
    ''' Value should not update the enumeration 
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub Dynamic1()
        Dim val As NativeValue = NativeValue.CreateNumber(1)
        Assert.AreEqual(NativeValueKind.Number, val.ValueKind)
        val.Value = 42
        Assert.AreEqual(NativeValueKind.Number, val.ValueKind)
        Assert.AreEqual(42, val.Value)
    End Sub

    ''' <summary>
    ''' Changing the type should not update the kind
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub Dynamic2()
        Dim val As NativeValue = NativeValue.CreateNumber(42)
        Assert.AreEqual(NativeValueKind.Number, val.ValueKind)
        val.Value = "foo"
        Assert.AreEqual(NativeValueKind.Number, val.ValueKind)
        Assert.AreEqual("foo", val.Value)
    End Sub

    <TestMethod()> _
    Public Sub IsValueResolved1()
        Dim val As NativeValue = NativeValue.CreateBoolean(True)
        Assert.IsTrue(val.IsValueResolved)
        val.Value = Nothing
        Assert.IsFalse(val.IsValueResolved)
    End Sub

    <TestMethod()> _
    Public Sub IsValueResolved2()
        Dim val As NativeValue = NativeValue.CreateCharacter("c"c)
        Assert.IsTrue(val.IsValueResolved)
        val.Value = Nothing
        Assert.IsFalse(val.IsValueResolved)
    End Sub

    <TestMethod()> _
    Public Sub IsValueResolved3()
        Dim val As NativeValue = NativeValue.CreateNumber(42)
        Assert.IsTrue(val.IsValueResolved)
        val.Value = Nothing
        Assert.IsFalse(val.IsValueResolved)
        val.Value = 42
        Assert.IsTrue(val.IsValueResolved)
    End Sub

    <TestMethod()> _
    Public Sub IsValueResolved4()
        Dim val As NativeValue = NativeValue.CreateSymbolType("foo")
        Assert.IsFalse(val.IsValueResolved)
        val.Value = New NativeStruct("foo")
        Assert.IsTrue(val.IsValueResolved)
        val.Value = Nothing
        Assert.IsFalse(val.IsValueResolved)
    End Sub

    <TestMethod()> _
    Public Sub IsValueResolved5()
        Dim val As NativeValue = NativeValue.CreateSymbolValue("foo")
        Assert.IsFalse(val.IsValueResolved)
        val.Value = New NativeStruct("foo")
        Assert.IsTrue(val.IsValueResolved)
        val.Value = Nothing
        Assert.IsFalse(val.IsValueResolved)
    End Sub

End Class


<TestClass()> _
Public Class NativeConstantTest

    <TestMethod()> _
    Public Sub Empty()
        Dim c1 As New NativeConstant("c1")
        Assert.AreEqual(ConstantKind.Macro, c1.ConstantKind)
        Assert.AreEqual("c1", c1.Name)
    End Sub

    <TestMethod()> _
    Public Sub Value1()
        Dim c1 As New NativeConstant("p", "1+2")
        Assert.AreEqual(ConstantKind.Macro, c1.ConstantKind)
        Assert.AreEqual("1+2", c1.Value.Expression)
        Assert.AreEqual("p", c1.Name)
    End Sub

    ''' <summary>
    ''' Make sure that we quote macro method values to ensure that they
    ''' are "resolvable"
    ''' </summary>
    ''' <remarks></remarks>
    <TestMethod()> _
    Public Sub Method1()
        Dim sig As String = "(x) x+1"
        Dim c1 As New NativeConstant("c1", sig, ConstantKind.MacroMethod)
        Assert.AreEqual(ConstantKind.MacroMethod, c1.ConstantKind)
        Assert.AreEqual("""" & sig & """", c1.Value.Expression)
        Assert.AreEqual("c1", c1.Name)
    End Sub
End Class
