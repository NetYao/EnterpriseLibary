using System;
using System.Collections;
using System.DirectoryServices.AccountManagement;
using System.Text.RegularExpressions;
using Enterprises.Framework.Domain;
using Enterprises.Framework.Plugin.Domain.AD;

namespace Enterprises.Framework.Plugin.Domain
{
    /// <summary>
    /// AD用户管理类
    /// </summary>
    public class AdMethodsAccountManagement
    {
        #region Variables

        private string sDomain = "test.com";
        private string sDefaultOU = "OU=Test Users,OU=Test,DC=test,DC=com";
        private string sDefaultRootOU = "DC=test,DC=com";
        private string sServiceUser = @"ServiceUser";
        private string sServicePassword = "ServicePassword";

        #endregion

        public AdMethodsAccountManagement(AdAdminConfig config)
        {
            sDefaultOU = config.DefaultOu;
            sDomain = config.ServerIpOrDomain;
            sDefaultRootOU = config.DcContainer;
            sServiceUser = config.AdminAccount;
            sServicePassword = config.AdminPwd;
        }

        #region Validate Methods

        /// <summary>
        /// Validates the username and password of a given user
        /// </summary>
        /// <param name="sUserName">The username to validate</param>
        /// <param name="sPassword">The password of the username to validate</param>
        /// <returns>Returns True of user is valid</returns>
        public bool ValidateCredentials(string sUserName, string sPassword)
        {
            PrincipalContext oPrincipalContext = GetPrincipalContext();
            return oPrincipalContext.ValidateCredentials(sUserName, sPassword);
        }

