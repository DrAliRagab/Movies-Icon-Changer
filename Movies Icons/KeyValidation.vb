Imports System.Text.RegularExpressions
Module Module1
    Public Enum ValidationType
        Only_Numbers = 1
        Only_Characters = 2
        Not_Null = 3
        Only_Email = 4
        Phone_Number = 5
    End Enum
    Public Sub AssignValidation(ByRef CTRL As Windows.Forms.TextBox, ByVal Validation_Type As ValidationType)
        Dim txt As Windows.Forms.TextBox = CTRL
        Select Case Validation_Type
            Case ValidationType.Only_Numbers
                AddHandler txt.KeyPress, AddressOf number_Leave
            Case ValidationType.Only_Characters
                AddHandler txt.KeyPress, AddressOf OCHAR_Leave
            Case ValidationType.Not_Null
                AddHandler txt.Leave, AddressOf NotNull_Leave
            Case ValidationType.Only_Email
                AddHandler txt.Leave, AddressOf Email_Leave
            Case ValidationType.Phone_Number
                AddHandler txt.KeyPress, AddressOf Phonenumber_Leave
        End Select
    End Sub
    Public Sub number_Leave(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        Dim numbers As Windows.Forms.TextBox = sender
        If InStr("1234567890.", e.KeyChar) = 0 And Asc(e.KeyChar) <> 8 Or (e.KeyChar = "." And InStr(numbers.Text, ".") > 0) Then
            e.KeyChar = Chr(0)
            e.Handled = True
        End If
    End Sub
    Public Sub Phonenumber_Leave(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        Dim numbers As Windows.Forms.TextBox = sender
        If InStr("1234567890.()-+ ", e.KeyChar) = 0 And Asc(e.KeyChar) <> 8 Or (e.KeyChar = "." And InStr(numbers.Text, ".") > 0) Then
            e.KeyChar = Chr(0)
            e.Handled = True
        End If
    End Sub
    Public Sub OCHAR_Leave(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        If InStr("1234567890!@#$%^&*()_+=-", e.KeyChar) > 0 Then
            e.KeyChar = Chr(0)
            e.Handled = True
        End If
    End Sub
    Public Sub NotNull_Leave(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim No As Windows.Forms.TextBox = sender
        If No.Text.Trim = "" Then
            MsgBox("This field Must be filled!")
            No.Focus()
        End If
    End Sub
    Public Sub Email_Leave(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim Email As Windows.Forms.TextBox = sender
        If Email.Text <> "" Then
            Dim rex As Match = Regex.Match(Trim(Email.Text), "^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,3})$", RegexOptions.IgnoreCase)
            If rex.Success = False Then
                MessageBox.Show("Please Enter a valid Email Address", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Email.BackColor = Color.Red
                Email.Focus()
                Exit Sub
            Else
                Email.BackColor = Color.White
            End If
        End If
    End Sub
End Module