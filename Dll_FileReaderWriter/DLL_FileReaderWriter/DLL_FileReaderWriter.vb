
Imports System.Xml
Imports System.Xml.XmlReader

Public Class WritingToTextFile
    'Belirtilen konumdaki dosyaya metni ekler..
    Public Sub WritingAppendFile(ByVal FilePath As String, ByVal Text As String)
        'Belirtilen konumdaki dosyaya FileStream ile ula��yoruz..
        Dim FS As System.IO.FileStream = New System.IO.FileStream(FilePath, IO.FileMode.Append, IO.FileAccess.Write)
        'StreamWriter ise, FS'da belirtti�miz dosyaya metin eklemesi yapar.

        Dim SW As System.IO.StreamWriter = New System.IO.StreamWriter(FS, System.Text.ASCIIEncoding.GetEncoding("iso-8859-9"))
        '(Dim SW As System.IO.StreamWriter = New System.IO.StreamWriter(FS, System.Text.UTF8Encoding.UTF8) yukardakinin ayn�s�n� yapar..

        'StreamWriter kullanarak metni yaz diyoruz..
        SW.Write(Text)
        'Sonra StreamWriter'� kapat�yoruz.
        SW.Close()
        'Dosyay� FileStream '� kapat�yoruz.
        FS.Close()
    End Sub

    'Dosya olusturulup kodlar�n eklenmesi..
    Public Sub WritingCreateFile(ByVal FilePath As String, ByVal Text As String)
        'Belirtilen konuma str(eklenecek metni alarak) dosya olusturma..
        Dim FileStream As System.IO.FileStream
        FileStream = New System.IO.FileStream(FilePath, IO.FileMode.Create, IO.FileAccess.Write)

        Dim FileWriter As System.IO.StreamWriter
        FileWriter = New System.IO.StreamWriter(FileStream, System.Text.Encoding.UTF8)
        'FileWriter = New System.IO.StreamWriter(FileStream, System.Text.ASCIIEncoding.GetEncoding("ISO-8859-9"))

        FileWriter.Write(Text)

        FileWriter.Close()
        FileStream.Close()
    End Sub

    Public Sub New()

    End Sub
End Class

Public Class FileReading
    Public Function FileRead(ByVal FilePath As String) As String
        Dim Text As String = ""

        Dim FS As System.IO.FileStream = New System.IO.FileStream(FilePath, IO.FileMode.Open, IO.FileAccess.Read)
        Dim SR As System.IO.StreamReader = New System.IO.StreamReader(FS, System.Text.ASCIIEncoding.GetEncoding("iso-8859-1"))

        Text += SR.ReadToEnd

        SR.Close()
        FS.Close()

        Return Text
    End Function

    Public Sub New()

    End Sub
End Class

Public Class WritingFromDataTable

    'DataTable'Dan TextFile i�eri�ine ekleme..
    Public Sub WriteTEXTFile(ByVal SourceTable As DataTable, ByVal FilePath As String, ByVal Toplam As String)
        Dim Statement As String = ""

        'DataTable'�n t�m sat�rlar� i�in d�n�l�r..
        For i As Integer = 0 To SourceTable.Rows.Count - 1
            'Herbir kolon i�in..
            For j As Integer = 0 To SourceTable.Columns.Count - 1
                Statement &= SourceTable.Columns.Item(j).Caption.ToString & " : "
                Statement &= SourceTable.Rows(i)(j).ToString & vbCrLf
            Next
            Statement &= vbCrLf
        Next

        Statement &= vbCrLf & "=============================" & vbCrLf & "      Toplam : " & Toplam & vbCrLf

        'Belirtilen konuma Text dosya olusturma..
        Dim FC As WritingToTextFile = New WritingToTextFile
        FC.WritingCreateFile(FilePath, Statement)
    End Sub

    'DataTable'Dan HTML i�eri�i olu�turma..
    Public Sub WriteHTMLFile(ByVal SourceTable As DataTable, ByVal FilePath As String, ByVal TableCaption As String, ByVal CSSCodePath As String, ByVal JavaCodePath As String, ByVal Toplam As Single)

        '======================
        'Read CSS Code
        '======================
        Dim ReadCSS As FileReading = New FileReading
        Dim CssCode As String = ReadCSS.FileRead(CSSCodePath)

        '======================
        'Read Java Code
        '======================
        Dim ReadJava As FileReading = New FileReading
        Dim JavaCode As String = ReadJava.FileRead(JavaCodePath)

        '
        'First Statment(HTML File Information)
        '
        Dim TopStatment As String = "<!DOCTYPE html PUBLIC " & Chr(34) & "-//W3C//DTD XHTML 1.0 Transitional//TR" & Chr(34) & " " & Chr(34) & "http://www.w3.org//TR/xhtml1/DTD/xhtml1-transitional.dtd" & Chr(34) & ">" & vbCrLf & _
