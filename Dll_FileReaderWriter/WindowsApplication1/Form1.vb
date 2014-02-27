Public Class Form1

    Private Sub btnKaydet_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnKaydetClass.Click

        'Kullan�c�ya birka� dosya uzant�s� se�me imkan� verdik.Bu k�sm� siz zenginle�tirebilirsiniz..Unutmay�ki uzant�s� ne olursa olsun yazd�rd���n�z dosya "Not Defteri" ile kolayca okunabilir.
        SaveFileDialog1.Filter = "Metin Belgesi(*.txt)|*.txt|Word Belgesi(*.doc)|*.doc|Rastgele Belge(*.abc)|*.abc"

        'SaveFileDialog1'in ShowDialog metodu ile Dosya Kaydetme penceresi a�t�r�yorum..
        If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            'Olu�turdu�umuz Class'dan instance(nesne) olu�turuyoruz.
            Dim cls As DLL_FileReaderWriter.WritingToTextFile = New DLL_FileReaderWriter.WritingToTextFile

            'WritingAppendFile metodunu �a��rarak yazd�rma,WritingCreateFile metodunuda �a��rablirdik.Tamamen amac�n�za ba�l� ikisininde ne i�e yarad��� biliyorsunuz..iki metotta da Dosya konumunu ve Metni g�nderiyorum.
            cls.WritingCreateFile(SaveFileDialog1.FileName, RichTextBox1.Text)
        End If

    End Sub

    Private Sub btnOkuClass_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOkuClass.Click
        'Kullan�c�n�n a�abilece�i dosyalar� k�s�tlayabilirsiniz..
        OpenFileDialog1.Filter = "Metin Belgesi(*.txt)|*.txt|Word Belgesi(*.doc)|*.doc|Rastgele Belge(*.abc)|*.abc"
        '�lk filter'�n se�ilmesini sa�lad�m.Yani *.txt lerin g�r�nmesini..
        OpenFileDialog1.FilterIndex = 0

        'OpenFileDialog1'in ShowDialog metodu ile Dosya a�ma penceresi a�t�r�yorum..
        If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            'Olu�turdu�umuz Class'dan instance(nesne) olu�turuyoruz.
            Dim cls As DLL_FileReaderWriter.FileReading = New DLL_FileReaderWriter.FileReading

            'Kullan�c�n�n OpenFileDialog1 penceresinden se�ti�i dosyan�n i�eri�ini yaratt���m�z Class'daki "FileRead" metodu ile okutuyoruz ve RichTextBox1.Text 'e aktar�yoruz.
            RichTextBox1.Text = cls.FileRead(OpenFileDialog1.FileName)
        End If

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        SaveFileDialog1.Filter = "Metin Belgesi(*.txt)|*.txt|Word Belgesi(*.doc)|*.doc|Rastgele Belge(*.abc)|*.abc"

        'SaveFileDialog1'in ShowDialog metodu ile Dosya Kaydetme penceresi a�t�r�yorum..
        If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            RichTextBox1.SaveFile(SaveFileDialog1.FileName, RichTextBoxStreamType.PlainText)
        End If

    End Sub
End Class
