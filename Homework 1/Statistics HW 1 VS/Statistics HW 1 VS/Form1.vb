Imports System.Diagnostics.Eventing.Reader

Public Class Form1
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim fib_number As Long
        fib_number = Convert.ToInt64(TextBox1.Text)
        If fib_number <= 45 Then
            Dim sw As Stopwatch
            sw = New Stopwatch
            sw.Start()
            Label2.Text = "Result : " + fib(fib_number).ToString()
            sw.Stop()
            Label3.Text = "Execution time in ms : " + sw.ElapsedMilliseconds.ToString()
        Else
            Label2.Text = "Result : "
            Label3.Text = "Execution time in ms : "
        End If
    End Sub

    Function fib(value As Long) As Long
        If value = 0 Then
            Return 0
        End If
        If value <= 2 Then
            Return 1
        End If
        Return fib(value - 1) + fib(value - 2)
    End Function
End Class