"<html xmlns=" & Chr(34) & "http://www.w3.org/1999/xhtml" & Chr(34) & ">" & vbCrLf & _
"<head>" & vbCrLf & _
"    <title>" & TableCaption & "</title>" & vbCrLf & _
"<meta http-equiv=" & Chr(34) & "content-type" & Chr(34) & vbCrLf & _
"    content=" & Chr(34) & "text/html; charset=iso-8859-1" & Chr(34) & " />" & vbCrLf & _
"<Style>" & vbCrLf & CssCode & vbCrLf & "</Style>" & vbCrLf & _
"</head>" & vbCrLf & _
"<body>" & vbCrLf & _
"<table class=" & Chr(34) & "datatable" & Chr(34) & ">" & vbCrLf & vbCrLf & _
"  <caption>" & TableCaption & "</caption>" & vbCrLf & _
"<!--Tablodaki kolon basliklari..-->"

        '
        'SecondStatment(Insert Columns Caption)
        '
        Dim MiddleStatment As String = "        <tr>" & vbCrLf
        For i As Integer = 0 To SourceTable.Columns.Count - 1
            MiddleStatment &= "    <th scope=" & Chr(34) & "col" & Chr(34) & ">" & SourceTable.Columns.Item(i).Caption.ToString & "</th>" & vbCrLf
        Next
        MiddleStatment &= "        </tr>" & vbCrLf

        '
        'ThirdStatment(Drawing Line)
        '
        Dim ThirdStatment As String = ""
        For i As Integer = 0 To SourceTable.Rows.Count
            '==========================
            'Sat�r i�in Comment yazma..
            '==========================
            ThirdStatment &= "<!--" & i & ".sat�r -->" & vbCrLf
            '==========================
            'Sat�r a��r� renkendirme..
            '==========================
            If i Mod 2 = 0 Then
                ThirdStatment &= "       <tr>" & vbCrLf
            Else
                ThirdStatment &= "       <tr class=" & Chr(34) & "altrow_" & Chr(34) & ">" & vbCrLf
            End If
            '==========================
            'Sat�rlar� Yazma..
            '==========================
            For j As Integer = 0 To SourceTable.Columns.Count - 1
                If Not i = SourceTable.Rows.Count Then
                    ThirdStatment &= "    <td>" & SourceTable.Rows(i)(j).ToString & "</td>" & vbCrLf
                Else
                    If j = SourceTable.Columns.Count - 2 Then ThirdStatment &= "    <td style=" & Chr(34) & " font-style:italic; font-weight:bold;" & Chr(34) & ">" & "TOPLAM : " & "</td>" & vbCrLf
                    If j = SourceTable.Columns.Count - 1 Then ThirdStatment &= "    <td style=" & Chr(34) & " font-style:italic; font-weight:bold;" & Chr(34) & ">" & Toplam.ToString & "</td>" & vbCrLf
                    If j < SourceTable.Columns.Count - 2 Then ThirdStatment &= "    <td></td>" & vbCrLf
                End If
            Next
            'Sat�r� sonland�rma..
            ThirdStatment &= "       </tr>" & vbCrLf
        Next

        '
        'EndStatement(Closing Statement)
        '
        Dim EndSatetment As String = "<!--Table Body HTML kapan�� cumleleri..-->" & vbCrLf
        EndSatetment &= "    </table>" & vbCrLf & vbCrLf

        '==========================
        'JavaScript Kodu ve Kapan��
        '==========================
        EndSatetment &= JavaCode & vbCrLf & _
        "</body>" & vbCrLf & _
        "</html>" & vbCrLf

        Dim FC As WritingToTextFile = New WritingToTextFile
        FC.WritingCreateFile(FilePath, TopStatment & MiddleStatment & ThirdStatment & EndSatetment)
    End Sub

    'Instance olusturma..
    Public Sub New()

    End Sub
End Class

