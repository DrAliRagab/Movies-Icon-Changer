Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.Reflection
Imports ImageMagick
Imports System.Text

Public Class Form1

    ' Variables.
    Dim _coveroverlay As String = Nothing
    Dim _ratingstar As String = Nothing
    Dim _ratingstarXpoint As Integer = Nothing
    Dim _ratingstarYpoint As Integer = Nothing
    Dim _ratingstarwidth As Integer = Nothing
    Dim _ratingstarheight As Integer = Nothing
    Dim _posterXpoint As Integer = Nothing
    Dim _posterYpoint As Integer = Nothing
    Dim _posterwidth As Integer = Nothing
    Dim _posterheight As Integer = Nothing
    Dim _ratingFont As New Font("Arial", 48)
    Dim _ratingFontcolor As New Color
    Dim _ratingXpoint As Integer = Nothing
    Dim _ratingYpoint As Integer = Nothing

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If (FolderBrowserDialog1.ShowDialog() = DialogResult.OK) Then
            TextBox1.Text = FolderBrowserDialog1.SelectedPath
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click

        'Check Variables
        Dim alltxt As New List(Of Control)
        Dim empty = FindControlRecursive(alltxt, Me, GetType(TextBox)).Where(Function(txt) txt.Text.Length = 0)
        If empty.Any Then
            MessageBox.Show(String.Format("All Fields Are Required" & vbNewLine & "Please fill following textboxes:" & vbNewLine & "- {0}",
                            String.Join(vbNewLine & "- ", empty.Select(Function(txt) txt.AccessibleName))))
            Exit Sub
        End If

        'Check Folders
        If (Not System.IO.Directory.Exists(TextBox1.Text)) Then
            MsgBox("Movies folder doesn't exist", MsgBoxStyle.Information)
            Exit Sub
        End If

        _coveroverlay = TextBox4.Text
        _ratingstar = TextBox5.Text
        _ratingstarXpoint = TextBox6.Text
        _ratingstarYpoint = TextBox7.Text
        _ratingstarwidth = TextBox8.Text
        _ratingstarheight = TextBox9.Text
        _posterXpoint = TextBox10.Text
        _posterYpoint = TextBox11.Text
        _posterwidth = TextBox12.Text
        _posterheight = TextBox13.Text
        _ratingXpoint = TextBox14.Text
        _ratingYpoint = TextBox15.Text

        Panel1.Location = New Point((ClientSize.Width - Panel1.Width) \ 2, (ClientSize.Height - Panel1.Height) \ 2)

        ProgressBar1.Visible = True
        Panel1.Visible = True
        Button4.Visible = False
        Button8.Visible = False

        Dim allctrl As New List(Of Control)
        For Each ctrl As Control In FindControlRecursive(allctrl, Me, GetType(TextBox))
            ctrl.Enabled = False
        Next
        For Each ctrl As Control In FindControlRecursive(allctrl, Me, GetType(Button))
            ctrl.Enabled = False
        Next

        BackgroundWorker1.RunWorkerAsync()

    End Sub

    Public Shared Function ResizeImage(ByVal image As Image, ByVal size As Size, Optional ByVal preserveAspectRatio As Boolean = True) As Image
        Dim newWidth As Integer
        Dim newHeight As Integer
        If preserveAspectRatio Then
            Dim originalWidth As Integer = image.Width
            Dim originalHeight As Integer = image.Height
            Dim percentWidth As Single = CSng(size.Width) / CSng(originalWidth)
            Dim percentHeight As Single = CSng(size.Height) / CSng(originalHeight)
            Dim percent As Single = If(percentHeight < percentWidth, percentHeight, percentWidth)
            newWidth = CInt(originalWidth * percent)
            newHeight = CInt(originalHeight * percent)
        Else
            newWidth = size.Width
            newHeight = size.Height
        End If
        Dim newImage As Image = New Bitmap(newWidth, newHeight)
        Using graphicsHandle As Graphics = Graphics.FromImage(newImage)
            graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic
            graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight)
        End Using
        Return newImage
    End Function

    Public Shared Function FolderIcon(ByVal path As String)
        Dim files As New ArrayList
        Dim directory = path
        Dim dList As New ArrayList

        For Each folder As String In IO.Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly)
            dList.Add(IO.Path.GetFullPath(folder))
        Next

        For z = 0 To dList.Count - 1
            Try
                For Each filename As String In IO.Directory.GetFiles(dList(z), "desktop.ini", IO.SearchOption.AllDirectories)
                    files.Add(IO.Path.GetFullPath(filename))
                Next
            Catch ex As Exception
            End Try
        Next

        For i = 0 To files.Count - 1

            Dim filename As String = files(i).ToString
            Dim fName As String = IO.Path.GetFullPath(filename)
            Dim path1 As String = IO.Path.GetDirectoryName(filename)
            Dim path2 As String
            Dim Newattr As FileAttributes = FileAttributes.Normal
            Dim fStream As New System.IO.FileStream(fName, IO.FileMode.Open)
            Dim sReader As New System.IO.StreamReader(fStream)
            Dim List As New List(Of String)

            Do While sReader.Peek >= 0
                List.Add(sReader.ReadLine)
            Loop

            fStream.Close()
            sReader.Close()

            For x = 0 To List.Count - 1
                Dim line As String = List(x).ToString
                Dim line2 As String = ""

                If line.Contains("IconResource=") Then
                    path2 = line.Replace("IconResource=", "")
                    path2 = path2.Replace(",0", "")

                    If path2.EndsWith(".ico", StringComparison.OrdinalIgnoreCase) Then
                        Try
                            File.Copy(path2, (path1.ToString + "\" + IO.Path.GetFileName(path2)), True)
                            File.SetAttributes((path1.ToString + "\" + IO.Path.GetFileName(path2)), FileAttributes.System)
                            File.SetAttributes((path1.ToString + "\" + IO.Path.GetFileName(path2)), FileAttributes.Hidden)
                        Catch ex As Exception
                        End Try
                    End If

                    File.SetAttributes((path1.ToString + "\" + IO.Path.GetFileName(path2)), FileAttributes.System)
                    File.SetAttributes((path1.ToString + "\" + IO.Path.GetFileName(path2)), FileAttributes.Hidden)
                    File.SetAttributes((path1.ToString), FileAttributes.ReadOnly)
                    File.SetAttributes(fName, Newattr)

                    line2 = "IconResource=" + IO.Path.GetFileName(path2) + ",0"
                    List(x) = line2

                    IO.File.WriteAllLines(fName, List, Encoding.Default)
                    File.SetAttributes(fName, FileAttributes.System)
                    File.SetAttributes(fName, FileAttributes.Hidden)
                End If
            Next
        Next
        Return Nothing
    End Function

    Public Shared Function CreateIcons(ByVal MovieFolder As String, _
                                       ByVal keyword As String, _
                                       ByVal coveroverlay As String, _
                                       ByVal ratingstar As String, _
                                       ByVal ratingstarXpoint As Integer, _
                                       ByVal ratingstarYpoint As Integer, _
                                       ByVal ratingstarwidth As Integer, _
                                       ByVal ratingstarheight As Integer, _
                                       ByVal posterwidth As Integer, _
                                       ByVal posterheight As Integer, _
                                       ByVal posterXpoint As Integer, _
                                       ByVal posterYpoint As Integer, _
                                       ByVal ratingfont As Font, _
                                       ByVal ratingfontcolor As Color, _
                                       ByVal ratingXpoint As Integer, _
                                       ByVal ratingYpoint As Integer)

        Dim directory = MovieFolder
        Dim dList As New ArrayList

        For Each filename As String In IO.Directory.GetFiles(directory, keyword, IO.SearchOption.AllDirectories)
            If Not filename.EndsWith("ico", StringComparison.OrdinalIgnoreCase) Then
                If IsValidImageFile(filename) = True Then
                    dList.Add(IO.Path.GetFullPath(filename))
                End If
            End If

        Next

        For i = 0 To dList.Count - 1

            Dim filename As String = dList(i)
            If Not filename.EndsWith("ico", StringComparison.OrdinalIgnoreCase) Then

                Dim Img1 As Image = Image.FromFile(filename)
                Dim Img2 As Image = Image.FromFile(coveroverlay)
                Dim img3 As Image = Image.FromFile(ratingstar)
                Dim bmp As New Bitmap(947, 947)
                Dim g As Graphics = Graphics.FromImage(bmp)

                Img1 = ResizeImage(Img1, New Size(posterwidth, posterheight), False)
                Img2 = ResizeImage(Img2, New Size(947, 947), False)
                img3 = ResizeImage(img3, New Size(ratingstarwidth, ratingstarheight), False)

                'Get Movie rating
                Dim drawString As [String] = ""
                For Each nfoFile As String In IO.Directory.GetFiles(IO.Path.GetDirectoryName(filename), "*.nfo", IO.SearchOption.TopDirectoryOnly)

                    Dim reader As New StreamReader(nfoFile, System.Text.Encoding.Default)
                    Dim line As String = Nothing
                    Dim lines As Integer = 0

                    While (reader.Peek() <> -1)
                        line = reader.ReadLine()
                        If line.Contains("<rating>") Then
                            Dim input As String = line
                            Dim numbers As System.Text.StringBuilder = New System.Text.StringBuilder()
                            For Each c As Char In input
                                If (Char.IsNumber(c)) Or (c = ".") Then
                                    numbers.Append(c)
                                End If
                            Next
                            'Display the output 
                            drawString = numbers.ToString
                            lines = lines + 1
                        End If
                    End While
                Next

                ' Create font and brush.
                Dim drawFont As New Font("Arial", 48)
                drawFont = ratingfont
                Dim drawBrush As New SolidBrush(ratingfontcolor)

                ' Create rectangle for drawing.
                Dim x As Single = ratingXpoint
                Dim y As Single = ratingYpoint
                Dim width As Single = 947.0F
                Dim height As Single = 947.0F
                Dim drawRect As New RectangleF(x, y, width, height)

                ' Draw rectangle to screen.
                ' Dim blackPen As New Pen(Color.Black)
                ' g.DrawRectangle(blackPen, x, y, width, height)

                ' Set format of string.
                Dim drawFormat As New StringFormat
                drawFormat.Alignment = StringAlignment.Center

                ' Draw Icon.

                If drawString = "" Then
                    g.DrawImage(Img1, posterXpoint, posterYpoint)
                    g.DrawImage(Img2, 0, 0)
                    'g.DrawImage(img3, ratingstarXpoint, ratingstarYpoint)
                    g.DrawString(drawString, drawFont, drawBrush, drawRect, drawFormat)
                Else
                    g.DrawImage(Img1, posterXpoint, posterYpoint)
                    g.DrawImage(Img2, 0, 0)
                    g.DrawImage(img3, ratingstarXpoint, ratingstarYpoint)
                    g.DrawString(drawString, drawFont, drawBrush, drawRect, drawFormat)
                End If

                Using image As New MagickImage(bmp)
                    Dim Iconfile As String
                    Iconfile = IO.Path.GetDirectoryName(filename) + "\" + IO.Path.GetFileNameWithoutExtension(filename) + ".ico"
                    image.Settings.SetDefine(MagickFormat.Icon, "auto-resize", "256,128,64,48,32,16")
                    If IO.File.Exists(Iconfile) Then
                        File.SetAttributes(Iconfile, FileAttributes.Normal)
                    End If
                    image.Write(Iconfile)
                    image.Dispose()
                    bmp.Dispose()
                    g.Dispose()

                End Using

            End If

        Next

        For i = 0 To dList.Count - 1
            Dim filename As String = dList(i)
            Dim FileInfo As New FileInfo(filename)
            Dim desFile As String = FileInfo.DirectoryName + "\" + IO.Path.GetFileNameWithoutExtension(filename) + ".ico"

            Dim iniFile As String = ""
            iniFile = IO.Path.GetDirectoryName(desFile) + "\desktop.ini"

            If IO.File.Exists(iniFile) Then
                File.SetAttributes(iniFile, FileAttributes.Normal)
                File.Delete(iniFile)
            End If
            Dim data As String() = {"[ViewState]", _
                                        "Mode=", _
                                        "Vid=", _
                                        "FolderType=Pictures", _
                                        "[.ShellClassInfo]", _
                                        "IconResource=" + desFile + ",0"}
            IO.File.WriteAllLines(iniFile, data)

            System.Threading.Thread.Sleep(500)

            File.SetAttributes(iniFile, FileAttributes.System)
            File.SetAttributes(iniFile, FileAttributes.Hidden)

            File.SetAttributes(desFile, FileAttributes.System)
            File.SetAttributes(desFile, FileAttributes.Hidden)

        Next

        Return Nothing
    End Function

    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork

        CreateIcons(TextBox1.Text, TextBox2.Text, _coveroverlay, _
                    _ratingstar, _ratingstarXpoint, _ratingstarYpoint, _ratingstarwidth, _ratingstarheight, _
                    _posterwidth, _posterheight, _posterXpoint, _posterYpoint, _
                    _ratingFont, _ratingFontcolor, _ratingXpoint, _ratingYpoint)

        FolderIcon(TextBox1.Text)

        System.Threading.Thread.Sleep(1000)
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        MsgBox("Done", MsgBoxStyle.Information)
        ProgressBar1.Visible = False
        ProgressBar1.Value = 0
        Button4.Visible = True
        Panel1.Visible = False
        TextBox2.ReadOnly = False
        Button8.Visible = True

        Dim allctrl As New List(Of Control)
        For Each ctrl As Control In FindControlRecursive(allctrl, Me, GetType(TextBox))
            ctrl.Enabled = True
        Next
        For Each ctrl As Control In FindControlRecursive(allctrl, Me, GetType(Button))
            ctrl.Enabled = True
        Next

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        If (OpenFileDialog1.ShowDialog() = DialogResult.OK) Then
            TextBox4.Text = OpenFileDialog1.FileName
        End If

        Button7.PerformClick()

    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Me.Height = 153

        AssignValidation(Me.TextBox6, ValidationType.Only_Numbers)
        AssignValidation(Me.TextBox7, ValidationType.Only_Numbers)
        AssignValidation(Me.TextBox8, ValidationType.Only_Numbers)
        AssignValidation(Me.TextBox9, ValidationType.Only_Numbers)
        AssignValidation(Me.TextBox10, ValidationType.Only_Numbers)
        AssignValidation(Me.TextBox11, ValidationType.Only_Numbers)
        AssignValidation(Me.TextBox12, ValidationType.Only_Numbers)
        AssignValidation(Me.TextBox13, ValidationType.Only_Numbers)
        AssignValidation(Me.TextBox14, ValidationType.Only_Numbers)
        AssignValidation(Me.TextBox15, ValidationType.Only_Numbers)

        TextBox4.Text = System.IO.Directory.GetCurrentDirectory() + "\dvdbox.png"
        TextBox5.Text = System.IO.Directory.GetCurrentDirectory() + "\Plain.PNG"

        TextBox6.Text = 740
        TextBox7.Text = 740
        TextBox8.Text = 200
        TextBox9.Text = 200

        TextBox10.Text = 198
        TextBox11.Text = 30
        TextBox12.Text = 624
        TextBox13.Text = 880

        TextBox14.Text = 366
        TextBox15.Text = 815

        _ratingFont = FontDialog1.Font
        _ratingFontcolor = FontDialog1.Color

        Button7.PerformClick()

    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click

        If (OpenFileDialog1.ShowDialog() = DialogResult.OK) Then
            TextBox5.Text = OpenFileDialog1.FileName
        End If

        Button7.PerformClick()

    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        If FontDialog1.ShowDialog <> Windows.Forms.DialogResult.Cancel Then
            _ratingFont = FontDialog1.Font
            _ratingFontcolor = FontDialog1.Color

        End If

        Button7.PerformClick()

    End Sub

    Public Shared Function Preview(ByVal filename As String, _
                                   ByVal coveroverlay As String, _
                                       ByVal ratingstar As String, _
                                       ByVal ratingstarXpoint As Integer, _
                                       ByVal ratingstarYpoint As Integer, _
                                       ByVal ratingstarwidth As Integer, _
                                       ByVal ratingstarheight As Integer, _
                                       ByVal posterwidth As Integer, _
                                       ByVal posterheight As Integer, _
                                       ByVal posterXpoint As Integer, _
                                       ByVal posterYpoint As Integer, _
                                       ByVal ratingfont As Font, _
                                       ByVal ratingfontcolor As Color, _
                                       ByVal ratingXpoint As Integer, _
                                       ByVal ratingYpoint As Integer, _
                                       ByVal picturebox As PictureBox)

        Dim Img1 As Image = Image.FromFile(filename)
        Dim Img2 As Image = Image.FromFile(coveroverlay)
        Dim img3 As Image = Image.FromFile(ratingstar)

        Dim bmp As New Bitmap(947, 947)
        Dim g As Graphics = Graphics.FromImage(bmp)

        Img1 = ResizeImage(Img1, New Size(posterwidth, posterheight), False)
        Img2 = ResizeImage(Img2, New Size(947, 947), False)
        img3 = ResizeImage(img3, New Size(ratingstarwidth, ratingstarheight), False)

        g.DrawImage(Img1, posterXpoint, posterYpoint)
        g.DrawImage(Img2, 0, 0)
        g.DrawImage(img3, ratingstarXpoint, ratingstarYpoint)

        'Get Movie rating
        Dim drawString As [String] = "7.5"

        ' Create font and brush.
        Dim drawFont As New Font("Arial", 48)
        drawFont = ratingfont
        Dim drawBrush As New SolidBrush(ratingfontcolor)

        ' Create rectangle for drawing.
        Dim x As Single = ratingXpoint
        Dim y As Single = ratingYpoint
        Dim width As Single = 947.0F
        Dim height As Single = 947.0F
        Dim drawRect As New RectangleF(x, y, width, height)

        ' Draw rectangle to screen.
        ' Dim blackPen As New Pen(Color.Black)
        ' g.DrawRectangle(blackPen, x, y, width, height)

        ' Set format of string.
        Dim drawFormat As New StringFormat
        drawFormat.Alignment = StringAlignment.Center

        ' Draw string to screen.
        g.DrawString(drawString, drawFont, drawBrush, drawRect, drawFormat)
        bmp = ResizeImage(bmp, New Size(300, 300))

        picturebox.Image = bmp
        Return Nothing
    End Function

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click

        Dim alltxt As New List(Of Control)
        Dim empty = FindControlRecursive(alltxt, Me, GetType(TextBox)).Where(Function(txt) txt.Text.Length = 0 And Not txt.Name = "TextBox1")
        If empty.Any Then
            MessageBox.Show(String.Format("All Fields Are Required" & vbNewLine & "Please fill following textboxes:" & vbNewLine & "- {0}",
                            String.Join(vbNewLine & "- ", empty.Select(Function(txt) txt.AccessibleName))))
            Exit Sub
        End If

        _coveroverlay = TextBox4.Text
        _ratingstar = TextBox5.Text
        _ratingstarXpoint = TextBox6.Text
        _ratingstarYpoint = TextBox7.Text
        _ratingstarwidth = TextBox8.Text
        _ratingstarheight = TextBox9.Text
        _posterXpoint = TextBox10.Text
        _posterYpoint = TextBox11.Text
        _posterwidth = TextBox12.Text
        _posterheight = TextBox13.Text
        _ratingXpoint = TextBox14.Text
        _ratingYpoint = TextBox15.Text

        Dim testPoster As String = System.IO.Directory.GetCurrentDirectory() + "\Sample-poster.jpg"

        Preview(testPoster, _coveroverlay, _
                    _ratingstar, _ratingstarXpoint, _ratingstarYpoint, _ratingstarwidth, _ratingstarheight, _
                    _posterwidth, _posterheight, _posterXpoint, _posterYpoint, _
                    _ratingFont, _ratingFontcolor, _ratingXpoint, _ratingYpoint, PictureBox1)
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As System.Windows.Forms.Message, _
                                           ByVal keyData As System.Windows.Forms.Keys) _
                                           As Boolean

        If msg.WParam.ToInt32() = CInt(Keys.Enter) Then
            Button7.PerformClick()
            Return True
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click

        If Button8.Text = "Show settings" Then
            Button8.Text = "Hide settings"
            Panel2.Visible = True
            Me.Height = 505

        ElseIf Button8.Text = "Hide settings" Then
            Button8.Text = "Show settings"
            Panel2.Visible = False
            Me.Height = 153

        End If

    End Sub

    Public Shared Function FindControlRecursive(ByVal list As List(Of Control), ByVal parent As Control, ByVal ctrlType As System.Type) As List(Of Control)
        If parent Is Nothing Then Return list
        If parent.GetType Is ctrlType Then
            list.Add(parent)
        End If
        For Each child As Control In parent.Controls
            FindControlRecursive(list, child, ctrlType)
        Next
        Return list
    End Function

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        System.Diagnostics.Process.Start("https://github.com/DrAliRagab/Movies-Icon-Changer")
    End Sub

    Public Shared Function IsValidImageFile(imageFile As String) As Boolean
        Try
            ' the using is important to avoid stressing the garbage collector
            Using test = System.Drawing.Image.FromFile(imageFile)
                ' image has loaded and so is fine
                Return True
            End Using
        Catch
            ' technically some exceptions may not indicate a corrupt image, but this is unlikely to be an issue
            Return False
        End Try
    End Function

End Class
