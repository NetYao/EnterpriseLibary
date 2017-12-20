using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace Enterprises.Framework.Plugin.Domain.AD
{
    public class GroupMethodsManagement
    {
        public static bool AddUserToGroup(string samAccountName, string groupName, AdAdminConfig config)
        {
            if (IfUserExist(samAccountName, config))
            {
                if (!IfUserExistInGroup(samAccountName, groupName, config))
                {
                    try
                    {
                        PrincipalContext ctx = new PrincipalContext(ContextType.Domain,
                                        config.ServerIpOrDomain,
                                        config.AdminAccount,
                                        config.AdminPwd);
                        // find the group in question
                        GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, groupName);
                        UserPrincipal user = UserPrincipal.FindByIdentity(ctx, samAccountName);
                        group.Members.Add(user);
                        group.Save();
                        group.Dispose();
                        user.Dispose();
                        ctx.Dispose();

                    }
                    catch (Exception ex)
                    {
                        return false;
                    }

                    return true;
                }

                return false;
            }

            return false;
        }

        public static bool IfUserExist(string username,AdAdminConfig config)
        {
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain,
                                         config.ServerIpOrDomain,
                                         config.AdminAccount,
                                         config.AdminPwd);

            UserPrincipal usr = UserPrincipal.FindByIdentity(ctx,
                                                       IdentityType.SamAccountName,
                                                       username);


            if (usr != null)
            {
                usr.Dispose();
                ctx.Dispose();
                return true;

            }

            ctx.Dispose();
            return false;
        }

        public static bool EmptyGroup(string groupname, AdAdminConfig config)
        {
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain,
                                        config.ServerIpOrDomain,
                                        config.AdminAccount,
                                        config.AdminPwd);
            GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, groupname);
            if (group != null)
            {
                foreach (Principal p in group.GetMembers())
                {

                    UserPrincipal theUser = p as UserPrincipal;
                    if (theUser != null)
                    {
                        group.Members.Remove(theUser);
                        try
                        {
                            group.Save();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                }

                return true;
            }

            return false;
        }

        public static bool CreateGroup(string path, string name, AdAdminConfig config)
        {
            if (!DirectoryEntry.Exists(path))
            {
                try
                {
                    DirectoryEntry entry = new DirectoryEntry(path, config.AdminAccount, config.AdminPwd);
                    DirectoryEntry group = entry.Children.Add("CN=" + name, "group");
                    group.Properties["sAmAccountName"].Value = name;
                    group.CommitChanges();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            //EmptyGroup(name, config);
            return false;
        }

        public static bool ChangeGroupScope(string s, GroupScope gp, AdAdminConfig config)
        {
            try
            {
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain,
                                         config.ServerIpOrDomain,
                                         config.AdminAccount,
                                         config.AdminPwd);
                GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, s);
                if (group != null)
                {
                    group.GroupScope = gp;
                    group.Save();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return false;
            }


        }

        public static bool IfUserExistInGroup(string username, string groupname, AdAdminConfig config)
        {
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain,
                                          config.ServerIpOrDomain,
                                          config.AdminAccount,
                                          config.AdminPwd);

            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, username);
            GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, groupname);
            if (group != null && (user != null && user.IsMemberOf(group)))
            {
                return true;
            }

            return false;
        }

        public static bool IfGroupExist(string groupname,AdAdminConfig config)
        {
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain,
                                           config.ServerIpOrDomain,
                                           config.AdminAccount,
                                           config.AdminPwd);

            GroupPrincipal grp = GroupPrincipal.FindByIdentity(ctx,IdentityType.SamAccountName,groupname);
            if (grp != null)
            {
                grp.Dispose();
                ctx.Dispose();
                return true;

            }

            ctx.Dispose();
            return false;
        }
    }
}
