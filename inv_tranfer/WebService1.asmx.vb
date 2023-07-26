Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports Oracle.DataAccess.Client
Imports System.IO



Imports PdfSharp
Imports PdfSharp.Drawing
Imports PdfSharp.Pdf
Imports System.Data
Imports System.Web.Configuration
Imports PdfSharp.Drawing.BarCodes

'Imports inv_tranfer.mail

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class WebService1

    Inherits System.Web.Services.WebService
    Public errlocation As String = AppDomain.CurrentDomain.BaseDirectory & "error\"


    <WebMethod()>
    Public Function MainProcess(ByVal inv_tran As String) As String


        'Dim objOra As New OracleConn
        'Dim dtSQL001 As New DataTable
        'Dim timeStr As String = DateTime.Now.ToString("HH:mm:ss")
        'Dim formno As String = ""

        Try
            Dim config_spirit As String = "SCHM_EUC_MTL"
            Dim con_str As String = WebConfigurationManager.ConnectionStrings.Item("IMConnectionString").ToString
            Dim location As String
            ' If Left(inv_tran, 2) = "TF" Then
            location = WebConfigurationManager.ConnectionStrings.Item("PATH").ToString

            'Else
            'location = WebConfigurationManager.ConnectionStrings.Item("PATH_FOR_RETURN").ToString

            ' End If

            'Dim location As String = WebConfigurationManager.ConnectionStrings.Item("PATH").ToString
            'Dim cYear = Year(Now)
            'Dim cYear_Str = "2020"
            'Dim numm = cYear_Str.Substring(2, 1)
            'Dim orgMarking = "6D"
            'Dim mYear = orgMarking.Substring(0, 1)
            'Dim mMonth = orgMarking.Substring(1, 1)

            Dim TargetFile As String = location
            Dim Conn As New OracleConnection
            Dim cmd As New OracleCommand
            Conn.ConnectionString = con_str




            Conn.Open()




            cmd.Connection = Conn
            cmd.CommandText = "Select IMFR_UD_TRUCK_NO as truck from IMFR_UT_MTLLSL001 " +
                              " where IMFR_UD_INV_TRAN ='" & inv_tran & "' "
            cmd.CommandType = CommandType.Text
            Dim tbl_truck = cmd.ExecuteReader()
            Dim get_truck As DataTable = New DataTable()
            get_truck.Load(tbl_truck)





            cmd.Connection = Conn
            cmd.CommandText = "Select rownum ,IMFR_UD_MURATA_TYPE from (Select distinct IMFR_UD_MURATA_TYPE from imfr_ut_mtlgls0004 
                               where IMFR_UD_INV_NO ='" & inv_tran & "' and IMFR_UD_PKG_ID != 'CART' ORDER BY Imfr_Ud_Murata_Type)"
            cmd.CommandType = CommandType.Text
            Dim ex_sum = cmd.ExecuteReader()
            Dim tbl_sum As DataTable = New DataTable()
            tbl_sum.Load(ex_sum)

            cmd.Connection = Conn
            cmd.CommandText = "Select rownum ,IMFR_UD_MURATA_TYPE from (Select distinct IMFR_UD_MURATA_TYPE from imfr_ut_mtlgls0004 " +
                               "where IMFR_UD_INV_NO ='" & inv_tran & "' and IMFR_UD_PKG_ID = 'CART' ORDER BY IMFR_UD_MURATA_TYPE)"
            cmd.CommandType = CommandType.Text
            Dim ex_sum_C = cmd.ExecuteReader()
            Dim tbl_sum_C As DataTable = New DataTable()
            tbl_sum_C.Load(ex_sum_C)



            cmd.Connection = Conn
            If Left(inv_tran, 2) = "TF" Then

                cmd.CommandText = "SELECT IMFR_UD_MURATA_TYPE,IMFR_UD_RETURN_NO ,IMFR_UD_TARIFF,SUM(IMFR_UD_QTY) as qty ,IMFR_UD_PRICE,SUM(IMFR_UD_QTY*IMFR_UD_PRICE) as amount
                               FROM IMFR_UT_MTLGLS0004 WHERE IMFR_UD_INV_NO = '" & inv_tran & "' AND IMFR_UD_PKG_ID != 'CART'  
                               Group by IMFR_UD_MURATA_TYPE,IMFR_UD_PRICE,IMFR_UD_TARIFF  ,IMFR_UD_RETURN_NO
							   ORDER BY Imfr_Ud_Murata_Type"
            Else
                cmd.CommandText = " SELECT IMFR_UD_MURATATYPE as IMFR_UD_MURATA_TYPE ,IMFR_UD_RETURN_NO ,IMFR_UD_TARIFF_DES as IMFR_UD_TARIFF ,SUM(IMFR_UD_PICK_UP_QTY) as qty ,IMFR_UD_PRICE, SUM(IMFR_UD_PICK_UP_QTY*IMFR_UD_PRICE)  as amount
                                    FROM IMFR_UT_MTLGLS0002 WHERE Imfr_Ud_Inv_Rtn = '" & inv_tran & "' AND IMFR_UD_PKG_ID != 'CART'  
                                    Group By IMFR_UD_MURATATYPE,Imfr_Ud_Price,IMFR_UD_TARIFF_DES  ,Imfr_Ud_Return_No
							        ORDER BY IMFR_UD_MURATATYPE"
            End If


            cmd.CommandType = CommandType.Text
            Dim ex_tye_not_c = cmd.ExecuteReader()
            Dim tbl_tye_not_c As DataTable = New DataTable()
            tbl_tye_not_c.Load(ex_tye_not_c)




            cmd.Connection = Conn
            '  If Left(inv_tran, 2) = "TF" Then
            cmd.CommandText = "SELECT SUM(IMFR_UD_QTY) as qty ,SUM(IMFR_UD_PRICE), SUM(IMFR_UD_QTY*IMFR_UD_PRICE) as amount  FROM IMFR_UT_MTLGLS0004 WHERE IMFR_UD_INV_NO = '" & inv_tran & "'  AND IMFR_UD_PKG_ID = 'CART' AND substr(Imfr_Ud_Murata_Type,1,1) = 'P'"
            'Else
            '.CommandText = "SELECT SUM(IMFR_UD_PICK_UP_QTY) as qty ,SUM(IMFR_UD_PRICE), SUM(IMFR_UD_PICK_UP_QTY*IMFR_UD_PRICE) as amount FROM IMFR_UT_MTLGLS0002 WHERE Imfr_Ud_Inv_Rtn = '" & inv_tran & "'  AND IMFR_UD_PKG_ID = 'CART' AND substr(IMFR_UD_MURATATYPE,1,1) = 'P'"
            'End If

            cmd.CommandType = CommandType.Text
            Dim ex_type_head_p = cmd.ExecuteReader()
            Dim tbl_type_head_p As DataTable = New DataTable()
            tbl_type_head_p.Load(ex_type_head_p)


            cmd.Connection = Conn

            ' If Left(inv_tran, 2) = "TF" Then
            cmd.CommandText = "SELECT SUM(IMFR_UD_QTY) as qty , Max(IMFR_UD_PRICE) as price, SUM(IMFR_UD_QTY*IMFR_UD_PRICE) as amount FROM IMFR_UT_MTLGLS0004 WHERE IMFR_UD_INV_NO = '" & inv_tran & "'  AND IMFR_UD_PKG_ID = 'CART' AND (substr(Imfr_Ud_Murata_Type,1,1) = 'W' or substr(Imfr_Ud_Murata_Type,1,1) = 'C') "
            'Else
            '.CommandText = "SELECT SUM(IMFR_UD_PICK_UP_QTY) as qty ,SUM(IMFR_UD_PRICE) , SUM(IMFR_UD_PICK_UP_QTY*IMFR_UD_PRICE) as amount FROM IMFR_UT_MTLGLS0002 WHERE Imfr_Ud_Inv_Rtn = '" & inv_tran & "'  AND IMFR_UD_PKG_ID = 'CART' AND (substr(IMFR_UD_MURATATYPE,1,1) = 'W' or substr(IMFR_UD_MURATATYPE,1,1) = 'C') "
            ' End If

            cmd.CommandType = CommandType.Text
            Dim ex_type_head_c = cmd.ExecuteReader()
            Dim tbl_type_head_c As DataTable = New DataTable()
            tbl_type_head_c.Load(ex_type_head_c)




            cmd.Connection = Conn

            ' If Left(inv_tran, 2) = "TF" Then
            cmd.CommandText = "SELECT to_char(IMFR_UD_EDP_DATE, 'dd/mm/yyyy') as date_return  FROM IMFR_UT_MTLGLS0004 WHERE IMFR_UD_INV_NO = '" & inv_tran & "'  AND IMFR_UD_PKG_ID = 'CART' "
            'Else
            '.CommandText = "SELECT SUM(IMFR_UD_PICK_UP_QTY) as qty ,SUM(IMFR_UD_PRICE) , SUM(IMFR_UD_PICK_UP_QTY*IMFR_UD_PRICE) as amount FROM IMFR_UT_MTLGLS0002 WHERE Imfr_Ud_Inv_Rtn = '" & inv_tran & "'  AND IMFR_UD_PKG_ID = 'CART' AND (substr(IMFR_UD_MURATATYPE,1,1) = 'W' or substr(IMFR_UD_MURATATYPE,1,1) = 'C') "
            ' End If

            cmd.CommandType = CommandType.Text
            Dim get_date_return = cmd.ExecuteReader()
            Dim tbl_date_return As DataTable = New DataTable()
            tbl_date_return.Load(get_date_return)




            Conn.Close()





            Dim con_spirit As String = WebConfigurationManager.ConnectionStrings.Item("Connectionspirit").ToString
            Conn.ConnectionString = con_spirit

            Conn.Open()

            cmd.Connection = Conn
            If Left(inv_tran, 2) = "TF" Then
                cmd.CommandText = "Select CD12064,DH35137_01,DH35137_02,DH10637_01,DH10637_02,
                                DH10637_03,DH10637_04 from " & config_spirit & ".SV6030 WHERE CD12064 ='MTL'"
            Else
                cmd.CommandText = "Select CD12064,DH35137_01,DH35137_02,DH10637_01,DH10637_02," +
                       "DH10637_03,DH10637_04 from " & config_spirit & ".SV6030 WHERE CD12064 ='HUB'"
            End If

            cmd.CommandType = CommandType.Text
            Dim ex_mtl = cmd.ExecuteReader()
            Dim tbl_mtl As DataTable = New DataTable()
            tbl_mtl.Load(ex_mtl)

            If Left(inv_tran, 2) = "TF" Then
                cmd.CommandText = "Select CD12064,DH35137_01,DH35137_02,DH10637_01,DH10637_02,DH10637_03,DH10637_04 from " & config_spirit & ".SV6030 WHERE  CD12064 ='HUB' "
            Else
                cmd.CommandText = "Select CD12064,DH35137_01,DH35137_02,DH10637_01,DH10637_02,DH10637_03,DH10637_04 from " & config_spirit & ".SV6030 WHERE  CD12064 ='MTL' "
            End If
            cmd.CommandType = CommandType.Text
            Dim ex_hub = cmd.ExecuteReader()
            Dim tbl_hub As DataTable = New DataTable()
            tbl_hub.Load(ex_hub)

            If Left(inv_tran, 2) = "TF" Then

                cmd.CommandText = "select KB10990 as KB10990 from " & config_spirit & ".SV6129 WHERE NO10667= 'FOB' AND DH10946= 'SBM'"
            Else
                cmd.CommandText = "select KB10990 from " & config_spirit & ".SV6129 WHERE NO10667= 'FOL' AND DH10946= 'SBM'"
            End If
            cmd.CommandType = CommandType.Text
            Dim ex_bangkok = cmd.ExecuteReader()
            Dim tbl_bangkok As DataTable = New DataTable()
            tbl_bangkok.Load(ex_bangkok)

            Conn.Close()


            Dim con_logistic As String = WebConfigurationManager.ConnectionStrings.Item("ConnectionLogistic").ToString
            Conn.ConnectionString = con_logistic
            Conn.Open()
            'CheckBox section12
            cmd.CommandText = "select distinct EXW from imfr_ut_mtllsl001@im_test left join price_master on IMFR_UD_MURATA_TYPE = MURATATYPE
                               where imfr_ud_inv_tran =  '" & inv_tran & "' "


            cmd.CommandType = CommandType.Text
            Dim ex_section12 = cmd.ExecuteReader()
            Dim tbl_section12 As DataTable = New DataTable()
            tbl_section12.Load(ex_section12)

            Conn.Close()






            Dim pdf As PdfDocument = New PdfDocument
            pdf.Info.Title = "KEEPING LIST"
            Dim pdfPage As PdfPage = pdf.AddPage
            pdfPage.Size = PageSize.A4
            'PdfPage.Orientation = PageOrientation.Landscape
            Dim graph As XGraphics = XGraphics.FromPdfPage(pdfPage, XGraphicsPdfPageOptions.Append)
            Dim fontHead As XFont = New XFont("Courier New", 20, XFontStyle.Bold)
            Dim fontHead2 As XFont = New XFont("Courier New", 12, XFontStyle.Bold)
            'Dim fontHead2 As XFont = New XFont("CODE39", 12, XFontStyle.Bold)
            Dim font As XFont = New XFont("Courier New", 8.5, XFontStyle.Bold)
            Dim smallfont As XFont = New XFont("Courier New", 7, XFontStyle.Bold)
            Dim fontFoot As XFont = New XFont("Courier New", 14, XFontStyle.Bold)
            Dim smallfontNor As XFont = New XFont("Courier New", 7, XFontStyle.Bold)
            Dim tmpPage = ""

            Dim y As Integer
            ' Dim sum As Double

            Dim count As Integer = 1
            Dim sum_qty As Double
            Dim sum_amount As Double
            Dim sum_price As Double




            Dim image_logo As XImage = XImage.FromFile("\\163.50.57.11\wwwroot$\GLS004\image\logo.png")

            Dim image_stramp As XImage = XImage.FromFile("\\163.50.57.11\wwwroot$\GLS004\image\stamp.png")




            For i = 0 To tbl_sum.Rows.Count - 1

                If i Mod 9 = 0 Then 'แต่ละหน้ามี 9 บรรทัด
                    If tmpPage <> "" Then
                        count = count + 1
                        pdfPage = pdf.AddPage


                        pdfPage.Size = PageSize.A4

                        graph = XGraphics.FromPdfPage(pdfPage)


                    End If

                    graph.DrawImage(image_logo, 30, 20, 100, 30)

                    graph.DrawImage(image_stramp, 400, 730, 70, 72)

                    graph.DrawString("INVOICE", fontHead, XBrushes.Black,
                        New XRect(270, -800, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString("INVOICE NO : ", smallfont, XBrushes.Black,
                    New XRect(470, -800, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString((inv_tran).ToString, font, XBrushes.Black,
                    New XRect(520, -800, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString("_____________", font, XBrushes.Black,
                    New XRect(520, -800, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString("DATE : ", smallfont, XBrushes.Black,
                    New XRect(470, -790, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    If Left(inv_tran, 2) = "TF" Then
                        graph.DrawString(Now.ToString("dd'/'MM'/'yyyy"), font, XBrushes.Black,
                        New XRect(520, -790, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)
                    Else
                        graph.DrawString(tbl_date_return.Rows(0)("date_return"), font, XBrushes.Black,
                        New XRect(520, -790, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)
                    End If


                    graph.DrawString("_____________", font, XBrushes.Black,
                    New XRect(520, -790, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)




                    graph.DrawString(tbl_mtl.Rows(0)("DH35137_01").ToString, font, XBrushes.Black,
                    New XRect(20, -760, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    'ship from //////////////////// 
                    graph.DrawString("SHIP FORM: ", fontHead2, XBrushes.Black,
                    New XRect(20, -740, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString(tbl_mtl.Rows(0)("DH35137_01").ToString, font, XBrushes.Black,
                     New XRect(40, -725, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString("___________________________________________", font, XBrushes.Black,
                    New XRect(40, -725, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString(tbl_mtl.Rows(0)("DH35137_02").ToString, font, XBrushes.Black,
                    New XRect(40, -713, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString("___________________________________________", font, XBrushes.Black,
                    New XRect(40, -713, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString(tbl_mtl.Rows(0)("DH10637_01").ToString, font, XBrushes.Black,
                    New XRect(40, -703, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString("___________________________________________", font, XBrushes.Black,
                    New XRect(40, -703, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString(tbl_mtl.Rows(0)("DH10637_02").ToString, font, XBrushes.Black,
                    New XRect(40, -693, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString("___________________________________________", font, XBrushes.Black,
                    New XRect(40, -693, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)


                    graph.DrawString(tbl_mtl.Rows(0)("DH10637_03").ToString, font, XBrushes.Black,
                    New XRect(40, -683, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString("___________________________________________", font, XBrushes.Black,
                    New XRect(40, -683, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString("CONSIGNEE: ", fontHead2, XBrushes.Black,
                    New XRect(20, -663, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    If Left(inv_tran, 2) = "TF" Then
                        graph.DrawString(tbl_mtl.Rows(0)("DH35137_01").ToString, font, XBrushes.Black,
                        New XRect(40, -655, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        graph.DrawString("___________________________________________", font, XBrushes.Black,
                        New XRect(40, -655, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        graph.DrawString(tbl_mtl.Rows(0)("DH35137_02").ToString, font, XBrushes.Black,
                        New XRect(40, -645, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        graph.DrawString("___________________________________________", font, XBrushes.Black,
                        New XRect(40, -645, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        graph.DrawString(tbl_mtl.Rows(0)("DH10637_01").ToString, font, XBrushes.Black,
                        New XRect(40, -635, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        graph.DrawString("___________________________________________", font, XBrushes.Black,
                        New XRect(40, -635, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        graph.DrawString(tbl_mtl.Rows(0)("DH10637_02").ToString, font, XBrushes.Black,
                        New XRect(40, -625, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        graph.DrawString("___________________________________________", font, XBrushes.Black,
                        New XRect(40, -625, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        graph.DrawString(tbl_mtl.Rows(0)("DH10637_03").ToString, font, XBrushes.Black,
                        New XRect(40, -615, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        graph.DrawString("___________________________________________", font, XBrushes.Black,
                        New XRect(40, -615, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)
                    Else
                        graph.DrawString(tbl_hub.Rows(0)("DH35137_01").ToString, font, XBrushes.Black,
                        New XRect(40, -655, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        graph.DrawString("___________________________________________", font, XBrushes.Black,
                        New XRect(40, -655, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        graph.DrawString(tbl_hub.Rows(0)("DH35137_02").ToString, font, XBrushes.Black,
                        New XRect(40, -645, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        graph.DrawString("___________________________________________", font, XBrushes.Black,
                        New XRect(40, -645, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        graph.DrawString(tbl_hub.Rows(0)("DH10637_01").ToString, font, XBrushes.Black,
                        New XRect(40, -635, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        graph.DrawString("___________________________________________", font, XBrushes.Black,
                        New XRect(40, -635, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        graph.DrawString(tbl_hub.Rows(0)("DH10637_02").ToString, font, XBrushes.Black,
                        New XRect(40, -625, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        graph.DrawString("___________________________________________", font, XBrushes.Black,
                        New XRect(40, -625, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        graph.DrawString(tbl_hub.Rows(0)("DH10637_03").ToString, font, XBrushes.Black,
                        New XRect(40, -615, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        graph.DrawString("___________________________________________", font, XBrushes.Black,
                        New XRect(40, -615, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    End If



                    'ship to //////////////////// 
                    graph.DrawString("SHIP TO: ", fontHead2, XBrushes.Black,
                        New XRect(20, -595, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString(tbl_hub.Rows(0)("DH35137_01").ToString, font, XBrushes.Black,
                    New XRect(40, -586, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString("___________________________________________", font, XBrushes.Black,
                        New XRect(40, -586, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString(tbl_hub.Rows(0)("DH35137_02").ToString, font, XBrushes.Black,
                        New XRect(40, -576, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString("___________________________________________", font, XBrushes.Black,
                        New XRect(40, -576, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString(tbl_hub.Rows(0)("DH10637_01").ToString, font, XBrushes.Black,
                        New XRect(40, -566, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString("___________________________________________", font, XBrushes.Black,
                        New XRect(40, -566, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString(tbl_hub.Rows(0)("DH10637_02").ToString, font, XBrushes.Black,
                        New XRect(40, -556, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString("___________________________________________", font, XBrushes.Black,
                        New XRect(40, -556, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString(tbl_hub.Rows(0)("DH10637_03").ToString, font, XBrushes.Black,
                        New XRect(40, -546, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString("___________________________________________", font, XBrushes.Black,
                        New XRect(40, -546, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)


                    ' DH35137_01,DH35137_02,DH10637_01,DH10637_02,DH10637_03,DH10637_04


                    'SAILING NO OR ABOUT //////////////////// 
                    graph.DrawString("SAILING NO OR ABOUT : ", smallfont, XBrushes.Black,
                        New XRect(330, -740, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)
                    If Left(inv_tran, 2) = "TF" Then
                        graph.DrawString(Now.ToString("dd'/'MM'/'yyyy"), font, XBrushes.Black,
                      New XRect(330, -730, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)
                    Else
                        graph.DrawString(tbl_date_return.Rows(0)("date_return"), font, XBrushes.Black,
                     New XRect(330, -730, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    End If


                    graph.DrawString("________________", font, XBrushes.Black,
                        New XRect(330, -730, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)
                    '/////////////////////////////////
                    graph.DrawString("FROM : ", smallfont, XBrushes.Black,
                        New XRect(330, -710, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString(tbl_mtl.Rows(0)("DH10637_04").ToString, font, XBrushes.Black,
                     New XRect(330, -700, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)


                    graph.DrawString("________________", font, XBrushes.Black,
                        New XRect(330, -700, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)
                    '//////////////////////////////////////
                    graph.DrawString("PAYMENT(L/C NO)", smallfont, XBrushes.Black,
                    New XRect(330, -680, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString("NO COMMERCIAL VALUE(VALUE FOR CUSTOMS PURPOSE ONLY)", smallfontNor, XBrushes.Black,
                        New XRect(330, -670, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString("____________________________________________________", font, XBrushes.Black,
                        New XRect(330, -670, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)
                    '/////////////////////////////////////////
                    graph.DrawString("INCOTERM : ", smallfont, XBrushes.Black,
                    New XRect(330, -650, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    If (Not IsDBNull(tbl_section12.Rows(0)("EXW"))) Then
                        graph.DrawString(tbl_section12.Rows(0)("EXW").ToString & " BANGKOK, THAILAND", font, XBrushes.Black,
                        New XRect(330, -640, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)
                    Else
                        graph.DrawString(tbl_bangkok.Rows(0)("KB10990").ToString, font, XBrushes.Black,
                        New XRect(330, -640, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)
                    End If



                    graph.DrawString("___________________________________", font, XBrushes.Black,
                    New XRect(330, -640, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)
                        '//////////////////////////////////////
                        graph.DrawString("REMARK : ", smallfont, XBrushes.Black,
                    New XRect(330, -620, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)
                        If Left(inv_tran, 2) = "TF" Then
                            graph.DrawString("INTERNAL TRANSFER FROM MURATA NRIE-IEAT FZ ", font, XBrushes.Black,
                            New XRect(330, -610, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)
                            graph.DrawString("TO SBIA FZ", font, XBrushes.Black,
                            New XRect(330, -600, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)
                        Else
                            graph.DrawString("INTERNAL TRANSFER FROM SBIA FZ ", font, XBrushes.Black,
                        New XRect(330, -610, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)
                            graph.DrawString("TO MURATA NRIE-IEAT FZ", font, XBrushes.Black,
                        New XRect(330, -600, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)
                        End If
                        graph.DrawString("_____________________________________________", font, XBrushes.Black,
                    New XRect(330, -610, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)



                        graph.DrawString("_____________________________________________", font, XBrushes.Black,
                    New XRect(330, -600, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        'SHIPPED PER //////////////////// 
                        graph.DrawString("SHIPPED PER : ", smallfont, XBrushes.Black,
                New XRect(440, -740, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        graph.DrawString("TRUCK", font, XBrushes.Black,
                New XRect(440, -730, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        graph.DrawString("________________", font, XBrushes.Black,
                New XRect(440, -730, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)


                        '////////////////////////////
                        graph.DrawString("TO : ", smallfont, XBrushes.Black,
                New XRect(440, -710, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)


                        graph.DrawString(tbl_hub.Rows(0)("DH10637_04").ToString, font, XBrushes.Black,
                   New XRect(440, -700, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)



                        graph.DrawString("________________", font, XBrushes.Black,
                    New XRect(440, -700, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)



                        '////////////////////////\\\\\\\\\\\\\\\\\\\\\\\///////////////////////////////////////////////////////////
                        If Left(inv_tran, 2) = "TF" Then
                            graph.DrawString("___________________________________________________________________________________________________________", font, XBrushes.Black,
                        New XRect(20, -520, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                            graph.DrawString("ITEM", font, XBrushes.Black,
                        New XRect(40, -500, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                            graph.DrawString("DESCRIPTION", font, XBrushes.Black,
                        New XRect(100, -500, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                            graph.DrawString("QUANTITY", font, XBrushes.Black,
                        New XRect(300, -500, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                            graph.DrawString("(PCS)", font, XBrushes.Black,
                        New XRect(305, -490, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)


                            graph.DrawString("UNIT PRICE", font, XBrushes.Black,
                        New XRect(400, -500, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                            graph.DrawString("(THB.)", font, XBrushes.Black,
                        New XRect(405, -490, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                            graph.DrawString("AMOUNT", font, XBrushes.Black,
                        New XRect(530, -500, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                            graph.DrawString("(THB.)", font, XBrushes.Black,
                        New XRect(535, -490, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                            graph.DrawString("___________________________________________________________________________________________________________", font, XBrushes.Black,
                        New XRect(20, -480, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)
                        Else
                            graph.DrawString("___________________________________________________________________________________________________________", font, XBrushes.Black,
                        New XRect(20, -520, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                            graph.DrawString("ITEM", font, XBrushes.Black,
                        New XRect(40, -500, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                            graph.DrawString("COSTOMER", font, XBrushes.Black,
                        New XRect(85, -500, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                            graph.DrawString("P/O NUMBER", font, XBrushes.Black,
                        New XRect(80, -490, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                            graph.DrawString("DESCRIPTION", font, XBrushes.Black,
                        New XRect(150, -500, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                            graph.DrawString("QUANTITY", font, XBrushes.Black,
                        New XRect(-200, -500, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                            graph.DrawString("(PCS)", font, XBrushes.Black,
                        New XRect(-205, -490, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)


                            graph.DrawString("UNIT PRICE", font, XBrushes.Black,
                        New XRect(-120, -500, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                            graph.DrawString("(THB.)", font, XBrushes.Black,
                        New XRect(-125, -490, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                            graph.DrawString("AMOUNT", font, XBrushes.Black,
                        New XRect(-30, -500, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                            graph.DrawString("(THB.)", font, XBrushes.Black,
                        New XRect(-35, -490, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                            graph.DrawString("___________________________________________________________________________________________________________", font, XBrushes.Black,
                        New XRect(20, -480, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        End If

                        '/////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\///////////////////////////////////////////////////////////////////
                        y = -460

                        Dim chk = Math.Ceiling(tbl_sum.Rows.Count / 9)
                        'If (tbl_sum.Rows.Count >= 0 And tbl_sum.Rows.Count <= 9) Then
                        'chk = 1
                        ' Else
                        '  chk = tbl_sum.Rows.Count Mod 9


                        'If chk = 0 Then
                        'chk = tbl_sum.Rows.Count / 9
                        'Else
                        'chk = chk
                        'End If

                        'End If

                        If (count <> chk) Then
                            graph.DrawString("NEXT PAGE", smallfont, XBrushes.Black,
                    New XRect(480, y + 288, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        End If







                    End If



                    If Left(inv_tran, 2) = "TF" Then
                    graph.DrawString(Right("000" & tbl_sum.Rows(i)("rownum").ToString, 3), font, XBrushes.Black,
                    New XRect(40, y, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString((tbl_sum.Rows(i)("IMFR_UD_MURATA_TYPE")).ToString, font, XBrushes.Black,
                    New XRect(100, y, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString((tbl_tye_not_c.Rows(i)("IMFR_UD_TARIFF")).ToString, font, XBrushes.Black,
                    New XRect(100, y + 10, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)
                    '//////////////////////////////////////////////////////////////////////////////////////////
                    graph.DrawString(CDbl(tbl_tye_not_c.Rows(i)("qty")).ToString("N0"), font, XBrushes.Black,
                    New XRect(-250, y, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)


                    graph.DrawString((CDbl(tbl_tye_not_c.Rows(i)("IMFR_UD_PRICE"))).ToString("N6"), font, XBrushes.Black,
                    New XRect(-140, y, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)


                    graph.DrawString((CDbl(tbl_tye_not_c.Rows(i)("amount"))).ToString("N2"), font, XBrushes.Black,
                    New XRect(-30, y, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)
                Else
                    graph.DrawString(Right("000" & tbl_sum.Rows(i)("rownum").ToString, 3), font, XBrushes.Black,
                    New XRect(40, y, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString((tbl_tye_not_c.Rows(i)("IMFR_UD_RETURN_NO")).ToString, font, XBrushes.Black,
                    New XRect(80, y, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString((tbl_sum.Rows(i)("IMFR_UD_MURATA_TYPE")).ToString, font, XBrushes.Black,
                    New XRect(150, y, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                    graph.DrawString((tbl_tye_not_c.Rows(i)("IMFR_UD_TARIFF")).ToString, font, XBrushes.Black,
                    New XRect(150, y + 10, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)


                    '//////////////////////////////////////////////////////////////////////////////////////////
                    graph.DrawString(CDbl(tbl_tye_not_c.Rows(i)("qty")).ToString("N0"), font, XBrushes.Black,
                    New XRect(-200, y, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)


                    graph.DrawString((CDbl(tbl_tye_not_c.Rows(i)("IMFR_UD_PRICE"))).ToString("N6"), font, XBrushes.Black,
                    New XRect(-120, y, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)


                    graph.DrawString((CDbl(tbl_tye_not_c.Rows(i)("amount"))).ToString("N2"), font, XBrushes.Black,
                    New XRect(-30, y, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                End If




                sum_qty = sum_qty + CDbl(tbl_tye_not_c.Rows(i)("qty").ToString)
                sum_amount = sum_amount + CDbl(tbl_tye_not_c.Rows(i)("amount").ToString)
                sum_price = sum_price + CDbl(tbl_tye_not_c.Rows(i)("IMFR_UD_PRICE").ToString)


                If i = tbl_sum.Rows.Count - 1 Then


                    'CASE HAVE CART
                    If (Not IsDBNull(tbl_type_head_c.Rows(0)("qty"))) Then

                        'graph.DrawString(Right("000" & tbl_sum.Rows.Count + 1.ToString, 3), font, XBrushes.Black,
                        'New XRect(40, y + 23, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        'graph.DrawString(Right("000" & tbl_sum.Rows.Count + 2.ToString, 3), font, XBrushes.Black,
                        'New XRect(40, y + 46, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        Dim sum_amout_c = CDbl(tbl_type_head_c.Rows(0)("price")) * CDbl(tbl_type_head_c.Rows(0)("qty"))
                        ' Dim sum_amount_p = CDbl(tbl_type_head_p.Rows(0)("SUM(IMFR_UD_PRICE)")) * CDbl(tbl_type_head_p.Rows(0)("SUM(IMFR_UD_QTY)"))


                        If Left(inv_tran, 2) = "TF" Then
                            'graph.DrawString("PLASTIC PALLET(PACKING FOR RETURN)", font, XBrushes.Black,
                            'New XRect(100, y + 23, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                            'graph.DrawString(tbl_type_head_p.Rows(0)("Sum(Imfr_Ud_Qty)").ToString, font, XBrushes.Black,
                            'New XRect(-250, y + 23, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                            'graph.DrawString(tbl_type_head_p.Rows(0)("Sum(Imfr_Ud_Price)").ToString, font, XBrushes.Black,
                            'New XRect(-140, y + 23, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                            'graph.DrawString(sum_amount_p.ToString("N2"), font, XBrushes.Black,
                            'New XRect(-30, y + 23, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                            graph.DrawString(Right("000" & tbl_sum.Rows.Count + 1.ToString, 3), font, XBrushes.Black,
                            New XRect(40, y + 23, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                            graph.DrawString("CART (PACKING FOR RETURN)", font, XBrushes.Black,
                            New XRect(100, y + 23, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                            graph.DrawString(tbl_type_head_c.Rows(0)("qty").ToString(), font, XBrushes.Black,
                            New XRect(-250, y + 23, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                            graph.DrawString(tbl_type_head_c.Rows(0)("Price").ToString(), font, XBrushes.Black,
                            New XRect(-140, y + 23, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                            graph.DrawString(sum_amout_c.ToString("N2"), font, XBrushes.Black,
                            New XRect(-30, y + 23, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                            '  graph.DrawString("CART (PACKING FOR RETURN)", font, XBrushes.Black,
                            'New XRect(100, y + 46, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                            '  graph.DrawString(tbl_type_head_c.Rows(0)("Sum(Imfr_Ud_Qty)").ToString(), font, XBrushes.Black,
                            '  New XRect(-250, y + 46, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                            '  graph.DrawString(tbl_type_head_c.Rows(0)("Sum(Imfr_Ud_Price)").ToString(), font, XBrushes.Black,
                            '  New XRect(-140, y + 46, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                            '  graph.DrawString(sum_amout_c.ToString("N2"), font, XBrushes.Black,
                            '  New XRect(-30, y + 46, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                            graph.DrawString("TOTAL PACKING FOR RETURN", font, XBrushes.Black,
                            New XRect(20, y + 85, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                            graph.DrawString((CDbl(tbl_type_head_c.Rows(0)("qty"))).ToString, font, XBrushes.Black,
                            New XRect(-250, y + 85, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                            graph.DrawString((sum_amout_c).ToString("N2"), font, XBrushes.Black,
                            New XRect(-30, y + 85, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                            graph.DrawString("GRAND TOTAL", font, XBrushes.Black,
                            New XRect(20, y + 105, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                            graph.DrawString((CDbl(tbl_type_head_c.Rows(0)("qty")) + sum_qty).ToString("N0"), font, XBrushes.Black,
                            New XRect(-250, y + 105, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                            graph.DrawString((sum_amout_c + sum_amount).ToString("N2"), font, XBrushes.Black,
                            New XRect(-30, y + 105, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                            graph.DrawString("( BATH :" + NumeriCon.ConvertNum(sum_amout_c + sum_amount) + ")", font, XBrushes.Black,
                            New XRect(20, y + 125, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)
                        Else
                            graph.DrawString("TOTAL PACKING FOR RETURN", font, XBrushes.Black,
                            New XRect(20, y + 85, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                            graph.DrawString("0", font, XBrushes.Black,
                            New XRect(-200, y + 85, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                            graph.DrawString("0", font, XBrushes.Black,
                            New XRect(-30, y + 85, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                            graph.DrawString("GRAND TOTAL", font, XBrushes.Black,
                            New XRect(20, y + 105, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                            graph.DrawString((0 + sum_qty).ToString("N0"), font, XBrushes.Black,
                            New XRect(-200, y + 105, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                            graph.DrawString((0 + sum_amount).ToString("N2"), font, XBrushes.Black,
                            New XRect(-30, y + 105, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                            graph.DrawString("( BATH :" + NumeriCon.ConvertNum(0 + sum_amount) + ")", font, XBrushes.Black,
                           New XRect(20, y + 125, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        End If







                        ' graph.DrawString((CDbl(tbl_type_head_p.Rows(0)("Sum(Imfr_Ud_Qty)")) + CDbl(tbl_type_head_c.Rows(0)("Sum(Imfr_Ud_Qty)"))).ToString, font, XBrushes.Black,
                        ' New XRect(-250, y + 85, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                        ' graph.DrawString((sum_amout_c + sum_amount_p).ToString("N2"), font, XBrushes.Black,
                        ' New XRect(-30, y + 85, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                        ' graph.DrawString("GRAND TOTAL", font, XBrushes.Black,
                        ' New XRect(20, y + 105, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        ' graph.DrawString((CDbl(tbl_type_head_p.Rows(0)("Sum(Imfr_Ud_Qty)")) + CDbl(tbl_type_head_c.Rows(0)("Sum(Imfr_Ud_Qty)")) + sum_qty).ToString("N0"), font, XBrushes.Black,
                        ' New XRect(-250, y + 105, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                        ' graph.DrawString((sum_amout_c + sum_amount_p + sum_amount).ToString("N2"), font, XBrushes.Black,
                        ' New XRect(-30, y + 105, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                        ' graph.DrawString("( BATH :" + NumeriCon.ConvertNum(sum_amout_c + sum_amount_p + sum_amount) + ")", font, XBrushes.Black,
                        'New XRect(20, y + 125, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        'CASE HAVE NOT CART
                    ElseIf (IsDBNull(tbl_type_head_c.Rows(0)("qty"))) Then
                        'graph.DrawString(Right("000" & tbl_sum.Rows.Count + 1.ToString, 3), font, XBrushes.Black,
                        'New XRect(40, y + 23, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)


                        Dim sum_amount_p = CDbl(tbl_type_head_p.Rows(0)("SUM(IMFR_UD_PRICE)")) * CDbl(tbl_type_head_p.Rows(0)("qty"))

                        'graph.DrawString("PLASTIC PALLET (PACKING FOR RETURN)", font, XBrushes.Black,
                        'New XRect(100, y + 23, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        'graph.DrawString(tbl_type_head_p.Rows(0)("Sum(Imfr_Ud_Qty)").ToString, font, XBrushes.Black,
                        'New XRect(-250, y + 23, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                        'graph.DrawString(tbl_type_head_p.Rows(0)("Sum(Imfr_Ud_Price)").ToString, font, XBrushes.Black,
                        'New XRect(-140, y + 23, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                        'graph.DrawString((CDbl(tbl_type_head_p.Rows(0)("SUM(IMFR_UD_PRICE)")) * CDbl(tbl_type_head_p.Rows(0)("SUM(IMFR_UD_QTY)"))).ToString("N2"), font, XBrushes.Black,
                        'New XRect(-30, y + 23, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)
                        If Left(inv_tran, 2) = "TF" Then
                            graph.DrawString("PACKING FOR RETURN", font, XBrushes.Black,
                            New XRect(20, y + 85, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                            graph.DrawString("0", font, XBrushes.Black,
                            New XRect(-250, y + 85, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                            graph.DrawString("0", font, XBrushes.Black,
                            New XRect(-30, y + 85, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                            graph.DrawString("GRAND TOTAL", font, XBrushes.Black,
                            New XRect(20, y + 105, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                            graph.DrawString((0 + sum_qty).ToString("N0"), font, XBrushes.Black,
                            New XRect(-250, y + 105, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                            graph.DrawString((0 + sum_amount).ToString("N2"), font, XBrushes.Black,
                            New XRect(-30, y + 105, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                            graph.DrawString("( BATH :" + NumeriCon.ConvertNum(0 + sum_amount) + ")", font, XBrushes.Black,
                            New XRect(20, y + 125, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)
                        Else
                            graph.DrawString("TOTAL PACKING FOR RETURN", font, XBrushes.Black,
                            New XRect(20, y + 85, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                            graph.DrawString("0", font, XBrushes.Black,
                            New XRect(-200, y + 85, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                            graph.DrawString("0", font, XBrushes.Black,
                            New XRect(-30, y + 85, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                            graph.DrawString("GRAND TOTAL", font, XBrushes.Black,
                            New XRect(20, y + 105, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                            graph.DrawString((0 + sum_qty).ToString("N0"), font, XBrushes.Black,
                            New XRect(-200, y + 105, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                            graph.DrawString((0 + sum_amount).ToString("N2"), font, XBrushes.Black,
                            New XRect(-30, y + 105, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                            graph.DrawString("( BATH :" + NumeriCon.ConvertNum(0 + sum_amount) + ")", font, XBrushes.Black,
                            New XRect(20, y + 125, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)
                        End If




                        'Case cart
                        'ElseIf (IsDBNull(tbl_type_head_p.Rows(0)("Sum(Imfr_Ud_Qty)")) And Not IsDBNull(tbl_type_head_c.Rows(0)("Sum(Imfr_Ud_Qty)"))) Then
                        '    Dim sum_amout_c = CDbl(tbl_type_head_c.Rows(0)("SUM(IMFR_UD_PRICE)")) * CDbl(tbl_type_head_c.Rows(0)("SUM(IMFR_UD_QTY)"))

                        '    If Left(inv_tran, 2) = "TF" Then
                        '        graph.DrawString(Right("000" & tbl_sum.Rows.Count + 1.ToString, 3), font, XBrushes.Black,
                        '        New XRect(40, y + 23, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)


                        '        graph.DrawString("CART (PACKING FOR RETURN)", font, XBrushes.Black,
                        '        New XRect(100, y + 23, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        '        graph.DrawString(tbl_type_head_c.Rows(0)("Sum(Imfr_Ud_Qty)").ToString, font, XBrushes.Black,
                        '        New XRect(-250, y + 23, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                        '        graph.DrawString(tbl_type_head_c.Rows(0)("Sum(Imfr_Ud_Price)").ToString, font, XBrushes.Black,
                        '        New XRect(-140, y + 23, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                        '        graph.DrawString((CDbl(tbl_type_head_c.Rows(0)("SUM(IMFR_UD_PRICE)")) * CDbl(tbl_type_head_c.Rows(0)("SUM(IMFR_UD_QTY)"))).ToString("N2"), font, XBrushes.Black,
                        '        New XRect(-30, y + 23, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)
                        '    End If


                        '    graph.DrawString("TOTAL PACKING FOR RETURN", font, XBrushes.Black,
                        '    New XRect(20, y + 85, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        '    graph.DrawString(tbl_type_head_c.Rows(0)("Sum(Imfr_Ud_Qty)").ToString, font, XBrushes.Black,
                        '    New XRect(-250, y + 85, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                        '    graph.DrawString((CDbl(tbl_type_head_c.Rows(0)("SUM(IMFR_UD_PRICE)")) * CDbl(tbl_type_head_c.Rows(0)("SUM(IMFR_UD_QTY)"))).ToString("N2"), font, XBrushes.Black,
                        '    New XRect(-30, y + 85, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                        '    graph.DrawString("GRAND TOTAL", font, XBrushes.Black,
                        '    New XRect(20, y + 105, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        '    graph.DrawString((CDbl(tbl_type_head_c.Rows(0)("Sum(Imfr_Ud_Qty)")) + sum_qty).ToString("N0"), font, XBrushes.Black,
                        '    New XRect(-250, y + 105, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                        '    graph.DrawString((sum_amout_c + sum_amount).ToString("N2"), font, XBrushes.Black,
                        '    New XRect(-30, y + 105, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                        '    graph.DrawString("( BATH :" + NumeriCon.ConvertNum(sum_amout_c + sum_amount) + ")", font, XBrushes.Black,
                        '    New XRect(20, y + 125, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)


                    End If

                    If Left(inv_tran, 2) = "TF" Then
                        graph.DrawString("TOTAL PRODUCT ", font, XBrushes.Black,
                        New XRect(20, y + 65, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        graph.DrawString(sum_qty.ToString("N0"), font, XBrushes.Black,
                        New XRect(-250, y + 65, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                        graph.DrawString(sum_amount.ToString("N2"), font, XBrushes.Black,
                        New XRect(-30, y + 65, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)


                        graph.DrawString("LAST PAGE", smallfont, XBrushes.Black,
                        New XRect(-30, y + 145, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)
                    Else
                        graph.DrawString("TOTAL PRODUCT ", font, XBrushes.Black,
                        New XRect(20, y + 65, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)

                        graph.DrawString(sum_qty.ToString("N0"), font, XBrushes.Black,
                        New XRect(-200, y + 65, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)

                        graph.DrawString(sum_amount.ToString("N2"), font, XBrushes.Black,
                        New XRect(-30, y + 65, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)


                        graph.DrawString("LAST PAGE", smallfont, XBrushes.Black,
                        New XRect(-30, y + 145, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomRight)
                    End If


                    'graph.DrawString("GRAND TOTAL ", font, XBrushes.Black,
                    'New XRect(20, y, pdfPage.Width.Point, pdfPage.Height.Point), XStringFormats.BottomLeft)


                End If

                tmpPage = tbl_sum.Rows(i)("rownum").ToString

                y = y + 26
            Next







            If Left(inv_tran, 2) = "TF" Then
                location = location & Now.ToString("yyyyMMdd") & "\" & get_truck.Rows(0)("truck").ToString()
            Else
                'test.MapDrive()
                location = location & "HUB" & "\" & Now.ToString("yyyyMMdd")
            End If


            'get_truck.Rows(0)("truck").ToString()
            Try
                ' Determine whether the directory exists.
                If Directory.Exists(location) = False Then
                    Directory.CreateDirectory(location)

                End If
                TargetFile = location & "\"
            Catch ex As Exception
                TargetFile = TargetFile
            End Try

            Dim pdfFilename As String = TargetFile & inv_tran & "_" & tbl_mtl.Rows(0)("CD12064").ToString() & "_" & "IV" & ".pdf"
            pdf.Save(pdfFilename)

            Conn.Dispose()

        Catch ex As Exception

            'status = False
            'MessageBox.Show(ex.ToString)

            Dim errlog As String
            errlog = "Error Invoice No " & inv_tran & ":" & ex.ToString
            WriteLogFile(errlocation, errlog)
            'sendmail("[ERROR]GLSKEEPINGNO", errlog, "KEEPINGLIST")
            Return errlog

        Finally
            'If conn.State() = ConnectionState.Open Then
            '    objOra.EndConnection(conn)
            'End If
        End Try


        Return "OK"
        ' Return "OK"


    End Function


    Sub WriteLogFile(ByVal location As String, ByVal text As String)

        Dim fileName As String
        fileName = location + DateTime.Today.ToString("yyyyMMdd") + ".txt"
        Dim objWriter As New System.IO.StreamWriter(fileName, True)  'True = append text

        Try
            If Not System.IO.File.Exists(fileName) Then
                System.IO.File.Create(location).Dispose()
            End If
            objWriter.WriteLine(DateTime.Now + "  : " + text)
            objWriter.Close()
            FileClose(1)
        Catch ex As Exception
            objWriter.Close()
            FileClose(1)
        End Try

    End Sub

    Sub tranNumbertoString(ByVal number As Integer)
        Dim numString = number.ToString



    End Sub

End Class

Friend Class PdfPoint
    Private x As Object
    Private y As Integer

    Public Sub New(x As Object, y As Integer)
        Me.x = x
        Me.y = y
    End Sub
End Class



Public Class NumeriCon

    Public Shared Function ConvertNum(ByVal Input As Double) As String 'Call this function passing the number you desire to be changed  
        Dim output As String = Nothing
        Dim search = Input.ToString.IndexOf(".")
        Dim arr As String = ""
        Dim inputt As Double
        If search > 0 Then
            arr = Input.ToString.Substring(0, search)
            inputt = CDbl(arr)
        Else
            inputt = CDbl(Input)
        End If


        If inputt < 1000 Then
            output = FindNumber(inputt) 'if its less than 1000 then just look it up  
        Else
            Dim nparts() As String 'used to break the number up into 3 digit parts  
            Dim n As String = inputt 'string version of the number  
            Dim i As Long = inputt.ToString.Length 'length of the string to help break it up  

            Do Until i - 3 <= 0
                n = n.Insert(i - 3, ",") 'insert commas to use as splitters  
                i = i - 3 'this insures that we get the correct number of parts  
            Loop
            nparts = n.Split(",") 'split the string into the array  

            i = inputt.ToString.Length 'return i to initial value for reuse  
            Dim p As Integer = 0 'p for parts, used for finding correct suffix  
            For Each s As String In nparts
                Dim x As Long = CLng(s) 'x is used to compare the part value to other values  
                p = p + 1
                If p = nparts.Length Then 'if p = number of elements in the array then we need to do something different  
                    If x <> 0 Then
                        If CLng(s) < 100 Then
                            output = output & " " & FindNumber(CLng(s))   ' look up the number, no suffix   
                        Else                                                ' required as this is the last part  
                            output = output & " " & FindNumber(CLng(s))
                        End If
                    End If
                Else 'if its not the last element in the array  
                    If x <> 0 Then
                        If output = Nothing Then 'we have to check this so we don't add a leading space  
                            output = output & FindNumber(CLng(s)) & " " & FindSuffix(i, CLng(s))  'look up the number and suffix  
                        Else 'spaces must go in the right place  
                            output = output & " " & FindNumber(CLng(s)) & " " & FindSuffix(i, CLng(s))  'look up the snumber and suffix  
                        End If
                    End If
                End If
                i = i - 3 'reduce the suffix counter by 3 to step down to the next suffix  
            Next
        End If
        If search > 0 Then
            Dim point As String = Finpoint(Input)
            Return output + " " + point + " " + "ONLY"

        End If
        Return output
    End Function

    Private Shared Function FindNumber(ByVal Number As Double) As String
        Dim Words As String = Nothing
        Dim Digits() As String = {"ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN",
      "EIGHT", "NINE", "TEN"}
        Dim Teens() As String = {"", "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN", "SEVENTEEN",
       "EIGHTEEN", "NINETEEN"}

        If Number < 11 Then

            Words = Digits(Number)

        ElseIf Number < 20 Then
            Words = Teens(Number - 10)

        ElseIf Number = 20 Then
            Words = "TWENTY"

        ElseIf Number < 30 Then
            Words = "TWENTY " & Digits(Number - 20)

        ElseIf Number = 30 Then
            Words = "THIRTY"

        ElseIf Number < 40 Then
            Words = "THIRTY " & Digits(Number - 30)

        ElseIf Number = 40 Then
            Words = "FORTY"

        ElseIf Number < 50 Then
            Words = "FORTY " & Digits(Number - 40)

        ElseIf Number = 50 Then
            Words = "FIFTY"

        ElseIf Number < 60 Then
            Words = "FIFTY " & Digits(Number - 50)

        ElseIf Number = 60 Then
            Words = "SIXTY"

        ElseIf Number < 70 Then
            Words = "SIXTY " & Digits(Number - 60)

        ElseIf Number = 70 Then
            Words = "SEVENTY"

        ElseIf Number < 80 Then
            Words = "SEVENTY " & Digits(Number - 70)

        ElseIf Number = 80 Then
            Words = "EIGHTY"

        ElseIf Number < 90 Then
            Words = "EIGHTY " & Digits(Number - 80)

        ElseIf Number = 90 Then
            Words = "NINETY"

        ElseIf Number < 100 Then
            Words = "NINETY " & Digits(Number - 90)

        ElseIf Number < 1000 Then
            Words = Number.ToString
            Words = Words.Insert(1, ",")
            Dim wparts As String() = Words.Split(",")
            Words = FindNumber(wparts(0)) & " " & "HUNDRED"
            Dim n As String = FindNumber(wparts(1))
            If CLng(wparts(1)) <> 0 Then
                Words = Words & " " & n
            End If
        End If

        Return Words
    End Function

    Private Shared Function FindSuffix(ByVal Length As Double, ByVal l As Double) As String
        Dim word As String

        If l <> 0 Then
            If Length > 12 Then
                word = "TRILLION"
            ElseIf Length > 9 Then
                word = "BILLION"
            ElseIf Length > 6 Then
                word = "MILLION"
            ElseIf Length > 3 Then
                word = "THOUSAND"
            ElseIf Length > 2 Then
                word = "HUNDRED"
            Else
                word = ""
            End If
        Else
            word = ""
        End If

        Return word
    End Function

    Private Shared Function Finpoint(ByVal Number As Double) As String
        Dim word As String = ""
        Dim str_num As String = Number
        Dim search = str_num.IndexOf(".")

        If search > 0 Then

            Dim arr As String = str_num.Substring(str_num.Length - 2)
            If arr = ".1" Or arr = ".2" Or arr = ".3" Or arr = ".4" Or arr = ".5" Or arr = ".6" Or arr = ".7" Or arr = ".8" Or arr = ".9" Then
                arr = arr + "0"
                arr = arr.Substring(1)
                arr = CInt(arr)
                word = FindNumber(arr)
                Return "POINT " + word
            Else
                word = FindNumber(CLng(arr))
                Return "POINT " + word
            End If

        End If
        Return word

    End Function



End Class

Public Class test
    Public Shared Function EnsureConnection(server As String)
        'Give more or less ping attempts depending on how reliable your connection is.
        'I have found that one ping can give false negative easily even on reliable connections
        If My.Computer.Network.Ping(server) Then
            Return True
        ElseIf My.Computer.Network.Ping(server) Then
            Return True
        Else
            Return False
        End If
    End Function


    Public Shared Function MapDrive() As Boolean

        Dim proc As New Process
        proc.StartInfo.FileName = "net"
        proc.StartInfo.UseShellExecute = True
        proc.EnableRaisingEvents = False

        Dim server As String = "172.16.80.4"
        Dim user As String = "\*Administrator"
        Dim pass As String = "Admin!@#"

        If EnsureConnection(server) = True Then
            proc.StartInfo.Arguments = "\\" & server & "\pcapl$/" & server & "\" & user & " " & pass
            proc.Start()
            Return True
        Else
            Return False

        End If
    End Function
End Class