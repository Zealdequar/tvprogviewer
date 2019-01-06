using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TVProgViewer.BusinessLogic.Users;
using servRef = TVProgViewer.TVProgApp.TvProgServiceReference;

namespace TVProgViewer.TVProgApp.Controllers
{
    internal class UserController: BaseController
    {
        internal static int AddUser(string userName, string passHash, string passExtend, string lastName,
            string firstName, string middleName, DateTime birthDate, bool? gender, string email, string mobPhone,
            string otherPhone1, string otherPhone2, string address, string gmtZone)
        {
            return TvProgService.AddUser(userName, passHash, passExtend, lastName, firstName, middleName, birthDate, gender, email, mobPhone,
                otherPhone1, otherPhone2, address, gmtZone);
        }

        internal static SecureData GetHashes(string username)
        {
            return TvProgService.GetHashes(username);
        }

        internal static User GetUser(long uid, out int errCode)
        {
            return TvProgService.GetUser(uid, out errCode).Mapper<User>(new User());
        }

       
    }
}