Public Class XML

    Public Sub New()

    End Sub

    'DataTable'dan XML File olu�turacak Sub..
    Public Sub XMLFileWriter(ByVal Source As DataTable, ByVal TargetPath As String)

        Dim XTW As XmlTextWriter = New XmlTextWriter(TargetPath, System.Text.Encoding.GetEncoding("ISO-8859-9"))


        Dim Caption As String = ""
        Dim Value As String = ""

        XTW.Formatting = Formatting.Indented

        XTW.WriteStartDocument()

        XTW.WriteStartElement("Main", "")

        'Herbir sat�r i�in d�n�l�r..
        For i As Integer = 0 To Source.Rows.Count - 1

            'Her sat�r i�in yukardaki �rnekte oldugu gibi "row" tag a��l�r.Row tag'�n� a�t�k ama kapan��� kolonlar� okuduktan sonra olacak..
            XTW.WriteStartElement("Row")
            'Bu k�s�m iste�e ba�l�d�r ben herbir sat�r i�in birde Attribute dedi�imiz XML de,row elementine "rowID" �zelli�i ekliyorum.
            XTW.WriteStartAttribute("rowID", i)

            'Yukarda her sat�r i�in d�nerken; her sat�r i�in her kolon i�in d�n�l�r..
            For k As Integer = 0 To Source.Columns.Count - 1
                'E�er kolon ba�l���nda bo�luk karakteri varsa bu �ekilde bir tag tan�mlamam�z hataya yol a�aca��ndan onu kald�rmam�z gerekiyor..
                'E�er kolon ba�l���nda bo�luk varsa;
                If Not Source.Columns(k).Caption.ToString.IndexOf(" ") = -1 Then
                    'Ba�l��� s�f�rla..
                    Caption = ""
                    'Ba�l�ktaki herbir karakter i�in d�n!
                    For Each chr As Char In Source.Columns(k).Caption.ToString.ToCharArray
                        'E�er okunan karakter bo�luk de�ilse "Caption" 'a ekle.Bo�luk ise ekleme yapmayacakt�r.B�ylece �rne�in; "G�nl�k Sat��" �eklinde bir ba�l�k "G�nl�kSat��" olacakt�r..
                        If Not Char.IsWhiteSpace(chr) = True Then
                            Caption &= chr
                        End If
                    Next
                Else
                    'E�er kolon ad�nda bir bo�luk karakteri yoksa o zaman aynen al�nacakt�r..Asl�nda bu Else k�sm� yaz�lmay�p her halukarda ForEach ile t�m karakterler kontrol edilip Caption olu�turulabilirdi.Ama bi d���n�n �ok fazla kolon varsa ve e�er bo�luk karakteri i�ermiyorsa neden bidaha t�m karakterleri i�in bo�luk varm� diye sorgulayal�m!Dimi? :)
                    Caption = Source.Columns(k).Caption.ToString
                End If

                'DataTable'dan o anki okudu�umuz sat�rda ,okudu�umuz kolonun de�eri "Value" 'ya aktar�l�r..
                Value = Source.Rows(i)(k).ToString
                'XmlTextWriter'�n WriteElementString metodu ile Element(Caption) ve De�eri(Value) eklenir..
                XTW.WriteElementString(Caption, Value)
            Next
            'Yukarda Row tag'� a�m��t�k ve i�te yazd���m�z sat�r i�in row tag'�n� kapat�yoruz.D�ng� DataTable'daki sat�r say�s� kadar devam eder ve sat�rlar tek tek "row" tag'� a��larak XML dosyaya yaz�l�r..
            XTW.WriteEndElement()
        Next
        'Root tag de kapat�l�r..
        XTW.WriteEndElement()
        'XML d�k�man yaz�m� sonland�r�l�r..Bu k�sm� sak�n unutmay�n yoksa XML dosya �al��maz.XML d�k�man yazarken mutlaka "WriteStartDocument" ile ba�lar ve "WriteEndDocument" ile biter.
        XTW.WriteEndDocument()
        'XML dosya kapat�l�r..Bir XML dosya a��kken onun �zerinde ba�ka bir i�lem yap�lamaz..
        XTW.Close()
    End Sub

    'Belirtilen konumdaki XML dosyadan DataTable olu�tural�m.
    Public Function ReadXMLFile(ByVal TargetPath As String) As DataTable

        'Function sonunda geri d�nd�r�lecek DataTable..
        Dim DT As DataTable = New DataTable

        'Yeni bir xmlDocument nesnesi olu�turulur..
        Dim myXmlDoc As XmlDocument = New XmlDocument()
        'Okunacak Xml d�k�man� myXmlDoc nesnesine, konumu belirtilerek atan�r
        myXmlDoc.Load(TargetPath)

        'Root tag 'i "RootNode" nesnesine atan�yor.
        Dim RootNode As XmlNode = myXmlDoc.ChildNodes(1)

        'A�a��daki d�ng� i�inde her seferinde kolon olu�mas�n diye KolonlarOlustu adl� de�i�ken False oldu�u s�rece kolon olu�acak. Hemen ilk kolon olu�mas�nda biz bu de�eri True yapacaz ve bidaha kolon olu�mayacak..
        Dim KolonlarOlustu As Boolean = False

        'Herbir RootNode alt�ndaki ChildNode i�in..
        'Burdaki ChildNode'lar�n her biri Datatable'�m�z 
        'i�in Row 'lar oluyor..
        For Each NodeSatir As XmlNode In RootNode.ChildNodes

            Dim DR As DataRow = DT.NewRow

            'Herbir Row alt�ndaki child node i�in..
            For i As Integer = 0 To NodeSatir.ChildNodes.Count - 1
                'Herbir RootNode alt�ndaki ChildNode(sat�rlar) alt�ndaki ChildNode(kolonlar) i�in.
                If KolonlarOlustu = False Then
                    'Bu k�s�m sadece node1'in childNode say�s� kadar d�n�yor.
                    'B�ylece DataTable i�in kolonlar olu�uyor..
                    DT.Columns.Add(NodeSatir.ChildNodes(i).Name)
                End If
                'Olu�turulan DataRow 'a XML'deki de�erler aktar�l�r..
                DR(i) = NodeSatir.ChildNodes(i).InnerText
            Next

            'Tekrar Kolonlar olu�turulmas�n diye de�i�ken "True" yap�l�r..
            KolonlarOlustu = True

            'DataRow(DataSat�r�) DataTable'a eklenir..
            DT.Rows.Add(DR)
        Next

        'DataTable g�nderilir..
        Return DT
    End Function
End Class

