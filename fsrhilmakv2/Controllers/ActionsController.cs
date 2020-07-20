﻿using fsrhilmakv2.Extra;
using fsrhilmakv2.Models;
using fsrhilmakv2.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace fsrhilmakv2.Controllers
{
    [Authorize]
    [RoutePrefix("api/actions")]
    public class ActionsController : ApiController
    {
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();
        private CoreController core = new CoreController();
        private UserHelperLibrary helper = new UserHelperLibrary();


        // POST api/ActionsController/AddExplanation
        [Route("AddExplanation")]
        [HttpPost]
        public Service AddExplanation([FromBody] Service temp)
        {
            if (temp.id.Equals(null))
            {
                core.throwExcetpion("Id is null");
            }
            Service service = db.Services.Where(a => a.id.Equals(temp.id))
                .FirstOrDefault();
            if (temp.Explanation.Equals("") || temp.Explanation == null)
            {
                core.throwExcetpion("Explanation can't be null");
            }

            service.Explanation = temp.Explanation;
            service.ExplanationDate = DateTime.Now;
            service.Status = CoreController.ServiceStatus.Done.ToString();
           
            SaveService(service);
            return service;
        }

        // POST api/ActionsController/AddRating
        [Route("AddRating")]
        [HttpPost]
        public Service AddRating([FromBody] Service temp)
        {
            if (temp.id.Equals(null))
            {
                core.throwExcetpion("Id is null");
            }
            Service service = db.Services.Where(a => a.id.Equals(temp.id))
                .FirstOrDefault();
            if (!service.Status.Equals("Done"))
                core.throwExcetpion("User can't rate interpreter until he explain the dream!");
            if (temp.UserRating < 0 || temp.UserRating > 5)
            {
                core.throwExcetpion("Rating can only be between 0 and 5");
            }
            service.RatingMessage = temp.RatingMessage;
            service.UserRating = temp.UserRating;
            service.RatingDate = DateTime.Now;

            SaveService(service);
            return service;
        }


        [Route("GetSingleServiceInfo")]
        [HttpGet]
        [AllowAnonymous]
        public ServiceViewModel GetSingleServiceInfo([FromUri] int id)
        {
            if (id.Equals(null))
            {
                core.throwExcetpion("Id is null");
            }
            Service service = db.Services.Where(a => a.id.Equals(id))
                .Include("Comments")
                .Include("ServicePath")
                .Include("UserWork")
                .Include("ServiceProvider")
                .Include("Creator")
                .FirstOrDefault();
            return getMapping(service);
            
        }
        public void SaveService(Service Service)
        {
            Service.LastModificationDate = DateTime.Now;
            Service.ModifierId = core.getCurrentUser().Id;
            db.Entry(Service).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }

        public ServiceViewModel getMapping(Service service)
        {
            List<Service> allService = db.Services.Where(a => a.ServiceProviderId.Equals(service.ServiceProviderId)
                && a.ServicePathId.Equals(service.ServicePathId)
                && a.Status.Equals(CoreController.ServiceStatus.Active.ToString())).ToList();

            List<Service> services = helper.getUserServices(service.ServiceProviderId);
            List<Service> activeSerives = helper.getServicesFiltered(services, CoreController.ServiceStatus.Active.ToString());
            List<Service> doneServices = helper.getServicesFiltered(services, CoreController.ServiceStatus.Done.ToString());
            double speed = UserHelperLibrary.ServiceProviderSpeed(helper.findUser(service.ServiceProviderId), doneServices.Count);

            AccountController accountCont = new AccountController();
            ServiceViewModel result = new ServiceViewModel();
            result.Comments = service.Comments;
            result.Country = service.Country;
            result.CreationDate = service.CreationDate;
            result.Creator = service.Creator;
            result.Description = service.Description;
            result.CreatorId = service.CreatorId;
            result.DidYouExorcism = service.DidYouExorcism;
            result.DreamDate = service.DreamDate;
            result.Explanation = service.Explanation;
            result.ExplanationDate = service.ExplanationDate;
            result.HaveYouPrayedBeforeTheDream = service.HaveYouPrayedBeforeTheDream;
            result.Id = service.id;
            result.IsThereWakefulness = service.IsThereWakefulness;
            result.JobStatus = service.JobStatus;
            result.KidsStatus = service.KidsStatus;
            result.LastModificationDate = service.LastModificationDate;
            result.Modifier = service.Modifier;
            result.ModifierId = service.ModifierId;
            result.Name = service.Name;
            result.numberOfLikes = service.numberOfLikes;
            result.numberOfViews = service.numberOfViews;
            result.PrivateService = service.PrivateService;
            result.PrivateServicePrice = service.PrivateServicePrice;
            result.PublicServiceAction = service.PublicServiceAction;
            result.RegligionStatus = service.RegligionStatus;
            result.ServicePathId = service.ServicePathId;
            result.ServiceProvider = service.ServiceProvider;
            result.ServiceProviderId = service.ServiceProviderId;
            result.Sex = service.Sex;
            result.SocialStatus = service.SocialStatus;
            result.Status = service.Status;
            result.UserWork = service.UserWork;
            result.UserWorkId = service.UserWorkId;
            result.ServicePath = service.ServicePath;
            result.NumberOfAllPeopleWaiting = allService.Count > 0 ? allService.Count : 0;
            result.NumberOfRemainingPeople = allService.Count > 0 ? allService.Where(a => a.CreationDate.CompareTo(service.CreationDate) < 0).Count() : 0;
            result.AvgWaitingTime= UserHelperLibrary.getWaitingTimeMessage(Double.Parse(speed.ToString()),
                Double.Parse(result.NumberOfRemainingPeople.ToString())).Replace("Your average waiting time is ", "");




            return result;
        }
    }
}