#Region "Imports"
Imports System.Xml
Imports System.Data
Imports system.Data.OleDb
#End Region

Public Class Form1

#Region "LocalVariables"
    Private SeciliBtn As ElementalButton
    Private SeciliTp As TabPage
    Private lstViewControl As ListView
    Private txtToplamControl As TextBox
#End Region

#Region "Methods"

    Private Function SetDataTable() As DataTable
        SetDataTable = New DataTable
        Dim DR As DataRow

        SetDataTable.Columns.Add("Atom Simgesi")
        SetDataTable.Columns.Add("Atom Ismi")
        SetDataTable.Columns.Add("Atom No")
        SetDataTable.Columns.Add("Grup")
        SetDataTable.Columns.Add("Periyot")
        SetDataTable.Columns.Add("Adet")
        SetDataTable.Columns.Add("Birim A��rl�k")
        SetDataTable.Columns.Add("K�smi A��rl�k")

        For Each itm As myListViewNesnesi In Me.lstViewControl.Items
            DR = SetDataTable.NewRow
            DR.Item(0) = itm.ElementButonu.Text
            DR.Item(1) = itm.ElementButonu.ElementNesnesi.Isim
            DR.Item(2) = itm.ElementButonu.ElementNesnesi.AtomNo
            DR.Item(3) = itm.ElementButonu.ElementNesnesi.Grup
            DR.Item(4) = itm.ElementButonu.ElementNesnesi.Periyot
            DR.Item(5) = itm.SubItems(2).Text
            DR.Item(6) = itm.ElementButonu.ElementNesnesi.AtomAgirlik
            DR.Item(7) = itm.SubItems(3).Text
            SetDataTable.Rows.Add(DR)
        Next

        Return SetDataTable
    End Function

    Private Sub GetDataTable(ByVal DataTable As DataTable)

        For Each dr As DataRow In DataTable.Rows

            Dim itm As myListViewNesnesi
            Dim btn As ElementalButton = New ElementalButton(New ElementObject(dr.Item(2), dr.Item(6), dr.Item(3).ToString, dr.Item(4), dr.Item(1).ToString, ""))

            btn.Text = dr.Item(0).ToString()

            itm = New myListViewNesnesi(btn)
            itm.SubItems.Add(dr.Item(0).ToString())
            itm.SubItems.Add(dr.Item(5).ToString())
            itm.SubItems.Add(dr.Item(7).ToString())

            Me.lstViewControl.Items.Add(itm)
        Next
    End Sub

    Private Function ToplamAl() As Single

        ToplamAl = 0.0

        If Not Me.lstViewControl.Items.Count = 0 Then
            For Each itm As myListViewNesnesi In Me.lstViewControl.Items
                ToplamAl += CType(itm.SubItems(3).Text, Single)
            Next
        End If

        Return ToplamAl
    End Function

    Private Sub NewTabPage(ByVal TabPageText As String)

        Dim tp As TabPage = New TabPage
        tp.Text = TabPageText

        Dim myCtrl As myMoleculeControl = New myMoleculeControl
        myCtrl.Dock = DockStyle.Fill
        tp.Controls.Add(myCtrl)

        Me.TabControl1.TabPages.Add(tp)

        Me.TabControl1.SelectedIndex = -1
        Me.TabControl1.SelectedTab = tp
    End Sub

    Private Sub Save(ByVal SavePath As String, ByVal FilterIndex As Integer)

        Select Case FilterIndex
            Case 1
                'Xml dosya olu�turma..
                Dim FRW As DLL_FileReaderWriter.XML
                FRW = New DLL_FileReaderWriter.XML
                FRW.XMLFileWriter(SetDataTable(), SavePath)
            Case 2
                'Html dokuman� olu�turma..
                Dim FRW As DLL_FileReaderWriter.WritingFromDataTable
                FRW = New DLL_FileReaderWriter.WritingFromDataTable
                FRW.WriteHTMLFile(SetDataTable(), SavePath, SeciliTp.Text, Application.StartupPath & "\Style.csscode", Application.StartupPath & "\java_script.javacode", ToplamAl())
            Case 3
                'Txt dosya olu�turma..
                Dim FRW As DLL_FileReaderWriter.WritingFromDataTable
                FRW = New DLL_FileReaderWriter.WritingFromDataTable
                FRW.WriteTEXTFile(SetDataTable(), SavePath, ToplamAl)
        End Select

    End Sub

    Private Sub ButonlariOlustur()

        Dim ButtonsPnl As Panel

        'SplitContainer i�indeki "Buttons" isimli panel ele ge�irilir. ��nk� bu butonlr bu kontrol i�ine eklenecektir..
        For Each ctrl As Control In Me.SplitContainer1.Panel1.Controls(1).Controls
            If TypeOf ctrl Is Panel AndAlso ctrl.Name = "Buttons" Then
                ButtonsPnl = CType(ctrl, Panel)
                ButtonsPnl.Controls.Clear()
                Exit For
            End If
        Next

        '========
        'BUTTONS
        '========
        'Okunacak XML d�k�man� elde edilir.
        Dim Doc As XmlDocument = New XmlDocument
        Doc.Load(Application.StartupPath & "\DatabaseOfElements.xml")

        '<Atom> node'lar�n� i�eren RootNode ele ge�irilir..
        Dim root As XmlElement = Doc.ChildNodes(1)

        'RootNode i�indeki childNode'lar tek tek okunur.
        'ChildNode i�inde Atom �zelliklerini i�eren Node'lara sahiptir..
        For Each child As XmlElement In root.ChildNodes
            Dim btn As ElementalButton = New ElementalButton(New ElementObject(CInt(child.ChildNodes(6).InnerText), CType(child.ChildNodes(5).InnerText, Single), child.ChildNodes(1).InnerText, CInt(child.ChildNodes(2).InnerText), child.ChildNodes(7).InnerText, child.ChildNodes(8).InnerText))

            btn.Size = New Size(35, 35)
            btn.Location = New Point(CInt(child.ChildNodes(3).InnerText) * 35, CInt(child.ChildNodes(4).InnerText) * 35)
            btn.Text = child.ChildNodes(0).InnerText

            'Butona basilinca yapilacak i�lemler i�in Event y�nlendirilcek..
            AddHandler btn.Click, AddressOf AtomDetails

            'Me.SplitContainer1.Controls.Add(btn)
            ButtonsPnl.Controls.Add(btn)
        Next

        'Me.SplitContainer1.Panel1.Controls(1).Controls.Add(pnl2)
    End Sub

    Private Sub ButonlariYenile(ByVal sender As Object, ByVal e As FormClosedEventArgs)
        'Ayarlar penceresinde(Form4) Element �zellikleri de�i�tikten sonra Ana Ekrana gelince(Form1) butonlar�n g�ncellenmi� �zelliklerinin g�r�nt�lenmesi i�in ; de�i�en XML kayna��ndan elementlerin yeni �zellikleri okunarak butonlar yeniden olu�turulur..(Form4 '�n Closed Event'inde �al��t�r�l�r.)
        ButonlariOlustur()
        Temizle()
    End Sub

    Private Sub Temizle()
        For Each ctrl As Control In Me.grpAtomDetaylari.Controls
            If TypeOf ctrl Is TextBox Then
                CType(ctrl, TextBox).Text = ""
            End If
        Next
    End Sub

