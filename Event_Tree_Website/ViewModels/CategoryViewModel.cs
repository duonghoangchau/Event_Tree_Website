﻿using Event_Tree_Website.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Event_Tree_Website.ViewModels
{
    public class CategoryViewModel
    {
        internal string cateName;

        public List<Category> Cats { get; set; }
        public List<Menu> Menus { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public Category Category { get; internal set; }
        public List<SelectListItem> HideOptions { get; internal set; }
        public Category Categorys { get; internal set; }
    }
}