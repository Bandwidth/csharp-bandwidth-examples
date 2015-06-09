using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Bandwidth.Net.Model;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TranscriptionApp.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public virtual IList<VoiceMessage> VoiceMessages { get; set; }

        public virtual IList<ActiveCall> ActiveCalls { get; set; }

        public string Greeting { get; set; }

        public Task PlayGreeting(Call call)
        {
            var data = new Dictionary<string, object>{{"tag", "greeting"}};
            if(!string.IsNullOrEmpty(Greeting)){
              data["fileUrl"] = Greeting;
            }
            else{
              data["gender"] = "female";
              data["locale"] = "en_US";
              data["voice"] = "kate";
              data["sentence"] = string.Format("You have reached the voice mailbox for {0}. Please leave a message at the beep", PhoneNumber);
            }
            return call.PlayAudio(data);
        }
    }

    public class VoiceMessage
    {
        public string Url { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public virtual ApplicationUser User { get; set; }
    }

    public class ActiveCall
    {
        public string Id { get; set; }
        public virtual ApplicationUser User { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public IDbSet<ActiveCall> ActiveCalls { get; set; }
        public IDbSet<VoiceMessage> VoiceMessages { get; set; } 
    }
}