        /// <summary>
        /// 判断用户是否过期
        /// </summary>
        /// <param name="sUserName">The username to check</param>
        /// <returns>Returns true if Expired</returns>
        public bool IsUserExpired(string sUserName)
        {
            UserPrincipal oUserPrincipal = GetUser(sUserName);
            if (oUserPrincipal.AccountExpirationDate != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 判断用户是否存在
        /// </summary>
        /// <param name="sUserName">The username to check</param>
        /// <returns>Returns true if username Exists</returns>
        public bool IsUserExisiting(string sUserName)
        {
            if (GetUser(sUserName) == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 判断用户是否锁定
        /// </summary>
        /// <param name="sUserName">The username to check</param>
        /// <returns>Returns true of Account is locked</returns>
        public bool IsAccountLocked(string sUserName)
        {
            UserPrincipal oUserPrincipal = GetUser(sUserName);
            return oUserPrincipal.IsAccountLockedOut();
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="sUserName">The username to get</param>
        /// <returns>Returns the UserPrincipal Object</returns>
        public UserPrincipal GetUser(string sUserName)
        {
            PrincipalContext oPrincipalContext = GetPrincipalContext();

            UserPrincipal oUserPrincipal =UserPrincipal.FindByIdentity(oPrincipalContext, sUserName);
            return oUserPrincipal;
        }

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="sUserName">The username to get</param>
        /// <param name="identityType">类型</param>
        /// <returns>Returns the UserPrincipal Object</returns>
        public UserPrincipal GetUser(string sUserName, IdentityType identityType)
        {
            PrincipalContext oPrincipalContext = GetPrincipalContext();

            UserPrincipal oUserPrincipal = UserPrincipal.FindByIdentity(oPrincipalContext, identityType, sUserName);
            return oUserPrincipal;
        }

        /// <summary>
        /// Gets a certain group on Active Directory
        /// </summary>
        /// <param name="sGroupName">The group to get</param>
        /// <returns>Returns the GroupPrincipal Object</returns>
        public GroupPrincipal GetGroup(string sGroupName)
        {
            PrincipalContext oPrincipalContext = GetPrincipalContext();

            GroupPrincipal oGroupPrincipal =
                GroupPrincipal.FindByIdentity(oPrincipalContext, sGroupName);
            return oGroupPrincipal;
        }

        #endregion

        #region User Account Methods

        /// <summary>
        /// Sets the user password
        /// </summary>
        /// <param name="sUserName">The username to set</param>
        /// <param name="sNewPassword">The new password to use</param>
        /// <param name="sMessage">Any output messages</param>
        public void SetUserPassword(string sUserName, string sNewPassword, out string sMessage)
        {
            try
            {
                UserPrincipal oUserPrincipal = GetUser(sUserName);
                oUserPrincipal.SetPassword(sNewPassword);
                sMessage = "";
            }
            catch (Exception ex)
            {
                sMessage = ex.Message;
            }
        }

        /// <summary>
        /// Enables a disabled user account
        /// </summary>
        /// <param name="sUserName">The username to enable</param>
        public void EnableUserAccount(string sUserName)
        {
            UserPrincipal oUserPrincipal = GetUser(sUserName);
            oUserPrincipal.Enabled = true;
            oUserPrincipal.Save();
        }

        /// <summary>
        /// Force disabling of a user account
        /// </summary>
        /// <param name="sUserName">The username to disable</param>
        public void DisableUserAccount(string sUserName)
        {
            UserPrincipal oUserPrincipal = GetUser(sUserName);
            oUserPrincipal.Enabled = false;
            oUserPrincipal.Save();
        }

        /// <summary>
        /// Force expire password of a user
        /// </summary>
        /// <param name="sUserName">The username to expire the password</param>
        public void ExpireUserPassword(string sUserName)
        {
            UserPrincipal oUserPrincipal = GetUser(sUserName);
            oUserPrincipal.ExpirePasswordNow();
            oUserPrincipal.Save();
        }

        /// <summary>
        /// Unlocks a locked user account
        /// </summary>
        /// <param name="sUserName">The username to unlock</param>
        public void UnlockUserAccount(string sUserName)
        {
            UserPrincipal oUserPrincipal = GetUser(sUserName);
            oUserPrincipal.UnlockAccount();
            oUserPrincipal.Save();
        }

        /// <summary>
        /// Creates a new user on Active Directory
        /// </summary>
        /// <param name="sOu">The OU location you want to save your user</param>
        /// <param name="sUserName">The username of the new user</param>
        /// <param name="sPassword">The password of the new user</param>
        /// <param name="sGivenName">The given name of the new user</param>
        /// <param name="sSurname">The surname of the new user</param>
        /// <returns>returns the UserPrincipal object</returns>
        public UserPrincipal CreateNewUser(string sOu,
            string sUserName, string sPassword, string sGivenName, string sSurname)
        {
            if (!IsUserExisiting(sUserName))
            {
                PrincipalContext oPrincipalContext = GetPrincipalContext(sOu);

                UserPrincipal oUserPrincipal = new UserPrincipal
                    (oPrincipalContext, sUserName, sPassword, true /*Enabled or not*/);

                //User Log on Name
                oUserPrincipal.UserPrincipalName = sUserName;
                oUserPrincipal.GivenName = sGivenName;
                oUserPrincipal.Surname = sSurname;
                oUserPrincipal.SamAccountName = sUserName;

                oUserPrincipal.Save();

                return oUserPrincipal;
            }
            else
            {
                return GetUser(sUserName);
            }
        }


        /// <summary>
        /// Creates a new user on Active Directory
        /// </summary>
        /// <param name="sOu">The OU location you want to save your user</param>
        /// <param name="domainUser">创建用户对象</param>
        /// <returns>returns the UserPrincipal object</returns>
        public UserPrincipal CreateNewUser(string sOu,DomainUser domainUser)
        {
            if (!IsUserExisiting(domainUser.Account.SamAccountName))
            {
                PrincipalContext oPrincipalContext = GetPrincipalContext(sOu);

                UserPrincipal oUserPrincipal = new UserPrincipal
                    (oPrincipalContext, domainUser.Account.SamAccountName, domainUser.Account.Password,
                        domainUser.Account.IsEnabled)
                {
                    UserPrincipalName = domainUser.Account.DomainUserAccount,
                    GivenName = domainUser.User.GivenName,
                    Surname = domainUser.User.LastName,
                    SamAccountName = domainUser.Account.SamAccountName,
                    Name = domainUser.User.Name,
                    EmailAddress = domainUser.User.OfficeEMailAddress,
                    DisplayName = domainUser.User.DisplayName,
                    Description = domainUser.User.Description,
                    PasswordNeverExpires = false
                };

                oUserPrincipal.Save();

                return oUserPrincipal;
            }

            return GetUser(domainUser.Account.SamAccountName);
        }

        /// <summary>
        /// Deletes a user in Active Directory
        /// </summary>
        /// <param name="sUserName">The username you want to delete</param>
        /// <returns>Returns true if successfully deleted</returns>
        public bool DeleteUser(string sUserName)
        {
            try
            {
                UserPrincipal oUserPrincipal = GetUser(sUserName);

                oUserPrincipal.Delete();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Group Methods

        /// <summary>
        /// Creates a new group in Active Directory
        /// </summary>
        /// <param name="sOu">The OU location you want to save your new Group</param>
        /// <param name="sGroupName">The name of the new group</param>
        /// <param name="sDescription">The description of the new group</param>
        /// <param name="oGroupScope">The scope of the new group</param>
        /// <param name="bSecurityGroup">True is you want this group 
        /// to be a security group, false if you want this as a distribution group</param>
        /// <returns>Returns the GroupPrincipal object</returns>
        public GroupPrincipal CreateNewGroup(string sOu, string sGroupName,
            string sDescription, GroupScope oGroupScope, bool bSecurityGroup)
        {
            PrincipalContext oPrincipalContext = GetPrincipalContext(sOu);

            GroupPrincipal oGroupPrincipal = new GroupPrincipal(oPrincipalContext, sGroupName);
            oGroupPrincipal.Description = sDescription;
            oGroupPrincipal.GroupScope = oGroupScope;
            oGroupPrincipal.IsSecurityGroup = bSecurityGroup;
            oGroupPrincipal.Save();

            return oGroupPrincipal;
        }

        /// <summary>
        /// Adds the user for a given group
        /// </summary>
        /// <param name="sUserName">The user you want to add to a group</param>
        /// <param name="sGroupName">The group you want the user to be added in</param>
        /// <returns>Returns true if successful</returns>
        public bool AddUserToGroup(string sUserName, string sGroupName)
        {
            try
            {
                UserPrincipal oUserPrincipal = GetUser(sUserName);
                GroupPrincipal oGroupPrincipal = GetGroup(sGroupName);
                if (oUserPrincipal == null || oGroupPrincipal == null)
                {
                    return false;
                }

                if (!IsUserGroupMember(sUserName, sGroupName))
                {
                    oGroupPrincipal.Members.Add(oUserPrincipal);
                    oGroupPrincipal.Save();
                }


                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Removes user from a given group
        /// </summary>
        /// <param name="sUserName">The user you want to remove from a group</param>
        /// <param name="sGroupName">The group you want the user to be removed from</param>
        /// <returns>Returns true if successful</returns>
        public bool RemoveUserFromGroup(string sUserName, string sGroupName)
        {
            try
            {
                UserPrincipal oUserPrincipal = GetUser(sUserName);
                GroupPrincipal oGroupPrincipal = GetGroup(sGroupName);
                if (oUserPrincipal == null || oGroupPrincipal == null)
                {
                    return false;
                }

                if (IsUserGroupMember(sUserName, sGroupName))
                {
                    oGroupPrincipal.Members.Remove(oUserPrincipal);
                    oGroupPrincipal.Save();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if user is a member of a given group
        /// </summary>
        /// <param name="sUserName">The user you want to validate</param>
        /// <param name="sGroupName">The group you want to check the 
        /// membership of the user</param>
        /// <returns>Returns true if user is a group member</returns>
        public bool IsUserGroupMember(string sUserName, string sGroupName)
        {
            UserPrincipal oUserPrincipal = GetUser(sUserName);
            GroupPrincipal oGroupPrincipal = GetGroup(sGroupName);

            if (oUserPrincipal != null && oGroupPrincipal != null)
            {
                return oGroupPrincipal.Members.Contains(oUserPrincipal);
            }

            return false;
        }

        /// <summary>
        /// Gets a list of the users group memberships
        /// </summary>
        /// <param name="sUserName">The user you want to get the group memberships</param>
        /// <returns>Returns an arraylist of group memberships</returns>
        public ArrayList GetUserGroups(string sUserName)
        {
            ArrayList myItems = new ArrayList();
            UserPrincipal oUserPrincipal = GetUser(sUserName);

            PrincipalSearchResult<Principal> oPrincipalSearchResult = oUserPrincipal.GetGroups();

            foreach (Principal oResult in oPrincipalSearchResult)
            {
                myItems.Add(oResult.Name);
            }
            return myItems;
        }

        /// <summary>
        /// Gets a list of the users authorization groups
        /// </summary>
        /// <param name="sUserName">The user you want to get authorization groups</param>
        /// <returns>Returns an arraylist of group authorization memberships</returns>
        public ArrayList GetUserAuthorizationGroups(string sUserName)
        {
            ArrayList myItems = new ArrayList();
            UserPrincipal oUserPrincipal = GetUser(sUserName);

            PrincipalSearchResult<Principal> oPrincipalSearchResult =
                oUserPrincipal.GetAuthorizationGroups();

            foreach (Principal oResult in oPrincipalSearchResult)
            {
                myItems.Add(oResult.Name);
            }
            return myItems;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets the base principal context
        /// </summary>
        /// <returns>Returns the PrincipalContext object</returns>
        public PrincipalContext GetPrincipalContext()
        {
            PrincipalContext oPrincipalContext = new PrincipalContext
                (ContextType.Domain, sDomain, sDefaultOU, ContextOptions.SimpleBind,
                    sServiceUser, sServicePassword);
            return oPrincipalContext;
        }

        /// <summary>
        /// Gets the principal context on specified OU
        /// </summary>
        /// <param name="sOu">The OU you want your Principal Context to run on</param>
        /// <returns>Returns the PrincipalContext object</returns>
        public PrincipalContext GetPrincipalContext(string sOu)
        {
            PrincipalContext oPrincipalContext =
                new PrincipalContext(ContextType.Domain, sDomain, sOu,sServiceUser, sServicePassword);
            return oPrincipalContext;
        }

        #endregion
        
        
        
        /// <summary>
        /// 验证密码是否符合规则
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool PasswordMeetsComplexity(string password)
        {
            var criteria = 0;
            //check lowercase
            var lowercase = new Regex("[a-z]");
            if (lowercase.IsMatch(password))
            {
                criteria++;
            }

            //check uppercase
            var uppercase = new Regex("[A-Z]");
            if (uppercase.IsMatch(password))
            {
                criteria++;
            }

            //check numbers
            var numeric = new Regex("[0-9]");
            if (numeric.IsMatch(password))
            {
                criteria++;
            }

            //check symbols
            var symbolic = new Regex(@"[.,@_\-\'$#!%^Z&*()+=?<>{}\[\]/\\:;|~`]");
            if (symbolic.IsMatch(password))
            {
                criteria++;
            }

            return (criteria >= 3) && (password.Length >= 7);
        }
    }
}