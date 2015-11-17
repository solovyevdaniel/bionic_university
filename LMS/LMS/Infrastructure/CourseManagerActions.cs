using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using LMS.Models;
using System.Data.Entity;
using System.Text;

namespace LMS.Infrastructure
{
    /*
     * Analogue of UserStates in DB
     */
    public enum CourseUsers : int
    {
        Learner = 1,
        Student,
        Teacher,
        Main
    }
    sealed public class CourseManagerActions
    {
        /*GROUP ACTIONS*/

        /*
         * Returns List of users groups with modified UserState_ID  == user
         * users - list of usernames which will be modified
         * if exception then returns current list of groups
         * DOESN'T CHECK CURRENT USERS ROLES!!!
         */
        public async Task<IList<Group>> ChangeCourseUserGroupAsync(IEnumerable<String> users, ApplicationDbContext appContext, Course course, CourseUsers user, Boolean addNever = true)
        {
            List<Group> groups = new List<Group>();
            try
            {
                if (users != null && course != null)
                {
                    Int32 userType = (Int32)user;
                    Group currGroup = null;
                    String currUser = null;
                    for (Int32 i = 0; i < users.Count(); i++)
                    {
                        currUser = users.ElementAt(i);
                        currGroup = await appContext.Groups
                            .Where(g => String.Compare(g.User.UserName, currUser, true) == 0 && g.Course_ID == course.ID)
                            .FirstAsync();
                        if (!addNever || currGroup.UserState_ID != userType)
                        {
                            currGroup.UserState_ID = userType;
                            groups.Add(currGroup);
                        }
                    }
                }
                return groups;
            }
            catch (InvalidOperationException)
            {
                return groups;
            }
            catch (NullReferenceException)
            {
                return groups;
            }
        }
        /*
         * returns users with current UserState(user) for mainGroup(group)
         * if checkName then checks if groupName == null (userHasn't any group) || comparesName - user is in current Group 
         */
        public async Task RefreshGroupOfUsersTypeAsync(IEnumerable<String> userNames, Group group, ApplicationDbContext appContext, CourseUsers userType, CourseUsers defaultType)
        {
            if (userNames == null)
            {
                Group[] groups = await appContext.Groups
                .Where(g => String.Compare(g.Name, group.Name, false) == 0 && g.UserState_ID == (Int32)userType).ToArrayAsync();
                foreach (var gr in groups)
                {
                    gr.UserState_ID = (Int32)defaultType;
                    gr.Name = null;
                    appContext.Entry(gr).State = EntityState.Modified;
                }
            }
            else
            {
                /*updating role for users*/
                Int32 currentRole = (Int32)userType;
                Group[] groups = await appContext.Users
                    .Where(u => userNames.Contains(u.UserName))
                    .Select(u => u.Groups.FirstOrDefault(g => g.Course_ID == group.Course_ID))
                    .ToArrayAsync();
                foreach (var i in groups)
                {
                    i.Name = group.Name;
                    i.UserState_ID = (Int32)userType;
                    appContext.Entry(i).State = EntityState.Modified;
                }

                ApplicationUser[] users = await appContext.Users
                    .Where(u => !userNames.Contains(u.UserName)).ToArrayAsync();
                Group selected = null;
                if (users != null)
                {
                    foreach (var user in users)
                    {
                        if (user.Groups.Any(g => String.Compare(g.Name, group.Name, false) == 0 && g.UserState_ID == currentRole))
                        {
                            selected = user.Groups.First(g => String.Compare(g.Name, group.Name, false) == 0 && g.UserState_ID == currentRole);
                            selected.Name = null;
                            selected.UserState_ID = (Int32)defaultType;
                            appContext.Entry(selected).State = EntityState.Modified;
                        }
                    }
                }
            }
        }
        public async Task<Boolean> WriteEmailsToUsersAsync(IEnumerable<String> userNames, ApplicationDbContext appContext, String message, String subj)
        {
            if (userNames != null)
            {
                List<String> userMails = new List<String>();
                foreach (var name in userNames)
                {
                    if (await appContext.Users.AnyAsync(u => String.Compare(u.UserName, name, false) == 0))
                        userMails.Add(appContext.Users.First(u => String.Compare(u.UserName, name, false) == 0).Email);
                }
                if (userMails.Count != 0)
                {
                    LMSEmailSender lmsEmailSender = new LMSEmailSender();
                    lmsEmailSender.WriteEmailsAsync(userMails, appContext, subj, message);
                    return true;
                }
            }
            return false;
        }
        public async Task<IList<ApplicationUser>> GetUsersFromMainGroupAsync(ApplicationDbContext appContext, Group group, CourseUsers user, Boolean checkName = false)
        {
            List<ApplicationUser> users = new List<ApplicationUser>();
            try
            {
                if (group != null)
                {
                    Int32 userState = (Int32)user;
                    /*adds users with groups where names == null which means that user is not in any main group*/
                    users.AddRange(await appContext.Users.Where
                        (
                            u => u.Groups
                            .Any(gr => gr.Course_ID == group.Course_ID && gr.UserState_ID == userState && (checkName ? String.Compare(group.Name, gr.Name, false) == 0 : gr.Name == null))
                        )
                        .ToListAsync());
                }
                return users;
            }
            catch (NullReferenceException)
            {
                return users;
            }
        }
        public async Task<IList<ApplicationUser>> FilterUsersByRole(IEnumerable<ApplicationUser> users, String requiredRole, ApplicationUserManager userManager)
        {
            List<ApplicationUser> filteredUsers = new List<ApplicationUser>();
            IList<String> userRoles = null;
            foreach (var user in users)
            {
                userRoles = await userManager.GetRolesAsync(user.Id);
                if (userRoles.Contains(requiredRole))
                    filteredUsers.Add(user);
            }
            return filteredUsers;
        }
        /*
         * Gets all users of current group(user) type
         * if exception then returns current list of users
         */
        public async Task<IList<ApplicationUser>> GetUsersFromGroupsAsync(ApplicationDbContext appContext, Course course, CourseUsers userType, Boolean checkGroupName = false)
        {
            List<ApplicationUser> users = new List<ApplicationUser>();
            try
            {
                if (course != null)
                {
                    Int32 userState = (Int32)userType;
                    /*adds users with groups where names == null which means that user is not in any main group*/
                    users.AddRange(await appContext.Users.Where
                        (
                            u => u.Groups
                            .Any(gr => gr.Course_ID == course.ID && gr.UserState_ID == userState && checkGroupName ? gr.Name == null : true)
                        )
                        .ToListAsync());
                }
                return users;
            }
            catch (NullReferenceException)
            {
                return users;
            }
        }
        /*
         * Gets ALL users of current group(user)
         * if exception then returns current list of groups
         */
        public async Task<IList<ApplicationUser>> GetAllUsersFromGroupsAsync(ApplicationDbContext appContext, Course course)
        {
            List<ApplicationUser> users = new List<ApplicationUser>();
            try
            {
                if (course != null)
                {
                    users.AddRange(await appContext.Users.Where
                        (
                            u => u.Groups
                            .Any(gr => gr.Course_ID == course.ID)
                        )
                        .ToListAsync());
                }
                return users;
            }
            catch (NullReferenceException)
            {
                return users;
            }
        }
        /*
         * Gets avaible users with userRole for group
         * 
         */
        public async Task<IList<ApplicationUser>> GetUsersWithDefinedRoleForGroupAsync(ApplicationDbContext appContext, ApplicationUserManager userManager, String userRole = "Teacher")
        {
            List<ApplicationUser> foundUsers = new List<ApplicationUser>();
            try
            {
                if (appContext != null && userManager != null)
                {
                    IList<ApplicationUser> users = await appContext.Users.ToListAsync();
                    /*get all teachers to List*/
                    IList<String> userRoles = null;
                    for (Int32 i = 0; i < users.Count; i++)
                    {
                        userRoles = await userManager.GetRolesAsync(users[i].Id);
                        if (userRoles.Contains(userRole))
                            foundUsers.Add(users[i]);
                    }
                }
                return foundUsers;
            }
            catch (InvalidOperationException)
            {
                return foundUsers;
            }
            catch (NullReferenceException)
            {
                return foundUsers;
            }
        }
        public async Task<IList<ApplicationUser>> GetUserWithDefRoleAndCurrCourse(ApplicationDbContext appContext, ApplicationUserManager userManager, Course course, String groupName, String userRole, params String[] groupRoles)
        {
            List<ApplicationUser> foundUsers = new List<ApplicationUser>();
            try
            {
                if (appContext != null && userManager != null)
                {
                    IList<ApplicationUser> users = await appContext.Users.ToListAsync();
                    /*get all teachers to List*/
                    IList<String> userRoles = null;
                    for (Int32 i = 0; i < users.Count; i++)
                    {
                        userRoles = await userManager.GetRolesAsync(users[i].Id);
                        if (userRoles.Contains(userRole))
                            foundUsers.Add(users[i]);
                    }
                }
                IEnumerable<ApplicationUser> usersWithRole = foundUsers.Where(u => CheckGroupUserForRole(u, course, groupName, groupRoles));
                if (usersWithRole != null)
                    return usersWithRole.ToArray();
                return new List<ApplicationUser>();
            }
            catch (InvalidOperationException)
            {
                return new List<ApplicationUser>();
            }
            catch (NullReferenceException)
            {
                return new List<ApplicationUser>();
            }
        }
        /*
         * Search users to change their roles
         * for example if someone deleted user from group
         * user status changes to defaultUser status
         * USE DBSAVECHANGES AFTER THIS METHOD
         */
        public async Task<IList<Group>> ClearUsersWithOldRoles(IEnumerable<String> users, Group group, ApplicationDbContext appContext, CourseUsers defaultUser)
        {
            IEnumerable<ApplicationUser> foundUsers = await appContext.Users.Where(u => !users.Contains(u.UserName)).ToArrayAsync();
            List<Group> editableGroups = new List<Group>();
            Course course = await appContext.Courses.FindAsync(group.Course_ID);
            if (foundUsers != null && course != null)
            {
                ApplicationUser foundUser = null;
                for (Int32 i = 0; i < foundUsers.Count(); i++)
                {
                    foundUser = foundUsers.ElementAt(i);
                    editableGroups.AddRange(await appContext.Groups.Where(g => g.Course_ID == course.ID && g.User.Id == foundUser.Id).ToArrayAsync());
                }
                foreach (var gr in editableGroups)
                {
                    gr.UserState_ID = (Int32)defaultUser;
                    gr.Name = null;
                }
            }
            return editableGroups;
        }
        /*
         * Returns users id from current username
         * if can't find user with such username returns empty string
         */
        private async Task<String> GetUsersIdFromName(String userName, ApplicationDbContext appContext)
        {
            if (userName != null && userName != String.Empty && appContext != null)
            {
                if (await appContext.Users.AnyAsync(u => String.Compare(userName, u.UserName, true) == 0))
                    return appContext.Users.First(u => String.Compare(userName, u.UserName, true) == 0).Id;
            }
            return String.Empty;
        }
        /*
         * Copies data from one group to another
         * It will be better to override method CopyTo or others in Group entity
         */
        private void CopyMainDataOfGroupTo(Group from, Group to)
        {
            if (from != null && to != null)
            {
                to.Finish = from.Finish;
                to.Start = from.Start;
                to.Duration = from.Duration;
                to.Name = from.Name;
            }
        }
        /*
         * Checks if this user has role in the group 
         */
        public Boolean CheckGroupUserForRole(ApplicationUser user, Course course, String groupName, params String[] roles)
        {
            CourseUsers uGroupRole;
            for (Int32 i = 0; i < roles.Length; i++)
            {
                if (Enum.TryParse(roles[i], out uGroupRole))
                {
                    if (user.Groups.Any(g => g.Course_ID == course.ID && g.UserState_ID == (Int32)uGroupRole && (String.Compare(g.Name, groupName, false) == 0 || g.Name == null)))
                        return true;
                }
            }
            return false;
        }

