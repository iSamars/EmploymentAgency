using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PdfSharp;
using Image = System.Drawing.Image;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;
using System.Diagnostics;

namespace EmploymentAgency.Helpers
{
    class helper
    {
        public enum UserColumns
        {
            id=1,
            login,
            password,
            admin,
            name,
            lastname,
            patronymic,
            sex,
            dateOfBirth,
            country
        }

        public static int CalculateAge(DateTime BirthDate)
        {
            int YearsPassed = DateTime.Now.Year - BirthDate.Year;
            if (DateTime.Now.Month < BirthDate.Month || (DateTime.Now.Month == BirthDate.Month && DateTime.Now.Day < BirthDate.Day))
            {
                YearsPassed--;
            }
            return YearsPassed;
        }

        public static string WelcomeTime(DateTime dateTime)
        {
            if (dateTime.Hour >= 0 && dateTime.Hour <= 5)
                return "Доброй ночи";
            else if(dateTime.Hour >= 6 && dateTime.Hour <= 11)
                return "Доброе утро";
            else if (dateTime.Hour >= 12 && dateTime.Hour <= 18)
                return "Добрый день";
            else
                return "Добрый вечер";
        }

        public static string WelcomeText(int sex)
        {
            return sex == 1 ? "уважаемый" : "уважаемая";
        }

        public static string ToNameFormat(string str)
        {
            StringBuilder stringBuilder = new StringBuilder(str);
            stringBuilder[0] = Char.ToUpper(str[0]);
            for(int i = 1; i < str.Length; i++)
            {
                stringBuilder[i] = Char.ToLower(str[i]);
            }
            return stringBuilder.ToString();
        }

