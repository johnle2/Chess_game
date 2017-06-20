Public Class Form1
    Private Board(7, 7) As PictureBox
    Private sel_x As Integer = -1
    Private sel_y As Integer = -1
    Private gb(7, 7) As Tile
    Private pieces(31) As Piece

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
    End Sub

    Private Sub pb_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim this_tag = DirectCast(sender.Tag, Integer)
        Dim this_x As Integer = this_tag Mod 8
        Dim this_y As Integer = this_tag >> 3

        If sel_x = -1 Then
            If gb(this_x, this_y).getval <> 33 Then
                sel_x = this_x
                sel_y = this_y
            End If
        Else
            If pieces(gb(sel_x, sel_y).getval).check_move(this_tag) Then
                Dim special As Integer = 0
                updateboard(this_x, this_y, sel_x, sel_y, special)
                updatemove()
                'MessageBox.Show(" Piece id" + gb(this_x, this_y).getval.ToString, "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk)
            Else
                MessageBox.Show("Invalid Move!", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk)
            End If
            sel_x = -1
            sel_y = -1
        End If

    End Sub
    Private Sub initialize_data()
        For x As Integer = 0 To 31
            Dim offset As Byte = 0
            Select Case x Mod 8
                Case 0, 7   'rock
                    offset = &H1
                Case 1, 6   'knight
                    offset = &H2
                Case 2, 5   'bishop
                    offset = &H3
                Case 3      'queen
                    offset = &H4
                Case 4      'king
                    offset = &H5
            End Select

            If x > 15 Then
                offset = offset Or &H80 'black
                If x > 7 Then
                    offset = offset And &HF0    'pawn
                End If
            Else
                If x < 23 Then                  'pawn
                    offset = offset And &HF0
                End If
            End If
            pieces(x) = New Piece(x + (Math.Truncate(x / 16)) * 32, offset)
        Next

        For j As Integer = 0 To 7
            For i As Integer = 0 To 7
                If j < 2 Or j > 5 Then
                    gb(i, j) = New Tile(i + (j Mod 4) * 8)
                Else
                    gb(i, j) = New Tile(33)
                End If

            Next
        Next
    End Sub
    Private Sub initialize_graphics()
        For x As Integer = 0 To 7
            For y As Integer = 0 To 7
                'Create and initialize the PictureBox
                Board(x, y) = New PictureBox
                Board(x, y).Size = New Size(32, 32)
                Board(x, y).Location = New Point(32 * x + 45, 32 * (8 - y) + 45)

                If (x + y) Mod 2 = 1 Then
                    Board(x, y).BackColor = Drawing.Color.White
                Else
                    Board(x, y).BackColor = Drawing.Color.Maroon
                End If
                Dim pc As String
                Select Case x
                    Case 0, 7
                        pc = New String("rock_")
                    Case 1, 6
                        pc = New String("knight_")
                    Case 2, 5
                        pc = New String("bish_")
                    Case 3
                        pc = New String("queen_")
                    Case 4
                        pc = New String("king_")
                    Case Else
                        pc = New String("")
                End Select

                Select Case y
                    Case 0
                        Board(x, y).Image = Bitmap.FromFile("C:\Users\Henry\Downloads\" + pc + "w.png")
                    Case 1
                        Board(x, y).Image = Bitmap.FromFile("C:\Users\Henry\Downloads\pawn_w.png")
                    Case 6
                        Board(x, y).Image = Bitmap.FromFile("C:\Users\Henry\Downloads\pawn_b.png")
                    Case 7
                        Board(x, y).Image = Bitmap.FromFile("C:\Users\Henry\Downloads\" + pc + "b.png")
                    Case Else
                        Board(x, y).Image = Nothing
                End Select


                Board(x, y).BorderStyle = BorderStyle.FixedSingle
                Board(x, y).Tag = y * 8 + x
                Board(x, y).BringToFront()
                AddHandler Board(x, y).Click, AddressOf pb_Click

                'Add the PictureBox to the Form
                Controls.Add(Board(x, y))
            Next
        Next
    End Sub
    Private Sub NewGameToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NewGameToolStripMenuItem.Click
        initialize_data()
        initialize_graphics()
    End Sub
    Private Sub updateboard(ByVal dest_x As Byte, ByVal dest_y As Byte,
                            ByVal scr_x As Byte, ByVal scr_y As Byte,
                            ByVal special As Byte)
        'bit 8 is castling, bit 7 is enpassant, bit 6 is promotion
        'bit 1-0 is 00 - 01 - 10 - 11 is the four corner for castling ( BOTLEFT, BOTRIGHT, TOPLEFT, TOP RIGHT)
        'bit 0 is 0 for left enpassant, 1 for right
        'bit 1-0 is 00 - 01 - 10 - 11 is value for promotion (rock, knight, bish, queen)
        If special = 0 Then
            'Update graphic
            Board(dest_x, dest_y).Image = Board(scr_x, scr_y).Image
            Board(scr_x, scr_y).Image = Nothing
            'Update pieces
            pieces(gb(scr_x, scr_y).getval).set_loc(dest_x + dest_y * 8)
            If gb(dest_x, dest_y).getval <> 33 Then
                pieces(gb(dest_x, dest_y).getval).set_type(&H6)
            End If
            'Update gameboard
            gb(dest_x, dest_y).setval(gb(scr_x, scr_y).getval)
            gb(scr_x, scr_y).setval(33)

        Else
            If special >> 7 Then    'castling

            ElseIf special >> 6 And &H1 Then   'enpassant

            ElseIf special >> 5 And &H1 Then    'promotion
                'Update pieces
                pieces(gb(scr_x, scr_y).getval).set_loc(dest_x + dest_y * 8)
                pieces(gb(scr_x, scr_y).getval).set_type((special And &H3) + 1)
                If gb(dest_x, dest_y).getval <> 33 Then
                    pieces(gb(dest_x, dest_y).getval).set_type(&H6)
                End If

                Dim pc As String
                Select Case special And &H3
                    Case 0
                        pc = "rock_"
                    Case 1
                        pc = "knight_"
                    Case 2
                        pc = "bish_"
                    Case 3
                        pc = "queen_"
                    Case Else
                        pc = ""
                End Select
                If gb(scr_x, scr_y).getval > 15 Then
                    Board(dest_x, dest_y).Image = Bitmap.FromFile("C:\Users\Henry\Downloads\" + pc + "b.png")
                Else
                    Board(dest_x, dest_y).Image = Bitmap.FromFile("C:\Users\Henry\Downloads\" + pc + "w.png")
                End If
                Board(scr_x, scr_y).Image = Nothing

                'Update gameboard
                gb(dest_x, dest_y).setval(gb(scr_x, scr_y).getval)
                gb(scr_x, scr_y).setval(33)
            End If
        End If
    End Sub

    Private Function updatemove() As Byte
        For x As Integer = 0 To 31
            If pieces(x).get_type <> &H6 Then
                pieces(x).mod_valid(gb)
            End If
        Next
        Return 0
    End Function

End Class

Class Piece
    Dim loc As Byte
    Dim pc_type As Byte
    Dim valid_mv As List(Of Byte)
    Public Function check_move(ByVal dest As Byte) As Boolean
        Return True
        For Each x As Byte In valid_mv
            If dest = x Then
                Return True
            End If
        Next
        Return False
    End Function
    Public Function get_type() As Byte
        Return pc_type And &H7
    End Function

    Public Sub mod_valid(ByRef game As Tile(,))
        valid_mv.Clear()
        Dim loc_x As Integer = loc Mod 8
        Dim loc_y As Integer = loc >> 3
        Select Case pc_type And &H7
            Case 0  'pawn
                If loc_y Mod 5 = 1 Then
                End If
            Case 1  'rock

            Case 2  'knight

            Case 3  'bishop

            Case 4  'Queen

            Case 5  'King


        End Select
        'BIG FUNCTION!
    End Sub
    Public Sub set_loc(ByVal a As Byte)
        loc = a
    End Sub
    Public Sub set_type(ByVal a As Byte)
        pc_type = pc_type And &HF0
        pc_type = pc_type Or a
    End Sub
    Public Sub New(ByVal l As Byte, ByVal t As Byte)
        loc = l
        pc_type = t
    End Sub
End Class

Class Tile
    'Private loc As Byte
    Private val As Byte
    Public Function getval() As Byte
        Return val
    End Function
    Public Function isWhite() As Boolean
        Return val < 16
    End Function
    Public Sub setval(ByVal v As Byte)
        val = v
    End Sub
    Public Sub New(ByVal v As Byte)
        val = v
    End Sub

End Class