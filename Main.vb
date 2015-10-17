Imports System.IO
Imports System.Security.Permissions

Public Class Main

    Function FileSplit(ByVal mapfile As String) As String()
        Dim i = mapfile.LastIndexOf("\")
        Return { _
            mapfile.Substring(0, i), _
            mapfile.Substring(i + 1, mapfile.Length - i - 1)}
    End Function

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            txtFile.Text = My.Settings.URT
            Dim args() As String = System.Environment.GetCommandLineArgs(), demo = False

            If args.Count >= 2 Then
                ' Enable support for new q3ut4\download directory
                Dim mapfile = args(1).Replace("\download", "")
                Dim fsplit() As String = FileSplit(mapfile)
                ' Go one directory up from the file
                Dim _fsplit() As String = FileSplit(fsplit(0))

                ' Check if map is in the correct location
                If fsplit(1).Contains(".pk3") Then
                    If _fsplit(1) <> "q3ut4" Then
                        ' Try to copy it to user-defined URT folder
                        If My.Settings.URT <> "" Then
                            Dim _mapfile = FileSplit(My.Settings.URT)(0) & "\q3ut4\" & fsplit(1)

                            ' Caution: will overwrite old file
                            My.Computer.FileSystem.CopyFile(mapfile, _mapfile, True)

                            ' Reconfigure variables
                            mapfile = _mapfile
                            fsplit = FileSplit(mapfile)
                            _fsplit = FileSplit(fsplit(0))
                        Else : Return
                        End If
                    End If
                    ' Remove ".PK3"
                    fsplit(1) = fsplit(1).Substring(0, fsplit(1).LastIndexOf("."))

                ElseIf fsplit(1).Contains(".bsp") Then
                    If _fsplit(1) <> "maps" Then
                        MsgBox(".BSP must be in q3ut4\maps directory.")
                        Me.Close()
                        Return
                    End If
                    ' Remove ".BSP"
                    fsplit(1) = fsplit(1).Substring(0, fsplit(1).LastIndexOf("."))
                    ' Go one more directory up
                    _fsplit = FileSplit(_fsplit(0))

                ElseIf fsplit(1).Contains(".urtdemo") Then
                    If _fsplit(1) <> "demos" Then
                        ' Try to copy it to user-defined URT folder
                        If My.Settings.URT <> "" Then
                            Dim _mapfile = FileSplit(My.Settings.URT)(0) & "\q3ut4\demos\" & fsplit(1)

                            ' Caution: will overwrite old file
                            My.Computer.FileSystem.CopyFile(mapfile, _mapfile, True)

                            ' Reconfigure variables
                            mapfile = _mapfile
                            fsplit = FileSplit(mapfile)
                            _fsplit = FileSplit(fsplit(0))
                        Else : Return
                        End If
                    End If
                    ' Remove ".urtdemo"
                    fsplit(1) = fsplit(1).Substring(0, fsplit(1).LastIndexOf("."))
                    ' Go one more directory up
                    _fsplit = FileSplit(_fsplit(0))
                    demo = True

                Else
                    MsgBox("Invalid file type.")
                    Me.Close()
                    Return
                End If

                Dim cmd = ""

                If My.Computer.FileSystem.FileExists(_fsplit(0) & "\Handler.exe") Then
                    cmd = _fsplit(0) & "\Handler.exe"
                ElseIf My.Computer.FileSystem.FileExists(_fsplit(0) & "\Quake3-UrT.exe") Then
                    cmd = _fsplit(0) & "\Quake3-UrT.exe"
                ElseIf My.Computer.FileSystem.FileExists(_fsplit(0) & "\ioUrbanTerror.exe") Then
                    cmd = _fsplit(0) & "\ioUrbanTerror.exe"
                Else
                    MsgBox("Unable to locate Quake3-UrT.exe or ioUrbanTerror.exe in" & vbNewLine & _fsplit(0))
                    Me.Close()
                    Return
                End If

                Dim cmdargs = "+g_gametype 9 +sv_pure 0 " & " +devmap " & fsplit(1) & " +set fs_homepath """ & _fsplit(0) & """ +set fs_basepath """ & _fsplit(0) & """"

                If demo Then cmdargs = cmdargs.Replace("+devmap", "+demo")

                Process.Start(cmd, cmdargs)
                Me.Close()
            End If

        Catch ex As Exception
            MsgBox("An error occurred: " & ex.ToString)
            Me.Close()
        End Try
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        My.Settings.URT = txtFile.Text
        My.Settings.Save()
    End Sub

    Private Sub btnBrowse_Click(sender As Object, e As EventArgs) Handles btnBrowse.Click
        Dim openFileDialog1 As New OpenFileDialog()

        openFileDialog1.InitialDirectory = "C:\"
        openFileDialog1.Filter = "Urban Terror .EXE |*.exe"
        openFileDialog1.Title = "Please select your Urban Terror .EXE"

        If openFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            txtFile.Text = openFileDialog1.FileName
        End If
    End Sub
End Class
