﻿Imports System.Configuration
Imports System.Net.Mail
Imports System.Net

Imports System
Imports System.DirectoryServices
Imports System.Collections
Imports System.Reflection

Public Class resetpassword
    Inherits System.Web.UI.Page

    Private mstrFuncName As String

    Private mstrDomain As String
    Private mstrADAdminUser As String
    Private mstrADAdminPass As String
    Private mstrDomainAdminUser As String

    Private mstrSmtpSrv As String
    Private mstrSmtpUser As String
    Private mstrSmtpPass As String
    Private mstrUserListFile As String

    Private mstrADUserID As String
    Private mstrADUserName As String
    Private mstrADUserEmail As String
    Private mstrAdUserPass As String
    Private mstrADBindPath As String

    Private mstrEmailSubject As String
    Private mstrEmailBody As String

    'Private mstrAdminUserEmail As String
    'Private mstrAdminEmailSubject As String
    'Private mstrAdminEmailBody As String

    Private smtp As New SmtpClient
    Private objADAM As DirectoryEntry
    Private search As DirectorySearcher


    Private Function quote(ByVal vstrData As String) As String

        Return "'" + Replace(vstrData, "'", "''") + "'"

    End Function

    Private Function logMe(ByVal vstrData As String, Optional ByVal vstrData1 As String = vbNullString) As Boolean

        If Request.QueryString("debug") = 1 Then
            ' txtMsg.Text = txtMsg.Text & vstrData & vbNewLine & vstrData1
            Response.Write(vstrData & "<br />" & vstrData1 & "<br />")
        End If
        Return True
    End Function



    Private Function blnGetSettings() As Boolean

        blnGetSettings = False

        Try


            Dim time As DateTime = DateTime.Now
            Dim format As String = "MMM ddd d HH:mm yyyy"
            logMe("===========================" & time.ToString(format) & "=================================================")

            mstrFuncName = System.Reflection.MethodBase.GetCurrentMethod().Name

            mstrDomain = ConfigurationManager.ConnectionStrings("mstrDomain").ConnectionString
            mstrADBindPath = ConfigurationManager.ConnectionStrings("mstrADBindPath").ConnectionString

            mstrADAdminUser = ConfigurationManager.ConnectionStrings("mstrADAdminUser").ConnectionString
            mstrADAdminPass = ConfigurationManager.ConnectionStrings("mstrADAdminPass").ConnectionString

            mstrSmtpSrv = ConfigurationManager.ConnectionStrings("mstrSmtpSrv").ConnectionString
            mstrSmtpUser = ConfigurationManager.ConnectionStrings("mstrSmtpUser").ConnectionString
            mstrSmtpPass = ConfigurationManager.ConnectionStrings("mstrSmtpPass").ConnectionString
            mstrUserListFile = ConfigurationManager.ConnectionStrings("mstrUserListFile").ConnectionString
            mstrEmailSubject = ConfigurationManager.ConnectionStrings("mstrEmailSubject").ConnectionString
            mstrEmailBody = ConfigurationManager.ConnectionStrings("mstrEmailBody").ConnectionString

            'mstrAdminUserEmail = ConfigurationManager.ConnectionStrings("mstrAdminUserEmail").ConnectionString
            'mstrAdminEmailSubject = ConfigurationManager.ConnectionStrings("mstrAdminEmailSubject").ConnectionString
            'mstrAdminEmailBody = ConfigurationManager.ConnectionStrings("mstrAdminEmailBody").ConnectionString

            mstrDomainAdminUser = mstrDomain & "\" & mstrADAdminUser
            logMe("mstrDomain: " & mstrDomain)
            logMe("mstrADBindPath: " & mstrADBindPath)
            logMe("mstrSmtpSrv: " & mstrSmtpSrv)
            logMe("mstrSmtpUser: " & mstrSmtpUser)
            logMe("mstrUserListFile: " & mstrUserListFile)
            logMe("mstrEmailSubject: " & mstrEmailSubject)
            logMe("mstrEmailBody: " & mstrEmailBody)

            'logMe("mstrAdminUserEmail" & mstrAdminUserEmail)
            'logMe("mstrAdminEmailSubject" & mstrAdminEmailSubject)
            'logMe("mstrAdminEmailBody" & mstrAdminEmailBody)


            logMe(vbNewLine)

            ' Configure the SMTP client 
            logMe("Connecting to SMTP server")
            smtp.Host = mstrSmtpSrv
            smtp.Credentials = New NetworkCredential(mstrSmtpUser, mstrSmtpPass)
            smtp.EnableSsl = True
            logMe("Connecting to SMTP server Success")


            logMe("Trying to Bind :", mstrADBindPath)
            objADAM = New DirectoryEntry()
            objADAM.Path = mstrADBindPath
            objADAM.Username = mstrDomainAdminUser
            objADAM.Password = mstrADAdminPass

            'logMe("Trying to Bind :", mstrADBindPath)
            search = New DirectorySearcher(objADAM)
            logMe("Bind Success:", mstrADBindPath)

        Catch ex As Exception
            logMe(mstrFuncName, ex.Message & " " & ex.StackTrace.ToString)
            Return False
        End Try

        Return True

    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click

        If blnGetSettings() = False Then
            Return
        End If

        If mblnResetPass() = False Then
            Return
        End If

    End Sub



    Private Function mblnResetPass() As Boolean

        'http://www.morgantechspace.com/2015/01/reset-ad-user-password-with-C-sharp.html

        mblnResetPass = False

        Try
            mstrFuncName = System.Reflection.MethodBase.GetCurrentMethod().Name

            'Dim domainEntry As DirectoryEntry       ' Binding object.
            Dim objUser As DirectoryEntry       ' User object.
            'Dim strDisplayName As String        ' Display name of user.
            'Dim strPath As String               ' Binding path.
            Dim strUser As String               ' User to create.
            'Dim strUserPrincipalName As String  ' Principal name of user.

            ' Construct the binding string.
            'strPath = mstrADBindPath  '"LDAP://localhost:389/O=Fabrikam,C=US"
            'strDisplayName = mstrADUserName
            'strUser = "CN=" & mstrADUserID
            'strUserPrincipalName = mstrADUserEmail

            mstrADUserID = txtUserName.Text
            mstrADUserEmail = txtEmail.Text
            mstrAdUserPass = mstrGetRandomString(10) 'currentRow.GetValue(3)

            logMe("mstrADUserID :" & mstrADUserID)
            logMe("mstrADUserEmail :" & mstrADUserEmail)
            'logMe("mstrAdUserPass :" & mstrAdUserPass)

            ' Get AD LDS object.
            Try
                'domainEntry = New DirectoryEntry(mstrADBindPath)
                'domainEntry.UsePropertyCache = False
                'domainEntry.RefreshCache()

                search.SearchScope = SearchScope.Subtree
                search.Filter = "(sAMAccountName=" & mstrADUserID & ")"
                Dim result As SearchResult = search.FindOne()

                strUser = CType(result.Properties("sAMAccountName")(0), String)

                If strUser <> Nothing And strUser = mstrADUserID Then

                    objUser = result.GetDirectoryEntry()
                    objUser.UsePropertyCache = False

                    If 1 = 2 Then
                        logMe("Reading user properties")
                        For Each strProperty As String In objUser.Properties.PropertyNames
                            Dim value As String = objUser.Properties(strProperty)(0).ToString
                            logMe(strProperty & ":" & value)
                        Next
                    End If

                    Dim strADCN As String = objUser.Properties("cn")(0).ToString
                    Dim strADEmail As String = objUser.Properties("mail")(0).ToString

                    If strADCN <> mstrADUserID Or strADEmail <> mstrADUserEmail Then
                        lblMsg.Text = "User not found. Please enter correct User ID and Email Address"
                        Return False
                    End If

                    'UF_DONT_EXPIRE_PASSWD 0x10000
                    'Dim exp As Integer = CInt(objUser.Properties("userAccountControl").Value)
                    'objUser.Properties("userAccountControl").Value = exp Or &H1
                    'objUser.CommitChanges()
                    '
                    'https://www.codeproject.com/Articles/19689/Working-with-Active-Directory-in-VB-NET

                    'logMe("Enable User Account: ", mstrADUserID)

                    'Const ADS_UF_ACCOUNTDISABLE As Integer = 2
                    'Dim val As Integer = CType(objUser.Properties("userAccountControl").Value, Integer)
                    'objUser.Properties("userAccountControl").Value = val Or Not ADS_UF_ACCOUNTDISABLE
                    'Const ADS_UF_NORMAL_ACCOUNT = &H200              ' Typical user account.
                    'objUser.Properties("userAccountControl").Value = ADS_UF_NORMAL_ACCOUNT
                    'objUser.Properties("useraccountcontrol").IntegerValue = ADS_UF_NORMAL_ACCOUNT

                    ' If Not result Is Nothing Then
                    'Dim ADUser As DirectoryEntry = result.GetDirectoryEntry()
                    'ADUser.NativeObject.AccountDisabled = True
                    'ADUser.CommitChanges()
                    'End If


                    ' Dim val As Integer = CInt(objUser.Properties("userAccountControl").Value)

                    'objUser.Properties("userAccountControl").Value = val And Not 2

                    'ADS_UF_NORMAL_ACCOUNT;

                    'objUser.CommitChanges()

                    'Dim ent As Object = objUser.NativeObject
                    'Dim type As Type = ent.GetType
                    'Type.InvokeMember("AccountDisabled", BindingFlags.SetProperty, Nothing, ent, New Object() {False})
                    'objUser.CommitChanges()


                    'logMe("Enable User Account Success: ", mstrADUserID)


                    'objUser.Properties("LockOutTime").Value = 0 ' unlock account
                    'objUser.Properties("pwdLastSet").Value = 0 ' force change at Next logon
                    'objUser.CommitChanges()


                    'entry.Username = mstrDomain & "\" & strUser
                    'entry.Password = mstrAdUserPass
                    'entry.AuthenticationType = AuthenticationTypes.Secure
                    'entry.Options.Referral = ReferralChasingOption.All
                    'userEntry.Properties["pwdlastset"][0] = 0;

                    logMe("Updating User Password: ", mstrADUserID)
                        'Dim password As Object() = New Object() {mstrAdUserPass}

                        objUser.Invoke("SetPassword", New Object() {mstrAdUserPass})

                        'objUser.Password = mstrAdUserPass
                        logMe("Updating User Password Success: ", mstrADUserID)

                        ' objUser.Invoke("SetPassword", mstrAdUserPass)
                        'objUser.Properties("pwdLastSet").Value = 0


                        objUser.CommitChanges()
                        objUser.Dispose()

                    If mblnSendEmail() = False Then
                        'continue with next user password reset
                    End If

                    lblMsg.Text = "Password rest successfull. Your password has sent to your email Address"

                        'logMe("Updating User Password Sucess: ", mstrADUserID)
                    Else
                        lblMsg.Text = "User not found. Please enter correct User ID and Email Address"
                End If

                search.Dispose()
                objADAM.Dispose()

                'domainEntry.Close()

            Catch ex As Exception
                'logMe(mstrFuncName, "Error:   Bind failed.")
                logMe(mstrFuncName, ex.Message & " " & ex.StackTrace.ToString)
                Return False
            End Try

            ' Specify User.
            'strUser = "CN=TestUser"
            'strDisplayName = "Test User"
            'strUserPrincipalName = "TestUser@Fabrikam.Us"
            'logMe("Create:  {0}", strUser)


        Catch ex As Exception
            logMe(mstrFuncName, ex.Message & " " & ex.StackTrace.ToString)
            Return False
        End Try

        Return True
    End Function

    Private Function mblnSendEmail() As Boolean

        mblnSendEmail = False

        Try
            mstrFuncName = System.Reflection.MethodBase.GetCurrentMethod().Name

            'User email message
            Dim mail As New MailMessage
            mail.To.Add(mstrADUserEmail)
            mail.From = New MailAddress(mstrSmtpUser)

            Dim strTmp As String = mstrDomain & "\" & mstrADUserID

            'Dim strEmailSubject As String
            Dim strEmailBody As String

            strEmailBody = Replace(mstrEmailBody, "ADuserid", strTmp)
            strEmailBody = Replace(strEmailBody, "ADpass", mstrAdUserPass)

            mail.Subject = mstrEmailSubject
            mail.Body = strEmailBody
            mail.IsBodyHtml = True

            logMe("Sending email:" & mstrADUserEmail & ":" & mstrEmailSubject)
            'logMe("Email Body:" & mstrEmailBody)

