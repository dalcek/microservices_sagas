using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Messages.Events;
using Messages.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NServiceBus;
using UsersMicroservice.Models;
using UsersMicroservice.Models.AppSettings;
using UsersMicroservice.Models.ContextData;
using UsersMicroservice.Models.Model;

namespace UsersMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppUserController : ControllerBase
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private readonly ApplicationSettings _appSettings;
        private ApplicationUsersContext _context2;
        private IMessageSession _messageSession;
        public IConfiguration Configuration { get; }

        public AppUserController(ApplicationUsersContext app2, UserManager<User> userManager, SignInManager<User> signInManager, IOptions<ApplicationSettings> appSettings, IConfiguration configuration, IMessageSession messageSession)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appSettings = appSettings.Value;
            _context2 = app2;
            Configuration = configuration;
            _messageSession = messageSession;
        }


        [HttpPost]
        [Route("MakeCombinedReservation")]
        public async Task<Object> MakeCombinedReservation(Messages.Model.CombinedReservation combinedReservation)
        {
            //napravimo pending rezervaciju
            //napravimo event da se rezervise let

            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RegisteredUser")
            {
                return "Forbbiden action for this role.";
            }

            var user = await _userManager.FindByIdAsync(userId);
            if(user.CombinedReservations == null)
            {
                user.CombinedReservations = new List<Models.Model.CombinedReservation>();
            }
            Models.Model.CombinedReservation reservation = new Models.Model.CombinedReservation();
            reservation.Status = "Pending";
            reservation.DateMade = DateTime.Now.ToShortDateString();
            reservation.Description = "";
            reservation.Description += "Flight reservation \n" + Environment.NewLine + "Departing date: " + DateTime.Parse(combinedReservation.car.from).ToShortDateString() + " at: " + DateTime.Parse(combinedReservation.car.from).ToShortTimeString() + "\n" + System.Environment.NewLine + "Seat type: " + combinedReservation.tickets[0].seat.Type + ", number of tickets: " + combinedReservation.tickets.Count.ToString() + ". \n";
            reservation.Description += System.Environment.NewLine + "Car reservation \n" + System.Environment.NewLine + "From: " + DateTime.Parse(combinedReservation.car.from).ToShortDateString() + " - To: " + DateTime.Parse(combinedReservation.car.to).ToShortDateString() + ".";
            
            user.CombinedReservations.Add(reservation);

            await _userManager.UpdateAsync(user);

            _context2.Update(user);

            try
            {
                _context2.SaveChanges();
            }
            catch (Exception e)
            {
                return "Error creating reservation";
            }

            ReservationCreated resCreated = new ReservationCreated();
            resCreated.combinedReservationId = Guid.NewGuid().ToString();
            resCreated.resId = reservation.Id;
            resCreated.tickets = combinedReservation.tickets;
            resCreated.car = combinedReservation.car;
            resCreated.userId = User.Claims.First(c => c.Type == "UserID").Value;
            resCreated.pointsS = User.Claims.First(c => c.Type == "Points").Value;
            await _messageSession.Publish(resCreated).ConfigureAwait(false);
            return Ok();
        }

        [HttpPost]
        [Route("ResetPoints")]
        public async Task<Object> ResetPoints()
        {
            var user = await _userManager.FindByNameAsync("ivana");
            if(user != null)
            {
                user.Points = 0;
                await _userManager.UpdateAsync(user);
                _context2.Update(user);
                _context2.SaveChanges();
            }
            return Ok();
        }

        [HttpPost]
        [Route("CheckUsername")]
        public async Task<bool> CheckUsername(UsernameModel id)
        {
            var user = await _userManager.FindByNameAsync(id.username);
            if (user == null)
                return true;
            else
                return false;
        }

        [HttpPost]
        [Route("CheckPassword")]
        public async Task<IActionResult> CheckPassword(UsernameModel id)
        {

            string userid = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userid);
            var ret = await _userManager.CheckPasswordAsync(user, id.username);
            return Ok(new { ret });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("IsAdmin")]
        public async Task<Object> IsAdmin(IsAdminModel model)
        {
            string userid = User.Claims.First(c => c.Type == "UserID").Value;
            string role = User.Claims.First(c => c.Type == "Roles").Value;
            var user = await _userManager.FindByIdAsync(userid);

            if (role == "AirlineAdministrator" && user.CompanyId == model.id)
                return true;

            return false;

        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("SetPoints")]
        public async Task<Object> SetPoints()
        {
            string userid = User.Claims.First(c => c.Type == "UserID").Value;
            string role = User.Claims.First(c => c.Type == "Roles").Value;
            var user = await _userManager.FindByIdAsync(userid);

            user.Points += 5;
            _context2.SaveChanges();
            return false;

        }

        [HttpPost]
        [Route("ChangePassword")]
        public async Task<Object> ChangePassword(ChangePasswordModel model)
        {
            List<User> users = _context2.ApplicationUsers.
               Include(comp => comp.FriendRequests).

               Include(comp => comp.SentRequests)
               .Include(comp => comp.Friends)
               .Include(comp => comp.Notifications).ToList();
            string userid = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userid);
            await _userManager.ChangePasswordAsync(user, model.password, model.newPassword);
            user.ChangedPassword = true;
            _context2.Update(user);
            _context2.SaveChanges();
            UserModel um = new UserModel();
            um.FullName = user.FullName;
            um.Address = user.Address;
            um.Birthday = user.Birthday.ToString();
            um.Passport = user.Passport;


            um.Id = user.Id;
            um.Username = user.UserName;
            um.Points = user.Points;
            um.Email = user.Email;
            um.CompanyId = user.CompanyId;


            um.Friends = new List<UserModel>();
            um.SentRequests = new List<FriendRequestSentModel>();
            um.Notifications = new List<Notification>();
            um.FriendRequests = new List<FriendRequestReceivedModel>();


            if (user.Friends == null)
                user.Friends = new List<User>();
            foreach (var fr in user.Friends)
            {
                um.Friends.Add(new UserModel()
                {
                    Address = fr.Address,
                    Email = fr.Email,
                    Birthday = fr.Birthday.ToString(),
                    FullName = fr.FullName,
                    Username = fr.UserName,
                    Id = fr.Id,
                    Passport = fr.Passport
                });
            }
            if (user.SentRequests == null)
                user.SentRequests = new List<FriendRequestSent>();
            foreach (var fr in user.SentRequests)
            {
                User pom = await _userManager.FindByIdAsync(fr.User);
                um.SentRequests.Add(new FriendRequestSentModel()
                {
                    Id = fr.Id,
                    User = new UserModel()
                    {
                        Address = pom.Address,
                        Email = pom.Email,
                        Birthday = pom.Birthday.ToString(),
                        FullName = pom.FullName,
                        Username = pom.UserName,
                        Id = pom.Id,
                        Passport = pom.Passport
                    }
                });
            }
            if (user.FriendRequests == null)
                user.FriendRequests = new List<FriendRequestReceived>();
            foreach (var fr in user.FriendRequests)
            {
                User pom = await _userManager.FindByIdAsync(fr.User);
                um.FriendRequests.Add(new FriendRequestReceivedModel()
                {
                    Id = fr.Id,
                    User = new UserModel()
                    {
                        Address = pom.Address,
                        Email = pom.Email,
                        Birthday = pom.Birthday.ToString(),
                        FullName = pom.FullName,
                        Username = pom.UserName,
                        Id = pom.Id,
                        Passport = pom.Passport
                    }
                });
            }
            if (user.Notifications == null)
                user.Notifications = new List<Notification>();
            foreach (var fr in user.Notifications)
            {
                um.Notifications.Add(new Notification() { Id = fr.Id, Text = fr.Text });
            }

            return um;
        }

        [HttpPost]
        [Route("SaveNewAccountDetails")]
        public async Task<Object> SaveNewAccountDetails(ChangeAccountModel model)
        {
            List<User> users = _context2.ApplicationUsers.
               Include(comp => comp.FriendRequests).

               Include(comp => comp.SentRequests)
               .Include(comp => comp.Friends)
               .Include(comp => comp.Notifications).ToList();

            string userid = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userid);
            if (user.Email != model.email)
            {
                user.EmailConfirmed = false;
                string toMail = "http://localhost:57886/api/AppUser/VerifyEmail/" + user.Id;

                MailMessage mail = new MailMessage();
                mail.To.Add(model.email);
                mail.From = new MailAddress("web2020tim1718@gmail.com");
                mail.Subject = "Projekat";
                mail.Body = "Please verify your e-mail address by clicking this link: ";
                mail.Body += toMail;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new System.Net.NetworkCredential("web2020tim1718@gmail.com", "NinaStanka987");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
            user.Email = model.email;
            user.FullName = model.fullName;
            user.Address = model.address;
            _context2.Update(user);
            _context2.SaveChanges();
            UserModel um = new UserModel();
            um.FullName = user.FullName;
            um.Address = user.Address;
            um.Birthday = user.Birthday.ToString();
            um.Passport = user.Passport;


            um.Id = user.Id;
            um.Username = user.UserName;
            um.Points = user.Points;
            um.Email = user.Email;
            um.CompanyId = user.CompanyId;

            um.Friends = new List<UserModel>();
            um.SentRequests = new List<FriendRequestSentModel>();
            um.Notifications = new List<Notification>();
            um.FriendRequests = new List<FriendRequestReceivedModel>();

            if (user.Friends == null)
                user.Friends = new List<User>();
            foreach (var fr in user.Friends)
            {
                um.Friends.Add(new UserModel()
                {
                    Address = fr.Address,
                    Email = fr.Email,
                    Birthday = fr.Birthday.ToString(),
                    FullName = fr.FullName,
                    Username = fr.UserName,
                    Id = fr.Id,
                    Passport = fr.Passport
                });
            }
            if (user.SentRequests == null)
                user.SentRequests = new List<FriendRequestSent>();
            foreach (var fr in user.SentRequests)
            {
                User pom = await _userManager.FindByIdAsync(fr.User);
                um.SentRequests.Add(new FriendRequestSentModel()
                {
                    Id = fr.Id,
                    User = new UserModel()
                    {
                        Address = pom.Address,
                        Email = pom.Email,
                        Birthday = pom.Birthday.ToString(),
                        FullName = pom.FullName,
                        Username = pom.UserName,
                        Id = pom.Id,
                        Passport = pom.Passport
                    }
                });
            }
            if (user.FriendRequests == null)
                user.FriendRequests = new List<FriendRequestReceived>();
            foreach (var fr in user.FriendRequests)
            {
                User pom = await _userManager.FindByIdAsync(fr.User);
                um.FriendRequests.Add(new FriendRequestReceivedModel()
                {
                    Id = fr.Id,
                    User = new UserModel()
                    {
                        Address = pom.Address,
                        Email = pom.Email,
                        Birthday = pom.Birthday.ToString(),
                        FullName = pom.FullName,
                        Username = pom.UserName,
                        Id = pom.Id,
                        Passport = pom.Passport
                    }
                });
            }
            if (user.Notifications == null)
                user.Notifications = new List<Notification>();
            foreach (var fr in user.Notifications)
            {
                um.Notifications.Add(new Notification() { Id = fr.Id, Text = fr.Text });
            }

            return um;
        }

        [HttpPost]
        [Route("ChangeUserName")]
        public async Task<Object> ChangeUserName(UsernameModel id)
        {
            List<User> users = _context2.ApplicationUsers.
               Include(comp => comp.FriendRequests).

               Include(comp => comp.SentRequests)
               .Include(comp => comp.Friends)
               .Include(comp => comp.Notifications).ToList();
            string userid = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userid);
            user.UserName = id.username;
            _context2.Update(user);
            _context2.SaveChanges();
            UserModel um = new UserModel();
            um.FullName = user.FullName;
            um.Address = user.Address;
            um.Birthday = user.Birthday.ToString();
            um.Passport = user.Passport;


            um.Id = user.Id;
            um.Username = user.UserName;
            um.Points = user.Points;
            um.Email = user.Email;
            um.CompanyId = user.CompanyId;

            um.Friends = new List<UserModel>();
            um.SentRequests = new List<FriendRequestSentModel>();
            um.Notifications = new List<Notification>();
            um.FriendRequests = new List<FriendRequestReceivedModel>();

            if (user.Friends == null)
                user.Friends = new List<User>();
            foreach (var fr in user.Friends)
            {
                um.Friends.Add(new UserModel()
                {
                    Address = fr.Address,
                    Email = fr.Email,
                    Birthday = fr.Birthday.ToString(),
                    FullName = fr.FullName,
                    Username = fr.UserName,
                    Id = fr.Id,
                    Passport = fr.Passport
                });
            }
            if (user.SentRequests == null)
                user.SentRequests = new List<FriendRequestSent>();
            foreach (var fr in user.SentRequests)
            {
                User pom = await _userManager.FindByIdAsync(fr.User);
                um.SentRequests.Add(new FriendRequestSentModel()
                {
                    Id = fr.Id,
                    User = new UserModel()
                    {
                        Address = pom.Address,
                        Email = pom.Email,
                        Birthday = pom.Birthday.ToString(),
                        FullName = pom.FullName,
                        Username = pom.UserName,
                        Id = pom.Id,
                        Passport = pom.Passport
                    }
                });
            }
            if (user.FriendRequests == null)
                user.FriendRequests = new List<FriendRequestReceived>();
            foreach (var fr in user.FriendRequests)
            {
                User pom = await _userManager.FindByIdAsync(fr.User);
                um.FriendRequests.Add(new FriendRequestReceivedModel()
                {
                    Id = fr.Id,
                    User = new UserModel()
                    {
                        Address = pom.Address,
                        Email = pom.Email,
                        Birthday = pom.Birthday.ToString(),
                        FullName = pom.FullName,
                        Username = pom.UserName,
                        Id = pom.Id,
                        Passport = pom.Passport
                    }
                });
            }
            if (user.Notifications == null)
                user.Notifications = new List<Notification>();
            foreach (var fr in user.Notifications)
            {
                um.Notifications.Add(new Notification() { Id = fr.Id, Text = fr.Text });
            }

            return um;
        }

        [HttpPost]
        [Route("SocialLogIn")]
        public async Task<IActionResult> SocialLogIn(SocialLogInModel model)
        {
            if (!VerifyToken(model.IdToken))
            {
                return BadRequest(new { message = "Account token could not be verified." });
            }
            var user = await _userManager.FindByNameAsync(model.UserId);
            if (user == null)
            {
                // pravimo novog korisnika
                var applicationUser = new User()
                {
                    UserName = model.UserId,
                    Email = model.EmailAddress,
                    FullName = model.FirstName + " " + model.LastName,
                    Birthday = DateTime.Now,
                    Address = "",
                    PhoneNumber = "",
                    Passport = "",
                    SocialUser = true,
                };

                try
                {
                    IdentityResult result = await _userManager.CreateAsync(applicationUser);

                    if (result.Succeeded)
                    {
                        _userManager.AddToRoleAsync(applicationUser, "RegisteredUser").Wait();
                        user = await _userManager.FindByNameAsync(model.UserId);

                        string toMail = "http://localhost:57886/api/AppUser/VerifyEmail/" + user.Id;

                        MailMessage mail = new MailMessage();
                        mail.To.Add(user.Email);
                        mail.From = new MailAddress("web2020tim1718@gmail.com");
                        mail.Subject = "Projekat";
                        mail.Body = "<h1>Please verify your account by clicking on the link below<h1><br>";
                        mail.Body += toMail;

                        using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                        {
                            smtp.Credentials = new System.Net.NetworkCredential("web2020tim1718@gmail.com", "NinaStanka987");
                            smtp.EnableSsl = true;
                            smtp.Send(mail);
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Count > 0)
            {
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserID",user.Id.ToString()),
                        new Claim("Roles", roles[0])
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(60),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Ok(new { token });
            }
            else
                return BadRequest(new { message = "User does not have a role." });
        }

        private const string GoogleApiTokenInfoUrl = "https://www.googleapis.com/oauth2/v3/tokeninfo?id_token={0}";

        public bool VerifyToken(string providerToken)
        {
            var httpClient = new HttpClient();
            var requestUri = new Uri(string.Format(GoogleApiTokenInfoUrl, providerToken));

            HttpResponseMessage httpResponseMessage;

            try
            {
                httpResponseMessage = httpClient.GetAsync(requestUri).Result;
            }
            catch (Exception ex)
            {
                return false;
            }

            if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }

            var response = httpResponseMessage.Content.ReadAsStringAsync().Result;
            var googleApiTokenInfo = JsonConvert.DeserializeObject<GoogleApiTokenInfo>(response);

            return true;
        }

        [HttpPost]
        [Route("SocialLogInFb")]
        public async Task<IActionResult> SocialLogInFb(SocialLogInModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserId);
            if (user == null)
            {
                // pravimo novog korisnika
                var applicationUser = new User()
                {
                    UserName = model.UserId,
                    Email = model.EmailAddress,
                    FullName = model.FirstName + " " + model.LastName,
                    Birthday = DateTime.Now,
                    Address = "",
                    PhoneNumber = "",
                    Passport = "",
                    SocialUser = true,
                };

                try
                {
                    IdentityResult result = await _userManager.CreateAsync(applicationUser);

                    if (result.Succeeded)
                    {
                        _userManager.AddToRoleAsync(applicationUser, "RegisteredUser").Wait();
                        user = await _userManager.FindByNameAsync(model.UserId);

                        string toMail = "http://localhost:57886/api/AppUser/VerifyEmail/" + user.Id;

                        MailMessage mail = new MailMessage();
                        mail.To.Add(user.Email);
                        mail.From = new MailAddress("web2020tim1718@gmail.com");
                        mail.Subject = "Projekat";
                        mail.Body = "<h1>Please verify your account by clicking on the link below<h1><br>";
                        mail.Body += toMail;

                        using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                        {
                            smtp.Credentials = new System.Net.NetworkCredential("web2020tim1718@gmail.com", "NinaStanka987");
                            smtp.EnableSsl = true;
                            smtp.Send(mail);
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Count > 0)
            {
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserID",user.Id.ToString()),
                        new Claim("Roles", roles[0])
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(60),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Ok(new { token });
            }
            else
                return BadRequest(new { message = "User does not have a role." });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LogInModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                if (user.EmailConfirmed)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Count > 0)
                    {
                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(new Claim[]
                            {
                                new Claim("UserID",user.Id.ToString()),
                                new Claim("Roles", roles[0]),
                                new Claim("Points", user.Points.ToString())
                            }),
                            Expires = DateTime.UtcNow.AddMinutes(60),
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)
                                ),SecurityAlgorithms.HmacSha256Signature
                                )
                        };
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                        var token = tokenHandler.WriteToken(securityToken);
                        return Ok(new { token });
                    }
                    else
                        return BadRequest(new { message = "User does not have a role." });
                }
                else
                    return BadRequest(new { message = "Please verify your e-mail first." });
            }
            else
                return BadRequest(new { message = "Username or password is incorrect." });
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterModel rm)
        {
            var user = await _userManager.FindByNameAsync(rm.username);
            if (user != null)
                return BadRequest(new { message = "Username already exists!." });
            var applicationUser = new User()
            {
                UserName = rm.username,
                Email = rm.email,
                FullName = rm.fullName,
                Birthday = DateTime.Parse(rm.birthday),
                Address = rm.address,
                PhoneNumber = rm.phone,
                Passport = rm.passport,
                SocialUser = false,
            };

            try
            {
                IdentityResult result = await _userManager.CreateAsync(applicationUser, rm.password);

                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(applicationUser, "RegisteredUser").Wait();

                    string toMail = "http://localhost:57886/api/AppUser/VerifyEmail/" + applicationUser.Id;

                    MailMessage mail = new MailMessage();
                    mail.To.Add(applicationUser.Email);
                    mail.From = new MailAddress("web2020tim1718@gmail.com");
                    mail.Subject = "Projekat";
                    mail.Body = "<h1>Please verify your account by clicking on the link below<h1><br>";
                    mail.Body += toMail;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new System.Net.NetworkCredential("web2020tim1718@gmail.com", "NinaStanka987");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }

                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpGet]
        [Route("VerifyEmail/{id}")]
        public async Task VerifyEmail(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            try
            {
                VerifyService service = new VerifyService(Configuration);
                service.Verify(user);
            }
            catch (Exception e)
            {

            }

        }


        [HttpPost]
        [Route("SearchUserByPassport")]
        public UserModel SearchUserByPassport(SearchUserModel sm)
        {

            List<User> users = _context2.ApplicationUsers.ToList();

            foreach (var u in users)
            {
                if (u.Passport == sm.passport)
                {
                    return new UserModel() { Id = u.Id, Username = u.UserName, Passport = u.Passport, FullName = u.FullName, Email = u.Email };
                }
            }


            return null;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("LoadPeople")]
        public async Task<Object> LoadPeople()
        {
            List<UserModel> ret = new List<UserModel>();
            List<User> users = _context2.ApplicationUsers.
                Include(comp => comp.FriendRequests).

                Include(comp => comp.SentRequests)
                .Include(comp => comp.Friends)

                .Include(comp => comp.Notifications).ToList();


            foreach (var usermodel in users)
            {
                bool uZahtjevima = false;
                bool uZahtjevima2 = false;
                bool reg = false;
                UserModel um = new UserModel();
                string userid = User.Claims.First(c => c.Type == "UserID").Value;
                var user = await _userManager.FindByIdAsync(userid);
                List<FriendRequestSent> frs = new List<FriendRequestSent>();
                if (user.SentRequests == null)
                    user.SentRequests = new List<FriendRequestSent>();
                foreach (var fr in user.SentRequests)
                {
                    if (fr.User == usermodel.Id)
                    {
                        uZahtjevima = true;
                    }
                }

                if (user.FriendRequests == null)
                    user.FriendRequests = new List<FriendRequestReceived>();
                foreach (var fr in user.FriendRequests)
                {
                    if (fr.User == usermodel.Id)
                    {
                        uZahtjevima2 = true;
                    }
                }
                var userIdentity = await _userManager.IsInRoleAsync(usermodel, "RegisteredUser");
                if (!userIdentity)
                {
                    reg = true;
                }
                if (user.Friends == null)
                    user.Friends = new List<User>();
                if (userid != usermodel.Id && !reg && !user.Friends.Contains(usermodel) && !uZahtjevima && !uZahtjevima2)
                {
                    um.FullName = usermodel.FullName;
                    um.Address = usermodel.Address;
                    um.Birthday = usermodel.Birthday.ToString();
                    um.Passport = usermodel.Passport;


                    um.Id = usermodel.Id;
                    um.Username = usermodel.UserName;
                    um.Points = usermodel.Points;
                    um.Email = usermodel.Email;
                    um.CompanyId = usermodel.CompanyId;
                    ret.Add(um);
                }
            }
            return ret;
        }

        [HttpPost]
        [Route("SendRequest")]
        public async Task<Object> SendRequest(SendRequestModel model)
        {
            List<UserModel> ret = new List<UserModel>();
            List<User> users = _context2.ApplicationUsers.
                   Include(comp => comp.FriendRequests).
                   Include(comp => comp.SentRequests)
                   .Include(comp => comp.Friends)
                   .Include(comp => comp.Notifications).ToList();

            User reciver = await _userManager.FindByIdAsync(model.idTo);
            User sender = await _userManager.FindByIdAsync(model.idFrom);

            if (sender.SentRequests == null)
                sender.SentRequests = new List<FriendRequestSent>();
            sender.SentRequests.Add(new FriendRequestSent()
            {
                User = reciver.Id

            });

            if (reciver.FriendRequests == null)
                reciver.FriendRequests = new List<FriendRequestReceived>();
            reciver.FriendRequests.Add(new FriendRequestReceived()
            {
                User = sender.Id

            });
            _context2.Update(reciver);
            _context2.Update(sender);
            _context2.SaveChanges();

            foreach (var usermodel in users)
            {

                UserModel um = new UserModel();


                if (model.idFrom != usermodel.Id && model.idTo != usermodel.Id)
                {
                    um.FullName = usermodel.FullName;
                    um.Address = usermodel.Address;
                    um.Birthday = usermodel.Birthday.ToString();
                    um.Passport = usermodel.Passport;

                    um.Id = usermodel.Id;
                    um.Username = usermodel.UserName;
                    um.Points = usermodel.Points;
                    um.Email = usermodel.Email;
                    um.CompanyId = usermodel.CompanyId;

                    um.Friends = new List<UserModel>();
                    um.SentRequests = new List<FriendRequestSentModel>();
                    um.Notifications = new List<Notification>();
                    um.FriendRequests = new List<FriendRequestReceivedModel>();

                    if (usermodel.Friends == null)
                        usermodel.Friends = new List<User>();
                    foreach (var fr in usermodel.Friends)
                    {
                        um.Friends.Add(new UserModel()
                        {
                            Address = fr.Address,
                            Email = fr.Email,
                            Birthday = fr.Birthday.ToString(),
                            FullName = fr.FullName,
                            Username = fr.UserName,
                            Id = fr.Id,
                            Passport = fr.Passport
                        });
                    }
                    if (usermodel.SentRequests == null)
                        usermodel.SentRequests = new List<FriendRequestSent>();
                    foreach (var fr in usermodel.SentRequests)
                    {
                        User pom = await _userManager.FindByIdAsync(fr.User);
                        um.SentRequests.Add(new FriendRequestSentModel()
                        {
                            Id = fr.Id,
                            User = new UserModel()
                            {
                                Address = pom.Address,
                                Email = pom.Email,
                                Birthday = pom.Birthday.ToString(),
                                FullName = pom.FullName,
                                Username = pom.UserName,
                                Id = pom.Id,
                                Passport = pom.Passport
                            }
                        });
                    }
                    if (usermodel.FriendRequests == null)
                        usermodel.FriendRequests = new List<FriendRequestReceived>();
                    foreach (var fr in usermodel.FriendRequests)
                    {
                        User pom = await _userManager.FindByIdAsync(fr.User);
                        um.FriendRequests.Add(new FriendRequestReceivedModel()
                        {
                            Id = fr.Id,
                            User = new UserModel()
                            {
                                Address = pom.Address,
                                Email = pom.Email,
                                Birthday = pom.Birthday.ToString(),
                                FullName = pom.FullName,
                                Username = pom.UserName,
                                Id = pom.Id,
                                Passport = pom.Passport
                            }
                        });


                        ret.Add(um);
                    }
                }
            }
            return ret;
        }

        [HttpPost]
        [Route("AcceptRequest")]
        public async Task<Object> AcceptRequest(SendRequestModel model)
        {

            List<User> users = _context2.ApplicationUsers.
                Include(comp => comp.FriendRequests).
                Include(comp => comp.SentRequests)
                .Include(comp => comp.Friends)
                .Include(comp => comp.Notifications).ToList();

            User reciver = await _userManager.FindByIdAsync(model.idTo);
            User sender = await _userManager.FindByIdAsync(model.idFrom);

            if (sender.Friends == null)
                sender.Friends = new List<User>();
            if (sender.FriendRequests == null)
                sender.FriendRequests = new List<FriendRequestReceived>();
            if (reciver.Friends == null)
                reciver.Friends = new List<User>();
            if (reciver.SentRequests == null)
                reciver.SentRequests = new List<FriendRequestSent>();
            if (reciver.Notifications == null)
                reciver.Notifications = new List<Notification>();
            for (int i = 0; i < sender.FriendRequests.Count(); i++)
            {
                if (sender.FriendRequests[i].User == reciver.Id)
                {
                    sender.Friends.Add(reciver);
                    sender.FriendRequests.Remove(sender.FriendRequests[i]);
                }
            }

            for (int i = 0; i < reciver.SentRequests.Count(); i++)
            {
                if (reciver.SentRequests[i].User == sender.Id)
                {
                    reciver.Friends.Add(sender);
                    reciver.Notifications.Add(new Notification() { Text = "User " + sender.FullName + " accepted your friend request!" });
                    reciver.SentRequests.Remove(reciver.SentRequests[i]);
                }
            }
            _context2.Update(reciver);
            _context2.Update(sender);
            _context2.SaveChanges();

            UserModel um = new UserModel();
            string userid = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userid);
            um.FullName = user.FullName;
            um.Address = user.Address;
            um.Birthday = user.Birthday.ToString();
            um.Passport = user.Passport;

            um.Id = user.Id;
            um.Username = user.UserName;
            um.Points = user.Points;
            um.Email = user.Email;
            um.CompanyId = user.CompanyId;

            um.Friends = new List<UserModel>();
            um.SentRequests = new List<FriendRequestSentModel>();
            um.Notifications = new List<Notification>();
            um.FriendRequests = new List<FriendRequestReceivedModel>();


            if (user.Friends == null)
                user.Friends = new List<User>();
            foreach (var fr in user.Friends)
            {
                um.Friends.Add(new UserModel()
                {
                    Address = fr.Address,
                    Email = fr.Email,
                    Birthday = fr.Birthday.ToString(),
                    FullName = fr.FullName,
                    Username = fr.UserName,
                    Id = fr.Id,
                    Passport = fr.Passport
                });
            }
            if (user.SentRequests == null)
                user.SentRequests = new List<FriendRequestSent>();
            foreach (var fr in user.SentRequests)
            {
                User pom = await _userManager.FindByIdAsync(fr.User);
                um.SentRequests.Add(new FriendRequestSentModel()
                {
                    Id = fr.Id,
                    User = new UserModel()
                    {
                        Address = pom.Address,
                        Email = pom.Email,
                        Birthday = pom.Birthday.ToString(),
                        FullName = pom.FullName,
                        Username = pom.UserName,
                        Id = pom.Id,
                        Passport = pom.Passport
                    }
                });
            }
            if (user.FriendRequests == null)
                user.FriendRequests = new List<FriendRequestReceived>();
            foreach (var fr in user.FriendRequests)
            {
                User pom = await _userManager.FindByIdAsync(fr.User);
                um.FriendRequests.Add(new FriendRequestReceivedModel()
                {
                    Id = fr.Id,
                    User = new UserModel()
                    {
                        Address = pom.Address,
                        Email = pom.Email,
                        Birthday = pom.Birthday.ToString(),
                        FullName = pom.FullName,
                        Username = pom.UserName,
                        Id = pom.Id,
                        Passport = pom.Passport
                    }
                });
            }
            if (user.Notifications == null)
                user.Notifications = new List<Notification>();
            foreach (var fr in user.Notifications)
            {
                um.Notifications.Add(new Notification() { Id = fr.Id, Text = fr.Text });
            }

            return um;
        }

        [HttpPost]
        [Route("CancelRequest")]
        public async Task<Object> CancelRequest(SendRequestModel model)
        {

            List<User> users = _context2.ApplicationUsers.
                Include(comp => comp.FriendRequests).

                Include(comp => comp.SentRequests)
                .Include(comp => comp.Friends)
                .Include(comp => comp.Notifications).ToList();

            User reciver = await _userManager.FindByIdAsync(model.idTo);
            User sender = await _userManager.FindByIdAsync(model.idFrom);

            if (sender.Friends == null)
                sender.Friends = new List<User>();
            if (sender.FriendRequests == null)
                sender.FriendRequests = new List<FriendRequestReceived>();
            if (reciver.Friends == null)
                reciver.Friends = new List<User>();
            if (reciver.SentRequests == null)
                reciver.SentRequests = new List<FriendRequestSent>();
            if (reciver.Notifications == null)
                reciver.Notifications = new List<Notification>();
            for (int i = 0; i < sender.FriendRequests.Count(); i++)
            {
                if (sender.FriendRequests[i].User == reciver.Id)
                {
                    sender.FriendRequests.Remove(sender.FriendRequests[i]);
                }
            }

            for (int i = 0; i < reciver.SentRequests.Count(); i++)
            {
                if (reciver.SentRequests[i].User == sender.Id)
                {
                    reciver.Notifications.Add(new Notification() { Text = "User " + sender.FullName + " canceled your friend request!" });
                    reciver.SentRequests.Remove(reciver.SentRequests[i]);
                }
            }
            _context2.Update(reciver);
            _context2.Update(sender);
            _context2.SaveChanges();

            UserModel um = new UserModel();
            string userid = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userid);
            um.FullName = user.FullName;
            um.Address = user.Address;
            um.Birthday = user.Birthday.ToString();
            um.Passport = user.Passport;


            um.Id = user.Id;
            um.Username = user.UserName;
            um.Points = user.Points;
            um.Email = user.Email;
            um.CompanyId = user.CompanyId;

            ;
            um.Friends = new List<UserModel>();
            um.SentRequests = new List<FriendRequestSentModel>();
            um.Notifications = new List<Notification>();
            um.FriendRequests = new List<FriendRequestReceivedModel>();
            if (user.Friends == null)
                user.Friends = new List<User>();
            foreach (var fr in user.Friends)
            {
                um.Friends.Add(new UserModel()
                {
                    Address = fr.Address,
                    Email = fr.Email,
                    Birthday = fr.Birthday.ToString(),
                    FullName = fr.FullName,
                    Username = fr.UserName,
                    Id = fr.Id,
                    Passport = fr.Passport
                });
            }
            if (user.SentRequests == null)
                user.SentRequests = new List<FriendRequestSent>();
            foreach (var fr in user.SentRequests)
            {
                User pom = await _userManager.FindByIdAsync(fr.User);
                um.SentRequests.Add(new FriendRequestSentModel()
                {
                    Id = fr.Id,
                    User = new UserModel()
                    {
                        Address = pom.Address,
                        Email = pom.Email,
                        Birthday = pom.Birthday.ToString(),
                        FullName = pom.FullName,
                        Username = pom.UserName,
                        Id = pom.Id,
                        Passport = pom.Passport
                    }
                });
            }
            if (user.FriendRequests == null)
                user.FriendRequests = new List<FriendRequestReceived>();
            foreach (var fr in user.FriendRequests)
            {
                User pom = await _userManager.FindByIdAsync(fr.User);
                um.FriendRequests.Add(new FriendRequestReceivedModel()
                {
                    Id = fr.Id,
                    User = new UserModel()
                    {
                        Address = pom.Address,
                        Email = pom.Email,
                        Birthday = pom.Birthday.ToString(),
                        FullName = pom.FullName,
                        Username = pom.UserName,
                        Id = pom.Id,
                        Passport = pom.Passport
                    }
                });
            }
            if (user.Notifications == null)
                user.Notifications = new List<Notification>();
            foreach (var fr in user.Notifications)
            {
                um.Notifications.Add(new Notification() { Id = fr.Id, Text = fr.Text });
            }

            return um;
        }

        [HttpPost]
        [Route("RemoveFriend")]
        public async Task<Object> RemoveFriend(SendRequestModel model)
        {

            List<User> users = _context2.ApplicationUsers.
                Include(comp => comp.FriendRequests).
                Include(comp => comp.SentRequests)
                .Include(comp => comp.Friends)
                .Include(comp => comp.Notifications).ToList();

            User reciver = await _userManager.FindByIdAsync(model.idTo);
            User sender = await _userManager.FindByIdAsync(model.idFrom);

            if (sender.Friends == null)
                sender.Friends = new List<User>();
            if (sender.FriendRequests == null)
                sender.FriendRequests = new List<FriendRequestReceived>();
            if (reciver.Friends == null)
                reciver.Friends = new List<User>();
            if (reciver.SentRequests == null)
                reciver.SentRequests = new List<FriendRequestSent>();
            if (reciver.Notifications == null)
                reciver.Notifications = new List<Notification>();
            for (int i = 0; i < sender.Friends.Count(); i++)
            {
                if (sender.Friends[i].Id == reciver.Id)
                {
                    sender.Friends.Remove(sender.Friends[i]);
                }
            }

            for (int i = 0; i < reciver.Friends.Count(); i++)
            {
                if (reciver.Friends[i].Id == sender.Id)
                {
                    reciver.Friends.Remove(reciver.Friends[i]);
                    reciver.Notifications.Add(new Notification() { Text = "User " + sender.FullName + " removed you from friends!" });
                }
            }
            _context2.Update(reciver);
            _context2.Update(sender);
            _context2.SaveChanges();

            UserModel um = new UserModel();
            string userid = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userid);
            um.FullName = user.FullName;
            um.Address = user.Address;
            um.Birthday = user.Birthday.ToString();
            um.Passport = user.Passport;


            um.Id = user.Id;
            um.Username = user.UserName;
            um.Points = user.Points;
            um.Email = user.Email;
            um.CompanyId = user.CompanyId;


            um.Friends = new List<UserModel>();
            um.SentRequests = new List<FriendRequestSentModel>();
            um.Notifications = new List<Notification>();
            um.FriendRequests = new List<FriendRequestReceivedModel>();


            if (user.Friends == null)
                user.Friends = new List<User>();
            foreach (var fr in user.Friends)
            {
                um.Friends.Add(new UserModel()
                {
                    Address = fr.Address,
                    Email = fr.Email,
                    Birthday = fr.Birthday.ToString(),
                    FullName = fr.FullName,
                    Username = fr.UserName,
                    Id = fr.Id,
                    Passport = fr.Passport
                });
            }
            if (user.SentRequests == null)
                user.SentRequests = new List<FriendRequestSent>();
            foreach (var fr in user.SentRequests)
            {
                User pom = await _userManager.FindByIdAsync(fr.User);
                um.SentRequests.Add(new FriendRequestSentModel()
                {
                    Id = fr.Id,
                    User = new UserModel()
                    {
                        Address = pom.Address,
                        Email = pom.Email,
                        Birthday = pom.Birthday.ToString(),
                        FullName = pom.FullName,
                        Username = pom.UserName,
                        Id = pom.Id,
                        Passport = pom.Passport
                    }
                });
            }
            if (user.FriendRequests == null)
                user.FriendRequests = new List<FriendRequestReceived>();
            foreach (var fr in user.FriendRequests)
            {
                User pom = await _userManager.FindByIdAsync(fr.User);
                um.FriendRequests.Add(new FriendRequestReceivedModel()
                {
                    Id = fr.Id,
                    User = new UserModel()
                    {
                        Address = pom.Address,
                        Email = pom.Email,
                        Birthday = pom.Birthday.ToString(),
                        FullName = pom.FullName,
                        Username = pom.UserName,
                        Id = pom.Id,
                        Passport = pom.Passport
                    }
                });
            }
            if (user.Notifications == null)
                user.Notifications = new List<Notification>();
            foreach (var fr in user.Notifications)
            {
                um.Notifications.Add(new Notification() { Id = fr.Id, Text = fr.Text });
            }
            return um;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("ChangeAdminAccountDetails")]
        public async Task<object> ChangeAdminAccountDetails(AdminDetailsModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (await _userManager.CheckPasswordAsync(user, model.password))
            {
                user.FullName = model.name;
                user.Address = model.addr;
                if (user.Email != model.email)
                {
                    user.EmailConfirmed = false;
                    string toMail = "http://localhost:57886/api/AppUser/VerifyEmail/" + user.Id;

                    MailMessage mail = new MailMessage();
                    mail.To.Add(model.email);
                    mail.From = new MailAddress("web2020tim1718@gmail.com");
                    mail.Subject = "Projekat";
                    mail.Body = "Please verify your e-mail address by clicking this link: ";
                    mail.Body += toMail;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new System.Net.NetworkCredential("web2020tim1718@gmail.com", "NinaStanka987");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
                user.Email = model.email;
                user.Birthday = DateTime.Parse(model.bd);
                await _userManager.UpdateAsync(user);
                _context2.Update(user);
                _context2.SaveChanges();

                user = await _userManager.FindByIdAsync(userId);

                CarRentalAdminModel admin = new CarRentalAdminModel();
                admin.address = user.Address;
                admin.birthday = user.Birthday.ToShortDateString();
                admin.companyId = user.CompanyId;
                admin.email = user.Email;
                admin.fullName = user.FullName;
                admin.username = user.UserName;
                admin.verifiedEmail = true;
                admin.changedPassword = user.ChangedPassword;

                return admin;
            }


            return BadRequest(new { message = "Wrong password." });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("ChangeAdminUsername")]
        public async Task<object> ChangeAdminUsername(AdminUsernameModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            var user = await _userManager.FindByIdAsync(userId);


            if (await _userManager.CheckPasswordAsync(user, model.password))
            {
                var users = _context2.Users.ToList();
                foreach (var us in users)
                {
                    if (us.UserName == model.username)
                    {
                        return BadRequest(new { message = "Username already exists." });
                    }
                }
                user.UserName = model.username;
                await _userManager.UpdateAsync(user);
                _context2.Update(user);
                _context2.SaveChanges();

                user = await _userManager.FindByIdAsync(userId);

                CarRentalAdminModel admin = new CarRentalAdminModel();
                admin.address = user.Address;
                admin.birthday = user.Birthday.ToShortDateString();
                admin.companyId = user.CompanyId;
                admin.email = user.Email;
                admin.fullName = user.FullName;
                admin.username = user.UserName;
                admin.verifiedEmail = true;
                admin.changedPassword = user.ChangedPassword;

                return admin;
            }


            return BadRequest(new { message = "Wrong password." });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("ChangeAdminPassword")]
        public async Task<object> ChangeAdminPassword(AdminPasswordModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            var user = await _userManager.FindByIdAsync(userId);


            if (await _userManager.CheckPasswordAsync(user, model.password))
            {
                var res = await _userManager.ChangePasswordAsync(user, model.password, model.newPassword);
                user.ChangedPassword = true;
                await _userManager.UpdateAsync(user);
                _context2.Update(user);
                _context2.SaveChanges();

                user = await _userManager.FindByIdAsync(userId);

                CarRentalAdminModel admin = new CarRentalAdminModel();
                admin.address = user.Address;
                admin.birthday = user.Birthday.ToShortDateString();
                admin.companyId = user.CompanyId;
                admin.email = user.Email;
                admin.fullName = user.FullName;
                admin.username = user.UserName;
                admin.verifiedEmail = true;
                admin.changedPassword = user.ChangedPassword;

                return admin;
            }


            return BadRequest(new { message = "Wrong password." });
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("LoadWebsiteAdmin")]
        public async Task<object> LoadWebsiteAdmin()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "WebsiteAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            var user = await _userManager.FindByIdAsync(userId);
            var allUsers = _context2.Users.Include(us => us.Discount).ToList();



            WebsiteAdminModel admin = new WebsiteAdminModel();
            admin.address = user.Address;
            admin.birthday = user.Birthday.ToShortDateString();
            admin.email = user.Email;
            admin.fullName = user.FullName;
            admin.username = user.UserName;
            admin.verifiedEmail = true;
            admin.changedPassword = user.ChangedPassword;
            admin.mainAdmin = user.MainWebsiteAdministrator;
            admin.websiteAdministrators = new List<OtherAdmin>();
            foreach (var u in allUsers)
            {
                if (u.Id == userId)
                {
                    admin.discount = new DiscountModel() { bronzeTier = u.Discount.BronzeTier, discountPercent = u.Discount.DiscountPercent, goldTier = u.Discount.GoldTier, silverTier = u.Discount.SilverTier };
                }
            }

            var users = await _userManager.GetUsersInRoleAsync("WebsiteAdministrator");

            foreach (var u in users)
            {
                if (!u.MainWebsiteAdministrator)
                {
                    admin.websiteAdministrators.Add(new OtherAdmin() { email = u.Email, fullname = u.FullName, username = u.UserName });
                }
            }

            return admin;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("GetAdminsForCompanies")]
        public async Task<object> GetAdminsForCompanies(Companies comps)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "WebsiteAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            var users = await _userManager.GetUsersInRoleAsync("RentACarAdministrator");
            List<string> ret = new List<string>();
            foreach (var c in comps.companies)
            {
                foreach (var u in users)
                {
                    if (u.CompanyId == c.id)
                    {
                        c.admins.Add(u.UserName);
                    }
                }
            }

            return comps.companies;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("GetAdminsForAirCompanies")]
        public async Task<object> GetAdminsForAirCompanies(CompaniesAir comps)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "WebsiteAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            var users = await _userManager.GetUsersInRoleAsync("AirlineAdministrator");
            List<string> ret = new List<string>();
            foreach (var c in comps.companies)
            {
                foreach (var u in users)
                {
                    if (u.CompanyId == c.id)
                    {
                        c.admins.Add(u.UserName);
                    }
                }
            }

            return comps.companies;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("ChangeWAdminAccountDetails")]
        public async Task<object> ChangeWAdminAccountDetails(AdminDetailsModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "WebsiteAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (await _userManager.CheckPasswordAsync(user, model.password))
            {
                user.FullName = model.name;
                user.Address = model.addr;
                if (user.Email != model.email)
                {
                    user.EmailConfirmed = false;
                    string toMail = "http://localhost:57886/api/AppUser/VerifyEmail/" + user.Id;

                    MailMessage mail = new MailMessage();
                    mail.To.Add(model.email);
                    mail.From = new MailAddress("web2020tim1718@gmail.com");
                    mail.Subject = "Projekat";
                    mail.Body = "Please verify your e-mail address by clicking this link: ";
                    mail.Body += toMail;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new System.Net.NetworkCredential("web2020tim1718@gmail.com", "NinaStanka987");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }

                user.Email = model.email;
                user.Birthday = DateTime.Parse(model.bd);
                await _userManager.UpdateAsync(user);
                _context2.Update(user);
                _context2.SaveChanges();

                user = await _userManager.FindByIdAsync(userId);

                WebsiteAdminModel admin = new WebsiteAdminModel();
                admin.address = user.Address;
                admin.birthday = user.Birthday.ToShortDateString();
                admin.email = user.Email;
                admin.fullName = user.FullName;
                admin.username = user.UserName;
                admin.verifiedEmail = true;
                admin.changedPassword = user.ChangedPassword;
                admin.mainAdmin = user.MainWebsiteAdministrator;

                return admin;
            }


            return BadRequest(new { message = "Wrong password." });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("ChangeWAdminUsername")]
        public async Task<object> ChangeWAdminUsername(AdminUsernameModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "WebsiteAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            var user = await _userManager.FindByIdAsync(userId);


            if (await _userManager.CheckPasswordAsync(user, model.password))
            {
                var users = _context2.Users.ToList();
                foreach (var us in users)
                {
                    if (us.UserName == model.username)
                    {
                        return BadRequest(new { message = "Username already exists." });
                    }
                }
                user.UserName = model.username;
                await _userManager.UpdateAsync(user);
                _context2.Update(user);
                _context2.SaveChanges();

                user = await _userManager.FindByIdAsync(userId);

                WebsiteAdminModel admin = new WebsiteAdminModel();
                admin.address = user.Address;
                admin.birthday = user.Birthday.ToShortDateString();
                admin.email = user.Email;
                admin.fullName = user.FullName;
                admin.username = user.UserName;
                admin.verifiedEmail = true;
                admin.changedPassword = user.ChangedPassword;
                admin.mainAdmin = user.MainWebsiteAdministrator;

                return admin;
            }


            return BadRequest(new { message = "Wrong password." });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("ChangeWAdminPassword")]
        public async Task<object> ChangeWAdminPassword(AdminPasswordModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "WebsiteAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            var user = await _userManager.FindByIdAsync(userId);

            if (await _userManager.CheckPasswordAsync(user, model.password))
            {
                var res = await _userManager.ChangePasswordAsync(user, model.password, model.newPassword);
                user.ChangedPassword = true;
                await _userManager.UpdateAsync(user);
                _context2.Update(user);
                _context2.SaveChanges();

                user = await _userManager.FindByIdAsync(userId);

                WebsiteAdminModel admin = new WebsiteAdminModel();
                admin.address = user.Address;
                admin.birthday = user.Birthday.ToShortDateString();
                admin.email = user.Email;
                admin.fullName = user.FullName;
                admin.username = user.UserName;
                admin.verifiedEmail = true;
                admin.changedPassword = user.ChangedPassword;
                admin.mainAdmin = user.MainWebsiteAdministrator;
                return admin;
            }
            return BadRequest(new { message = "Wrong password." });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("AddNewWebAdmin")]
        public async Task<object> AddNewWebAdmin(NewWebAdminModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "WebsiteAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user.MainWebsiteAdministrator)
            {
                if (_userManager.FindByNameAsync(model.username).Result == null)
                {
                    User newAdmin = new User();
                    newAdmin.UserName = model.username;
                    newAdmin.Email = model.email;
                    newAdmin.FullName = "";
                    newAdmin.Birthday = new DateTime(2000, 1, 1);
                    newAdmin.Address = "";
                    newAdmin.PhoneNumber = "";
                    newAdmin.ChangedPassword = false;
                    newAdmin.MainWebsiteAdministrator = false;

                    var users = _context2.Users.Include(us => us.Discount).ToList();

                    foreach (var u in users)
                    {
                        if (u.Id == userId)
                        {
                            newAdmin.Discount = u.Discount;
                        }
                    }

                    IdentityResult result = _userManager.CreateAsync(newAdmin, model.password).Result;

                    if (result.Succeeded)
                    {
                        try
                        {
                            _userManager.AddToRoleAsync(newAdmin, "WebsiteAdministrator").Wait();

                            string toMail = "http://localhost:57886/api/AppUser/VerifyEmail/" + newAdmin.Id;

                            MailMessage mail = new MailMessage();
                            mail.To.Add(newAdmin.Email);
                            mail.From = new MailAddress("web2020tim1718@gmail.com");
                            mail.Subject = "Projekat";
                            mail.Body = "Dear new Administrator, your username is " + newAdmin.UserName + " and your password is: " + model.password + ". To activate your account you must change your password. Please verify your e-mail address by clicking this link: ";
                            mail.Body += toMail;

                            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                            {
                                smtp.Credentials = new System.Net.NetworkCredential("web2020tim1718@gmail.com", "NinaStanka987");
                                smtp.EnableSsl = true;
                                smtp.Send(mail);
                            }


                            List<OtherAdmin> admins = new List<OtherAdmin>();

                            var users2 = await _userManager.GetUsersInRoleAsync("WebsiteAdministrator");

                            foreach (var u in users2)
                            {
                                if (!u.MainWebsiteAdministrator)
                                {
                                    admins.Add(new OtherAdmin() { email = u.Email, fullname = u.FullName, username = u.UserName });
                                }
                            }
                            return admins;
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
                else
                {
                    return BadRequest(new { message = "Username already exists." });
                }


                user = await _userManager.FindByIdAsync(userId);

                WebsiteAdminModel admin = new WebsiteAdminModel();
                admin.address = user.Address;
                admin.birthday = user.Birthday.ToShortDateString();
                admin.email = user.Email;
                admin.fullName = user.FullName;
                admin.username = user.UserName;
                admin.verifiedEmail = true;
                admin.changedPassword = user.ChangedPassword;
                admin.mainAdmin = user.MainWebsiteAdministrator;
                return admin;
            }
            else
                return BadRequest(new { message = "Not a main administrator." });

        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("AddAdminToCompany")]
        public async Task<object> AddAdminToCompany(AddAdminModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "WebsiteAdministrator")
            {
                return "Forbbiden action for this role.";
            }

            if (_userManager.FindByNameAsync(model.username).Result == null)
            {
                //var companies = _context2.RentACarCompanies.ToList();
                //bool exists = false;
                //foreach(var comp in companies)
                //{
                //    if(comp.ID == model.companyId)
                //    {
                //        exists = true;
                //    }
                //}

                //if(!exists)
                //{
                //    return "Company no longer exists.";
                //}

                User newAdmin = new User();
                newAdmin.UserName = model.username;
                newAdmin.Email = model.email;
                newAdmin.FullName = "";
                newAdmin.Birthday = new DateTime(2000, 1, 1);
                newAdmin.Address = "";
                newAdmin.PhoneNumber = "";
                newAdmin.ChangedPassword = false;
                newAdmin.CompanyId = model.companyId;

                IdentityResult result = _userManager.CreateAsync(newAdmin, model.password).Result;

                if (result.Succeeded)
                {
                    try
                    {
                        _userManager.AddToRoleAsync(newAdmin, "RentACarAdministrator").Wait();

                        string toMail = "http://localhost:57886/api/AppUser/VerifyEmail/" + newAdmin.Id;

                        MailMessage mail = new MailMessage();
                        mail.To.Add(newAdmin.Email);
                        mail.From = new MailAddress("web2020tim1718@gmail.com");
                        mail.Subject = "Projekat";
                        mail.Body = "Dear car rental administrator, your username is " + newAdmin.UserName + " and your password is: " + model.password + ". To activate your account you must change your password. Please verify your e-mail address by clicking this link: ";
                        mail.Body += toMail;

                        using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                        {
                            smtp.Credentials = new System.Net.NetworkCredential("web2020tim1718@gmail.com", "NinaStanka987");
                            smtp.EnableSsl = true;
                            smtp.Send(mail);
                        }

                        List<OtherAdmin> admins = new List<OtherAdmin>();

                        var users = await _userManager.GetUsersInRoleAsync("RentACarAdministrator");


                        //List<CarCompanyModel> carCompanies = new List<CarCompanyModel>();

                        //foreach (var comp in companies)
                        //{
                        //    CarCompanyModel com = new CarCompanyModel();
                        //    com.name = comp.Name;
                        //    com.image = comp.Image;
                        //    com.id = comp.ID;
                        //    List<string> carAdmins = new List<string>();
                        //    foreach (var u in users)
                        //    {
                        //        if (u.CompanyId == comp.ID)
                        //        {
                        //            carAdmins.Add(u.UserName);
                        //        }
                        //    }
                        //    com.admins = carAdmins;
                        //    carCompanies.Add(com);
                        //}
                        return Ok();
                    }
                    catch (Exception e)
                    {

                    }
                }
                else
                {
                    return BadRequest(new { message = "Error creating user" });
                }
            }
            else
            {
                return BadRequest(new { message = "Username already exists." });
            }
            return BadRequest(new { message = "Error." });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("AddAdminToAirCompany")]
        public async Task<object> AddAdminToAirCompany(AddAdminModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "WebsiteAdministrator")
            {
                return "Forbbiden action for this role.";
            }

            if (_userManager.FindByNameAsync(model.username).Result == null)
            {
                //var companies = _context2.AirlineCompanies.ToList();
                //bool exists = false;
                //foreach (var comp in companies)
                //{
                //    if (comp.Id == model.companyId)
                //    {
                //        exists = true;
                //    }
                //}

                //if (!exists)
                //{
                //    return "Company no longer exists.";
                //}

                User newAdmin = new User();
                newAdmin.UserName = model.username;
                newAdmin.Email = model.email;
                newAdmin.FullName = "";
                newAdmin.Birthday = new DateTime(2000, 1, 1);
                newAdmin.Address = "";
                newAdmin.PhoneNumber = "";
                newAdmin.ChangedPassword = false;
                newAdmin.CompanyId = model.companyId;

                IdentityResult result = _userManager.CreateAsync(newAdmin, model.password).Result;

                if (result.Succeeded)
                {
                    try
                    {
                        _userManager.AddToRoleAsync(newAdmin, "AirlineAdministrator").Wait();

                        string toMail = "http://localhost:57886/api/AppUser/VerifyEmail/" + newAdmin.Id;

                        MailMessage mail = new MailMessage();
                        mail.To.Add(newAdmin.Email);
                        mail.From = new MailAddress("web2020tim1718@gmail.com");
                        mail.Subject = "Projekat";
                        mail.Body = "Dear new airline administrator, your username is " + newAdmin.UserName + " and your password is: " + model.password + ". To activate your account you must change your password. Please verify your e-mail address by clicking this link: ";
                        mail.Body += toMail;

                        using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                        {
                            smtp.Credentials = new System.Net.NetworkCredential("web2020tim1718@gmail.com", "NinaStanka987");
                            smtp.EnableSsl = true;
                            smtp.Send(mail);
                        }

                        List<OtherAdmin> admins = new List<OtherAdmin>();

                        var users = await _userManager.GetUsersInRoleAsync("AirlineAdministrator");


                        //List<AirCompanyModel> airCompanies = new List<AirCompanyModel>();

                        //foreach (var comp in companies)
                        //{
                        //    AirCompanyModel com = new AirCompanyModel();
                        //    com.name = comp.Name;
                        //    com.image = comp.Img;
                        //    com.id = comp.Id;
                        //    List<string> airAdmins = new List<string>();
                        //    foreach (var u in users)
                        //    {
                        //        if (u.CompanyId == comp.Id)
                        //        {
                        //            airAdmins.Add(u.UserName);
                        //        }
                        //    }
                        //    com.admins = airAdmins;
                        //    airCompanies.Add(com);
                        //}
                        //return airCompanies;
                        return Ok();
                    }
                    catch (Exception e)
                    {

                    }
                }
                else
                {
                    return BadRequest(new { message = "Error creating user" });
                }
            }
            else
            {
                return BadRequest(new { message = "Username already exists." });
            }
            return BadRequest(new { message = "Error." });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("SaveDiscount")]
        public async Task<object> SaveDiscount(DiscountModel model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "WebsiteAdministrator")
            {
                return BadRequest(new { message = "Forbbiden action for this role." });
            }

            var users = _context2.Users.Include(u => u.Discount).ToList();

            var u = await _userManager.FindByIdAsync(userId);
            if (u == null)
                BadRequest(new { message = "User not found." });


            u.Discount.BronzeTier = model.bronzeTier;
            u.Discount.SilverTier = model.silverTier;
            u.Discount.GoldTier = model.goldTier;
            u.Discount.DiscountPercent = model.discountPercent;

            await _userManager.UpdateAsync(u);
            _context2.Update(u);

            try
            {
                await _context2.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return BadRequest(new { message = "Cannot update, please reload the page." });
            }
            return Ok();

        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("LoadCarRentalAdmin")]
        public async Task<object> LoadCarRentalAdmin()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RentACarAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            var user = await _userManager.FindByIdAsync(userId);

            CarRentalAdminModel admin = new CarRentalAdminModel();
            admin.address = user.Address;
            admin.birthday = user.Birthday.ToShortDateString();
            admin.companyId = user.CompanyId;
            admin.email = user.Email;
            admin.fullName = user.FullName;
            admin.username = user.UserName;
            admin.verifiedEmail = true;
            admin.changedPassword = user.ChangedPassword;

            return admin;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("LoadAirlineAdmin")]
        public async Task<object> LoadAirlineAdmin()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "AirlineAdministrator")
            {
                return "Forbbiden action for this role.";
            }
            var user = await _userManager.FindByIdAsync(userId);

            AirlineAdminModel admin = new AirlineAdminModel();
            admin.address = user.Address;
            admin.birthday = user.Birthday.ToShortDateString();
            admin.companyId = user.CompanyId;
            admin.email = user.Email;
            admin.fullName = user.FullName;
            admin.username = user.UserName;
            admin.verifiedEmail = true;
            admin.changedPassword = user.ChangedPassword;

            return admin;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("AddPointsToUser")]
        public async Task<object> AddPointsToUser()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RegisteredUser")
            {
                return "Forbbiden action for this role.";
            }
            var user = await _userManager.FindByIdAsync(userId);
            user.Points += 5;

            await _userManager.UpdateAsync(user);
            _context2.Update(user);
            _context2.SaveChanges();

            return Ok();
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("RemovePointsFromUser")]
        public async Task<object> RemovePointsFromUser()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RegisteredUser")
            {
                return "Forbbiden action for this role.";
            }
            var user = await _userManager.FindByIdAsync(userId);
            user.Points -= 5;

            await _userManager.UpdateAsync(user);
            _context2.Update(user);
            _context2.SaveChanges();

            return Ok();
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("GetDiscount")]
        public async Task<object> GetDiscount()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RegisteredUser")
            {
                return "Forbbiden action for this role.";
            }

            var allUsers = _context2.Users.Include(us => us.Discount).ToList();
            var users = await _userManager.GetUsersInRoleAsync("WebsiteAdministrator");

            Discount dis = new Discount();

            foreach (var us in allUsers)
            {
                if (us.Id == users[0].Id)
                {
                    dis = us.Discount;
                }
            }

            return dis;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("LoadUser")]
        public async Task<Object> LoadUser()
        {
            List<User> users = _context2.ApplicationUsers
                .Include(comp => comp.FriendRequests).
                Include(comp => comp.SentRequests)
                .Include(comp => comp.Friends)
                .Include(comp => comp.Notifications).ToList();

            UserModel um = new UserModel();
            string userid = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userid);
            um.FullName = user.FullName;
            um.Address = user.Address;
            um.Birthday = user.Birthday.ToString();
            um.Passport = user.Passport;
            um.changedPassword = user.ChangedPassword;

            um.socialUser = user.SocialUser;

            um.Id = user.Id;
            um.Username = user.UserName;
            um.Points = user.Points;
            um.Email = user.Email;
            um.CompanyId = user.CompanyId;

            um.Friends = new List<UserModel>();
            um.SentRequests = new List<FriendRequestSentModel>();
            um.Notifications = new List<Notification>();
            um.FriendRequests = new List<FriendRequestReceivedModel>();


            if (user.Friends == null)
                user.Friends = new List<User>();
            foreach (var fr in user.Friends)
            {
                um.Friends.Add(new UserModel()
                {
                    Address = fr.Address,
                    Email = fr.Email,
                    Birthday = fr.Birthday.ToString(),
                    FullName = fr.FullName,
                    Username = fr.UserName,
                    Id = fr.Id,
                    Passport = fr.Passport
                });
            }
            if (user.SentRequests == null)
                user.SentRequests = new List<FriendRequestSent>();
            foreach (var fr in user.SentRequests)
            {
                User pom = await _userManager.FindByIdAsync(fr.User);
                um.SentRequests.Add(new FriendRequestSentModel()
                {
                    Id = fr.Id,
                    User = new UserModel()
                    {
                        Address = pom.Address,
                        Email = pom.Email,
                        Birthday = pom.Birthday.ToString(),
                        FullName = pom.FullName,
                        Username = pom.UserName,
                        Id = pom.Id,
                        Passport = pom.Passport
                    }
                });
            }
            if (user.FriendRequests == null)
                user.FriendRequests = new List<FriendRequestReceived>();
            foreach (var fr in user.FriendRequests)
            {
                User pom = await _userManager.FindByIdAsync(fr.User);
                um.FriendRequests.Add(new FriendRequestReceivedModel()
                {
                    Id = fr.Id,
                    User = new UserModel()
                    {
                        Address = pom.Address,
                        Email = pom.Email,
                        Birthday = pom.Birthday.ToString(),
                        FullName = pom.FullName,
                        Username = pom.UserName,
                        Id = pom.Id,
                        Passport = pom.Passport
                    }
                });
            }
            if (user.Notifications == null)
                user.Notifications = new List<Notification>();
            foreach (var fr in user.Notifications)
            {
                um.Notifications.Add(new Notification() { Id = fr.Id, Text = fr.Text });
            }

            return um;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("LoadReservationsStatus")]
        public async Task<Object> LoadReservationsStatus()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            string userRole = User.Claims.First(c => c.Type == "Roles").Value;
            if (userRole != "RegisteredUser")
            {
                return "Forbbiden action for this role.";
            }

            var users = _context2.Users.Include(us => us.CombinedReservations).ToList();
            var user = await _userManager.FindByIdAsync(userId);

            if (user.CombinedReservations == null)
                return new List<Models.Model.CombinedReservation>();
            else
                return user.CombinedReservations;
        }

        [HttpPost]
        [Route("LoadUserById")]
        public async Task<Object> LoadUserById(SearchUserModel id)
        {
            List<User> users = _context2.ApplicationUsers.
                Include(comp => comp.FriendRequests).
                Include(comp => comp.SentRequests)
                .Include(comp => comp.Friends)
                .Include(comp => comp.Notifications).ToList();

            UserModel um = new UserModel();

            var user = await _userManager.FindByIdAsync(id.passport);


            um.FullName = user.FullName;
            um.Address = user.Address;
            um.Birthday = user.Birthday.ToString();
            um.Passport = user.Passport;
            um.changedPassword = user.ChangedPassword;

            um.Id = user.Id;
            um.Username = user.UserName;
            um.Points = user.Points;
            um.Email = user.Email;
            um.CompanyId = user.CompanyId;

            return um;
        }
    }
}