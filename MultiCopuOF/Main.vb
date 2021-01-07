Public Class Main
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim result As DialogResult = OpenFile.ShowDialog
        If result = DialogResult.OK Then
            TextBox1.Text = OpenFile.FileName
        End If
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim result As DialogResult = OpenFile.ShowDialog
        If result = DialogResult.OK Then
            TextBox2.Text = OpenFile.FileName
        End If
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Try
            If My.Computer.FileSystem.FileExists(TextBox1.Text) = False Then Error 1
            If My.Computer.FileSystem.FileExists(TextBox2.Text) = False Then Error 1
            If My.Computer.FileSystem.GetFileInfo(TextBox1.Text).DirectoryName = My.Computer.FileSystem.GetFileInfo(TextBox2.Text).DirectoryName Then Error 1
            ProcessCopy.ShowDialog()
        Catch
            MsgBox("Произошла ошибка! Причиной ошибки могло стать: отсутствие указанных файлов, совпадение исходной папки с папкой замены.",, "Ошибка")
        End Try
    End Sub
    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        If CheckBox2.Checked Then
            CheckBox4.Checked = False
            CheckBox4.Enabled = False
        Else
            CheckBox4.Enabled = True
        End If
    End Sub
End Class
