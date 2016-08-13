using DesignAndPrint.Helpers;
using DesignAndPrint.Models;
using HtmlAgilityPack;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DesignAndPrint.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View();
        }
        [HttpPost]
        public virtual ActionResult CropImage(
         string imagePath,
         int? cropPointX,
         int? cropPointY,
         int? imageCropWidth,
         int? imageCropHeight)
        {
            if (string.IsNullOrEmpty(imagePath)
                || !cropPointX.HasValue
                || !cropPointY.HasValue
                || !imageCropWidth.HasValue
                || !imageCropHeight.HasValue)
            {
                return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);
            }

            byte[] imageBytes = System.IO.File.ReadAllBytes(Server.MapPath(imagePath));
            byte[] croppedImage = ImageHelper.CropImage(imageBytes, cropPointX.Value, cropPointY.Value, imageCropWidth.Value, imageCropHeight.Value);

            string fileName = Path.GetFileName(imagePath);

            try
            {
                FileHelper.SaveFile(croppedImage, Server.MapPath(imagePath));
            }
            catch (Exception ex)
            {
                //Log an error     
                return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError);
            }

            string photoPath = string.Concat("/", fileName);
            return Json(new { photoPath = imagePath }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ChangeTemplate(string templateName)
        {
            var templates = new List<Templates>() {
                                new DesignAndPrint.Models.Templates()
                                {
                                    TemplateName = "template-square",
                                    TemplateClassName = "col-sm-12 template-square",
                                         BoxClassName = "col-sm-3 template-box-square",
                                    BoxCount = 16
                                },
                                 new DesignAndPrint.Models.Templates()
                                {
                                    TemplateName = "template-rectangle",
                                    TemplateClassName = "col-sm-12 template-rectangle",
                                    BoxClassName = "col-sm-6 template-box-rectangle",
                                    BoxCount = 8
                                },
                                  new DesignAndPrint.Models.Templates()
                                {
                                    TemplateName = "template-ellipsis",
                                    TemplateClassName = "col-sm-12 template-ellipsis",
                                         BoxClassName = "col-sm-6 template-box-ellipsis",
                                    BoxCount = 8
                                },
                                   new DesignAndPrint.Models.Templates()
                                {
                                    TemplateName = "template-round",
                                    TemplateClassName = "col-sm-12 template-round",
                                       BoxClassName = "col-sm-3 template-box-round",
                                    BoxCount = 16
                                }
                            };
            var template = new Templates();

            foreach (var item in templates)
            {
                if (item.TemplateName == templateName)
                    template = item;

            }
            return PartialView("Rows/Template", template);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult DownloadStickers(string html, string pagesize)
        {
            if (String.IsNullOrWhiteSpace(pagesize))
                return Json(false);
            try
            {
                byte[] bytes = GeneratePDF(html, pagesize);

                // Generate a new unique identifier against which the file can be stored
                string handle = Guid.NewGuid().ToString();
                Session[handle] = bytes.ToArray();

                //return File(bytes, "application/pdf", DateTime.Now.ToString(CultureInfo.InvariantCulture));

                // Note we are returning a filename as well as the handle
                return new JsonResult()
                {
                    Data = new { fileGuid = handle }
                };
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }
        [HttpGet]
        public virtual ActionResult Download(string fileGuid)
        {
            if (Session[fileGuid] != null)
            {
                byte[] data = Session[fileGuid] as byte[];
                return File(data, "application/pdf", DateTime.Now.ToString(CultureInfo.InvariantCulture) + ".pdf");
            }
            else
            {
                // Problem - Log the error, generate a blank file,
                //           redirect to another controller action - whatever fits with your application
                return new EmptyResult();
            }
        }
        public byte[] GeneratePDF(string html, string pageSize)
        {
            #region Generate PDF
            Byte[] bytes;
            var ms = new MemoryStream();
            //Create an iTextSharp Document wich is an abstraction of a PDF but **NOT** a PDF
            var doc = new Document();
            if (pageSize == "A4")
                doc = new Document(PageSize.A4);
            else
                doc = new Document(PageSize.LETTER);
            var writer = PdfWriter.GetInstance(doc, ms);
            doc.Open();
            doc.NewPage();
            var hDocument = new HtmlDocument()
            {
                OptionWriteEmptyNodes = true,
                OptionAutoCloseOnEnd = true
            };
            hDocument.LoadHtml(html);

            var closedTags = hDocument.DocumentNode.WriteTo();
            var example_html = closedTags;
            var example_css = System.IO.File.ReadAllText(Server.MapPath("~/Content/Site.css"));
            example_css += System.IO.File.ReadAllText(Server.MapPath("~/Content/Templates.min.css"));
            var msCss = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(example_css));
            var msHtml = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(example_html));
            iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msHtml, msCss, Encoding.UTF8);
            doc.Close();
            bytes = ms.ToArray();

            var testFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "test.pdf");
            System.IO.File.WriteAllBytes(testFile, bytes);
            #endregion

            return bytes;
        }

        public ActionResult SentEmailAjax(string Email, string NameUser, string Subject, string Body)
        {
            if (Request.IsAjaxRequest())
            {
                var sb = new StringBuilder();
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine("From Email : " + Email);
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine("User Name : " + NameUser);
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine("Subject : " + Subject);
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine(Body);
                if (!SendEmail(ConfigurationManager.AppSettings["ContactEmail"], "New Email - " + DateTime.Now.ToString("dd-MM-yyyy"), sb.ToString()))
                {

                    return Json(false);
                }

                return Json(true);

            }
            return Json(false);
        }

        public Boolean SendEmail(string reciever, string subject, string body, byte[] FileByte, string NameAtachemnt)
        {
            try
            {
                SmtpClient SmtpServer = new SmtpClient();
                MailMessage mail = new MailMessage();
                mail.To.Add(reciever);
                mail.Subject = subject;
                mail.Body = body;
                Attachment file = new Attachment(new MemoryStream(FileByte), NameAtachemnt);
                mail.Attachments.Add(file);
                mail.IsBodyHtml = true;
                SmtpServer.Send(mail);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public Boolean SendEmail(string reciever, string subject, string body)
        {
            try
            {
                SmtpClient SmtpServer = new SmtpClient();
                MailMessage mail = new MailMessage();
                mail.To.Add(reciever);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                SmtpServer.Send(mail);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
