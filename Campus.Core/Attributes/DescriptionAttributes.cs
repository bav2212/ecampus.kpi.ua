﻿using System;

namespace Campus.Core.Attributes
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public sealed class DescriptionAttribute : AbstractAttribute
    {
        private readonly string _description;

        public DescriptionAttribute(string description)
        {
            _description = description;
        }

        public string Description
        {
            get { return _description; }
        }

        private static DescriptionAttribute _instance;
        public static DescriptionAttribute Instance
        {
            get { return _instance ?? (_instance = new DescriptionAttribute(null)); }
        }
    }
}
