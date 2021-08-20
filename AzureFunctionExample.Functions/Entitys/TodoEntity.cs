using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureFunctionExample.Functions.Entitys
{
    public class TodoEntity : TableEntity
    {
        public DateTime CreatedTime { get; set; }

        public string taskDescription { get; set; }

        public bool isCompleted { get; set; }

    }
}
