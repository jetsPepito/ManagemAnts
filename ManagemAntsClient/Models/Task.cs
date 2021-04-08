using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagemAntsClient.Models
{
    public class Task
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Duration { get; set; }
        public int State { get; set; }
        public int? TimeSpent { get; set; }


        public string getState()
        {
            switch(State)
            {
                case 0:
                    return "A faire";
                case 1:
                    return "En cours";
                case 2:
                    return "Fait";
                default:
                    return "Rendu";
            }
        }
    }
}
