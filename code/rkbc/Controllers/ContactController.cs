using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using ElmahCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using rkbc.config.models;
using rkbc.core.models;
using rkbc.core.repository;
using rkbc.core.service;
using rkbc.web.controllers;

namespace rkbc.web.controllers
{
    public class ContactController : AppBaseController
    {
        
        protected EmailSettings emailSettings;
        public ContactController(IUnitOfWork _unitOfWork, UserService _userService, IOptions<EmailSettings> _emailSettings) : base(_unitOfWork, _userService)
        {
            emailSettings = _emailSettings.Value;
        }
        public IActionResult Index()
        {
            Contact vmodel = new Contact();
            return View(vmodel);
        }
        [HttpPost]
        public async Task<IActionResult> SendEmail()
        {
            Contact vmodel = new Contact();
            await TryUpdateModelAsync(vmodel);
            var currentUser = userService.CurrentUserSettings;

            var modelObj = new Contact();
            modelObj.createDt = DateTime.UtcNow;
            modelObj.createUser = currentUser.userId;
            modelObj.name = vmodel.name;
            modelObj.email = vmodel.email;
            modelObj.subject = vmodel.subject;
            modelObj.message = vmodel.message;
            unitOfWork.contacts.add(modelObj);

            var composeMessage = "<p>Email From: " + modelObj.email + "</p>";
            composeMessage += "<p>Name: " + modelObj.name + "</p>";
            composeMessage += "<p>Message: " + modelObj.message + "</p>";

            using (SmtpClient smtp = new SmtpClient(emailSettings.PrimaryDomain, emailSettings.PrimaryPort))
            {
                
                using (MailMessage mail = new MailMessage())
                {
                    try
                    {
                        mail.From = new MailAddress(modelObj.email, modelObj.name);
                        string toEmail = emailSettings.ToEmail;
                        mail.To.Add(new MailAddress(toEmail));
                        //mail.CC.Add(new MailAddress(_emailSettings.CcEmail));

                        mail.Subject = modelObj.subject;
                        mail.Body = composeMessage;
                        mail.IsBodyHtml = true;
                        //mail.Priority = MailPriority.High;
                        smtp.Credentials = new NetworkCredential(emailSettings.UsernameEmail, emailSettings.UsernamePassword);
                        smtp.EnableSsl = true;
                        await smtp.SendMailAsync(mail);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Failed sent an email.");
                        HttpContext.RiseError(new InvalidOperationException("Failed to send an email."));
                    }
                }
            }
            if (ModelState.ErrorCount == 0)
            {
                var success = await unitOfWork.tryCommitAsync();
                if (success)
                {
                   TempData["message"] = "Email Sent.";
                    return RedirectToAction("Index", "Contact");
                }
                ModelState.AddModelError("", "Unable to update data.");
            }
            
            return View("Index", vmodel);
        }
    }
}