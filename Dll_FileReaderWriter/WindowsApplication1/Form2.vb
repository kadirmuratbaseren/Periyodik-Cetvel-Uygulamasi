Public Class Form2

    Private Sub Form2_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Bo� bir Datatable olu�turuyoruz.
        Dim DataTab As DataTable = New DataTable

        '3 tane kolon ekliyoruz.Veri girebilmek i�in..
        DataTab.Columns.Add("Element")
        DataTab.Columns.Add("Agirlik")
        DataTab.Columns.Add("Aciklama")

        'Datagrid 'nin DataSource(DataKayna��) k�sm�na olu�turdu�umuz DataTable'� ayarl�yoruz..
        DataGridView1.DataSource = DataTab
    End Sub

    Private Sub btnKaydet_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnKaydet.Click

        SaveFileDialog1.Filter = "Metin Belgesi(*.txt)|*.txt"

        If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim DosyayaYaz As DLL_FileReaderWriter.WritingFromDataTable
            DosyayaYaz = New DLL_FileReaderWriter.WritingFromDataTable

            Dim DataTab As DataTable = New DataTable
            DataTab = DataGridView1.DataSource

            DosyayaYaz.WriteTEXTFile(DataTab, SaveFileDialog1.FileName)

            MessageBox.Show("Dosyan�z Yaz�ld�..", "��lem Tamamland� !", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

    End Sub

    Private Sub btnXML_Yaz_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnXML_Yaz.Click
        'Sadece uzant�s� *.xml olan dosyalar�n g�r�nmesi sa�lan�r. 
        SaveFileDialog1.Filter = "XML Files(*.xml)|*.xml"

        'Dosya Kaydet penceresi a��l�r ve "OK" butonu ile kaydet denildiyse.
        If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then

            'Dll dosyam�zadaki XML class '�ndan Instance olu�turduk..
            Dim XML_Yaz As DLL_FileReaderWriter.XML = New DLL_FileReaderWriter.XML

            'DataGridView '�m�z�n kayna��n� DataTable olarak ve SaveFileDialog da dosyay� kaydetmek i�in belirtilen konumuda XML dosyay� olu�turacak konum olarak belirttik..(Dll Class da Sub '� yazarken bahsetti�imiz parametreler bunlar� biz olu�turmu�tuk..)
            XML_Yaz.XMLFileWriter(DataGridView1.DataSource, SaveFileDialog1.FileName)

        End If
    End Sub

    Private Sub btnXML_Oku_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnXML_Oku.Click

        Dim DT As DataTable
        OpenFileDialog1.Filter = "XML Files(*.xml)|*.xml"

        If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim XML_Oku As DLL_FileReaderWriter.XML = New DLL_FileReaderWriter.XML
            'Bize �al��t�raca��m�z fonksiyon sonunda DataTable d�nece�i i�in olu�turduk..
            DT = New DataTable
            'Olu�turdu�umuz DataTable 'a ReadXMLFile function '� ile XML dosyas�n� okuyoruz. Parametre olarak okumas�n� istedi�imiz XML dosyan�n konumunu yolluyoruz..
            DT = XML_Oku.ReadXMLFile(OpenFileDialog1.FileName)
            'DataGridView 'in Data kayna��na bize d�nen DataTable '� g�nderiyoruz..
            DataGridView1.DataSource = DT
        End If
    End Sub

End Class