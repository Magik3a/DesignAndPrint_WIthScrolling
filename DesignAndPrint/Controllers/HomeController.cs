using DesignAndPrint.Helpers;
using DesignAndPrint.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
