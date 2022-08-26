using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SachDienTu.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace SachDienTu.Controllers
{
    public class InstallController : Controller
    {
        // GET: Install
        public ActionResult Index()
        {
            string openTheSettingsPage = ConfigurationManager.AppSettings["OpenTheSettingsPage"];
            if (openTheSettingsPage == "false")
                return RedirectToAction("Index", "Home");

            using(SqlConnection sqlConnection = new SqlConnection())
            {
                sqlConnection.ConnectionString = ConfigurationManager.ConnectionStrings["Installation"].ConnectionString;
                sqlConnection.Open();

                string sqlFilePath = HttpContext.Server.MapPath("~/SQL/SachDienTu.sql");

                string sql = System.IO.File.ReadAllText(sqlFilePath);
                IEnumerable<string> commandStrings = Regex.Split(sql, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

                SqlCommand sqlCommand = sqlConnection.CreateCommand();
               

                foreach (string commandString in commandStrings)
                {
                    if (!string.IsNullOrWhiteSpace(commandString.Trim()))
                    {
                        sqlCommand.CommandText = commandString;
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.ExecuteNonQuery();
                    }
                }
            }

            return RedirectToAction("CreateDefaultUser");
        }

        public ActionResult CreateDefaultUser()
        {
            string openTheSettingsPage = ConfigurationManager.AppSettings["OpenTheSettingsPage"];
            if (openTheSettingsPage == "false")
                return RedirectToAction("Index", "Home");

            ApplicationDbContext db = new ApplicationDbContext();

            RoleStore<IdentityRole> roleStore = new RoleStore<IdentityRole>(db);
            UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(db);
            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(roleStore);
            UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(userStore);

            roleManager.Create(new IdentityRole { Name = "Super Admin" });
            roleManager.Create(new IdentityRole { Name = "Admin" });
            roleManager.Create(new IdentityRole { Name = "SEOer" });

            ApplicationUser applicationUser = new ApplicationUser
            {
                FirstName = ConfigurationManager.AppSettings["DefaultUser_FirstName"],
                LastName = ConfigurationManager.AppSettings["DefaultUser_LastName"],
                UserName = ConfigurationManager.AppSettings["DefaultUser_UserName"],
                Email = ConfigurationManager.AppSettings["DefaultUser_Email"]
            };

            IdentityResult identityResult = userManager
                .Create(applicationUser, ConfigurationManager.AppSettings["DefaultUser_Password"]);

            if (identityResult.Succeeded)
                userManager.AddToRole(applicationUser.Id, "Super Admin");


            return RedirectToAction("Index", "Home");
        }
    }
}