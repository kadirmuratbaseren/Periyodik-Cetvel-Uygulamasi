Public Class frmHakkinda

    Private Sub frmHakkinda_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Click
        Me.Close()
    End Sub

    Private Sub frmHakkinda_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Label1.Text = "LostSoft by K.Murat BA�EREN" & vbCrLf & "T�m haklar� sakl�d�r.." & vbCrLf & "E-Mail : muratbaseren@yahoo.com"
    End Sub
End Class