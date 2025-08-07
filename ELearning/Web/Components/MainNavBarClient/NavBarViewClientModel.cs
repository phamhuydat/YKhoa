﻿using Share.Consts;

namespace Web.Components.MainNavBarClient
{
    public class NavBarViewClientModel
    {
        public NavBarViewClientModel()
        {
            Items = new List<MenuItem>();
        }
        public List<MenuItem> Items { get; set; }

    }
    public class MenuItem
    {
        public MenuItem()
        {
            Permission = AuthConst.NO_PERMISSION;
        }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string DisplayText { get; set; }
        public string SidebarText { get; set; }
        public string Icon { get; set; }
        public int Permission { get; set; }
        public MenuItem[] ChildrenItems { get; set; }
    }
}