        public Boolean CheckUserForRoleInMainGroup(ApplicationUser user, Group group, params String[] roles)
        {
            CourseUsers uGroupRole;
            for (Int32 i = 0; i < roles.Length; i++)
            {
                if (Enum.TryParse(roles[i], out uGroupRole))
                {
                    if (user.Groups.Any(g => g.Course_ID == group.Course_ID && g.UserState_ID == (Int32)uGroupRole))
                        return true;
                }
            }
            return false;
        }
        /*
         * Checks users role with applied roles
         * and if he has applied role then returns this role
         * else returns default role
         */
        private CourseUsers CheckUserForRoles(IEnumerable<String> roles, IEnumerable<String> userRoles, CourseUsers defaultRole)
        {
            try
            {
                CourseUsers userRole;
                for (Int32 j = 0; j < roles.Count(); j++)
                {
                    if (userRoles.Contains(roles.ElementAt(j)))
                    {
                        userRole = (CourseUsers)Enum.Parse(typeof(CourseUsers), roles.ElementAt(j));
                        return userRole;
                    }
                }
                return defaultRole;
            }
            catch (ArgumentException)
            {
                return defaultRole;
            }
        }
        /*
         * Updates all users group stats to new, for example learner -> teacher or learner -> student
         * checkableRoles - list of roles to check in userManager, for example:
         * learner can't be a teacher or if user has teachers role he will be automatically add to teachers of this group
         * this method also updates all group info: name, dates etc.... for each user with role of student or teacher
         */
        public async Task RefreshAllGroupsDataAsync(String oldName, Group group, ApplicationDbContext appContext)
        {
            if (oldName != null && group != null && appContext != null)
            {
                Group[] groups = await appContext.Groups.Where(g => String.Compare(g.Name, oldName, false) == 0).ToArrayAsync();
                foreach (var gr in groups)
                {
                    CopyMainDataOfGroupTo(group, gr);
                    appContext.Entry(gr).State = EntityState.Modified;
                }
            }
        }
        public async Task<IList<Group>> UpdateUserStatsInGroupAsync(IEnumerable<String> users, Group mainGroup, ApplicationDbContext appContext, ApplicationUserManager userManager, params String[] checkableRoles)
        {
            if (users != null && mainGroup != null && appContext != null)
            {
                Course groupCourse = await appContext.Courses.FindAsync(mainGroup.Course_ID);
                if (groupCourse != null)
                {
                    IList<String> userRoles = null;
                    List<Group> groups = new List<Group>();
                    CourseUsers user;
                    for (Int32 i = 0; i < users.Count(); i++)
                    {
                        userRoles = await userManager.GetRolesAsync(await GetUsersIdFromName(users.ElementAt(i), appContext));
                        user = CheckUserForRoles(checkableRoles, userRoles, CourseUsers.Student);
                        IList<Group> gr = await ChangeCourseUserGroupAsync(new String[] { users.ElementAt(i) }, appContext, groupCourse, user, false);
                        if (gr.Count > 0)
                        {
                            CopyMainDataOfGroupTo(mainGroup, gr.First());
                            groups.Add(gr.First());
                        }
                    }
                    return groups;
                }
            }
            return new List<Group>();
        }
    }
}