#If False Then
            logMe("mstrAdminUserEmail" & mstrAdminUserEmail)
            logMe("mstrAdminEmailSubject" & mstrAdminEmailSubject)
            logMe("mstrAdminEmailBody" & mstrAdminEmailBody)


            'Admin email message
            Dim Adminmail As New MailMessage
            Adminmail.To.Add(mstrAdminUserEmail)
            Adminmail.From = New MailAddress(mstrSmtpUser)

            Dim strAdminEmailSubject As String
            Dim strAdminEmailBody As String

            strAdminEmailSubject = Replace(mstrAdminEmailSubject, "ADuserid", mstrADUserID)
            strAdminEmailBody = Replace(mstrAdminEmailBody, "ADuserid", mstrADUserID)
            'mstrEmailBody = Replace(mstrEmailBody, "ADpass", mstrAdUserPass)

            Adminmail.Subject = strAdminEmailSubject
            Adminmail.Body = strAdminEmailBody
            Adminmail.IsBodyHtml = True

            logMe("Sending email:" & mstrAdminUserEmail & ":" & strAdminEmailSubject)
            'logMe("Email Body:" & mstrAdminEmailBody)

            ' Configure the SMTP client 
            ' Dim smtp As New SmtpClient
            'smtp.Host = mstrSmtpSrv
            'smtp.Credentials = New NetworkCredential(mstrSmtpUser, mstrSmtpPass)
            'smtp.EnableSsl = True

#End If

            ' Send the email 
            smtp.Send(mail)
            'smtp.Send(Adminmail)

            mail.Dispose()
            'Adminmail.Dispose()
            smtp.Dispose()

        Catch ex As Exception
            logMe(mstrFuncName, ex.Message & " " & ex.StackTrace.ToString)
            Return False
        End Try

        Return True

    End Function

    Private Function mstrGetRandomString(ByRef Length As String) As String

        Dim str As String = Nothing
        Dim rnd As New Random

        Try

            mstrFuncName = System.Reflection.MethodBase.GetCurrentMethod().Name

            For i As Integer = 0 To Length
                Dim chrInt As Integer = 0
                Do
                    chrInt = rnd.Next(30, 122)
                    If (chrInt >= 48 And chrInt <= 57) Or (chrInt >= 65 And chrInt <= 90) Or (chrInt >= 97 And chrInt <= 122) Then
                        Exit Do
                    End If
                Loop
                str &= Chr(chrInt)
            Next

            Return str

        Catch ex As Exception
            logMe(mstrFuncName, ex.Message & " " & ex.StackTrace.ToString)
            Return False
        End Try

    End Function

End Class