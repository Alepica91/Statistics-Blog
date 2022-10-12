

Imports System.Windows.Forms.VisualStyles.VisualStyleElement.Header

Public Class Form1

    Shared random As New Random()
    Dim timeleft = 30
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        timer1.Interval = 1000
        timer1.Start()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Timer1.Stop()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Timer1.Stop()
        timeleft = 30
        Label1.Text = timeleft.ToString()
        Label2.Text = ""
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If (timeleft > 0) Then
            timeleft -= 1
            Label1.Text = timeleft.ToString()
            Label2.Text = random.Next(100)
        Else
            Timer1.Stop()
        End If
    End Sub
End Class
