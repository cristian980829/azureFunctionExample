﻿using System;

namespace AzureFunctionExample.common.Models
{
    public class Todo
    {
        public DateTime CreatedTime { get; set; }

        public string taskDescription { get; set; }

        public bool isCompleted { get; set; }
    }
}