#End Region

#Region "Events"

    Private Sub Form1_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        'CrystalReport'un Raporlama yapabilmesi i�in kopyalanan Database silinir..
        Try
            If Not System.IO.File.Exists("c:\Db.mdb") = False Then
                Kill("c:\Db.mdb")
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Periyodik dizayn'�n a�a�� istenilen de�erde kaymas� i�in ba��nda Dizayn yazan a��klama sat�rlar�na bak!

        Dim lbl As Label
        Dim lblText As String
        Dim pnl As Panel

        pnl = New Panel
        pnl.Name = "labels"
        pnl.Dock = DockStyle.Fill
        pnl.BorderStyle = BorderStyle.FixedSingle

        '========
        'LABELS
        '========
        For i As Integer = 0 To 19
            If i < 18 Then
                lbl = New Label
                'Dizayn; Location(Y) de�erini istenilen �ekilde artt�r ama di�er Dizayn sekmelerinide ayn� de�erde artt�rmal�s�n!
                lbl.Location = New Point(i * 35 + 80, 40)
                lbl.Size = New Size(35, 35)
                lbl.TextAlign = ContentAlignment.MiddleCenter
                lbl.Font = New Font("Times New Roman", 8, FontStyle.Bold Or FontStyle.Underline)
                lbl.BorderStyle = BorderStyle.FixedSingle
                lbl.AutoSize = False
                lbl.BackColor = Color.LightSteelBlue
                lbl.ForeColor = Color.Navy

                Select Case i
                    Case 0
                        lblText = "1A"
                    Case 1
                        lblText = "2A"
                    Case 2
                        lblText = "3B"
                    Case 3
                        lblText = "4B"
                    Case 4
                        lblText = "5B"
                    Case 5
                        lblText = "6B"
                    Case 6
                        lblText = "7B"
                    Case 7
                        lblText = "8B"
                    Case 8
                        lblText = "8B"
                    Case 9
                        lblText = "8B"
                    Case 10
                        lblText = "1B"
                    Case 11
                        lblText = "2B"
                    Case 12
                        lblText = "3A"
                    Case 13
                        lblText = "4A"
                    Case 14
                        lblText = "5A"
                    Case 15
                        lblText = "6A"
                    Case 16
                        lblText = "7A"
                    Case 17
                        lblText = "8A"
                End Select

                lbl.Text = lblText
                pnl.Controls.Add(lbl)
            End If

            If i < 7 Then
                lbl = New Label
                'Dizayn; Location(Y) de�erini istenilen �ekilde artt�r(toplam k�sm�n�) ama di�er Dizayn sekmelerinide ayn� de�erde artt�rmal�s�n!
                lbl.Location = New Point(10, i * 35 + 75)
                lbl.Size = New Size(70, 35)
                lbl.TextAlign = ContentAlignment.MiddleCenter
                lbl.Font = New Font("Times New Roman", 8, FontStyle.Bold Or FontStyle.Underline)
                lbl.BorderStyle = BorderStyle.FixedSingle
                lbl.AutoSize = False
                lbl.BackColor = Color.LightSteelBlue
                lbl.ForeColor = Color.Navy
                lbl.Tag = i + 1
                Select Case i
                    Case 0
                        lblText = "1.Periyot"
                    Case 1
                        lblText = "2.Periyot"
                    Case 2
                        lblText = "3.Periyot"
                    Case 3
                        lblText = "4.Periyot"
                    Case 4
                        lblText = "5.Periyot"
                    Case 5
                        lblText = "6.Periyot"
                    Case 6
                        lblText = "7.Periyot"
                End Select
                lbl.Text = lblText
                pnl.Controls.Add(lbl)
            End If

            If i > 17 Then
                lbl = New Label
                'Dizayn; Location(Y) de�erini istenilen �ekilde artt�r(toplam k�sm�n�) ama di�er Dizayn sekmelerinide ayn� de�erde artt�rmal�s�n!
                lbl.Location = New Point(150, ((i - 10) * 35) + 75)
                lbl.Size = New Size(70, 35)
                lbl.TextAlign = ContentAlignment.MiddleCenter
                lbl.Font = New Font("Times New Roman", 9, FontStyle.Bold Or FontStyle.Underline Or FontStyle.Italic)
                lbl.AutoSize = False
                lbl.ForeColor = Color.Navy
                Select Case i
                    Case 18
                        lbl.Text = "Lantanitler"
                    Case 19
                        lbl.Text = "Aktinitler"
                End Select
                pnl.Controls.Add(lbl)
            End If
        Next

        'Butonlar� i�ine alacak panel olu�turuluyor..
        Dim pnl2 As Panel
        pnl2 = New Panel
        pnl2.Name = "Buttons"
        'Dizayn; Location(Y) de�erini istenilen �ekilde artt�r(toplam k�sm�n�) ama di�er Dizayn sekmelerinide ayn� de�erde artt�rmal�s�n!
        pnl2.Location = New Point(80, 75)
        pnl2.Size = New Size(18 * 35, 10 * 35)
        'pnl2.BorderStyle = BorderStyle.FixedSingle

        'Butonlar� i�ine alacak panel ; Label'lar� i�ine alan ana panel'e ekleniyor..
        pnl.Controls.Add(pnl2)

        'Ana Panel Splitcontainer'a ekleniyor..
        Me.SplitContainer1.Panel1.Controls.Add(pnl)

        'Buttonlar� pnl2 i�ine olu�turmak �zere Sub �al��t�r�l�yor..
        ButonlariOlustur()
    End Sub

    Public Sub AtomDetails(ByVal sender As Object, ByVal e As EventArgs)
        'Se�ilen buton ElementalButton kal�b�na d�n��t�r�lerek �zelliklerine ula��l�r..
        SeciliBtn = CType(sender, ElementalButton)

        'AtomDetaylar� groupbox'�ndaki alanlar temizlenir.
        Me.Temizle()

        'T�klanan ElementalButton �zellikleri gerekli yerlere aktar�l�r.
        Me.txtAtomSimge.Text = SeciliBtn.Text
        Me.txtAtomNo.Text = SeciliBtn.ElementNesnesi.AtomNo.ToString
        Me.txtAtomIsmi.Text = SeciliBtn.ElementNesnesi.Isim
        Me.txtAtomMA.Text = SeciliBtn.ElementNesnesi.AtomAgirlik.ToString
        Me.txtAciklama.Text = SeciliBtn.ElementNesnesi.Desc
        Me.txtGrup.Text = SeciliBtn.ElementNesnesi.Grup
        Me.txtPeriyot.Text = SeciliBtn.ElementNesnesi.Periyot.ToString

        'Periyodik Cetvelin Labell'lar�n� i�inde tutan panel'e eri�ilir ve Se�ilen buton 'un Grup ve Periyot Label'� k�rm�z� yaz� tipine d�n��t�r�l�r..
        For Each ctrl As Control In Me.SplitContainer1.Panel1.Controls(1).Controls
            If TypeOf ctrl Is Label Then
                Dim lbl As Label = CType(ctrl, Label)
                lbl.ForeColor = Color.Navy
                lbl.Font = New Font("Times New Roman", 9, FontStyle.Bold Or FontStyle.Underline)

                If lbl.Text = SeciliBtn.ElementNesnesi.Grup.ToString OrElse lbl.Tag = SeciliBtn.ElementNesnesi.Periyot.ToString Then
                    lbl.ForeColor = Color.Red
                    lbl.Font = New Font("Times New Roman", 9, FontStyle.Bold Or FontStyle.Underline Or FontStyle.Italic)
                End If
            End If
        Next
    End Sub

    Private Sub btnEkle_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEkle.Click

        Dim ListedeYok As Boolean = False
        Dim LsvItm As myListViewNesnesi

        'E�er a��lm�� bir sekme yok ise sekme a�ar..
        If Me.TabControl1.TabPages.Count = 0 Then NewToolStripButton_Click(sender, e)

        'E�er se�ili element yoksa uyar� ��kar�r..
        If Not Me.nudAdet.Value = 0 AndAlso Not Me.SeciliBtn.Text = "" Then

            'Se�ilen butondan bir myListViewNesnesi olu�turulur ve bu myListViewNesnesi �zellikleri in�a edilir..
            LsvItm = New myListViewNesnesi(SeciliBtn)

            'ilk olarak atom listede yok olarak i�aretlenir..
            ListedeYok = True
            For i As Integer = 0 To Me.lstViewControl.Items.Count - 1
                'E�er se�ili element listede varsa..
                If Me.lstViewControl.Items(i).SubItems(1).Text = LsvItm.ElementButonu.Text Then
                    'Atom listede var olarak i�aretlenir..
                    ListedeYok = False
                    'Varolan liste Item'�na de�erler eklenir..
                    Me.lstViewControl.Items(i).SubItems(2).Text += Me.nudAdet.Value
                    Me.lstViewControl.Items(i).SubItems(3).Text += CType(Me.nudAdet.Value * LsvItm.ElementButonu.ElementNesnesi.AtomAgirlik, Single)
                End If
            Next

            'E�er yeni eklenecek element listede yoksa olu�turulan myListViewNesnesi listeye eklenir.
            If ListedeYok = True Then
                Me.lstViewControl.Items.Add(LsvItm)
                Me.lstViewControl.Items(Me.lstViewControl.Items.Count - 1).SubItems.Add(LsvItm.ElementButonu.Text)
                Me.lstViewControl.Items(Me.lstViewControl.Items.Count - 1).SubItems.Add(Me.nudAdet.Value)
                Me.lstViewControl.Items(Me.lstViewControl.Items.Count - 1).SubItems.Add(Me.nudAdet.Value * LsvItm.ElementButonu.ElementNesnesi.AtomAgirlik)
            End If

            '==========================
            'Toplam hesaplan�r..
            Me.txtToplamControl.Text = ToplamAl()
        Else
            MessageBox.Show("Adet girmediniz ya da bir element se�mediniz!", "Element veya Adet Yok !", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        End If
    End Sub

    Private Sub btnSil_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSil.Click
        Dim Silinecek As Single = 0.0

        'i�aretlenmi� item'lardan Adette belirtilen miktarda azalt�l�r..
        For Each itm As myListViewNesnesi In Me.lstViewControl.CheckedItems
            'E�er belirtilen adet listedeki itm adetinden k���k yada e�it ise
            If itm.SubItems(2).Text >= Me.nudAdet.Value Then
                Silinecek = itm.ElementButonu.ElementNesnesi.AtomAgirlik * Me.nudAdet.Value
                itm.SubItems(2).Text -= Me.nudAdet.Value
                itm.SubItems(3).Text -= Silinecek

                'E�er itm adeti "0" ise listeden kald�r�l�r..
                If itm.SubItems(2).Text = 0 Then itm.Remove()
            Else
                MessageBox.Show(Chr(34) & itm.SubItems(1).Text & Chr(34) & " adl� element i�in adeti var olandan fazla girilmi�. L�tfen kontrol ediniz !", "Hatal� Giri� !", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            End If
        Next

        'Toplam al�n�r..
        Me.txtToplamControl.Text = ToplamAl()
    End Sub

    Private Sub btnTemizle_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTemizle.Click
        Dim Silinecek As Single = 0.0

        'i�aretlenen listItem'lar� listeden kald�r�l�r.
        If Not Me.lstViewControl.CheckedItems.Count = 0 Then
            For Each itm As myListViewNesnesi In Me.lstViewControl.CheckedItems
                itm.Remove()
            Next
        End If

        'Toplam al�n�r..
        Me.txtToplamControl.Text = ToplamAl()
    End Sub

    Private Sub TabControl1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TabControl1.SelectedIndexChanged
        'TabPage(sekme) se�ilince seciliTabPage "SeciliTp" de�i�kenine ,Toplam TextBox 'da "txtToplamControl" de�i�kenine al�n�r.
        If Not Me.TabControl1.SelectedIndex = -1 Then
            SeciliTp = Me.TabControl1.SelectedTab
            Me.lstViewControl = CType(SeciliTp.Controls(0).Controls(0), ListView)
            Me.txtToplamControl = CType(SeciliTp.Controls(0).Controls(1).Controls(0), TextBox)
        End If
    End Sub

#End Region

#Region "ToolStrip_Agirlik_Hesaplama"
    Private Sub NewToolStripButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NewToolStripButton.Click
        'Yeni bir sekme a��l�r..
        Dim MoleculName As String = InputBox("Molek�l �smi Giriniz :", "Molek�l �smi")
        NewTabPage(MoleculName)
    End Sub

    Private Sub SaveToolStripButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveToolStripButton.Click

        If Me.TabControl1.TabPages.Count = 0 Then Exit Sub

        'Secili TabPage ba�l��� dosya ismi olarak atan�r..
        Me.SaveFileDialog1.FileName = SeciliTp.Text
        Me.SaveFileDialog1.Filter = "XML Files(*.xml)|*.xml|Web Sayfas�(*.htm;*.html)|*.htm;*.html|Metin Belgesi(*.txt)|*.txt"
        Me.SaveFileDialog1.Title = "Verileri Kaydetme"
        Me.SaveFileDialog1.FilterIndex = 0

        'Dosya belirtilen �ekilde kaydedilir..
        If Not Me.lstViewControl.Items.Count = 0 Then
            If Me.SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                Save(Me.SaveFileDialog1.FileName, Me.SaveFileDialog1.FilterIndex)
            End If
        Else
            MessageBox.Show("Listenizde kaydedilecek element bulunmamaktad�r!", "Element Yok !", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
        End If

    End Sub

    Private Sub btnSaveAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSaveAll.Click

        If Me.TabControl1.TabPages.Count = 0 Then Exit Sub

        Me.SaveFileDialog1.DefaultExt = SeciliTp.Text
        Me.SaveFileDialog1.Filter = "XML Files(*.xml)|*.xml|Web Sayfas�(*.htm;*.html)|*.htm;*.html|Metin Belgesi(*.txt)|*.txt"
        Me.SaveFileDialog1.Title = "Verileri Kaydetme"
        Me.SaveFileDialog1.FilterIndex = 0

        If Me.SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            Me.TabControl1.SelectedIndex = -1

            'TabPage ba�l�klar� kullan�larak isimlendirme yap�larak sekmeler belirtilen formatta kaydedilir..
            For Each tp As TabPage In Me.TabControl1.TabPages
                Me.TabControl1.SelectedTab = tp
                If Not Me.lstViewControl.Items.Count = 0 Then
                    Dim DosyaIsmi As String = Me.SaveFileDialog1.FileName.Substring(0, Me.SaveFileDialog1.FileName.LastIndexOf("\") + 1) & tp.Text

                    Select Case Me.SaveFileDialog1.FilterIndex
                        Case 1
                            DosyaIsmi &= ".xml"
                        Case 2
                            DosyaIsmi &= ".html"
                        Case 3
                            DosyaIsmi &= ".txt"
                    End Select

                    Save(DosyaIsmi, Me.SaveFileDialog1.FilterIndex)
                Else
                    MessageBox.Show(tp.Text & " adl� listenizde kaydedilecek element bulunmamaktad�r! Bu liste kaydedilmeyecektir..", "Element Yok !", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
                End If
            Next
        End If

    End Sub

    Private Sub OpenToolStripButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenToolStripButton.Click
        Dim FRW As DLL_FileReaderWriter.XML

        Me.OpenFileDialog1.Filter = "XML Files(*.xml)|*.xml"
        Me.OpenFileDialog1.Title = "Verileri Okuma"

        If Me.OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then

            Dim DosyaIsmi As String = Me.OpenFileDialog1.FileName.ToString
            Dim DT As DataTable = New DataTable
            'Se�ilen XML dosyas�ndaki veriler DT adl� DataTable'a aktar�l�r.
            FRW = New DLL_FileReaderWriter.XML
            DT = FRW.ReadXMLFile(Me.OpenFileDialog1.FileName)

            'Se�ilen dosya ad� ile bir yeni bir tabpage olu�turulur..
            'Sadece dosya ad� al�n�r.�rne�in: "c:\deneme.xml" --> 'deneme'
            NewTabPage(Me.OpenFileDialog1.FileName.Substring(DosyaIsmi.LastIndexOf("\") + 1, DosyaIsmi.LastIndexOf(".") - DosyaIsmi.LastIndexOf("\") - 1))
            'DataTable 'a cevrilen XML verileri olu�turulan TabPage'deki myMoleculeControl'deki ListView 'a aktar�l�r..
            GetDataTable(DT)

            'Se�ili TabPage'deki txtToplam hanesine toplam hesaplan�r..
            Me.txtToplamControl.Text = ToplamAl()
        End If
    End Sub

    Private Sub PrintToolStripButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PrintToolStripButton.Click

        'Eger hesaplanm�� bir molekul ag�rl��� sekmesi yoksa i�lem iptal..
        If Me.TabControl1.TabPages.Count = 0 Then
            MessageBox.Show("Raporlama yap�lacak bir sekme yok! L�tfen Raporlama i�in bir sekme olu�turun..", "Yazd�r�lacak Rapor Yok", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1)
            Exit Sub
        End If

        'Connection ve Command olu�turulur..
        Dim ConnStr As String = "Provider=Microsoft.Jet.OleDb.4.0;Data Source=" & Application.StartupPath & "\Db.mdb;"
        Dim Conn As OleDbConnection
        Dim Cmd As OleDbCommand

        Try

            Conn = New OleDbConnection(ConnStr)
            Cmd = New OleDbCommand()
            Cmd.Connection = Conn
            Cmd.CommandType = CommandType.StoredProcedure

            '�lk olarak Tablodaki veriler silinir..
            Conn.Open()
            Cmd.CommandText = "ALL_DELETE"
            Cmd.ExecuteNonQuery()

            'ListView'deki her bir Item de�erleri Command'a parametre olarak verilir..
            For Each itm As myListViewNesnesi In Me.lstViewControl.Items
                Cmd.Parameters.AddWithValue("@param1", itm.ElementButonu.ElementNesnesi.AtomNo)
                Cmd.Parameters.AddWithValue("@param2", itm.SubItems(1).Text)
                Cmd.Parameters.AddWithValue("@param3", itm.ElementButonu.ElementNesnesi.Isim.ToString)
                Cmd.Parameters.AddWithValue("@param4", itm.ElementButonu.ElementNesnesi.Grup.ToString)
                Cmd.Parameters.AddWithValue("@param5", CInt(itm.ElementButonu.ElementNesnesi.Periyot))
                Cmd.Parameters.AddWithValue("@param6", CInt(itm.SubItems(2).Text))
                Cmd.Parameters.AddWithValue("@param7", CType(itm.ElementButonu.ElementNesnesi.AtomAgirlik, Single))
                Cmd.Parameters.AddWithValue("@param8", CType(itm.SubItems(3).Text, Single))
                Cmd.Parameters.AddWithValue("@param9", SeciliTp.Text)

                'Insert i�lemi..
                Cmd.CommandText = "ALL_INSERT"
                Cmd.ExecuteNonQuery()

                'parametre de�erleri temizlenir..
                Cmd.Parameters.Clear()
            Next

            System.Threading.Thread.Sleep(1000)

            'Raporlama i�in Database ,CrystalReport'un okuyabilmesi i�in C:\ ya kopyalan�r..
            FileCopy(Application.StartupPath & "\Db.mdb", "c:\Db.mdb")

        Catch exOle As OleDbException
            MessageBox.Show(exOle.Message.ToString, "DATABASE HATA", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString, "GENEL HATA", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
        Finally
            Conn.Close()
            Cmd.Dispose()
            Conn.Dispose()
        End Try

        'Raporlama Form 'u a��l�r..
        Dim frm As Form2 = New Form2
        frm.Show()
    End Sub

    Private Sub btnTabPageKapat_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTabPageKapat.Click
        If Not Me.TabControl1.SelectedIndex = -1 Then
            Me.TabControl1.SelectedTab.Dispose()
        End If
    End Sub

    Private Sub btnMolekulYaz_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnMolekulYaz.Click
        Dim Molekul As String = ""

        Molekul = InputBox("L�tfen Molek�l� Atomlar Aras�na " & vbCrLf & Chr(34) & "Tire(-)" & vbCrLf & Chr(34) & " Koyarak Yaz�n�z : " & vbCrLf & "�rne�in;" & vbCrLf & "AmonyumS�lfat : 2N-2H4-S-O4" & vbCrLf & "Su : H2-O  ... gibi.", "Molek�l Yazma")

        Molekul = Trim(Molekul)

        'E�er bir molekul yaz�ld�ysa..
        If Not Molekul.Length = 0 Then

            If MessageBox.Show("Yeni bir sekmede a�mak istiyorsan�z " & Chr(34) & "Evet" & Chr(34) & " 'i, var olan sekmeye eklenmesini istiyorsan�z " & Chr(34) & "Hay�r" & Chr(34) & " '� se�iniz..", "Yeni Sekmede A�", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) = Windows.Forms.DialogResult.Yes Then NewToolStripButton_Click(sender, e)

            'String ifade atomlar�na ayr�l�r..
            Dim Atoms As String() = Molekul.Split("-")

            'Atomun �n�nde ve arkas�nda  yer alan mol say�lar� ve element ismi i�in de�i�kenler tan�mlama..
            Dim OncekiMol As String = ""
            Dim SonrakiMol As String = ""
            Dim strElement As String = ""
            'Element ve mol say�s� �eklinde listelencek..
            Dim Liste As ArrayList = New ArrayList
            Dim param As Paramaters

            'Dizideki her bir atom i�in..
            For Each str As String In Atoms
                Dim HarfChr As Boolean = False
                OncekiMol = ""
                SonrakiMol = ""

                'Her karakter i�in..
                For Each chr As Char In str.ToCharArray
                    'E�er karakter say� ise ve daha hi� harf okunmad�ysa..
                    If Char.IsDigit(chr) = True AndAlso HarfChr = False Then
                        OncekiMol &= Char.ToString(chr)
                        'Eger karakter harf ise ve daha hi� harf okunmad�ysa.
                    ElseIf Char.IsLetter(chr) = True AndAlso HarfChr = False Then
                        strElement = Char.ToUpper(chr).ToString
                        HarfChr = True
                        'E�er karakter harf ise ve daha �nce harf okunduysa..
                    ElseIf Char.IsLetter(chr) = True AndAlso HarfChr = True Then
                        strElement &= Char.ToLower(chr).ToString
                        'E�er karakter say� ise ve daha �nce harf okunduysa..
                    ElseIf Char.IsDigit(chr) = True AndAlso HarfChr = True Then
                        SonrakiMol &= Char.ToString(chr)
                    End If
                Next

                If OncekiMol = "" Then OncekiMol = "1"
                If SonrakiMol = "" Then SonrakiMol = "1"

                'Ele ge�en Elemnt ismi ve Mol say�s� Listeye eklenir..
                param = New Paramaters(strElement, CStr(CInt(OncekiMol) * CInt(SonrakiMol)))
                Liste.Add(param)
            Next

            Dim ButtonsPnl As Panel

            'Butonlar�n bulundu�u panele eri�iyoruz..
            For Each ctrl As Control In Me.SplitContainer1.Panel1.Controls(1).Controls
                If TypeOf ctrl Is Panel AndAlso ctrl.Name = "Buttons" Then
                    ButtonsPnl = CType(ctrl, Panel)
                    Exit For
                End If
            Next

            'Butonlar�n bulundu�u paneldeki her buton i�in d�n�l�r.Her buton i�in liste kontrol edilir.E�er listede yer alan bir butona gelinilirse Element molek�l a��rl��� hesaplama sekmesine eklenir..
            For Each btn As ElementalButton In ButtonsPnl.Controls
                For Each prm As Paramaters In Liste
                    If btn.Text = prm.Deger1 Then
                        SeciliBtn = btn
                        Me.nudAdet.Value = prm.Deger2
                        btnEkle_Click(sender, e)
                        Exit For
                    End If
                Next
            Next

        End If
    End Sub

    Private Sub HelpToolStripButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HelpToolStripButton.Click
        Dim frm As frmHakkinda = New frmHakkinda
        frm.ShowDialog()
    End Sub
#End Region

#Region "ToolStrip_PeriyodikCetvel"
    Private Sub btnAtomYaricap_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAtomYaricap.Click
        Dim frm As Form3 = New Form3
        frm.DosyaAc(Application.StartupPath & "\extras\atom_yaricaplari.jpg", "Atom Yar��ap De�erleri")
        frm.Show()
    End Sub

    Private Sub btnElektronegatiflik_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnElektronegatiflik.Click
        Dim frm As Form3 = New Form3
        frm.DosyaAc(Application.StartupPath & "\extras\elektronegatiflik.htm", "Atom Elektronegatiflik De�erleri")
        frm.Show()
    End Sub

    Private Sub btnPeriyodikCetv2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPeriyodikCetv2.Click
        Dim frm As Form3 = New Form3
        frm.DosyaAc(Application.StartupPath & "\extras\periyodik_tablo.htm", "Alternatif Periyodik Cetvel")
        frm.Show()
    End Sub

    Private Sub btnHesapMakinesi_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHesapMakinesi.Click
        Shell("calc.exe")
    End Sub

    Private Sub btnAciklamaEkle_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAciklamaEkle.Click
        Dim frm As Form4 = New Form4
        'Elementler i�in veriler de�i�tirildikten sonra Form Kapan���na AnaForm'daki butonlar yenilenir..
        AddHandler frm.FormClosed, AddressOf ButonlariYenile
        frm.Show()
    End Sub

    Private Sub btnVarsayilanYap_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnVarsayilanYap.Click

        'Element verilerini i�eren XML dosyas� ilk haline d�n��t�r�l�r..Yede�i bulundu�u i�in kopyalama ile kullan�lan dosya ad�nda yedek kaydedilir.
        If System.IO.File.Exists(Application.StartupPath & "\DatabaseOfElements.xml") = True Then
            Kill(Application.StartupPath & "\DatabaseOfElements.xml")
        End If

        FileCopy(Application.StartupPath & "\DefaultDatabaseOfElements.xml", Application.StartupPath & "\DatabaseOfElements.xml")

        'Uygulama ufak bi beklemeye al�n�r..Bunu ne olur ne olmaz diye ekledim. ��nk� dosya kopyalama i�leminin ard�ndan butonlar yeni verilere g�re tekrardan olu�acak ve uygulama kodu h�zl� i�letti�inde XML dosya kopyalanmadan okumaya ge�ilirse hata olu�ur..
        Threading.Thread.Sleep(1000)

        ButonlariOlustur()
        Temizle()
    End Sub

    Private Sub cmbAra_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles cmbAra.KeyDown
        'Enter 'a bas�l�nca arama yap�lmas� sa�lan�r..
        If e.KeyCode = Keys.Enter Then btnAra_Click(sender, e)
    End Sub

    Private Sub btnAra_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAra.Click

        'Aranan s�zc�kteki �n ve arka bo�luklar kesilir..Harfleri k���lt�l�r.
        Dim Ara As String = Trim(Me.cmbAra.Text).ToLower
        'Aranan bulunamad� olarak i�aretlenir..
        Dim ArananVar As Boolean = False

        If Not Ara = "" Then
            Dim ButtonsPnl As Panel

            'Butonlar�n bulundu�u panele eri�iyoruz..
            For Each ctrl As Control In Me.SplitContainer1.Panel1.Controls(1).Controls
                If TypeOf ctrl Is Panel AndAlso ctrl.Name = "Buttons" Then
                    ButtonsPnl = CType(ctrl, Panel)
                    Exit For
                End If
            Next

            'Aranan atom butonu se�ilerek g�r�n�r hale getirilir.Hatta t�klanarak bilgileri g�sterilir..
            For Each btn As ElementalButton In ButtonsPnl.Controls
                If btn.ElementNesnesi.AtomNo.ToString = Ara OrElse btn.ElementNesnesi.Isim.ToString.ToLower = Ara Then
                    'btn.Focus()
                    btn.Select()
                    AtomDetails(btn, e)
                    ArananVar = True
                    Beep()
                    Exit For
                End If
            Next

            'Aranan bulunamazsa msgbox g�sterilir..
            If Not ArananVar = True Then
                MessageBox.Show(Chr(34) & Me.cmbAra.Text & Chr(34) & " atomu bulunamam��t�r.", "Bulunamad�!", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)
                Me.Temizle()
            End If

            ArananVar = False
            'Aranan s�zc�k combobox'a eklenir..
            For Each obj As Object In Me.cmbAra.Items
                'E�er arama yap�lan s�zc�k daha �nceden combobox'a kaydedilmi� ise, yakalan�r."ArananVar" de�i�keni True olarak i�aretlenir..
                If CType(obj, String).ToString = Ara Then
                    ArananVar = True
                    Exit For
                End If
            Next

            'ArananVar de�i�keni True de�lse,aranan s�zc�k daha �nce combobox'a kaydedilmemi� demektir.Aranan kelime combobox'a eklenir..
            If ArananVar = False Then
                Me.cmbAra.Items.Add(Me.cmbAra.Text)
            End If
        End If
    End Sub

    Private Sub cmbAra_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbAra.SelectedIndexChanged
        btnAra_Click(sender, e)
    End Sub

    Private Sub btnHakkinda_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHakkinda.Click
        Dim frm As frmHakkinda = New frmHakkinda
        frm.Text = "Hakk�nda"
        frm.ShowDialog()
    End Sub

    Private Sub btnYardim_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnYardim.Click
        Dim frm As Form3 = New Form3
        frm.DosyaAc(Application.StartupPath & "\Help\Contents.htm", "Yard�m")
        frm.Show()
    End Sub

#End Region

End Class