        public static void sendMainConfirm(string name, string code, string toEmail)
        {
            try
            {
                MailMessage m = new MailMessage(new MailAddress("employment-agency@samars.fun", "Подтверждение регистрации"), new MailAddress(toEmail));
                m.Subject = "Подтверждение регистрации";
                m.Body = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd""><html style=""width:100%;font-family:verdana, geneva, sans-serif;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%;padding:0;Margin:0;""> <head> <meta charset=""UTF-8""> <meta content=""width=device-width, initial-scale=1"" name=""viewport""> <meta name=""x-apple-disable-message-reformatting""> <meta http-equiv=""X-UA-Compatible"" content=""IE=edge""> <meta content=""telephone=no"" name=""format-detection""> <title>Подтверждение регистрации</title> <!--[if (mso 16)]> <style type=""text/css""> a {text-decoration: none;} </style> <![endif]--> <!--[if gte mso 9]><style>sup { font-size: 100% !important; }</style><![endif]--> <!--[if !mso]><!-- --> <link href=""https://fonts.googleapis.com/css?family=Open+Sans:400,400i,700,700i"" rel=""stylesheet""> <!--<![endif]--> <style type=""text/css"">@media only screen and (max-width:600px) {p, ul li, ol li, a { font-size:16px!important; line-height:150%!important } h1 { font-size:30px!important; text-align:center; line-height:120%!important } h2 { font-size:26px!important; text-align:center; line-height:120%!important } h3 { font-size:20px!important; text-align:center; line-height:120%!important } h1 a { font-size:30px!important } h2 a { font-size:26px!important } h3 a { font-size:20px!important } .es-menu td a { font-size:16px!important } .es-header-body p, .es-header-body ul li, .es-header-body ol li, .es-header-body a { font-size:16px!important } .es-footer-body p, .es-footer-body ul li, .es-footer-body ol li, .es-footer-body a { font-size:16px!important } .es-infoblock p, .es-infoblock ul li, .es-infoblock ol li, .es-infoblock a { font-size:12px!important } *[class=""gmail-fix""] { display:none!important } .es-m-txt-c, .es-m-txt-c h1, .es-m-txt-c h2, .es-m-txt-c h3 { text-align:center!important } .es-m-txt-r, .es-m-txt-r h1, .es-m-txt-r h2, .es-m-txt-r h3 { text-align:right!important } .es-m-txt-l, .es-m-txt-l h1, .es-m-txt-l h2, .es-m-txt-l h3 { text-align:left!important } .es-m-txt-r img, .es-m-txt-c img, .es-m-txt-l img { display:inline!important } .es-button-border { display:block!important } a.es-button { font-size:20px!important; display:block!important; border-width:10px 0px 10px 0px!important } .es-btn-fw { border-width:10px 0px!important; text-align:center!important } .es-adaptive table, .es-btn-fw, .es-btn-fw-brdr, .es-left, .es-right { width:100%!important } .es-content table, .es-header table, .es-footer table, .es-content, .es-footer, .es-header { width:100%!important; max-width:600px!important } .es-adapt-td { display:block!important; width:100%!important } .adapt-img { width:100%!important; height:auto!important } .es-m-p0 { padding:0px!important } .es-m-p0r { padding-right:0px!important } .es-m-p0l { padding-left:0px!important } .es-m-p0t { padding-top:0px!important } .es-m-p0b { padding-bottom:0!important } .es-m-p20b { padding-bottom:20px!important } .es-mobile-hidden, .es-hidden { display:none!important } .es-desk-hidden { display:table-row!important; width:auto!important; overflow:visible!important; float:none!important; max-height:inherit!important; line-height:inherit!important } .es-desk-menu-hidden { display:table-cell!important } table.es-table-not-adapt, .esd-block-html table { width:auto!important } table.es-social { display:inline-block!important } table.es-social td { display:inline-block!important } }#outlook a {padding:0;}.ExternalClass {width:100%;}.ExternalClass,.ExternalClass p,.ExternalClass span,.ExternalClass font,.ExternalClass td,.ExternalClass div {line-height:100%;}.es-button {mso-style-priority:100!important;text-decoration:none!important;}a[x-apple-data-detectors] {color:inherit!important;text-decoration:none!important;font-size:inherit!important;font-family:inherit!important;font-weight:inherit!important;line-height:inherit!important;}.es-desk-hidden {display:none;float:left;overflow:hidden;width:0;max-height:0;line-height:0;mso-hide:all;}</style> </head> <body style=""width:100%;font-family:verdana, geneva, sans-serif;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%;padding:0;Margin:0;""> <div class=""es-wrapper-color"" style=""background-color:#F6F6F6;""> <!--[if gte mso 9]><v:background xmlns:v=""urn:schemas-microsoft-com:vml"" fill=""t""><v:fill type=""tile"" src=""https://hfwjsa.stripocdn.email/content/guids/CABINET_63fbbc11db6741389cc3292b09a63e6d/images/7711511856111535.png"" color=""#f6f6f6"" origin=""0.5, 0"" position=""0.5,0""></v:fill></v:background><![endif]--> <table class=""es-wrapper"" width=""100%"" cellspacing=""0"" cellpadding=""0"" background=""https://hfwjsa.stripocdn.email/content/guids/CABINET_63fbbc11db6741389cc3292b09a63e6d/images/7711511856111535.png"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;padding:0;Margin:0;width:100%;height:100%;background-image:url(https://hfwjsa.stripocdn.email/content/guids/CABINET_63fbbc11db6741389cc3292b09a63e6d/images/7711511856111535.png);background-repeat:repeat;background-position:center top;""> <tr style=""border-collapse:collapse;""> <td valign=""top"" style=""padding:0;Margin:0;""> <table cellpadding=""0"" cellspacing=""0"" class=""es-header"" align=""center"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;background-color:transparent;background-repeat:repeat;background-position:center top;""> <tr style=""border-collapse:collapse;""> <td class=""es-adaptive"" align=""center"" style=""padding:0;Margin:0;""> <table class=""es-header-body"" width=""600"" cellspacing=""0"" cellpadding=""0"" align=""center"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;""> <tr style=""border-collapse:collapse;""> <td align=""left"" style=""padding:15px;Margin:0;""> <table width=""100%"" cellspacing=""0"" cellpadding=""0"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;""> <tr style=""border-collapse:collapse;""> <td width=""570"" valign=""top"" align=""center"" style=""padding:0;Margin:0;""> <table width=""100%"" cellspacing=""0"" cellpadding=""0"" role=""presentation"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;""> <tr style=""border-collapse:collapse;""> <td align=""center"" style=""padding:0;Margin:0;""><p style=""Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-size:23px;font-family:georgia, times, 'times new roman', serif;line-height:35px;color:#333333;"">Кадровое агенство</p></td> </tr> </table></td> </tr> </table></td> </tr> </table></td> </tr> </table> <table class=""es-content"" cellspacing=""0"" cellpadding=""0"" align=""center"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;""> <tr style=""border-collapse:collapse;""> <td align=""center"" style=""padding:0;Margin:0;""> <table class=""es-content-body"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;"" width=""600"" cellspacing=""0"" cellpadding=""0"" bgcolor=""#ffffff"" align=""center""> <tr style=""border-collapse:collapse;""> <td style=""padding:0;Margin:0;background-color:#F3F3F3;"" bgcolor=""#f3f3f3"" align=""left""> <table width=""100%"" cellspacing=""0"" cellpadding=""0"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;""> <tr style=""border-collapse:collapse;""> <td width=""600"" valign=""top"" align=""center"" style=""padding:0;Margin:0;""> <table width=""100%"" cellspacing=""0"" cellpadding=""0"" role=""presentation"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;""> <tr style=""border-collapse:collapse;""> <td align=""center"" style=""padding:0;Margin:0;padding-bottom:15px;font-size:0;""><img class=""adapt-img"" src=""https://hfwjsa.stripocdn.email/content/guids/CABINET_63fbbc11db6741389cc3292b09a63e6d/images/63541516368770627.png"" alt=""Handshake"" title=""Handshake"" width=""600"" style=""display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic;""></td> </tr> </table></td> </tr> </table></td> </tr> <tr style=""border-collapse:collapse;""> <td align=""left"" style=""Margin:0;padding-bottom:10px;padding-top:20px;padding-left:30px;padding-right:30px;""> <table width=""100%"" cellspacing=""0"" cellpadding=""0"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;""> <tr style=""border-collapse:collapse;""> <td width=""540"" valign=""top"" align=""center"" style=""padding:0;Margin:0;""> <table width=""100%"" cellspacing=""0"" cellpadding=""0"" role=""presentation"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;""> <tr style=""border-collapse:collapse;""> <td align=""center"" style=""padding:0;Margin:0;padding-top:15px;""><h3 style=""Margin:0;line-height:24px;mso-line-height-rule:exactly;font-family:georgia, times, 'times new roman', serif;font-size:20px;font-style:normal;font-weight:normal;color:#24578E;"">Уважаемый " + name + @" , спасибо за регистрацию в нашем приложении!</h3></td> </tr> <tr style=""border-collapse:collapse;""> <td class=""es-m-txt-c"" align=""left"" style=""padding:0;Margin:0;padding-top:10px;padding-bottom:10px;""><p style=""Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-size:14px;font-family:verdana, geneva, sans-serif;line-height:21px;color:#333333;"">Для продолжения регистрации вам необходимо подтвердить почту, введя в приложении код, предоставленный ниже:</p></td> </tr> </table></td> </tr> </table></td> </tr> <tr style=""border-collapse:collapse;""> <td style=""Margin:0;padding-bottom:15px;padding-top:20px;padding-left:20px;padding-right:20px;background-color:#A2C4C9;background-position:center top;"" bgcolor=""#a2c4c9"" align=""left""> <table width=""100%"" cellspacing=""0"" cellpadding=""0"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;""> <tr style=""border-collapse:collapse;""> <td width=""560"" valign=""top"" align=""center"" style=""padding:0;Margin:0;""> <table style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;border-left:2px dashed #FFFFFF;border-right:2px dashed #FFFFFF;border-top:2px dashed #FFFFFF;border-bottom:2px dashed #FFFFFF;background-position:left top;"" width=""100%"" cellspacing=""0"" cellpadding=""0"" role=""presentation""> <tr style=""border-collapse:collapse;""> <td align=""center"" style=""padding:0;Margin:0;padding-top:15px;padding-bottom:15px;""><p style=""Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-size:25px;font-family:'open sans', 'helvetica neue', helvetica, arial, sans-serif;line-height:38px;color:#FFFFFF;"">Ваш код подтверждения</p><h1 style=""Margin:0;line-height:54px;mso-line-height-rule:exactly;font-family:'open sans', 'helvetica neue', helvetica, arial, sans-serif;font-size:36px;font-style:normal;font-weight:normal;color:#FFFFFF;"">" + code + @"<br></h1><p style=""Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-size:14px;font-family:'open sans', 'helvetica neue', helvetica, arial, sans-serif;line-height:21px;color:#FFFFFF;"">*введите его в приложении</p></td> </tr> </table></td> </tr> </table></td> </tr> </table></td> </tr> </table></td> </tr> </table> </div> </body></html>";
                m.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient("mail.hosting.reg.ru", 25);
                smtp.Credentials = new NetworkCredential("employment-agency@samars.fun", "Violin146");
                smtp.EnableSsl = true;
                smtp.Send(m);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, e.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static string getRndCode()
        {
            string code = String.Empty;
            Random rnd = new Random();
            for (int i = 0; i < 5; i++)
            {
                if (rnd.Next(0, 2) == 1)
                {
                    code += rnd.Next(0, 10).ToString();
                }
                else
                {
                    code += (char)(rnd.Next(1072, 1103));
                }
            }

            return code;
        }

        public static byte[] imageToByteArray(Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

            return ms.ToArray();
        }

        public static ImageSource byteArrayToImage(byte[] byteArrayIn)
        {
            BitmapImage biImg = new BitmapImage();
            MemoryStream ms = new MemoryStream(byteArrayIn);
            biImg.BeginInit();
            biImg.StreamSource = ms;
            biImg.EndInit();

            return biImg as ImageSource;
        }

        public static string generateSerchQuery(ref ComboBox cbFilters, ref TextBox TSearch, ref ListBox lbFiltersType, string column)
        {
            string cmdText = "";
            if (TSearch.Text.Length > 0 && lbFiltersType.SelectedIndex == -1)
                cmdText = $"SELECT Vacancies.id, Vacancies.image, Vacancies.name, Vacancies.description, Vacancies.address, Vacancies.salary, JobModes.`mode`, Educations.education FROM Vacancies, JobModes, Educations WHERE JobModes.id = Vacancies.`mode` AND Educations.id = Vacancies.education AND Vacancies.{column} LIKE @Search";
            else if (TSearch.Text.Length > 0 && lbFiltersType.SelectedIndex > -1)
            {
                switch (cbFilters.SelectedIndex)
                {
                    case 0:
                    {
                        switch (lbFiltersType.SelectedIndex)
                        {
                            case 0:
                            {
                                cmdText = $"SELECT Vacancies.id, Vacancies.image, Vacancies.name, Vacancies.description, Vacancies.address, Vacancies.salary, JobModes.`mode`, Educations.education FROM Vacancies, JobModes, Educations WHERE JobModes.id = Vacancies.`mode` AND Educations.id = Vacancies.education AND Vacancies.{column} LIKE @Search AND Vacancies.salary < 30000";
                                break;
                            }
                            case 1:
                            {
                                cmdText = $"SELECT Vacancies.id, Vacancies.image, Vacancies.name, Vacancies.description, Vacancies.address, Vacancies.salary, JobModes.`mode`, Educations.education FROM Vacancies, JobModes, Educations WHERE JobModes.id = Vacancies.`mode` AND Educations.id = Vacancies.education AND Vacancies.{column} LIKE @Search AND salary >= 30000 AND Vacancies.salary <= 50000";
                                break;
                            }
                            case 2:
                            {
                                cmdText = $"SELECT Vacancies.id, Vacancies.image, Vacancies.name, Vacancies.description, Vacancies.address, Vacancies.salary, JobModes.`mode`, Educations.education FROM Vacancies, JobModes, Educations WHERE JobModes.id = Vacancies.`mode` AND Educations.id = Vacancies.education AND Vacancies.{column} LIKE @Search AND salary >= 50000 AND Vacancies.salary <= 75000";
                                break;
                            }
                            case 3:
                            {
                                cmdText = $"SELECT Vacancies.id, Vacancies.image, Vacancies.name, Vacancies.description, Vacancies.address, Vacancies.salary, JobModes.`mode`, Educations.education FROM Vacancies, JobModes, Educations WHERE JobModes.id = Vacancies.`mode` AND Educations.id = Vacancies.education AND Vacancies.{column} LIKE @Search AND salary >= 75000 AND Vacancies.salary <= 100000";
                                break;
                            }
                            case 4:
                            {
                                cmdText = $"SELECT Vacancies.id, Vacancies.image, Vacancies.name, Vacancies.description, Vacancies.address, Vacancies.salary, JobModes.`mode`, Educations.education FROM Vacancies, JobModes, Educations WHERE JobModes.id = Vacancies.`mode` AND Educations.id = Vacancies.education AND Vacancies.{column} LIKE @Search AND Vacancies.salary > 100000";
                                break;
                            }
                        }
                        break;
                    }
                    case 1:
                    {
                        cmdText = $"SELECT Vacancies.id, Vacancies.image, Vacancies.name, Vacancies.description, Vacancies.address, Vacancies.salary, JobModes.`mode`, Educations.education FROM Vacancies, JobModes, Educations WHERE JobModes.id = Vacancies.`mode` AND Educations.id = Vacancies.education AND Vacancies.{column} LIKE @Search AND Vacancies.mode = (SELECT id FROM JobModes WHERE mode = '{lbFiltersType.SelectedItem.ToString()}')";
                        break;
                    }
                    case 2:
                    {
                        cmdText = $"SELECT Vacancies.id, Vacancies.image, Vacancies.name, Vacancies.description, Vacancies.address, Vacancies.salary, JobModes.`mode`, Educations.education FROM Vacancies, JobModes, Educations WHERE JobModes.id = Vacancies.`mode` AND Educations.id = Vacancies.education AND Vacancies.{column} LIKE @Search AND Vacancies.education = (SELECT id FROM Educations WHERE education = '{lbFiltersType.SelectedItem.ToString()}')";
                        break;
                    }
                }
            }
            else
            {
                switch (cbFilters.SelectedIndex)
                {
                    case 0:
                    {
                        switch (lbFiltersType.SelectedIndex)
                        {
                            case 0:
                            {
                                cmdText = $"SELECT Vacancies.id, Vacancies.image, Vacancies.name, Vacancies.description, Vacancies.address, Vacancies.salary, JobModes.`mode`, Educations.education FROM Vacancies, JobModes, Educations WHERE JobModes.id = Vacancies.`mode` AND Educations.id = Vacancies.education AND Vacancies.salary < 30000";
                                break;
                            }
                            case 1:
                            {
                                cmdText = $"SELECT Vacancies.id, Vacancies.image, Vacancies.name, Vacancies.description, Vacancies.address, Vacancies.salary, JobModes.`mode`, Educations.education FROM Vacancies, JobModes, Educations WHERE JobModes.id = Vacancies.`mode` AND Educations.id = Vacancies.education AND Vacancies.salary >= 30000 AND Vacancies.salary <= 50000";
                                break;
                            }
                            case 2:
                            {
                                cmdText = $"SELECT Vacancies.id, Vacancies.image, Vacancies.name, Vacancies.description, Vacancies.address, Vacancies.salary, JobModes.`mode`, Educations.education FROM Vacancies, JobModes, Educations WHERE JobModes.id = Vacancies.`mode` AND Educations.id = Vacancies.education AND Vacancies.salary >= 50000 AND Vacancies.salary <= 75000";
                                break;
                            }
                            case 3:
                            {
                                cmdText = $"SELECT Vacancies.id, Vacancies.image, Vacancies.name, Vacancies.description, Vacancies.address, Vacancies.salary, JobModes.`mode`, Educations.education FROM Vacancies, JobModes, Educations WHERE JobModes.id = Vacancies.`mode` AND Educations.id = Vacancies.education AND Vacancies.salary >= 75000 AND Vacancies.salary <= 100000";
                                break;
                            }
                            case 4:
                            {
                                cmdText = $"SELECT Vacancies.id, Vacancies.image, Vacancies.name, Vacancies.description, Vacancies.address, Vacancies.salary, JobModes.`mode`, Educations.education FROM Vacancies, JobModes, Educations WHERE JobModes.id = Vacancies.`mode` AND Educations.id = Vacancies.education AND Vacancies.salary > 100000";
                                break;
                            }
                        }
                        break;
                    }
                    case 1:
                    {
                        cmdText = $"SELECT Vacancies.id, Vacancies.image, Vacancies.name, Vacancies.description, Vacancies.address, Vacancies.salary, JobModes.`mode`, Educations.education FROM Vacancies, JobModes, Educations WHERE JobModes.id = Vacancies.`mode` AND Educations.id = Vacancies.education AND  Vacancies.mode = (SELECT id FROM JobModes WHERE mode = '{lbFiltersType.SelectedItem.ToString()}')";
                        break;
                    }
                    case 2:
                    {
                        cmdText = $"SELECT Vacancies.id, Vacancies.image, Vacancies.name, Vacancies.description, Vacancies.address, Vacancies.salary, JobModes.`mode`, Educations.education FROM Vacancies, JobModes, Educations WHERE JobModes.id = Vacancies.`mode` AND Educations.id = Vacancies.education AND Vacancies.education = (SELECT id FROM Educations WHERE education = '{lbFiltersType.SelectedItem.ToString()}')";
                        break;
                    }
                }
            }
            return cmdText;
        }

        public static void summaryPdf(string fio, string phone, string adr, string spec, string dolj, string edu, string birth, string jobmode, string photoPath, string saveTo)
        {
            PdfDocument inputDocument = PdfReader.Open("stemplate.pdf", PdfDocumentOpenMode.Import);
            PdfPage notEditablePage = inputDocument.Pages[0];

            PdfDocument outputDocument = new PdfDocument();
            PdfPage editablePage = outputDocument.AddPage(notEditablePage);

            XGraphics gfx = XGraphics.FromPdfPage(editablePage);
            XImage img = XImage.FromFile(photoPath);
            XFont font = new XFont("Times New Roman", 14);
            gfx.DrawString(fio, font, XBrushes.Black, 120.31, 86.8);
            gfx.DrawString(phone, font, XBrushes.Black, 142.17, 105.26);
            gfx.DrawString(adr, font, XBrushes.Black, 127.81, 121.66);
            gfx.DrawString(spec, font, XBrushes.Black, 181.81, 139.16);
            gfx.DrawString(dolj, font, XBrushes.Black, 159.25, 156.66);
            gfx.DrawString(edu, font, XBrushes.Black, 167, 174.16);
            gfx.DrawString(birth, font, XBrushes.Black, 180.5, 191.66);
            gfx.DrawString(jobmode, font, XBrushes.Black, 176.34, 209.16);
            gfx.DrawString(DateTime.Now.ToString("MM.dd.yyyy"), font, XBrushes.Black, 223.5, 475.71);
            gfx.DrawImage(img, 224, 230.34, 155.35, 195.48);

            outputDocument.Save(saveTo);
            Process.Start(saveTo);
        }
        public static void blankPdf(string fio, string phone, string adr, string spec, string dolj, string edu, string oklad, string jobmode, string vacancyID, string saveTo)
        {
            PdfDocument inputDocument = PdfReader.Open("btemplate.pdf", PdfDocumentOpenMode.Import);
            PdfPage notEditablePage = inputDocument.Pages[0];

            PdfDocument outputDocument = new PdfDocument();
            PdfPage editablePage = outputDocument.AddPage(notEditablePage);

            XGraphics gfx = XGraphics.FromPdfPage(editablePage);
            XFont font = new XFont("Times New Roman", 14);
            gfx.DrawString(fio, font, XBrushes.Black, 120.31, 86.8);
            gfx.DrawString(phone, font, XBrushes.Black, 142.17, 105.26);
            gfx.DrawString(adr, font, XBrushes.Black, 127.81, 121.66);
            gfx.DrawString(spec, font, XBrushes.Black, 181.81, 139.16);
            gfx.DrawString(dolj, font, XBrushes.Black, 159.25, 156.66);
            gfx.DrawString(edu, font, XBrushes.Black, 167, 174.16);
            gfx.DrawString(oklad, font, XBrushes.Black, 129.18, 191.66);
            gfx.DrawString(jobmode, font, XBrushes.Black, 176.34, 209.16);
            gfx.DrawString(vacancyID, font, XBrushes.Black, 187.18, 227.60);
            gfx.DrawString(DateTime.Now.ToString("MM.dd.yyyy"), font, XBrushes.Black, 223.5, 260.41);

            outputDocument.Save(saveTo);
            Process.Start(saveTo);
        }
    }
}
