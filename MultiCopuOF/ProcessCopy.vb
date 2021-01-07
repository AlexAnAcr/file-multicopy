Public Class ProcessCopy
    Dim data(6) As String, file_select As String = "", progress As Short = 0, endresult As Short = 0, canceled As Boolean = False, adv_mode As Boolean = False, info(5) As UInteger, time As Long
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If adv_mode = False Then
            adv_mode = True
            Panel1.Visible = True
            Button2.Text = "/\"
            Me.Height = 248
        Else
            adv_mode = False
            Panel1.Visible = False
            Button2.Text = "\/"
            Me.Height = 164
        End If
    End Sub
    Private Sub ProcessCopy_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        data(0) = Main.TextBox1.Text
        data(1) = Main.TextBox2.Text
        data(2) = My.Computer.FileSystem.GetFileInfo(Main.TextBox2.Text).DirectoryName
        data(3) = Main.CheckBox2.Checked
        data(4) = Main.CheckBox3.Checked
        data(5) = Main.CheckBox1.Checked
        data(6) = Main.CheckBox4.Checked
        file_select = ""
        endresult = 0
        progress = 0
        canceled = False
        adv_mode = False
        Panel1.Visible = False
        Button2.Text = "\/"
        Me.Height = 164
        info(0) = 0
        info(1) = 0
        info(2) = 0
        info(3) = 0
        info(4) = 0
        info(5) = Math.Round(My.Computer.FileSystem.GetFileInfo(data(0)).Length / 1024)
        ListBox1.Items.Item(0) = "Осталось времени ------- : --"
        ListBox1.Items.Item(1) = "Прошло времени --------- : --"
        ListBox1.Items.Item(2) = "Время всей операции ---- : --"
        ListBox1.Items.Item(3) = "Заменено файлов -------- : --"
        ListBox1.Items.Item(4) = "Осталось файлов -------- : --"
        ListBox1.Items.Item(5) = "Кол-во файлов ---------- : --"
        If info(5) / 1024 / 1024 >= 1 Then
            ListBox1.Items.Item(6) = "Размер заменяющего файла : " & Format(info(5) / 1024 / 1024, "#.##") & "ГБ"
        ElseIf info(5) / 1024 >= 1 Then
            ListBox1.Items.Item(6) = "Размер заменяющего файла : " & Format(info(5) / 1024, "#.##") & "МБ"
        Else
            ListBox1.Items.Item(6) = "Размер заменяющего файла : " & info(5) & "KБ"
        End If
        ListBox1.Items.Item(7) = "Конечный размер файлов - : --"
        Timer_Main.Start()
        BackgroundWork.RunWorkerAsync()
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim result As MsgBoxResult = MsgBox("Вы уверены, что хотите отменить замену файлов? Уже выполнено: " & progress & "%", MsgBoxStyle.YesNo, "Сообщение")
        If result = MsgBoxResult.Yes Then
            Button1.Enabled = False
            Label1.Text = "Отмена..."
            BackgroundWork.CancelAsync()
            canceled = True
        End If
    End Sub
    Private Sub Timer_Main_Tick(sender As Object, e As EventArgs) Handles Timer_Main.Tick
        If canceled = False Then
            ProgressBar1.Value = progress
            Label1.Text = "Выполнено: " & progress & "%"
            TextBox1.Text = file_select
            If Math.IEEERemainder(time, 10) = 0 Then
                info(3) += 1
                If info(2) = 1 Then
                    info(2) = 0
                    info(4) = Math.Round((info(3) / info(1)) * (info(0) - info(1))) + info(3)
                    ListBox1.Items.Item(3) = "Заменено файлов -------- : " & info(1)
                    ListBox1.Items.Item(4) = "Осталось файлов -------- : " & info(0) - info(1)
                    ListBox1.Items.Item(5) = "Кол-во файлов ---------- : " & info(0)
                    If info(5) * info(0) / 1024 / 1024 >= 1 Then
                        ListBox1.Items.Item(7) = "Конечный размер файлов - : " & Format(info(5) * info(0) / 1024 / 1024, "#.##") & "ГБ"
                    ElseIf info(5) * info(0) / 1024 >= 1 Then
                        ListBox1.Items.Item(7) = "Конечный размер файлов - : " & Format(info(5) * info(0) / 1024, "#.##") & "МБ"
                    Else
                        ListBox1.Items.Item(7) = "Конечный размер файлов - : " & info(5) * info(0) & "KБ"
                    End If
                End If
                If info(4) >= info(3) Then
                    ListBox1.Items.Item(0) = "Осталось времени ------- : " & Get_dhms(info(4) - info(3))
                End If
                ListBox1.Items.Item(1) = "Прошло времени --------- : " & Get_dhms(info(3))
                ListBox1.Items.Item(2) = "Время всей операции ---- : " & Get_dhms(info(4))
            End If
            time += 1
            If endresult = 1 Then
                Timer_Main.Stop()
                MsgBox("Замена файлов успешно завершена!",, "Сообщение")
                Me.Close()
            ElseIf endresult = 2 Then
                Timer_Main.Stop()
                MsgBox("Произошла ошибка! Возможно, ваш диск переполнен, папка защищена от записи.",, "Ошибка")
                Me.Close()
            End If
        ElseIf canceled = True Then
            If BackgroundWork.IsBusy = False Then
                Label1.Text = "Отменено!"
                Timer_Main.Stop()
                MsgBox("Замена файлов отменена!",, "Сообщение")
                Me.Close()
            End If
        End If
    End Sub
    Private Function Get_dhms(seconds As String)
        Dim left As Integer = seconds
        Dim days_left As Integer = Int(((left / 24) / 60) / 60)
        left -= days_left * 86400
        Dim hours_left As Integer = Int((left / 60) / 60)
        left -= hours_left * 3600
        Dim minutes_left As Integer = Int(left / 60)
        left -= minutes_left * 60
        Return days_left & " д. " & hours_left & " ч. " & minutes_left & " м. " & left & " с. "
    End Function
    Private Sub BackgroundWork_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWork.DoWork
        Try
            Dim files() As String, exten As String = My.Computer.FileSystem.GetFileInfo(data(0)).Extension
            If data(3) = True Then
                If data(5) = True Then
                    files = IO.Directory.GetFiles(data(2), "*" & My.Computer.FileSystem.GetFileInfo(data(1)).Extension, IO.SearchOption.AllDirectories)
                Else
                    files = IO.Directory.GetFiles(data(2), "*" & My.Computer.FileSystem.GetFileInfo(data(1)).Extension, IO.SearchOption.TopDirectoryOnly)
                End If
            Else
                If data(5) = True Then
                    files = IO.Directory.GetFiles(data(2), "*", IO.SearchOption.AllDirectories)
                Else
                    files = IO.Directory.GetFiles(data(2), "*", IO.SearchOption.TopDirectoryOnly)
                End If
            End If
            If data(4) = True Then
                Dim files1(-1) As String, name As String = Mid(data(1), data(1).LastIndexOf("\") + 2, data(1).LastIndexOf(".") - data(1).LastIndexOf("\") - 1)
                For i As Integer = 0 To files.Length - 1
                    If Mid(files(i), files(i).LastIndexOf("\") + 2, files(i).LastIndexOf(".") - files(i).LastIndexOf("\") - 1) = name Then
                        Array.Resize(files1, files1.Length + 1)
                        files1(files1.Length - 1) = files(i)
                    End If
                Next
                Array.Resize(files, files1.Length)
                For i As Integer = 0 To files1.Length - 1
                    files(i) = files1(i)
                Next
            End If
            info(0) = files.Length
            progress = 2
            Dim prog(1) As Integer
            If files.Length < 98 Then
                prog(0) = 0
                prog(1) = Math.Round(98 / files.Length)
            Else
                prog(0) = 1
                prog(1) = Math.Round(files.Length / 98)
            End If
            For i As Integer = 0 To files.Length - 1
                file_select = files(i)
                If data(6) = True Then
                    My.Computer.FileSystem.CopyFile(data(0), files(i), True)
                Else
                    My.Computer.FileSystem.DeleteFile(files(i))
                    My.Computer.FileSystem.CopyFile(data(0), Mid(files(i), 1, files(i).LastIndexOf(".")) & exten, True)
                End If
                If prog(0) = 0 Then
                    If progress + prog(1) <= 100 Then
                        progress += prog(1)
                    Else
                        progress = 100
                    End If
                Else
                    If Math.IEEERemainder(i, prog(1)) = 0 Then
                        If progress + 1 < 100 Then
                            progress += 1
                        Else
                            progress = 100
                        End If
                    End If
                End If
                info(1) = i
                info(2) = 1
            Next
            progress = 100
            endresult = 1
        Catch
            endresult = 2
        End Try
    End Sub
End Class
